using System.Runtime.InteropServices;
using AutoKey.Models;
using AutoScript;
using KenerlImport;
using LogDll;
using WindowsDll;

namespace AutoKey.Services;

/// <summary>
/// 自动按键脚本：启动时记录当前前台窗口，之后通过窗口消息向该窗口发送按键
/// </summary>
public class KeyPresserScript : IAutoScript
{
    private const uint WmKeyDown = 0x0100;
    private const uint WmKeyUp = 0x0101;
    private const uint WmActivate = 0x0006;
    private const uint WmActivateApp = 0x001C;

    private CancellationTokenSource? _cts;

    public string Name => "自动按键";

    public bool IsRunning => _cts is { IsCancellationRequested: false };

    public object ScriptData => new object();

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        if (_cts != null) return;

        var config = AutoKeyConfig.Instance;
        var keyCode = ParseKeyCode(config.Key);
        if (keyCode == 0)
        {
            LogHelper.Info($"按键配置无效: {config.Key}");
            return;
        }

        if (config.RepeatCount <= 0)
        {
            LogHelper.Info($"重复次数必须大于 0，当前为 {config.RepeatCount}");
            return;
        }

        var hwnd = WindowHelper.GetActiveWindowHandle();
        if (hwnd == IntPtr.Zero)
        {
            LogHelper.Info("未检测到活动窗口，请确保游戏窗口处于前台。");
            return;
        }

        if (User32.IsIconic(hwnd))
        {
            LogHelper.Info("目标窗口已最小化，请恢复窗口后重试。");
            return;
        }

        LogHelper.Info($"目标窗口: {WindowHelper.GetWindowTitle(hwnd)} (句柄 {hwnd}, 类名 {WindowHelper.GetWindowClassName(hwnd)})");
        LogHelper.Info("按键将以窗口消息方式发送，窗口可被其他窗口遮挡，但不可最小化");
        LogHelper.Info($"{Name}开始：按键 {config.Key}，间隔 {config.IntervalMilliseconds} 毫秒，共 {config.RepeatCount} 次");

        var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _cts = cts;
        var token = cts.Token;
        try
        {
            for (var i = 1; i <= config.RepeatCount; i++)
            {
                if (token.IsCancellationRequested) break;

                if (config.SimulateActivation)
                {
                    PostActivation(hwnd);
                }

                if (!PostKey(hwnd, WmKeyDown, keyCode))
                {
                    break;
                }

                await Task.Delay(100, token);

                if (!PostKey(hwnd, WmKeyUp, keyCode))
                {
                    break;
                }

                LogHelper.Info($"第 {i}/{config.RepeatCount} 次按下 {config.Key}");

                if (i < config.RepeatCount)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(Math.Max(0, config.IntervalMilliseconds)), token);
                }
            }
        }
        catch (OperationCanceledException)
        {
            // 已停止
        }
        finally
        {
            _cts = null;
            cts.Dispose();
            LogHelper.Info($"{Name}结束");
        }
    }

    /// <summary>
    /// 投递激活消息，让目标窗口认为自身处于激活状态（部分游戏在非激活时会忽略按键）
    /// </summary>
    private static void PostActivation(IntPtr hWnd)
    {
        User32.PostMessage(hWnd, WmActivateApp, (IntPtr)1, IntPtr.Zero);
        User32.PostMessage(hWnd, WmActivate, (IntPtr)1, IntPtr.Zero);
    }

    /// <summary>
    /// 通过窗口消息发送按键，无需目标窗口处于前台
    /// </summary>
    private static bool PostKey(IntPtr hWnd, uint message, byte virtualKey)
    {
        var scanCode = (uint)User32.MapVirtualKey(virtualKey, 0);
        var keyUp = message == WmKeyUp;
        // 按下：重复次数 1 + 扫描码；抬起：额外置位第 30、31 位
        var lParam = (IntPtr)(1u | (scanCode << 16) | (keyUp ? (1u << 30) | 0x80000000u : 0u));
        if (User32.PostMessage(hWnd, message, (IntPtr)virtualKey, lParam))
        {
            return true;
        }

        var error = Marshal.GetLastWin32Error();
        LogHelper.Info(error == 5
            ? "发送按键消息被拒绝（错误码 5）：目标窗口可能以管理员权限运行，请尝试以管理员身份运行本程序"
            : $"发送按键消息失败，错误码 {error}");
        return false;
    }

    public async Task StopAsync()
    {
        await Task.Yield();
        var cts = _cts;
        if (cts == null) return;

        try
        {
            cts.Cancel();
        }
        catch (ObjectDisposedException)
        {
            // 循环已结束并释放
        }

        LogHelper.Info($"{Name}已停止");
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
}
