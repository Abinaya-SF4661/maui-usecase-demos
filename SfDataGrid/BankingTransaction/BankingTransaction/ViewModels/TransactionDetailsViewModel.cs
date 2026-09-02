using BankingTransaction.Models;
using BankingTransaction.Services;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace BankingTransaction.ViewModels;

[QueryProperty(nameof(TransactionId), "transactionId")]
public class TransactionDetailsViewModel : BindableObject
{
    private readonly TransactionDataService _dataService;
    private BankTransaction? _selectedTransaction;
    private string? _transactionId;

    public TransactionDetailsViewModel(TransactionDataService dataService)
    {
        _dataService = dataService;
    }

    public string? TransactionId
    {
        get => _transactionId;
        set
        {
            if (SetProperty(ref _transactionId, value))
            {
                _ = LoadTransactionAsync();
            }
        }
    }

    public BankTransaction? SelectedTransaction
    {
        get => _selectedTransaction;
        set => SetProperty(ref _selectedTransaction, value);
    }

    private bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(backingStore, value))
            return false;

        backingStore = value!;
        OnPropertyChanged(propertyName);
        return true;
    }

    private async Task LoadTransactionAsync()
    {
        if (string.IsNullOrWhiteSpace(TransactionId))
        {
            return;
        }

        try
        {
            var transactions = await _dataService.LoadTransactionsAsync();
            SelectedTransaction = transactions.FirstOrDefault(t => string.Equals(t.TransactionId, TransactionId, StringComparison.OrdinalIgnoreCase));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"LoadTransactionAsync failed: {ex}");
            SelectedTransaction = null;
        }
    }
}
