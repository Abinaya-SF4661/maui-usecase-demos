using Microsoft.Extensions.DependencyInjection;
using LogisticTrackingApp.Services;
using LogisticTrackingApp.Views;

namespace LogisticTrackingApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            InitializeTheme();
        }
        private void InitializeTheme()
        {
            // Initialize theme colors in app resources
            var themeService = ThemeService.Instance;

            // Add theme color keys to app resources
            Resources.Add("ThemePrimaryColor", Color.FromArgb("#6750A4"));
            Resources.Add("ThemeSecondaryColor", Color.FromArgb("#03DAC6"));
            Resources.Add("ThemeTertiaryColor", Color.FromArgb("#FFB703"));
            Resources.Add("ThemeErrorColor", Color.FromArgb("#CF6679"));
            Resources.Add("ThemeBackgroundColor", Color.FromArgb("#FFFBFE"));
            Resources.Add("ThemeSurfaceColor", Color.FromArgb("#FFFBFE"));
            Resources.Add("ThemeHeaderRowColor", Color.FromArgb("#f0f4f8"));
            Resources.Add("ThemeOnPrimaryColor", Color.FromArgb("#FFFFFF"));
            Resources.Add("ThemeOnSurfaceColor", Color.FromArgb("#1F1F1F"));
            Resources.Add("ThemeOnSurfaceVariantColor", Color.FromArgb("#49454F"));
           
            Resources.Add("TransitTemplateBackground", Color.FromArgb("#DCEBFA"));
            Resources.Add("TransitTemplateBorder", Color.FromArgb("#AFCDF0"));
            Resources.Add("TransitTextColor", Color.FromArgb("#0A5CC0"));

            Resources.Add("DeliveredTemplateBackground", Color.FromArgb("#D8ECE8"));
            Resources.Add("DeliveredTemplateBorder", Color.FromArgb("#A8D1CB"));
            Resources.Add("DeliveredTextColor", Color.FromArgb("#0B7D73"));

            Resources.Add("DelayedTemplateBackground", Color.FromArgb("#F8E5E5"));
            Resources.Add("DelayedTemplateBorder", Color.FromArgb("#EDC3C3"));
            Resources.Add("DelayedTextColor", Color.FromArgb("#D22D2D"));

            Resources.Add("StatusBadgeColor", Color.FromArgb("#005FB8"));
            Resources.Add("StatusBadgeTextColor", Color.FromArgb("#FFFFFF"));

            Resources.Add("HeaderLabelTextColor", Color.FromArgb("#111827"));
            Resources.Add("ValueLabelTextColor", Color.FromArgb("#374151"));
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}