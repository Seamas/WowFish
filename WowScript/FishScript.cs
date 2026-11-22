using System;
using System.Drawing.Imaging;
using System.IO;
using AutoScript;
using KenerlImport;
using LogDll;
using WindowsDll;

namespace WowScript;

public class FishScript : IAutoScript
{

    #region 成员变量

    private volatile bool _isRunning;

    private const int FishingKey = 0x32; // 钓鱼按键数字2,此键是Windows虚拟按钮，与AscII码不同

    private HWndInfo? _hwndInfo;

    /// <summary>
    /// 上次上鱼饵的时间，用于判断鱼饵是否仍然有效
    /// </summary>
    private DateTime _lastBaitTime = DateTime.MinValue;

    #endregion

    public string Name => "自动钓鱼";

    public bool IsRunning => _isRunning;


    public object ScriptData => new object();

    /// <summary>
    /// 视频帧就绪事件，参数为 PNG 图像字节数组与鱼漂中心点信息
    /// </summary>
    public event Action<byte[], (int X, int Y, int Count)>? VideoFrameReady;

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        if (_isRunning) return;
        _isRunning = true;
        LogHelper.Info($"{Name}开始...");

        if (!InitHWndInfo())
        {
            _isRunning = false;
            LogHelper.Info($"{Name} 初始化窗口失败，请重新尝试");
            return;
        }

        while(_isRunning)
        {
            await FishingProcess();
        }

    }

    private async Task FishingProcess()
    {
        try
        {
            var rect = WindowCapture.GetWindowRectangle(_hwndInfo!.HWnd);
            LogHelper.Info($"窗口位置: {rect.Left},{rect.Top} 大小: {rect.Width}x{rect.Height}");

            // 1. 使用填充键清理背包空格（如果启用）
            await ApplyFillKey();

            // 2. 上鱼饵（如果启用），该步骤耗时不计入本次钓鱼超时
            await ApplyBait();

            var utc = DateTime.Now.AddSeconds(30).ToFileTimeUtc();
            LogHelper.Info("--------------------");
            LogHelper.Info($"开始钓鱼流程..., 本次钓鱼结束时间最迟为 {utc}");

            // 3. 施放钓鱼技能
            await CastFishing(rect);

            // 4. 查找鱼漂位置
            if (FindBobber(rect, out var lastPos))
            {
                // 5. 等待鱼上钩
                if (await DetectBobberBite(rect, lastPos, utc))
                {
                    // 6. 收竿
                    await ReelIn(lastPos, rect);
                }
                else
                {
                    LogHelper.Info($"超时未检测到鱼上钩");
                }
            }
            else
            {
                LogHelper.Info("未检测到鱼漂....");
            }

            // 等待随机时间
            // LogHelper.Info($"随机等待3~8秒，准备下一次钓鱼...");
            // await Task.Delay(new Random().Next(3000, 8000));
        }
        catch (Exception ex)
        {
            LogHelper.Info($"发生错误: {ex.Message}", Color.Red);
            await Task.Delay(5000);
        }
    }

    public async Task StopAsync()
    {
        await Task.Yield();
        _isRunning = false;
        // 停止后重置鱼饵计时，下次启动重新上饵
        _lastBaitTime = DateTime.MinValue;
        LogHelper.Info($"{Name}结束!");
    }




    #region 过程

    /// <summary>
    /// 初始化程序句柄
    /// </summary>
    /// <returns></returns>
    private bool InitHWndInfo()
    {
        var hWnd = new HWndInfo();
        var activeWindowHandle = WindowHelper.GetActiveWindowHandle();
        if (activeWindowHandle == IntPtr.Zero)
        {
            LogHelper.Info("未检测到活动窗口，请确保游戏窗口处于前台。");
            return false;
        }

        hWnd.HWnd = activeWindowHandle;
        hWnd.Title = WindowHelper.GetWindowTitle(activeWindowHandle);
        hWnd.ProcessId = WindowHelper.GetWindowProcessId(activeWindowHandle);
        var dpiForWindow = ScreenCapture.GetDpiForWindow(activeWindowHandle);
        var scaleFactor = dpiForWindow / 96.0f; // 96 DPI 是标准DPI
        hWnd.Scale = scaleFactor;

        LogHelper.Info(hWnd.ToString());

        _hwndInfo = hWnd;
        return true;
    }

    /// <summary>
    /// 使用填充键（消耗蚌壳等物品，清理背包空格）
    /// </summary>
    private async Task ApplyFillKey()
    {
        var config = FishConfig.Instance;
        if (!config.UseFillKey) return;

        var keyCode = ParseKeyCode(config.FillKey);
        if (keyCode == 0)
        {
            LogHelper.Info($"填充键配置无效: {config.FillKey}");
            return;
        }

        LogHelper.Info($"使用填充键: 按下 {config.FillKey}");
        User32.keybd_event(keyCode, 0, InputEventConstant.KeyEventKeyDown, 0);
        await Task.Delay(100);
        User32.keybd_event(keyCode, 0, InputEventConstant.KeyEventKeyUp, 0);

        // 等待填充动作完成
        await Task.Delay(500);
    }

    /// <summary>
    /// 上鱼饵（按下游戏内配置好的宏按键）
    /// </summary>
    private async Task ApplyBait()
    {
        var config = FishConfig.Instance;
        if (!config.UseBait) return;

        // 鱼饵仍在有效期内，无需重复上饵
        if (_lastBaitTime != DateTime.MinValue)
        {
            var usedMinutes = (DateTime.Now - _lastBaitTime).TotalMinutes;
            if (usedMinutes < config.BaitDurationMinutes)
            {
                LogHelper.Info($"鱼饵仍有效（已使用 {usedMinutes:F1} 分钟 / {config.BaitDurationMinutes} 分钟），跳过上鱼饵");
                return;
            }
        }

        var keyCode = ParseKeyCode(config.BaitKey);
        if (keyCode == 0)
        {
            LogHelper.Info($"鱼饵按键配置无效: {config.BaitKey}");
            return;
        }

        LogHelper.Info($"上鱼饵: 按下 {config.BaitKey}");
        User32.keybd_event(keyCode, 0, InputEventConstant.KeyEventKeyDown, 0);
        await Task.Delay(100);
        User32.keybd_event(keyCode, 0, InputEventConstant.KeyEventKeyUp, 0);

        // 等待上鱼饵动作完成
        await Task.Delay(5000);
        _lastBaitTime = DateTime.Now;
    }

    /// <summary>
    /// 将按键名转换为 Windows 虚拟键码，支持 0-9、A-Z、F1-F12
    /// </summary>
    private static byte ParseKeyCode(string? key)
    {
        if (string.IsNullOrWhiteSpace(key)) return 0;
        key = key.Trim().ToUpperInvariant();

        if (key.Length == 1)
        {
            var c = key[0];
            if (c >= '0' && c <= '9') return (byte)(0x30 + c - '0');
            if (c >= 'A' && c <= 'Z') return (byte)(0x41 + c - 'A');
        }

        if (key.StartsWith('F') && int.TryParse(key[1..], out var n) && n >= 1 && n <= 12)
        {
            return (byte)(0x70 + n - 1);
        }

        return 0;
    }

    /// <summary>
    /// 进行钓鱼操作
    /// </summary>
    /// <returns></returns>
    private async Task CastFishing(RECT rect)
    {
        // 模拟按键按下和释放
        User32.keybd_event((byte)FishingKey, 0, InputEventConstant.KeyEventKeyDown, 0);
        await Task.Delay(100);
        User32.keybd_event((byte)FishingKey, 0, InputEventConstant.KeyEventKeyUp, 0);

        // 等待动画结束，期间持续刷新预览画面
        var end = DateTime.Now.AddMilliseconds(1000);
        while (_isRunning && DateTime.Now < end)
        {
            PublishPreviewFrame(rect);
            await Task.Delay(100);
        }
    }

    /// <summary>
    /// 查找鱼漂以及位置
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="point"></param>
    /// <returns></returns>
    private bool FindBobber(RECT rect, out (int, int, int) point)
    {
        // 检测鱼漂位置
        LogHelper.Info("开始检测鱼漂...");
        // 截图窗口区域的图片
        point = FindBobberPoint(rect);
        // 如果点数 >= 300， 则认为鱼漂存在
        return point.Item3 >= FishConfig.Instance.BobberPoints;
    }


    /// <summary>
    /// 通过多次检测位置变化
    /// </summary>
    /// <returns></returns>
    private async Task<bool> DetectBobberBite(RECT rect, (int, int, int) lastPos, long utc)
    {
        // 等待鱼上钩，如果没有停止或者超时，一直继续
        while (_isRunning && DateTime.Now.ToFileTimeUtc() < utc )
        {
            var current = FindBobberPoint(rect);
            if (current.Count < FishConfig.Instance.BobberPoints)
            {
                LogHelper.Info("未检测到鱼漂，可能是鱼漂消失或位置不正确");
                return true;
            }

            var distance = Math.Pow(current.X - lastPos.Item1, 2) + Math.Pow(current.Y - lastPos.Item2, 2);
            LogHelper.Info($"鱼漂运动检测差值 {distance}");
            if (distance >= FishConfig.Instance.Distance)
            {
                return true;
            }

            // 控制检测与视频帧率，避免 CPU 占用过高
            await Task.Delay(50);
        }

        return false;
    }

    private (int X, int Y, int Count) FindBobberPoint(RECT rect)
    {
        using var screenshot = ScreenCapture.CaptureScreenRegion(rect, _hwndInfo!.Scale);

        var (x, y, width, height) = GetDetectRegion(screenshot.Width, screenshot.Height);

        // 符合鱼漂颜色的点
        LogHelper.Info("开始筛选符合颜色的点....");
        var bobberColoredPoints = FilterImage(screenshot, x, y, width, height, FishConfig.Instance.Threshold);
        LogHelper.Info($"符合颜色的点的数量为 {bobberColoredPoints.Count} 个");

        // 经过中心化筛选出的鱼漂点
        var multipleIteration = bobberColoredPoints.Count > 0
            ? MultipleIteration(bobberColoredPoints)
            : new List<(int, int)>();

        // 鱼漂中心点
        var center = multipleIteration.Count > 0
            ? CenterOfBobber(multipleIteration)
            : (0, 0, 0);

        DebugScreenShot(screenshot, bobberColoredPoints, x, width, y, height);
        PublishVideoFrame(screenshot, bobberColoredPoints, multipleIteration, center, x, y, width, height);

        return center;
    }

    /// <summary>
    /// 计算检测区域（图像中心偏上区域）
    /// </summary>
    private static (int X, int Y, int Width, int Height) GetDetectRegion(int imageWidth, int imageHeight)
    {
        var x = imageWidth / 4;
        var y = imageHeight / 5;
        return (x, y, x * 2, y * 2);
    }

    /// <summary>
    /// 仅发布预览画面（不进行鱼漂检测），用于施法等待等空闲阶段
    /// </summary>
    private void PublishPreviewFrame(RECT rect)
    {
        if (VideoFrameReady == null) return;

        using var screenshot = ScreenCapture.CaptureScreenRegion(rect, _hwndInfo!.Scale);
        var (x, y, width, height) = GetDetectRegion(screenshot.Width, screenshot.Height);
        PublishVideoFrame(screenshot, new List<(int, int)>(), new List<(int, int)>(), (0, 0, 0), x, y, width, height);
    }

    private void DebugScreenShot(Bitmap screenshot, List<(int, int)> bobberColoredPoints, int x, int width, int y, int height)
    {
        if (FishConfig.Instance.Debug)
        {
            screenshot.Save("screenshotBefore.png", ImageFormat.Png);
            foreach (var (w, h) in bobberColoredPoints)
            {
                screenshot.SetPixel(w, h, Color.Red);
            }
        
            for (var i = x; i < x + width; i++)
            {
                var y1 = y;
                var y2 = y + height;
                screenshot.SetPixel(i, y1, Color.Red);
                screenshot.SetPixel(i, y2, Color.Red);
            }
        
            for (var j = y; j < y + height; j++)
            {
                var x1 = x;
                var x2 = x + width;
                screenshot.SetPixel(x1, j, Color.Red);
                screenshot.SetPixel(x2, j, Color.Red);
            }
        
            screenshot.Save("screenshotAfter.png", ImageFormat.Png);
        }
    }


    private void PublishVideoFrame(Bitmap screenshot, List<(int, int)> coloredPoints, List<(int, int)> bobberPoints,
        (int X, int Y, int Count) center, int x, int y, int width, int height)
    {
        if (VideoFrameReady == null) return;

        using var annotated = new Bitmap(screenshot);
        using var g = Graphics.FromImage(annotated);
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

        // 线宽随图片分辨率放大，避免缩放显示后看不见
        var thickness = Math.Max(2, screenshot.Width / 400);

        // 1. 检测区域边框（半透明黄色填充 + 实线边框），始终绘制
        using (var fillBrush = new SolidBrush(Color.FromArgb(32, Color.Yellow)))
        {
            g.FillRectangle(fillBrush, x, y, width, height);
        }

        using (var regionPen = new Pen(Color.Yellow, thickness))
        {
            g.DrawRectangle(regionPen, x, y, width, height);
        }

        // 2. 所有符合颜色的点用红色小方块标注
        var dotSize = Math.Max(2, thickness / 2);
        using (var coloredBrush = new SolidBrush(Color.Red))
        {
            foreach (var (px, py) in coloredPoints)
            {
                g.FillRectangle(coloredBrush, px - dotSize / 2, py - dotSize / 2, dotSize, dotSize);
            }
        }

        // 3. 经筛选后的鱼漂点用亮绿色标注
        var bobberDotSize = Math.Max(3, thickness);
        using (var bobberBrush = new SolidBrush(Color.Lime))
        {
            foreach (var (px, py) in bobberPoints)
            {
                g.FillRectangle(bobberBrush, px - bobberDotSize / 2, py - bobberDotSize / 2, bobberDotSize, bobberDotSize);
            }
        }

        // 4. 鱼漂中心点十字与圆圈
        if (center.Count > 0)
        {
            var radius = Math.Max(10, screenshot.Width / 150);
            using var centerPen = new Pen(Color.Lime, thickness);
            g.DrawEllipse(centerPen, center.X - radius, center.Y - radius, radius * 2, radius * 2);
            g.DrawLine(centerPen, center.X - radius - thickness * 2, center.Y, center.X + radius + thickness * 2, center.Y);
            g.DrawLine(centerPen, center.X, center.Y - radius - thickness * 2, center.X, center.Y + radius + thickness * 2);
        }

        using var ms = new MemoryStream();
        annotated.Save(ms, ImageFormat.Png);
        VideoFrameReady.Invoke(ms.ToArray(), center);
    }

    /// <summary>
    /// 收杆操作
    /// </summary>
    /// <param name="position"></param>
    /// <param name="rect"></param>
    /// <param name="hWndInfo"></param>
    private async Task ReelIn((int, int, int) position, RECT rect)
    {
        // 由于有DPI缩放，需要计算出鼠标点击位置
        var x = (int)((rect.Left + position.Item1) / _hwndInfo!.Scale);
        var y = (int)((rect.Top + position.Item2) / _hwndInfo.Scale);

        LogHelper.Info($"点击位置: {x}, {y} (原始位置: {position.Item1}, {position.Item2})");

        // 设置鼠标位置
        User32.SetCursorPos(x, y);
        await Task.Delay(100); // 等待鼠标移动

        // 模拟鼠标左键点击
        User32.mouse_event(InputEventConstant.MouseLeftDown, 0, 0, 0, 0);
        await Task.Delay(100); // 等待点击效果
        User32.mouse_event(InputEventConstant.MouseLeftUp, 0, 0, 0, 0);
    }


    
    #endregion

    #region  算法
    
    /// <summary>
    /// 计算图像中符合条件的点
    /// </summary>
    /// <param name="bitmap"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="threshold"></param>
    /// <returns></returns>
    private List<(int, int)> FilterImage(Bitmap bitmap, int x, int y, int width, int height, double threshold = 1)
    {
        var points = new List<(int, int)>();
        for (int i = x; i < x + width; i++)
        {
            for (int j = y; j < y + height; j++)
            {
                // 获取像素颜色
                var pixelColor = bitmap.GetPixel(i, j);
                if (pixelColor.R > Math.Max(pixelColor.B, pixelColor.G) * threshold)
                {
                    points.Add((i, j));
                }
            }
        }

        return points;
    }
    
    
    /// <summary>
    /// 计算中心点，以及点的数量
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    private (int X, int Y, int Count) CenterOfBobber(List<(int, int)> list)
    {
        // 计算质心
        double sumX = list.Sum(p => p.Item1);
        double sumY = list.Sum(p => p.Item2);
        var count = list.Count;
        
        LogHelper.Info($"中心点计算: X={sumX / count}, Y={sumY / count}, 点数={count}");

        return ((int)(sumX / count), (int)(sumY / count), count);
    }
    

    private List<(int, int)> Statistical(List<(int, int)> list, double threshold = 1.5)
    {
        // 计算x坐标的中位数和四分位距
        var xValues = list.Select(p => p.Item1).OrderBy(x => x).ToList();
        double xMedian = xValues[xValues.Count / 2];
        double xQ1 = xValues[xValues.Count / 4];
        double xQ3 = xValues[xValues.Count * 3 / 4];
        double xIQR = xQ3 - xQ1;

        // 计算y坐标的中位数和四分位距
        var yValues = list.Select(p => p.Item2).OrderBy(y => y).ToList();
        double yMedian = yValues[yValues.Count / 2];
        double yQ1 = yValues[yValues.Count / 4];
        double yQ3 = yValues[yValues.Count * 3 / 4];
        double yIQR = yQ3 - yQ1;

        // 过滤离群点
        var filteredPoints = list.Where(p =>
            p.Item1 >= xQ1 - threshold * xIQR &&
            p.Item1 <= xQ3 + threshold * xIQR &&
            p.Item2 >= yQ1 - threshold * yIQR &&
            p.Item2 <= yQ3 + threshold * yIQR).ToList();

        return filteredPoints;
    }
    

    /// <summary>
    /// 
    /// </summary>
    /// <param name="list"></param>
    /// <param name="threshold">标准差阈值</param>
    /// <param name="iterations">迭代次数</param>
    /// <returns></returns>
    private List<(int, int)> MultipleIteration(List<(int, int)> list, double threshold = 2.0, int iterations = 1)
    {
        var currentPoints = list;
        for(var index = 0; index < iterations; index++)
        {
            (double meanX, double meanY) = (currentPoints.Average(p => p.Item1), currentPoints.Average(p => p.Item2));
            // 计算每个点到均值的距离
            var distance = currentPoints
                .Select(p => Math.Sqrt(Math.Pow(p.Item1 - meanX, 2) + Math.Pow(p.Item2 - meanY, 2)))
                .ToList();
            
            // 计算标准差
            double stdDev = Math.Sqrt(distance.Average(d => Math.Pow(d - distance.Average(), 2)));

            // 过滤距离过大的点
            currentPoints = currentPoints
                .Where((p, i) => distance[i] < threshold * stdDev)
                .ToList();
        }

        return currentPoints;
    }
    
    #endregion
}