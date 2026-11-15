using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Fishing.Avalonia.ViewModels;
using Fishing.Avalonia.Views;
using HookScript;

namespace Fishing.Avalonia;

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
            DisableAvaloniaDataAnnotationValidation();
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

            if (e.KeyCode == 0x7A) // F11
            {
                Dispatcher.UIThread.Post(async () =>
                {
                    if (_mainViewModel.StartCommand.CanExecute(null))
                    {
                        await _mainViewModel.StartCommand.ExecuteAsync(null);
                    }
                });
            }
            else if (e.KeyCode == 0x7B) // F12
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

    private void DisableAvaloniaDataAnnotationValidation()
    {
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}
