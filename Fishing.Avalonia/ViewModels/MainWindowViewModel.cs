using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LogDll;
using WowScript;

namespace Fishing.Avalonia.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly FishScript _fishScript = new();
    private readonly FishConfig _config = FishConfig.Instance;
    private CancellationTokenSource? _cancellationTokenSource;

    [ObservableProperty]
    private bool _debug;

    [ObservableProperty]
    private double _threshold;

    [ObservableProperty]
    private double _distance;

    [ObservableProperty]
    private int _bobberPoints;

    [ObservableProperty]
    private bool _useBait;

    [ObservableProperty]
    private string _baitKey = "3";

    [ObservableProperty]
    private int _baitDurationMinutes = 10;

    [ObservableProperty]
    private bool _isRunning;

    [ObservableProperty]
    private Bitmap? _videoFrame;

    [ObservableProperty]
    private string _bobberStatus = "未检测到鱼漂";

    public ObservableCollection<string> Logs { get; } = new();

    public MainWindowViewModel()
    {
        _debug = _config.Debug;
        _threshold = _config.Threshold;
        _distance = _config.Distance;
        _bobberPoints = _config.BobberPoints;
        _useBait = _config.UseBait;
        _baitKey = _config.BaitKey;
        _baitDurationMinutes = _config.BaitDurationMinutes;

        _fishScript.VideoFrameReady += OnVideoFrameReady;
        LogHelper.LogReceived += OnLogReceived;
    }

    private void OnVideoFrameReady(byte[] imageData, (int X, int Y, int Count) center)
    {
        Dispatcher.UIThread.Post(() =>
        {
            var oldFrame = VideoFrame;
            VideoFrame = new Bitmap(new MemoryStream(imageData));
            oldFrame?.Dispose();

            BobberStatus = center.Count >= _config.BobberPoints
                ? $"鱼漂位置: ({center.X}, {center.Y})，点数: {center.Count}"
                : $"未检测到鱼漂，点数: {center.Count}";
        });
    }

    private void OnLogReceived(string message)
    {
        Dispatcher.UIThread.Post(() =>
        {
            Logs.Add(message);
            if (Logs.Count > 500)
            {
                Logs.RemoveAt(0);
            }
        });
    }

    partial void OnDebugChanged(bool value)
    {
        _config.Debug = value;
        _config.Save();
    }

    partial void OnThresholdChanged(double value)
    {
        _config.Threshold = value;
        _config.Save();
    }

    partial void OnDistanceChanged(double value)
    {
        _config.Distance = value;
        _config.Save();
    }

    partial void OnBobberPointsChanged(int value)
    {
        _config.BobberPoints = value;
        _config.Save();
    }

    partial void OnUseBaitChanged(bool value)
    {
        _config.UseBait = value;
        _config.Save();
    }

    partial void OnBaitKeyChanged(string value)
    {
        _config.BaitKey = value;
        _config.Save();
    }

    partial void OnBaitDurationMinutesChanged(int value)
    {
        _config.BaitDurationMinutes = value;
        _config.Save();
    }

    [RelayCommand]
    private async Task StartAsync()
    {
        if (IsRunning) return;

        IsRunning = true;
        _cancellationTokenSource = new CancellationTokenSource();
        try
        {
            await Task.Run(() => _fishScript.StartAsync(_cancellationTokenSource.Token));
        }
        catch (Exception ex)
        {
            LogHelper.Info($"启动异常: {ex.Message}");
        }
        finally
        {
            IsRunning = false;
        }
    }

    [RelayCommand]
    private async Task StopAsync()
    {
        _cancellationTokenSource?.Cancel();
        await _fishScript.StopAsync();
        IsRunning = false;
    }
}
