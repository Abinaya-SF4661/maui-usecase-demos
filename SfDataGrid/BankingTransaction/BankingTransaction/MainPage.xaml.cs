using BankingTransaction.Models;
using BankingTransaction.ViewModels;
using Syncfusion.Maui.DataGrid;
using Syncfusion.Maui.DataGrid.Exporting;
using Syncfusion.Pdf;

namespace BankingTransaction;

public partial class MainPage : ContentPage
{
    public MainPage(TransactionsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        Loaded += async (_, _) => await viewModel.InitializeAsync();
    }

    private void OnSelectionChanged(object? sender, DataGridSelectionChangedEventArgs e)
    {
        var vm = (sender as SfDataGrid)?.BindingContext as TransactionsViewModel;
        if (vm == null)
            return;

        if (e.AddedRows.FirstOrDefault() is BankTransaction transaction)
        {
            vm.SelectedTransaction = transaction;
        }
    }

    private void DataGrid_QueryRowHeight(object sender, DataGridQueryRowHeightEventArgs e)
    {
        if (e.RowIndex != 0)
        {
            //Calculates and sets the height of the row based on its content.
            e.Height = e.GetIntrinsicRowHeight(e.RowIndex);
            e.Handled = true;
        }
    }

    private void ExportButton_Clicked(object sender, EventArgs e)
    {
        MemoryStream stream = new MemoryStream();
        DataGridPdfExportingController pdfExport = new DataGridPdfExportingController();
        DataGridPdfExportingOption option = new DataGridPdfExportingOption();
        var pdfDoc = new PdfDocument();
        pdfDoc = pdfExport.ExportToPdf(this.TransactionsGrid, option);
        pdfDoc.Save(stream);
        pdfDoc.Close(true);
        SaveService saveService = new();
        saveService.SaveAndView("ExportFeature.pdf", "application/pdf", stream);
    }
}
