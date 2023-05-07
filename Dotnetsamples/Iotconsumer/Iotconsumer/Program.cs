using System;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Threading;
using System.Text;

namespace Iotconsumer
{
    class Program
    {
        private static ManualResetEvent manualResetEvent;

        const string pfxCertFile = @".\amazon.cert.pfx";
        const string rootCaCertFile = @".\amazon.pem.crt";

        static void Main(string[] args)
        {
            string iotEndpoint = "a1436zwng9p6hb-ats.iot.us-west-2.amazonaws.com";
            int brokerPort = 8883;

            Console.WriteLine("AWS IoT dotnet message consumer starting..");

            var client = new MqttClient(iotEndpoint, brokerPort, true, new X509Certificate(rootCaCertFile), new X509Certificate(pfxCertFile, "rainmaker64"), MqttSslProtocols.TLSv1_2);

            client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;
            client.MqttMsgSubscribed += Client_MqttMsgSubscribed;

            string clientId = Guid.NewGuid().ToString();
            client.Connect(clientId);
            Console.WriteLine($"Connected to AWS IoT with client ID: {clientId}");

            string topic = "Hello/World";
            client.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });

            // Keep the main thread alive for the event receivers to get invoked
            KeepConsoleAppRunning(() => {
                client.Disconnect();
                Console.WriteLine("Disconnecting client..");
            });
        }

        private static void Client_MqttMsgSubscribed(object sender, MqttMsgSubscribedEventArgs e)
        {
            Console.WriteLine($"Successfully subscribed to the AWS IoT topic.");
        }

        private static void Client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            Console.WriteLine("Message received: " + Encoding.UTF8.GetString(e.Message));
        }

        private static void KeepConsoleAppRunning(Action onShutdown)
        {
            manualResetEvent = new ManualResetEvent(false);
            Console.WriteLine("Press CTRL + C or CTRL + Break to exit...");

            Console.CancelKeyPress += (sender, e) =>
            {
                onShutdown();
                e.Cancel = true;
                manualResetEvent.Set();
            };

            manualResetEvent.WaitOne();
        }
    }
}
