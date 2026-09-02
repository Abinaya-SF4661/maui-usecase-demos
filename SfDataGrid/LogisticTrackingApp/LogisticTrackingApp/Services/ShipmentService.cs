using System.Diagnostics;
using System.Text.Json;

namespace LogisticTrackingApp.Services
{
    /// <summary>
    /// Service responsible for loading and managing shipment/tracking data.
    /// Follows the same pattern as EmployeeService for data loading from JSON.
    /// </summary>
    public class ShipmentService
    {
        /// <summary>
        /// Loads shipment data from the JSON file in Resources/Raw.
        /// Uses JsonPropertyName attributes for proper deserialization mapping.
        /// </summary>
        public async Task<List<Shipment>> GetShipmentsAsync()
        {
            try
            {
                // Access the JSON file from the app package resources
                using var stream = await FileSystem.OpenAppPackageFileAsync("LogisticsData.json");
                using var reader = new StreamReader(stream);
                var json = await reader.ReadToEndAsync();

                // Configure JSON serializer options for proper deserialization
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    WriteIndented = true
                };

                // Deserialize JSON to List<Shipment> using JsonPropertyName mappings
                var shipments = JsonSerializer.Deserialize<List<Shipment>>(json, options);

                if (shipments == null)
                {
                    Debug.WriteLine("Warning: Shipment list was null after deserialization");
                    return new List<Shipment>();
                }

                Debug.WriteLine($"Successfully loaded {shipments.Count} shipments from JSON");
                return shipments;
            }
            catch (JsonException jsonEx)
            {
                Debug.WriteLine($"JSON deserialization error: {jsonEx.Message}");
                Debug.WriteLine($"Line: {jsonEx.LineNumber}, Position: {jsonEx.BytePositionInLine}");
                return new List<Shipment>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading shipments: {ex.GetType().Name} - {ex.Message}");
                return new List<Shipment>();
            }
        }
    }
}
