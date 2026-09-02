using System.Globalization;

namespace LogisticTrackingApp.Converters
{
    public class StatusTemplateSelector : DataTemplateSelector
    {
        public DataTemplate InTransit { get; set; }

        public DataTemplate Delivery { get; set; }

        public DataTemplate Delayed { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var value = (item as Shipment)?.Status;
            if ( value == "In Transit")
                return InTransit;
            else if (value == "Delivered")
                return Delivery;
            else
                return Delayed;
        }
    }
}
