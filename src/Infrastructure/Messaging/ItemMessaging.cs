using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using Domain;
using Infrastructure.DB.Repository;
using Infrastructure.DB;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Messaging;

public class ItemMessagingConfig
{
    public string? ConnectionString { get; set; }
    public string? QueueName { get; set; }
}


public class ItemMessaging
{
    private readonly IQueueClient _queueClient;
    private readonly ItemMessagingConfig _itemMessagingConfig;
    private readonly ILogger<ItemMessaging> _logger;

    public ItemMessaging(IOptions<ItemMessagingConfig> itemMessagingConfig, ILogger<ItemMessaging> logger)
    {
        _itemMessagingConfig = itemMessagingConfig.Value;
        _queueClient = new QueueClient(_itemMessagingConfig.ConnectionString, _itemMessagingConfig.QueueName);
        _logger = logger;
    }

    public async Task SendMessageAsync(SimplifiedItem item)
    {
        string messageJson = JsonConvert.SerializeObject(item);
        var message = new Message(Encoding.UTF8.GetBytes(messageJson));

        await _queueClient.SendAsync(message);
    }

    public void StartMessageProcessing()
    {
        var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
        {
            MaxConcurrentCalls = 1,
            AutoComplete = false // Set to false to manually complete the message
        };

        _queueClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
    }

    public void StopMessageProcessing()
    {
        // Chame este método para parar o processamento de mensagens
        _queueClient.CloseAsync().Wait();
    }

    private async Task ProcessMessagesAsync(Message message, CancellationToken token)
    {
        // Process the message here
        _logger.LogInformation($"Received message: {Encoding.UTF8.GetString(message.Body)}");

        // Complete the message to remove it from the queue
        await _queueClient.CompleteAsync(message.SystemProperties.LockToken);
    }


    private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
    {
        // Handle exceptions received while processing messages
        _logger.LogError($"Message handler encountered an exception: {exceptionReceivedEventArgs.Exception}");

        return Task.CompletedTask;
    }


}

