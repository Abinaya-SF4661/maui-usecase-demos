using System.Globalization;

namespace BankingTransaction.Models;

public class BankTransaction
{
    public DateTime Date { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string FromAccount { get; set; } = string.Empty;
    public string ToAccount { get; set; } = string.Empty;
    public decimal Fee { get; set; }
    public string Notes { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;

    public string FormattedDate => Date.ToString("MMM dd, yyyy", CultureInfo.CurrentCulture);
    public string AmountDisplay => Amount < 0 ? $"{Math.Abs(Amount):C2}" : $"{Amount:C2}";
    public string FeeDisplay => Fee.ToString("C2");
    public bool IsCredit => string.Equals(Type, "deposit", StringComparison.OrdinalIgnoreCase);
    public bool IsPending => string.Equals(Status, "pending", StringComparison.OrdinalIgnoreCase);
}
