using System.Text.Json.Serialization;

namespace LogisticTrackingApp
{
    /// <summary>
    /// Represents a shipment/tracking record in the logistics system.
    /// Uses [JsonPropertyName] for proper JSON deserialization from LogisticsData.json.
    /// </summary>
    public class Shipment
    {
        [JsonPropertyName("trackingId")]
        public string TrackingId { get; set; } = string.Empty;

        [JsonPropertyName("customerName")]
        public string CustomerName { get; set; } = string.Empty;

        [JsonPropertyName("origin")]
        public string Origin { get; set; } = string.Empty;

        [JsonPropertyName("destination")]
        public string Destination { get; set; } = string.Empty;

        [JsonPropertyName("currentLocation")]
        public string CurrentLocation { get; set; } = string.Empty;

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("eta")]
        public string Eta { get; set; } = string.Empty;

        [JsonPropertyName("deliveryWindow")]
        public string DeliveryWindow { get; set; } = string.Empty;

        [JsonPropertyName("weightKg")]
        public double WeightKg { get; set; }

        [JsonPropertyName("serviceType")]
        public string ServiceType { get; set; } = string.Empty;

        [JsonPropertyName("vehicleId")]
        public string VehicleId { get; set; } = string.Empty;

        [JsonPropertyName("driverName")]
        public string DriverName { get; set; } = string.Empty;

        [JsonPropertyName("priority")]
        public string Priority { get; set; } = string.Empty;

        [JsonPropertyName("lastUpdated")]
        public string LastUpdated { get; set; } = string.Empty;

        [JsonPropertyName("arrivalTime")]
        public string ArrivalTime { get; set; } = string.Empty;

        [JsonPropertyName("notes")]
        public string Notes { get; set; } = string.Empty;

        [JsonPropertyName("progress")]
        public int Progress { get; set; }

        [JsonPropertyName("shipmentValue")]
        public double ShipmentValue { get; set; }

        [JsonPropertyName("category")]
        public string Category { get; set; } = string.Empty;

        [JsonPropertyName("isFragile")]
        public bool IsFragile { get; set; }

        [JsonPropertyName("requiresSignature")]
        public bool RequiresSignature { get; set; }

        /// <summary>
        /// Gets the status badge color based on the current status.
        /// </summary>
        public Color GetStatusColor()
        {
            return Status switch
            {
                "In Transit" => Color.FromArgb("#0096D6"),  // Blue
                "Delivered" => Color.FromArgb("#4CAF50"),   // Green
                "Delayed" => Color.FromArgb("#FF6B6B"),     // Red
                _ => Color.FromArgb("#757575")              // Gray
            };
        }

        /// <summary>
        /// Gets the priority badge color based on priority level.
        /// </summary>
        public Color GetPriorityColor()
        {
            return Priority switch
            {
                "High" => Color.FromArgb("#FF6B6B"),        // Red
                "Medium" => Color.FromArgb("#FFC107"),      // Orange
                "Low" => Color.FromArgb("#4CAF50"),         // Green
                _ => Color.FromArgb("#757575")              // Gray
            };
        }
    }
}
