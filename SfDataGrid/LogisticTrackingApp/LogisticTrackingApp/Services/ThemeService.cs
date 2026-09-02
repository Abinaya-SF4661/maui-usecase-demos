using Syncfusion.Maui.Themes;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LogisticTrackingApp.Services;

/// <summary>
/// Service for managing application-wide theming (Light/Dark mode)
/// </summary>
public class ThemeService : INotifyPropertyChanged
{
    private static ThemeService? _instance;
    private bool _isDarkTheme;

    public static ThemeService Instance => _instance ??= new ThemeService();

    public bool IsDarkTheme
    {
        get => _isDarkTheme;
        set
        {
            if (_isDarkTheme != value)
            {
                _isDarkTheme = value;
                OnPropertyChanged();
                ApplyTheme();
            }
        }
    }

    private ThemeService()
    {
        // Default to light theme
        _isDarkTheme = false;
    }

    private void ApplyTheme()
    {
        // Update resource dictionary based on theme
        var dict = new ResourceDictionary();

        if (_isDarkTheme)
        {
            // Dark theme colors

            dict.Add("ThemePrimaryColor", Color.FromArgb("#D0BCFF"));
            dict.Add("ThemeSecondaryColor", Color.FromArgb("#80F7F1"));
            dict.Add("ThemeTertiaryColor", Color.FromArgb("#FFD580"));
            dict.Add("ThemeErrorColor", Color.FromArgb("#F2B8C6"));
            dict.Add("ThemeBackgroundColor", Color.FromArgb("#121212"));
            dict.Add("ThemeSurfaceColor", Color.FromArgb("#1E1E1E"));
            dict.Add("ThemeHeaderRowColor", Color.FromArgb("#49454F"));
            dict.Add("ThemeOnPrimaryColor", Color.FromArgb("#1F1F1F"));
            dict.Add("ThemeOnSurfaceColor", Color.FromArgb("#E6E1E6"));
            dict.Add("ThemeOnSurfaceVariantColor", Color.FromArgb("#CAC7D0"));

            dict.Add("TransitTemplateBackground", Color.FromArgb("#1E3A5F"));
            dict.Add("TransitTemplateBorder", Color.FromArgb("#2F5EA8"));
            dict.Add("TransitTextColor", Color.FromArgb("#5FA0FF"));

            dict.Add("DeliveredTemplateBackground", Color.FromArgb("#0F4A45"));
            dict.Add("DeliveredTemplateBorder", Color.FromArgb("#1B7A68"));
            dict.Add("DeliveredTextColor", Color.FromArgb("#2ED8A3"));

            dict.Add("DelayedTemplateBackground", Color.FromArgb("#5A2733"));
            dict.Add("DelayedTemplateBorder", Color.FromArgb("#A34457"));
            dict.Add("DelayedTextColor", Color.FromArgb("#FF6B6B"));

            dict.Add("StatusBadgeColor", Color.FromArgb("#2B88FF"));
            dict.Add("StatusBadgeTextColor", Color.FromArgb("#FFFFFF"));

            dict.Add("HeaderLabelTextColor", Color.FromArgb("#94A3B8"));
            dict.Add("ValueLabelTextColor", Color.FromArgb("#E2E8F0"));
        }
        else
        {
            // Light theme colors
            dict.Add("ThemePrimaryColor", Color.FromArgb("#6750A4"));
            dict.Add("ThemeSecondaryColor", Color.FromArgb("#03DAC6"));
            dict.Add("ThemeTertiaryColor", Color.FromArgb("#FFB703"));
            dict.Add("ThemeErrorColor", Color.FromArgb("#CF6679"));
            dict.Add("ThemeBackgroundColor", Color.FromArgb("#FFFBFE"));
            dict.Add("ThemeSurfaceColor", Color.FromArgb("#FFFBFE"));
            dict.Add("ThemeHeaderRowColor", Color.FromArgb("#f0f4f8"));
            dict.Add("ThemeOnPrimaryColor", Color.FromArgb("#FFFFFF"));
            dict.Add("ThemeOnSurfaceColor", Color.FromArgb("#1F1F1F"));
            dict.Add("ThemeOnSurfaceVariantColor", Color.FromArgb("#49454F"));

            dict.Add("TransitTemplateBackground", Color.FromArgb("#DCEBFA"));
            dict.Add("TransitTemplateBorder", Color.FromArgb("#AFCDF0"));
            dict.Add("TransitTextColor", Color.FromArgb("#0A5CC0"));

            dict.Add("DeliveredTemplateBackground", Color.FromArgb("#D8ECE8"));
            dict.Add("DeliveredTemplateBorder", Color.FromArgb("#A8D1CB"));
            dict.Add("DeliveredTextColor", Color.FromArgb("#0B7D73"));

            dict.Add("DelayedTemplateBackground", Color.FromArgb("#F8E5E5"));
            dict.Add("DelayedTemplateBorder", Color.FromArgb("#EDC3C3"));
            dict.Add("DelayedTextColor", Color.FromArgb("#D22D2D"));

            dict.Add("StatusBadgeColor", Color.FromArgb("#005FB8"));
            dict.Add("StatusBadgeTextColor", Color.FromArgb("#FFFFFF"));

            dict.Add("HeaderLabelTextColor", Color.FromArgb("#111827"));
            dict.Add("ValueLabelTextColor", Color.FromArgb("#374151"));

        }
        // Then set MAUI theme
        if (Application.Current != null)
        {
            Application.Current.UserAppTheme = _isDarkTheme ? AppTheme.Dark : AppTheme.Light;
        }

        // Finally, update Syncfusion theme
        try
        {
            ICollection<ResourceDictionary> mergedDictionaries = Application.Current?.Resources.MergedDictionaries;
            var theme = mergedDictionaries?.OfType<SyncfusionThemeResourceDictionary>().FirstOrDefault();
            if (theme != null)
            {
                theme.VisualTheme = _isDarkTheme ? SfVisuals.MaterialDark : SfVisuals.MaterialLight;
            }
        }
        catch (Exception ex)
        {
            // Syncfusion theme update might fail in some scenarios, silently continue
        }
        // Merge with app resources
        foreach (var kvp in dict)
        {
            if (Application.Current?.Resources.ContainsKey(kvp.Key) == true)
            {
                Application.Current.Resources[kvp.Key] = kvp.Value;
            }
            else
            {
                Application.Current?.Resources.Add(kvp.Key, kvp.Value);
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}