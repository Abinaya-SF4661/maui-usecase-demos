namespace BankingTransaction;

public partial class App : Application
{
    private readonly AppShell _shell;

    public App(AppShell shell)
    {
        InitializeComponent();
        System.Diagnostics.Debug.WriteLine(
$"Has PrimaryText = {Resources.ContainsKey("PrimaryText")}");
        _shell = shell;
    }

    protected override Window CreateWindow(IActivationState? activationState)
        => new Window(_shell);
}