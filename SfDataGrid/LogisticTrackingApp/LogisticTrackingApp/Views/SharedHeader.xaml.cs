using System.Diagnostics;
using LogisticTrackingApp.Services;

namespace LogisticTrackingApp.Views;

public partial class SharedHeader : ContentView
{
    public static readonly BindableProperty TitleProperty = 
        BindableProperty.Create(nameof(Title), typeof(string), typeof(SharedHeader), string.Empty);

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public SharedHeader()
    {
        InitializeComponent();
    }

    private void OnThemeToggleClicked(object sender, EventArgs e)
    {
        var themeService = ThemeService.Instance;
        themeService.IsDarkTheme = !themeService.IsDarkTheme;
        UpdateThemeButtonIcon();
    }

    private void UpdateThemeButtonIcon()
    {
        if (ThemeToggleButton != null)
        {
            ThemeToggleButton.Text = ThemeService.Instance.IsDarkTheme ? "☀️" : "🌙";
        }
    }
}
