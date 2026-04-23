using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Confluent.Kafka;
using Confluent.Kafka.Admin;

namespace EPedidos.Consumer;

public sealed class OrderConsumerWorker : BackgroundService
{
    private readonly IConsumer<string, string> _consumer;
    private readonly IOrderConsumerService _orderService;
    private readonly ILogger<OrderConsumerWorker> _logger;
    private readonly string _topic;

    public OrderConsumerWorker(
        IConsumer<string, string> consumer,
        IOrderConsumerService orderService,
        ILogger<OrderConsumerWorker> logger)
    {
        _consumer = consumer;
        _orderService = orderService;
        _logger = logger;
        _topic = "orders";
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Consumer worker starting...");

        // Aguardar tópico estar disponível
        await WaitForTopicAsync(stoppingToken);

        _consumer.Subscribe(_topic);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = _consumer.Consume(stoppingToken);

                    if (result == null) continue;

                    _logger.LogInformation("Received message: {Message}", result.Message.Value);
                    await _orderService.ProcessOrderAsync(result.Message.Value, stoppingToken);
                }
                catch (ConsumeException ex)
                {
                    if (ex.Error.Code == ErrorCode.UnknownTopic || ex.Error.Reason.Contains("Unknown topic"))
                    {
                        _logger.LogWarning("Topic not available, waiting... Error: {Error}", ex.Error.Reason);
                        await Task.Delay(5000, stoppingToken);
                    }
                    else
                    {
                        _logger.LogError(ex, "Consume error");
                        throw;
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Consumer worker stopped.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in consumer worker");
            throw;
        }
        finally
        {
            _consumer.Close();
            _consumer.Dispose();
        }
    }

    private async Task WaitForTopicAsync(CancellationToken ct)
    {
        var adminConfig = new AdminClientConfig { BootstrapServers = "kafka:9092" };
        using var adminClient = new AdminClientBuilder(adminConfig).Build();

        while (!ct.IsCancellationRequested)
        {
            try
            {
                var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(5));
                if (metadata.Topics.Any(t => t.Topic == _topic))
                {
                    _logger.LogInformation("Topic '{Topic}' is available", _topic);
                    break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Waiting for topic '{Topic}': {Message}", _topic, ex.Message);
            }

            await Task.Delay(2000, ct);
        }
    }
}
