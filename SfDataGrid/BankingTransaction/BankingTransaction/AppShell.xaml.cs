namespace BankingTransaction;

public partial class AppShell : Shell
{
    public AppShell(IServiceProvider services)
    {
        InitializeComponent();

        var page = services.GetRequiredService<MainPage>();

        Items.Clear();

        Items.Add(new ShellContent
        {
            Content = page,
            Route = "main",
            Title = "Transactions"
        });
    }
}
