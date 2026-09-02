using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace BankingTransaction.Converters;

public class TypeToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        if (value is string type)
        {
            return type switch
            {
                "Deposit" => Colors.Green,
                "Withdrawal" => Colors.Red,
                "Transfer" => new Color(25, 118, 210), // #1976D2
                _ => Colors.Black
            };
        }
        return Colors.Black;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class TypeToBackgroundConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        if (value is string type)
        {
            return type switch
            {
                "Deposit" => new Color(232, 245, 233), // #E8F5E9
                "Withdrawal" => new Color(255, 235, 238), // #FFEBEE
                "Transfer" => new Color(227, 242, 253), // #E3F2FD
                _ => Colors.White
            };
        }
        return Colors.White;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class StatusToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        if (value is string status)
        {
            return status switch
            {
                "Completed" => new Color(46, 125, 50), // #2E7D32
                "Pending" => new Color(245, 127, 23), // #F57F17
                "Failed" => new Color(198, 40, 40), // #C62828
                _ => Colors.Black
            };
        }
        return Colors.Black;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class StatusToBackgroundConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        if (value is string status)
        {
            return status switch
            {
                "Completed" => new Color(200, 230, 201), // #C8E6C9
                "Pending" => new Color(255, 224, 178), // #FFE0B2
                "Failed" => new Color(255, 205, 210), // #FFCDD2
                _ => Colors.White
            };
        }
        return Colors.White;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
