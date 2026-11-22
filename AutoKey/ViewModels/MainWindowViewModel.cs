using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using AutoKey.Models;
using AutoKey.Services;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LogDll;

namespace AutoKey.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly KeyPresserScript _script = new();
    private readonly AutoKeyConfig _config = AutoKeyConfig.Instance;

    [ObservableProperty]
    private string _key = "1";

    [ObservableProperty]
    private int _intervalMilliseconds = 5000;

    [ObservableProperty]
    private int _repeatCount = 10;

    [ObservableProperty]
    private bool _simulateActivation = true;

    [ObservableProperty]
    private bool _isRunning;

    public ObservableCollection<string> Logs { get; } = new();

    public MainWindowViewModel()
    {
        _key = _config.Key;
        _intervalMilliseconds = _config.IntervalMilliseconds;
        _repeatCount = _config.RepeatCount;
        _simulateActivation = _config.SimulateActivation;
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

    partial void OnKeyChanged(string value)
    {
        _config.Key = value;
        _config.Save();
    }

    partial void OnIntervalMillisecondsChanged(int value)
    {
        _config.IntervalMilliseconds = value;
        _config.Save();
    }

    partial void OnRepeatCountChanged(int value)
    {
        _config.RepeatCount = value;
        _config.Save();
    }

    partial void OnSimulateActivationChanged(bool value)
    {
        _config.SimulateActivation = value;
        _config.Save();
    }

    [RelayCommand]
    private async Task StartAsync()
    {
        if (IsRunning) return;

        IsRunning = true;
        try
        {
            await Task.Run(() => _script.StartAsync());
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
        await _script.StopAsync();
        IsRunning = false;
    }
}
