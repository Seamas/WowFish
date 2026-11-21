using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using AutoKey.ViewModels;
using AutoKey.Views;
using HookScript;

namespace AutoKey;

public partial class App : Application
{
    private KeyboardHookHelper? _keyboardHook;
    private MainWindowViewModel? _mainViewModel;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            _mainViewModel = new MainWindowViewModel();
            desktop.MainWindow = new MainWindow
            {
                DataContext = _mainViewModel,
            };

            SetupHotkeys();
            desktop.Exit += (_, _) => _keyboardHook?.Unhook();
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void SetupHotkeys()
    {
        _keyboardHook = new KeyboardHookHelper();
        _keyboardHook.KeyPressed += (_, e) =>
        {
            if (_mainViewModel == null) return;

            if (e.KeyCode == 0x7A) // F11 开始
            {
                Dispatcher.UIThread.Post(async () =>
                {
                    if (_mainViewModel.StartCommand.CanExecute(null))
                    {
                        await _mainViewModel.StartCommand.ExecuteAsync(null);
                    }
                });
            }
            else if (e.KeyCode == 0x7B) // F12 停止
            {
                Dispatcher.UIThread.Post(async () =>
                {
                    if (_mainViewModel.StopCommand.CanExecute(null))
                    {
                        await _mainViewModel.StopCommand.ExecuteAsync(null);
                    }
                });
            }
        };
        _keyboardHook.SetHook();
    }
}
