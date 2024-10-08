﻿using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        private const int port = 13000;
        private static TcpListener? listener;
        private static readonly ConcurrentDictionary<int, TcpClient> clients = new ConcurrentDictionary<int, TcpClient>();
        private static int clientIdCounter = 0;
        private static int? firstClientId = null;

        // Zmieniono SemaphoreSlim na Semaphore
        private static readonly Semaphore semaphore = new Semaphore(1, 1);

        static async Task Main(string[] args)
        {
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            Console.WriteLine("Server started...");

            while (true)
            {
                var tcpClient = await listener.AcceptTcpClientAsync();
                int clientId = Interlocked.Increment(ref clientIdCounter);
                clients[clientId] = tcpClient;
                Console.WriteLine($"Client {clientId} connected.");

                _ = HandleClientAsync(clientId, tcpClient);
            }
        }

        private static async Task HandleClientAsync(int clientId, TcpClient tcpClient)
        {
            var buffer = new byte[1024];
            var stream = tcpClient.GetStream();

            try
            {
                string sign;

                // Zamiast WaitAsync używamy WaitOne, co jest operacją synchroniczną
                semaphore.WaitOne();
                try
                {
                    if (firstClientId == null)
                    {
                        firstClientId = clientId;
                        sign = "X";
                    }
                    else
                    {
                        sign = "O";
                    }
                }
                finally
                {
                    // Użycie Release w Semaphore
                    semaphore.Release();
                }

                var signMessage = $"SIGN:{sign}";
                var signBuffer = Encoding.UTF8.GetBytes(signMessage);
                await stream.WriteAsync(signBuffer, 0, signBuffer.Length);

                while (tcpClient.Connected)
                {
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"Received from client {clientId}: {message}");

                    await BroadcastMessageAsync(clientId, message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error with client {clientId}: {ex.Message}");
            }
            finally
            {
                Console.WriteLine($"Client {clientId} disconnected.");
                clients.TryRemove(clientId, out _);
                tcpClient.Close();
            }
        }

        private static async Task BroadcastMessageAsync(int senderId, string message)
        {
            foreach (var kvp in clients)
            {
                if (kvp.Key == senderId) continue;

                var client = kvp.Value;
                var stream = client.GetStream();
                var buffer = Encoding.UTF8.GetBytes(message);

                try
                {
                    await stream.WriteAsync(buffer, 0, buffer.Length);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending to client {kvp.Key}: {ex.Message}");
                }
            }
        }
    }
}
