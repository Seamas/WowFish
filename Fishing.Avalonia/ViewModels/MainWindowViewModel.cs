using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
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
    private bool _isRunning;

    public ObservableCollection<string> Logs { get; } = new();

    public MainWindowViewModel()
    {
        _debug = _config.Debug;
        _threshold = _config.Threshold;
        _distance = _config.Distance;
        _bobberPoints = _config.BobberPoints;

        LogHelper.LogReceived += OnLogReceived;
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
