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
    private CancellationTokenSource? _cancellationTokenSource;

    [ObservableProperty]
    private bool _debug = FishConfig.Debug;

    [ObservableProperty]
    private double _threshold = FishConfig.Threshold;

    [ObservableProperty]
    private double _distance = FishConfig.Distance;

    [ObservableProperty]
    private int _bobberPoints = FishConfig.BobberPoints;

    [ObservableProperty]
    private bool _isRunning;

    public ObservableCollection<string> Logs { get; } = new();

    public MainWindowViewModel()
    {
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

    partial void OnDebugChanged(bool value) => FishConfig.Debug = value;
    partial void OnThresholdChanged(double value) => FishConfig.Threshold = value;
    partial void OnDistanceChanged(double value) => FishConfig.Distance = value;
    partial void OnBobberPointsChanged(int value) => FishConfig.BobberPoints = value;

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
