
using Azure;
using Azure.Data.Tables;
using AgriStreamIoT.Models;

namespace AgriStreamIoT.Services
{
    public class TableStorageService
    {
        private readonly TableClient _telemetryTable;
        private readonly TableClient _irrigationAuditTable;

        public TableStorageService(string connectionString)
        {
            var serviceClient = new TableServiceClient(connectionString);

            _telemetryTable = serviceClient.GetTableClient("AgriTelemetry");
            _irrigationAuditTable = serviceClient.GetTableClient("IrrigationAudit");

            _telemetryTable.CreateIfNotExists();
            _irrigationAuditTable.CreateIfNotExists();
        }

        // Save sensor reading

        public async Task SaveSensorReadingAsync(SensorReading reading)
        {
            var entity = new TableEntity(
                "Sensor",
                Guid.NewGuid().ToString())
            {
                ["DeviceId"] = reading.DeviceId,
                ["Temperature"] = reading.Temperature,
                ["Humidity"] = reading.Humidity,
                ["SoilMoisture"] = reading.SoilMoisture,
                ["RecordedAt"] = reading.RecordedAt
            };

            await _telemetryTable.AddEntityAsync(entity);
        }


        // Retrieve all sensor readings
        public async Task<List<TableEntity>> GetSensorReadingsAsync()
        {
            var readings = new List<TableEntity>();

            await foreach (var entity in _telemetryTable.QueryAsync<TableEntity>())
            {
                readings.Add(entity);
            }

            return readings;
        }

        // Save irrigation command audit
        public async Task SaveIrrigationAuditAsync(IrrigationCommand command)
        {
            var entity = new TableEntity(
                "Irrigation",
                Guid.NewGuid().ToString())
            {
                ["DeviceId"] = command.DeviceId,
                ["Command"] = command.Command,
                ["Status"] = command.Status,
                ["RecordedAt"] = DateTime.UtcNow
            };

            await _irrigationAuditTable.AddEntityAsync(entity);
        }

        // Retrieve irrigation audit records
        public async Task<List<TableEntity>> GetIrrigationAuditsAsync()
        {
            var audits = new List<TableEntity>();

            await foreach (var entity in _irrigationAuditTable.QueryAsync<TableEntity>())
            {
                audits.Add(entity);
            }

            return audits;
        }
    }
}
