using System.Diagnostics;
using Microsoft.Maui.Controls;

namespace LogisticTrackingApp.Views;

public partial class ShipmentDetails : ContentPage
{
    public ShipmentDetails()
	{
		InitializeComponent();
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		BindingContext = ShipmentViewModel.Instance;

    }

    private void progressBar_StepStatusChanged(object sender, Syncfusion.Maui.ProgressBar.StepStatusChangedEventArgs e)
    {
        var viewmodel = ShipmentViewModel.Instance;
        if (viewmodel != null && viewmodel.SelectedShipment != null)
        {
			if (viewmodel.SelectedShipment.Status == "In Transit")
			{
				progressBar.ActiveStepIndex = 2;
				progressBar.ActiveStepProgressValue = 60;
			}
			else if (viewmodel.SelectedShipment.Status == "Delayed")
			{
				progressBar.ActiveStepIndex = 1;
				progressBar.ActiveStepProgressValue = 30;
			}
			else if (viewmodel.SelectedShipment.Status == "Delivered")
			{
				progressBar.ActiveStepIndex = 3;
			}
		}
    }
}