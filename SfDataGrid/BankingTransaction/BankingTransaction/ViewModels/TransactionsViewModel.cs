using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using BankingTransaction.Models;
using BankingTransaction.Services;
using Microsoft.Maui.Controls;

namespace BankingTransaction.ViewModels;

public class TransactionsViewModel : BindableObject
{
    private readonly TransactionDataService _dataService;
    private readonly IServiceProvider _services;
    private List<BankTransaction> _allTransactions = new List<BankTransaction>();
    private BankTransaction? _selectedTransaction;
    private string _searchText = string.Empty;
    private decimal _balance;

    public TransactionsViewModel(TransactionDataService dataService, IServiceProvider services)
    {
        _dataService = dataService;
        _services = services;
        Transactions = new ObservableCollection<BankTransaction>();
        OpenDetailsCommand = new Command<BankTransaction>(transaction => _ = OpenDetailsAsync(transaction));
        LoadCommand = new Command(async () => await InitializeAsync());
    }

    public ObservableCollection<BankTransaction> Transactions { get; }

    public ICommand OpenDetailsCommand { get; }

    public ICommand LoadCommand { get; }

    public BankTransaction? SelectedTransaction
    {
        get => _selectedTransaction;
        set
        {
            if (SetProperty(ref _selectedTransaction, value) && value is not null)
            {
                _ = OpenDetailsAsync(value);
            }
        }
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
            {
                ApplyFilter();
            }
        }
    }

    public decimal Balance
    {
        get => _balance;
        set
        {
            if (SetProperty(ref _balance, value))
            {
                OnPropertyChanged(nameof(BalanceText));
            }
        }
    }

    public string BalanceText => Balance.ToString("C2");

    private bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(backingStore, value))
            return false;

        backingStore = value!;
        OnPropertyChanged(propertyName);
        return true;
    }

    public async Task InitializeAsync()
    {
        if (_allTransactions.Count == 0)
        {
            try
            {
                await LoadAsync();
            }
            catch (FileNotFoundException fnf)
            {
                System.Diagnostics.Debug.WriteLine($"Transactions CSV not found: {fnf.Message}");
                // Leave transactions empty but don't crash the app
                _allTransactions = new List<BankTransaction>();
                Transactions.Clear();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load transactions: {ex}");
                // Swallow to avoid crashing the UI; consider surfacing an error to the user in production
            }
        }
    }

    private async Task LoadAsync()
    {
        try
        {
            var loaded = await _dataService.LoadTransactionsAsync();
            _allTransactions = loaded.ToList();
            Balance = _allTransactions.Sum(t => t.Type.Equals("deposit", StringComparison.OrdinalIgnoreCase) ? t.Amount : -t.Amount);
            ApplyFilter();
        }
        catch (FileNotFoundException)
        {
            // No CSV available; treat as no transactions
            _allTransactions = new List<BankTransaction>();
            Transactions.Clear();
            Balance = 0m;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"LoadAsync failed: {ex}");
            _allTransactions = new List<BankTransaction>();
            Transactions.Clear();
            Balance = 0m;
        }
    }

    private void ApplyFilter()
    {
        var query = SearchText?.Trim();
        IEnumerable<BankTransaction> filtered = _allTransactions;

        if (!string.IsNullOrWhiteSpace(query))
        {
            filtered = filtered.Where(t =>
                t.TransactionId.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                t.Description.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                t.Type.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                t.Status.Contains(query, StringComparison.OrdinalIgnoreCase));
        }

        Transactions.Clear();
        foreach (var transaction in filtered)
        {
            Transactions.Add(transaction);
        }
    }

    private async Task OpenDetailsAsync(BankTransaction? transaction)
    {
        if (transaction is null)
        {
            return;
        }

        try
        {
            // Resolve the details page from DI and navigate using Navigation.PushAsync to avoid Shell route issues.
            var detailsPage = _services.GetService<TransactionDetailsPage>() ?? new TransactionDetailsPage(_services.GetRequiredService<TransactionDetailsViewModel>());
            if (detailsPage.BindingContext is TransactionDetailsViewModel vm)
            {
                vm.TransactionId = transaction.TransactionId;
            }

            var nav = Application.Current?.MainPage?.Navigation;
            if (nav is not null)
            {
                await nav.PushAsync(detailsPage);
            }
            else
            {
                // As fallback, try using Shell if available
                if (Shell.Current is not null)
                {
                    await Shell.Current.GoToAsync($"//details?transactionId={Uri.EscapeDataString(transaction.TransactionId)}");
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"OpenDetailsAsync failed: {ex}");
        }
    }
}
