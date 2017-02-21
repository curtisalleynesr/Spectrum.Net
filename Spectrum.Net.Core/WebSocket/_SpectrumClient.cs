﻿#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Spectrum.Net.Core
{
    public delegate void FrameReceivedDelegate(String buffer);

    public partial class SpectrumClient : IDisposable
    {
        public event FrameReceivedDelegate FrameReceived;

        private String _wsToken;
        private String _wsRoot;

        public async Task ConnectAsync()
        {
            if (this._socketClient.State != WebSocketState.Open)
            {
                await this._socketClient.ConnectAsync(new Uri($"{this._wsRoot}/?token={this._wsToken}"), this._cancellationOwner.Token);

                Task.Run(async () =>
                {
                    while (!this._cancellationOwner.IsCancellationRequested && this._socketClient.State == WebSocketState.Open)
                    {
                        var buffer = new Byte[1024];
                        var sb = new StringBuilder();

                        var result = await this._socketClient.ReceiveAsync(new ArraySegment<Byte>(buffer), this._cancellationOwner.Token);

                        switch (result.MessageType)
                        {
                            case WebSocketMessageType.Close:
                                await this._socketClient.CloseAsync(WebSocketCloseStatus.NormalClosure, String.Empty, this._cancellationOwner.Token);
                                break;
                            case WebSocketMessageType.Text:
                                sb.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
                                while (!result.EndOfMessage)
                                {
                                    result = await this._socketClient.ReceiveAsync(new ArraySegment<Byte>(buffer), this._cancellationOwner.Token);
                                    sb.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
                                }

                                var str_buffer = sb.ToString();

                                if (str_buffer[0] == '{')
                                {
                                    var payload = str_buffer.FromJSON<WebSocket.Payload>();

                                    switch (payload.Type)
                                    {
                                        case PayloadType.Message_New:
                                            this.MessageReceived?.Invoke(str_buffer.FromJSON<Message.New.Payload>());
                                            break;
                                        case PayloadType.Unknown:
                                            File.AppendAllText("payload.unknown.log", $"\r\n{str_buffer}");
                                            Console.WriteLine($"Unknown Payload: {payload.Type}");
                                            this.FrameReceived?.Invoke(str_buffer);
                                            break;
                                        default:
                                            this.FrameReceived?.Invoke(str_buffer);
                                            break;
                                    }
                                }
                                break;
                            case WebSocketMessageType.Binary:
                                throw new NotImplementedException();
                        }
                    }
                });

                Task.Run(async () =>
                {
                    while (!this._cancellationOwner.IsCancellationRequested && this._socketClient.State == WebSocketState.Open)
                    {
                        await this._socketClient.SendAsync(KEEPALIVE, WebSocketMessageType.Text, true, this._cancellationOwner.Token);

                        Thread.Sleep(KEEPALIVE_TIMEOUT);
                    }
                });
            }
        }

        public async Task SendPayloadAsync(WebSocket.Payload payload)
        {
            await this._socketClient.SendAsync(new ArraySegment<Byte>(Encoding.ASCII.GetBytes(payload.ToJSON())), WebSocketMessageType.Text, true, this._cancellationOwner.Token);
        }

        public async Task DisconnectAsync()
        {
            this._cancellationOwner.Cancel(false);

            await this._socketClient.CloseAsync(WebSocketCloseStatus.NormalClosure, "OK", new CancellationTokenSource { }.Token);
        }
    }
}

#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed