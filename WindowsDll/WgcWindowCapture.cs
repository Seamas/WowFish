using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Windows.Graphics.Capture;
using Windows.Graphics.DirectX.Direct3D11;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.DXGI;
using WinRT;

namespace WindowsDll;

/// <summary>
/// 基于 Windows Graphics Capture (WGC) 的后台窗口捕获。
/// 支持窗口被其他窗口遮挡时截图，但窗口不能最小化。
/// </summary>
public class WgcWindowCapture : IDisposable
{
    private ID3D11Device? _d3dDevice;
    private ID3D11DeviceContext? _d3dContext;
    private IDirect3DDevice? _direct3DDevice;
    private Direct3D11CaptureFramePool? _framePool;
    private GraphicsCaptureSession? _session;
    private GraphicsCaptureItem? _captureItem;
    private Bitmap? _latestFrame;
    private readonly object _frameLock = new();
    private bool _disposed;

    public bool IsCapturing => _session != null;

    /// <summary>
    /// 获取最新一帧的副本。调用方负责 Dispose。
    /// </summary>
    public Bitmap? GetLatestFrame()
    {
        lock (_frameLock)
        {
            return _latestFrame == null ? null : new Bitmap(_latestFrame);
        }
    }

    public void Start(IntPtr hwnd)
    {
        if (IsCapturing) return;

        // 1. 创建 D3D11 设备
        const uint D3D11_SDK_VERSION = 7;
        var hr = D3D11CreateDevice(
            IntPtr.Zero,
            DriverType.Hardware,
            IntPtr.Zero,
            DeviceCreationFlags.BgraSupport,
            null,
            0,
            D3D11_SDK_VERSION,
            out _d3dDevice,
            out _,
            out _d3dContext);
        if (hr != 0) throw new InvalidOperationException($"D3D11CreateDevice failed: 0x{hr:X8}");

        // 2. 通过 DXGI 设备创建 WinRT IDirect3DDevice
        using var dxgiDevice = _d3dDevice!.QueryInterface<IDXGIDevice>();
        hr = CreateDirect3D11DeviceFromDXGIDevice(dxgiDevice.NativePointer, out var direct3DDevicePtr);
        if (hr != 0) throw new InvalidOperationException($"CreateDirect3D11DeviceFromDXGIDevice failed: 0x{hr:X8}");
        _direct3DDevice = MarshalInterface<IDirect3DDevice>.FromAbi(direct3DDevicePtr);

        // 3. 为指定窗口创建 GraphicsCaptureItem
        _captureItem = CreateCaptureItemForWindow(hwnd);

        // 4. 创建帧池
        var rect = WindowCapture.GetWindowRectangle(hwnd);
        _framePool = Direct3D11CaptureFramePool.CreateFreeThreaded(
            _direct3DDevice,
            Windows.Graphics.DirectX.DirectXPixelFormat.B8G8R8A8UIntNormalized,
            2,
            new Windows.Graphics.SizeInt32(rect.Width, rect.Height));
        _framePool.FrameArrived += OnFrameArrived;

        // 5. 开始捕获
        _session = _framePool.CreateCaptureSession(_captureItem);
        _session.StartCapture();
    }

    public void Stop()
    {
        _session?.Dispose();
        _session = null;
        _framePool?.Dispose();
        _framePool = null;
        _captureItem = null;
        lock (_frameLock)
        {
            _latestFrame?.Dispose();
            _latestFrame = null;
        }
    }

    private static GraphicsCaptureItem CreateCaptureItemForWindow(IntPtr hwnd)
    {
        var interop = GraphicsCaptureItem.As<IGraphicsCaptureItemInterop>();
        var iid = typeof(GraphicsCaptureItem).GUID;
        var ptr = interop.CreateForWindow(hwnd, ref iid);
        var item = MarshalInterface<GraphicsCaptureItem>.FromAbi(ptr);
        Marshal.Release(ptr);
        return item;
    }

    private void OnFrameArrived(Direct3D11CaptureFramePool sender, object args)
    {
        if (_disposed) return;

        using var frame = sender.TryGetNextFrame();
        if (frame == null) return;

        try
        {
            var bitmap = ConvertFrameToBitmap(frame);
            lock (_frameLock)
            {
                _latestFrame?.Dispose();
                _latestFrame = bitmap;
            }
        }
        catch
        {
            // 忽略单帧转换失败，避免影响后续捕获
        }
    }

    private Bitmap ConvertFrameToBitmap(Direct3D11CaptureFrame frame)
    {
        var access = (IDirect3DDxgiInterfaceAccess)frame.Surface;
        var iid = typeof(ID3D11Texture2D).GUID;
        var texturePtr = access.GetInterface(ref iid);
        using var texture = new ID3D11Texture2D(texturePtr);

        var desc = texture.Description;
        desc.BindFlags = BindFlags.None;
        desc.CPUAccessFlags = CpuAccessFlags.Read;
        desc.Usage = ResourceUsage.Staging;
        desc.MiscFlags = ResourceOptionFlags.None;

        using var staging = _d3dDevice!.CreateTexture2D(desc);
        _d3dContext!.CopyResource(staging, texture);

        var dataBox = _d3dContext.Map(staging, 0, MapMode.Read, Vortice.Direct3D11.MapFlags.None);
        try
        {
            var width = (int)desc.Width;
            var height = (int)desc.Height;
            var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            var bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.WriteOnly,
                PixelFormat.Format32bppArgb);

            for (var y = 0; y < height; y++)
            {
                var srcPtr = IntPtr.Add(dataBox.DataPointer, y * (int)dataBox.RowPitch);
                var dstPtr = IntPtr.Add(bitmapData.Scan0, y * bitmapData.Stride);
                CopyMemory(dstPtr, srcPtr, (uint)(width * 4));
            }

            bitmap.UnlockBits(bitmapData);
            return bitmap;
        }
        finally
        {
            _d3dContext.Unmap(staging, 0);
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        Stop();
        _direct3DDevice?.Dispose();
        _d3dContext?.Dispose();
        _d3dDevice?.Dispose();
        GC.SuppressFinalize(this);
    }

    [DllImport("d3d11.dll")]
    private static extern int D3D11CreateDevice(
        IntPtr pAdapter,
        DriverType DriverType,
        IntPtr Software,
        DeviceCreationFlags Flags,
        [In] FeatureLevel[]? pFeatureLevels,
        uint FeatureLevels,
        uint SDKVersion,
        out ID3D11Device? ppDevice,
        out FeatureLevel pFeatureLevel,
        out ID3D11DeviceContext? ppImmediateContext);

    [DllImport("d3d11.dll")]
    private static extern int CreateDirect3D11DeviceFromDXGIDevice(IntPtr dxgiDevice, out IntPtr graphicsDevice);

    [DllImport("kernel32.dll", EntryPoint = "RtlCopyMemory")]
    private static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);
}

[ComImport]
[Guid("3628E81B-3CAC-4C60-B7F0-23CE0E0C3356")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IGraphicsCaptureItemInterop
{
    IntPtr CreateForWindow([In] IntPtr window, [In] ref Guid iid);
}

[ComImport]
[Guid("A9B3D012-3DF2-4EE3-B8D1-8695F457D3C1")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IDirect3DDxgiInterfaceAccess
{
    IntPtr GetInterface([In] ref Guid iid);
}
