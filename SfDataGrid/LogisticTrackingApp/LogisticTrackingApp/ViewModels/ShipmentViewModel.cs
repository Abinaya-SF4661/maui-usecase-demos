using LogisticTrackingApp.Services;
using Syncfusion.Maui.ProgressBar;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace LogisticTrackingApp
{
    /// <summary>
    /// ViewModel for managing shipment data and operations.
    /// Implements MVVM pattern with INotifyPropertyChanged.
    /// Follows the same pattern as EmployeeViewModel.
    /// Implements singleton pattern to share state across pages.
    /// </summary>
    public class ShipmentViewModel : INotifyPropertyChanged
    {
        private static ShipmentViewModel? _instance;
        private readonly ShipmentService _shipmentService;
        private ObservableCollection<Shipment> _shipments;
        private ObservableCollection<Shipment> _filteredShipments;
        private string _searchText = string.Empty;
        private Shipment? _selectedShipment;
        private bool _isLoading = false;
        private string _selectedStatusFilter = "All";
        private ObservableCollection<string> _statusFilters;
        private ObservableCollection<StepProgressBarItem> stepProgressItem = new ObservableCollection<StepProgressBarItem>();
        public ObservableCollection<StepProgressBarItem> StepProgressItem
        {
            get
            {
                return stepProgressItem;
            }
            set
            {
                stepProgressItem = value;
            }
        }

        public ObservableCollection<Shipment> Shipments
        {
            get => _shipments;
            set
            {
                if (_shipments != value)
                {
                    _shipments = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<Shipment> FilteredShipments
        {
            get => _filteredShipments;
            set
            {
                if (_filteredShipments != value)
                {
                    _filteredShipments = value;
                    OnPropertyChanged();
                }
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged();
                    ApplyFilters();
                }
            }
        }

        public string SelectedStatusFilter
        {
            get => _selectedStatusFilter;
            set
            {
                if (_selectedStatusFilter != value)
                {
                    _selectedStatusFilter = value;
                    OnPropertyChanged();
                    ApplyFilters();
                }
            }
        }

        public ObservableCollection<string> StatusFilters
        {
            get => _statusFilters;
            set
            {
                if (_statusFilters != value)
                {
                    _statusFilters = value;
                    OnPropertyChanged();
                }
            }
        }

        public Shipment? SelectedShipment
        {
            get => _selectedShipment;
            set
            {
                if (_selectedShipment != value)
                {
                    _selectedShipment = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand LoadShipmentsCommand { get; }
        public ICommand RefreshCommand { get; }

        /// <summary>
        /// Gets the singleton instance of ShipmentViewModel
        /// </summary>
        public static ShipmentViewModel Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ShipmentViewModel();
                }
                return _instance;
            }
        }

        public ShipmentViewModel()
        {
            _shipmentService = new ShipmentService();
            _shipments = new ObservableCollection<Shipment>();
            _filteredShipments = new ObservableCollection<Shipment>();
            _statusFilters = new ObservableCollection<string> { "All", "In Transit", "Delivered", "Delayed" };

            LoadShipmentsCommand = new Command(async () => await LoadShipments());
            RefreshCommand = new Command(async () => await LoadShipments());
        }

        /// <summary>
        /// Loads shipments from the service.
        /// </summary>
        public async Task LoadShipments()
        {
            try
            {
                IsLoading = true;
                var shipments = await _shipmentService.GetShipmentsAsync();

                Shipments.Clear();
                FilteredShipments.Clear();

                foreach (var shipment in shipments)
                {
                    Shipments.Add(shipment);
                }

                // Display all shipments initially
                ApplyFilters();

                Debug.WriteLine($"Loaded {Shipments.Count} shipments into ViewModel");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in ViewModel: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        internal void ProgressbarItems()
        {
            if (SelectedShipment == null)
                return;

            FormattedString primaryFormattedText1 = new FormattedString();
            primaryFormattedText1.Spans.Add(new Span { Text = "Origin", FontSize = 12 });
            primaryFormattedText1.Spans.Add(new Span { Text = "\n"+ SelectedShipment.Origin, FontSize = 14 });

            FormattedString primaryFormattedText2 = new FormattedString();
            primaryFormattedText2.Spans.Add(new Span { Text = "Current location", FontSize = 12 });
            primaryFormattedText2.Spans.Add(new Span { Text = "\n" + SelectedShipment.CurrentLocation, FontSize = 14 });

            FormattedString primaryFormattedText3 = new FormattedString();
            primaryFormattedText3.Spans.Add(new Span { Text = "Destination", FontSize = 12 });
            primaryFormattedText3.Spans.Add(new Span { Text = "\n" + SelectedShipment.Destination, FontSize = 14 });

            stepProgressItem = new ObservableCollection<StepProgressBarItem>();
            stepProgressItem.Add(new StepProgressBarItem() { PrimaryFormattedText = primaryFormattedText1 });
            stepProgressItem.Add(new StepProgressBarItem() { PrimaryFormattedText = primaryFormattedText2 });
            stepProgressItem.Add(new StepProgressBarItem() { PrimaryFormattedText = primaryFormattedText3 });
        }

        /// <summary>
        /// Applies search and status filters to the shipment list.
        /// </summary>
        private void ApplyFilters()
        {
            FilteredShipments.Clear();

            var filtered = Shipments.AsEnumerable();

            // Apply status filter
            if (SelectedStatusFilter != "All")
            {
                filtered = filtered.Where(s => s.Status == SelectedStatusFilter);
            }

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var searchLower = SearchText.ToLower();
                filtered = filtered.Where(s =>
                    s.TrackingId.ToLower().Contains(searchLower) ||
                    s.CustomerName.ToLower().Contains(searchLower) ||
                    s.Origin.ToLower().Contains(searchLower) ||
                    s.Destination.ToLower().Contains(searchLower)
                );
            }

            // Add filtered results to the collection
            foreach (var shipment in filtered)
            {
                FilteredShipments.Add(shipment);
            }

            Debug.WriteLine($"Filtered to {FilteredShipments.Count} shipments");
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
