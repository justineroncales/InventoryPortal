using InvoiceManagement.Models;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Net.Http;
using System.Text;

namespace InvoiceManagement.BL
{
    public class AssetClient
    {
        private readonly HttpClient _httpClient;
        public AssetClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Asset>> GetAssetsAsync()
        {
            var assets = new List<Asset>();
            try
            {
                assets = await _httpClient.GetFromJsonAsync<List<Asset>>("");
                //await Task.Delay(TimeSpan.FromSeconds(30));
                var factory = new ConnectionFactory
                {
                    HostName = "localhost"
                };
                //Create the RabbitMQ connection using connection factory details as i mentioned above
                var connection = factory.CreateConnection();
                //Here we create channel with session and model
                using
                var channel = connection.CreateModel();
                //declare the queue after mentioning name and a few property related to that
                channel.QueueDeclare("asset", exclusive: false);
                //Set Event object which listen message from chanel which is sent by producer
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, eventArgs) => {
                    var body = eventArgs.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine($"Product message received: {message}");
                };
                //read the message
                channel.BasicConsume(queue: "asset", autoAck: true, consumer: consumer);

            }
            catch (Exception ex)
            {

                throw;
            }
            return assets;
        }

    }


}
