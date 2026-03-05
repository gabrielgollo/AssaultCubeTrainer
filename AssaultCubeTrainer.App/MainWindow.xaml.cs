using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AssaultCubeTrainer.App.Utils;
using AssaultCubeTrainer.App.ViewModels;

namespace AssaultCubeTrainer.App;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private GlobalKeyboardHook? _keyboardHook;

    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
        Loaded += OnLoaded;
        Closed += OnClosed;
    }

    private void Minimize_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void Maximize_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState == WindowState.Maximized
            ? WindowState.Normal
            : WindowState.Maximized;
    }

    private void Close_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _keyboardHook = new GlobalKeyboardHook();
        _keyboardHook.KeyboardPressed += OnKeyboardPressed;
    }

    private void OnClosed(object? sender, EventArgs e)
    {
        if (DataContext is MainViewModel viewModel)
        {
            viewModel.Shutdown();
        }

        if (_keyboardHook != null)
        {
            _keyboardHook.KeyboardPressed -= OnKeyboardPressed;
            _keyboardHook.Dispose();
            _keyboardHook = null;
        }
    }

    private void OnKeyboardPressed(object? sender, GlobalKeyboardHookEventArgs e)
    {
        if (e.KeyboardState != GlobalKeyboardHook.KeyboardState.KeyDown)
        {
            return;
        }

        bool handled = false;

        Dispatcher.Invoke(() =>
        {
            if (DataContext is not MainViewModel viewModel)
            {
                return;
            }

            switch (e.KeyboardData.VirtualCode)
            {
                case 45: // INS
                    if (viewModel.AttachCommand.CanExecute(null))
                    {
                        viewModel.AttachCommand.Execute(null);
                        handled = true;
                    }
                    break;
                case 112: // F1
                    viewModel.EspEnabled = !viewModel.EspEnabled;
                    handled = true;
                    break;
                case 113: // F2
                    viewModel.AimbotEnabled = !viewModel.AimbotEnabled;
                    handled = true;
                    break;
                case 114: // F3
                    viewModel.KeepAttributes = !viewModel.KeepAttributes;
                    handled = true;
                    break;
            }
        });

        e.Handled = handled;
    }

    private void NumericInput_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        e.Handled = !IsNumericText(e.Text);
    }

    private void NumericInput_OnPaste(object sender, DataObjectPastingEventArgs e)
    {
        if (e.DataObject.GetDataPresent(typeof(string)))
        {
            string text = (string)e.DataObject.GetData(typeof(string))!;
            if (!IsNumericText(text))
            {
                e.CancelCommand();
            }
        }
        else
        {
            e.CancelCommand();
        }
    }

    private void NumericInput_LostFocus(object sender, RoutedEventArgs e)
    {
        if (sender is TextBox textBox && string.IsNullOrWhiteSpace(textBox.Text))
        {
            textBox.Text = "0";
        }
    }

    private bool IsNumericText(string text)
    {
        foreach (char ch in text)
        {
            if (!char.IsDigit(ch))
            {
                return false;
            }
        }

        return true;
    }
}
