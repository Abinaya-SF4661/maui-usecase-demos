using System.Diagnostics;
using Syncfusion.Maui.Core;
using Syncfusion.Maui.DataGrid;

namespace LogisticTrackingApp.Views;

public partial class ShipmentTracking : ContentPage
{
    private ShipmentViewModel viewModel;
	
    public ShipmentTracking()
	{
		InitializeComponent();
        viewModel = new ShipmentViewModel();
        BindingContext = viewModel;
        viewModel.LoadShipmentsCommand.Execute(null);

        // set default selection in chip
        statusChipGroup.SelectedItem = statusChipGroup?.Items?.First();
    }


    private void ShipmentsDataGrid_SelectionChanged(object sender, DataGridSelectionChangedEventArgs e)
    {
        if (sender is SfDataGrid dataGrid && dataGrid.CurrentRow is Shipment selectedShipment)
        {
            var selectedItem = ShipmentViewModel.Instance;
            selectedItem.SelectedShipment = selectedShipment;
            selectedItem.ProgressbarItems();
            Navigation.PushAsync(new ShipmentDetails());
        }
    }

    /// <summary>
    /// Handle chip selection for status filtering
    /// </summary>
    private void SfChipGroup_SelectionChanged(object sender, Syncfusion.Maui.Core.Chips.SelectionChangedEventArgs e)
    {
        if (sender is SfChipGroup chipGroup && e.AddedItem is SfChip selectedChip)
        {
            // Get the text from the selected chip
            string selectedStatus = selectedChip.Text;
            
            viewModel.SelectedStatusFilter = selectedStatus;
        }
    }
}