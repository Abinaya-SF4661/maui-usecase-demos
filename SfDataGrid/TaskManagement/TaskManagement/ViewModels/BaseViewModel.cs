using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TaskManagement.ViewModels;

public class BaseViewModel : INotifyPropertyChanged
{
    private bool _isBusy;
    private string _title = string.Empty;

    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value);
    }

    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string name = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "", Action? onChanged = null)
    {
        if (EqualityComparer<T>.Default.Equals(backingStore, value))
            return false;

        backingStore = value;
        onChanged?.Invoke();
        OnPropertyChanged(propertyName);
        return true;
    }

    protected async Task HandleException(Exception ex)
    {
        await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
    }
}

// Helper to resolve services
public static class ServiceHelper
{
    public static T? GetService<T>() where T : class
    {
        if (Application.Current?.Handler?.MauiContext?.Services.GetService(typeof(T)) is T service)
            return service;

        return null;
    }
}
