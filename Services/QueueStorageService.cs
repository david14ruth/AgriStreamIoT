
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using System.Text.Json;
using AgriStreamIoT.Models;

namespace AgriStreamIoT.Services
{
    public class QueueStorageService
    {
        private readonly QueueClient _queueClient;

        public QueueStorageService(string connectionString)
        {
            _queueClient = new QueueClient(
                connectionString,
                "irrigation-commands");
        }

        public async Task InitializeAsync()
        {
            await _queueClient.CreateIfNotExistsAsync();
        }

        // Send irrigation command to Azure Queue Storage
        public async Task SendCommandAsync(IrrigationCommand command)
        {
            string message = JsonSerializer.Serialize(command);

            await _queueClient.SendMessageAsync(message);
        }

        // Retrieve pending irrigation commands
        public async Task<QueueMessage[]> GetPendingCommandsAsync()
        {
            var response = await _queueClient.ReceiveMessagesAsync(
                maxMessages: 10);

            return response.Value;
        }

        // Delete a processed irrigation command
        public async Task DeleteCommandAsync(QueueMessage message)
        {
            await _queueClient.DeleteMessageAsync(
                message.MessageId,
                message.PopReceipt);
        }

        // Get the number of messages in the queue
        public async Task<int> GetQueueMessageCountAsync()
        {
            var properties = await _queueClient.GetPropertiesAsync();

            return properties.Value.ApproximateMessagesCount;
        }
    }
}
