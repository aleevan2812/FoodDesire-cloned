﻿using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Windows.System;

namespace FoodDesire.IMS.Views;
// TODO: Update NavigationViewItem titles and icons in ShellPage.xaml.
public sealed partial class ShellPage : Page {
    public ShellViewModel ViewModel { get; }

    public ShellPage(ShellViewModel viewModel) {
        ViewModel = viewModel;
        InitializeComponent();

        // TODO: Set the title bar icon by updating /Assets/WindowIcon.ico.
        // A custom title bar is required for full window theme and Mica support.
        // https://docs.microsoft.com/windows/apps/develop/title-bar?tabs=winui3#full-customization
        App.MainWindow.ExtendsContentIntoTitleBar = true;
        App.MainWindow.SetTitleBar(AppTitleBar);
        App.MainWindow.Activated += MainWindow_Activated;
        AppTitleBarText.Text = "AppDisplayName".GetLocalized();
        Loaded += HomePage_Loaded;
    }

    private async void HomePage_Loaded(object sender, RoutedEventArgs e) {
        await ViewModel.GetUser();
        ContentDialog dialog = new ContentDialog() {
            XamlRoot = XamlRoot,
            Style = (Style)Application.Current.Resources["DefaultContentDialogStyle"],
            PrimaryButtonText = "Sign in",
            DefaultButton = ContentDialogButton.Primary,
            Title = $"Sign in to {"AppDisplayName".GetLocalized()}",
            Content = "To Continue, Please Sign in",
        };

        while (App.CurrentUserAccount == null) {
            await dialog.ShowAsync();
            try {
                await ViewModel.AuthenticateUser(App.Configuration["ClientId"]!);
            } catch (HttpRequestException) {
                dialog.Content = "No Internet Connection";
                dialog.PrimaryButtonText = "Try Again";
            } catch (Exception ex) {
                dialog.Content = ex.Message;
                dialog.PrimaryButtonText = "Sign in";
            }
        }
        ContentGrid.IsHitTestVisible = true;
        ViewModel.NavigationService.Frame = NavigationFrame;
        ViewModel.NavigationViewService.Initialize(NavigationViewControl);
        ViewModel.NavigationService.NavigateTo("FoodDesire.IMS.ViewModels.HomeViewModel");
    }

    private void OnLoaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) {
        KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.Left, VirtualKeyModifiers.Menu));
        KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.GoBack));
    }

    private void MainWindow_Activated(object sender, WindowActivatedEventArgs args) {
        var resource = args.WindowActivationState == WindowActivationState.Deactivated ? "WindowCaptionForegroundDisabled" : "WindowCaptionForeground";

        AppTitleBarText.Foreground = (SolidColorBrush)App.Current.Resources[resource];
    }

    private void NavigationViewControl_DisplayModeChanged(NavigationView sender, NavigationViewDisplayModeChangedEventArgs args) {
        AppTitleBar.Margin = new Thickness() {
            Left = sender.CompactPaneLength * (sender.DisplayMode == NavigationViewDisplayMode.Minimal ? 2 : 1),
            Top = AppTitleBar.Margin.Top,
            Right = AppTitleBar.Margin.Right,
            Bottom = AppTitleBar.Margin.Bottom
        };
    }

    private static KeyboardAccelerator BuildKeyboardAccelerator(VirtualKey key, VirtualKeyModifiers? modifiers = null) {
        var keyboardAccelerator = new KeyboardAccelerator() { Key = key };

        if (modifiers.HasValue) {
            keyboardAccelerator.Modifiers = modifiers.Value;
        }

        keyboardAccelerator.Invoked += OnKeyboardAcceleratorInvoked;

        return keyboardAccelerator;
    }

    private static void OnKeyboardAcceleratorInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args) {
        var navigationService = App.GetService<INavigationService>();

        var result = navigationService.GoBack();

        args.Handled = result;
    }
}
