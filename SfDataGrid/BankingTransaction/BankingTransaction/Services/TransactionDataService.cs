using System.Globalization;
using System.Text;
using BankingTransaction.Models;

namespace BankingTransaction.Services;

public class TransactionDataService
{
    private const string CsvFileName = "BankingTransaction_Data.csv";

    public async Task<IReadOnlyList<BankTransaction>> LoadTransactionsAsync()
    {
        try
        {
            await using var stream = await OpenDataStreamAsync();
            using var reader = new StreamReader(stream, Encoding.UTF8);

            var header = await reader.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(header))
            {
                return Array.Empty<BankTransaction>();
            }

            var transactions = new List<BankTransaction>();
            string? line;
            while ((line = await reader.ReadLineAsync()) is not null)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                var values = ParseCsvLine(line);
                if (values.Count < 11)
                {
                    continue;
                }

                transactions.Add(new BankTransaction
                {
                    Date = DateTime.TryParse(values[0], CultureInfo.InvariantCulture, DateTimeStyles.None, out var date) ? date : DateTime.MinValue,
                    TransactionId = values[1],
                    Type = values[2],
                    Category = values[3],
                    Amount = decimal.TryParse(values[4], NumberStyles.Any, CultureInfo.InvariantCulture, out var amount) ? amount : 0m,
                    FromAccount = values[5],
                    ToAccount = values[6],
                    Fee = decimal.TryParse(values[7], NumberStyles.Any, CultureInfo.InvariantCulture, out var fee) ? fee : 0m,
                    Notes = values[8],
                    Description = values[9],
                    Status = values[10]
                });
            }

            return transactions.OrderByDescending(t => t.Date).ToList();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"LoadTransactionsAsync failed: {ex}");
            return Array.Empty<BankTransaction>();
        }
    }

    public async Task<Stream> OpenDataStreamAsync()
    {
        var assembly = typeof(TransactionDataService).Assembly;
        var resourceName = assembly.GetManifestResourceNames().FirstOrDefault(name => name.EndsWith(CsvFileName, StringComparison.OrdinalIgnoreCase));

        if (resourceName is not null)
        {
            return assembly.GetManifestResourceStream(resourceName)!;
        }

        var filePath = Path.Combine(FileSystem.AppDataDirectory, CsvFileName);
        if (File.Exists(filePath))
        {
            return File.OpenRead(filePath);
        }

        var sourcePath = Path.Combine(AppContext.BaseDirectory, CsvFileName);
        if (File.Exists(sourcePath))
        {
            return File.OpenRead(sourcePath);
        }

        throw new FileNotFoundException($"Could not locate {CsvFileName}.");
    }

    private static List<string> ParseCsvLine(string line)
    {
        var values = new List<string>();
        var current = new StringBuilder();
        var inQuotes = false;

        for (var i = 0; i < line.Length; i++)
        {
            var c = line[i];
            if (c == '"')
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    current.Append('"');
                    i++;
                }
                else
                {
                    inQuotes = !inQuotes;
                }
            }
            else if (c == ',' && !inQuotes)
            {
                values.Add(current.ToString());
                current.Clear();
            }
            else
            {
                current.Append(c);
            }
        }

        values.Add(current.ToString());
        return values;
    }
}
