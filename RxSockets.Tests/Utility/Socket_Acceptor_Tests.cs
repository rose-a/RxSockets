﻿using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace RxSockets.Tests;

public class Socket_Acceptor_Tests : TestBase
{
    public Socket_Acceptor_Tests(ITestOutputHelper output) : base(output) { }

    [Fact]
    public async Task T00_Success()
    {
        EndPoint endPoint = Utilities.CreateIPEndPointOnPort(0);
        Socket serverSocket = Utilities.CreateSocket();
        serverSocket.Bind(endPoint);
        serverSocket.Listen(10);
        endPoint = serverSocket.LocalEndPoint ?? throw new InvalidOperationException();

        Task task = Task.Run(async () =>
        {
            SocketAcceptor acceptor = new(serverSocket, SocketServerLogger);
            await foreach (IRxSocketClient cli in acceptor.AcceptAllAsync(default))
            {
                Logger.LogDebug("client");
            }
        });

        IRxSocketClient client = await endPoint.CreateRxSocketClientAsync(SocketClientLogger, default);
        Assert.True(client.Connected);

        await Task.Delay(100);

        await client.DisposeAsync();
    }
}
