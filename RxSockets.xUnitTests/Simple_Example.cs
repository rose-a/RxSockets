﻿using System;
using System.Net;
using System.Threading.Tasks;
using System.Reactive.Linq;
using Xunit;

namespace RxSockets.xUnitTests
{
    public class Simple_Example
    {
        [Fact]
        public async Task Example()
        {
            // Create an IPEndPoint on the local machine on an available port.
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.IPv6Loopback, 12345);

            // Create a socket server.
            IRxSocketServer server = new RxSocketServer(ipEndPoint);

            // Start accepting connections from clients.
            server.AcceptObservable.Subscribe(acceptClient =>
            {
                // After the server accepts a client connection, start receiving messages from the client and...
                acceptClient.ReceiveObservable.ToStrings().Subscribe(onNext: message =>
                {
                    // Echo each message received back to the client.
                    acceptClient.Send(message.ToBuffer());
                });
            });



            // Create a socket client by first connecting to the server at the IPEndPoint.
            IRxSocketClient client = await ipEndPoint.ConnectRxSocketClientAsync();

            // Start receiving messages from the server.
            client.ReceiveObservable.ToStrings().Subscribe(onNext: message =>
            {
                // The message received from the server is "Hello!".
                Assert.Equal("Hello!", message);
            });

            // Send the message "Hello" to the server, which the server will then echo back to the client.
            client.Send("Hello!".ToBuffer());

            // Allow time for communication to complete.
            await Task.Delay(10);

            // Disconnect.
            await client.DisposeAsync();
            await server.DisposeAsync();
        }
    }
}
