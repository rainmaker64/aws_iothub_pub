using System;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using uPLibrary.Networking.M2Mqtt;
using System.IO;

namespace Iotpublisher
{
    class Program
    {
        const string pfxCertFile = @".\amazon.cert.pfx";
        const string rootCaCertFile = @".\amazon.pem.crt";

        static void Main(string[] args)
        {
            string iotEndpoint = "a1436zwng9p6hb-ats.iot.us-west-2.amazonaws.com";
            Console.WriteLine("AWS IoT Dotnet message publisher starting..");

            int brokerPort = 8883;
            string topic = "Hello/World";
            string message = "Test message";

            var client = new MqttClient(iotEndpoint, brokerPort, true, new X509Certificate(rootCaCertFile), new X509Certificate(pfxCertFile, "rainmaker64"), MqttSslProtocols.TLSv1_2);

            string clientId = Guid.NewGuid().ToString();
            client.Connect(clientId);
            Console.WriteLine($"Connected to AWS IoT with client id: {clientId}.");

            int i = 0;
            while (true)
            {
                client.Publish(topic, Encoding.UTF8.GetBytes($"{message} {i}"));
                Console.WriteLine($"Published: {message} {i}");
                i++;
                Thread.Sleep(5000);
            }
        }
    }
}

