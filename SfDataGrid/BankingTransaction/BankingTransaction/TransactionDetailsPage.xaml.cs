using BankingTransaction.ViewModels;

namespace BankingTransaction;

public partial class TransactionDetailsPage : ContentPage
{
    public TransactionDetailsPage(TransactionDetailsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
