using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

class Server
{
    static void Main()
    {
        TcpListener? server = null;
        try
        {
            // Adres IP i port
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");
            int port = 13000;

            // Utwórz serwer na danym adresie i porcie
            server = new TcpListener(localAddr, port);

            // Start serwera
            server.Start();

            // Oczekiwanie na połączenie klienta
            Console.WriteLine("Serwer uruchomiony. Oczekiwanie na połączenie...");

            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("Połączono z klientem!");

                ClientHandler clientHandler = new ClientHandler(client);
                clientHandler.HandleClient();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
        finally
        {
            server?.Stop();
        }
    }
}

class ClientHandler
{
    private TcpClient client;

    public ClientHandler(TcpClient client)
    {
        this.client = client;
    }

    public void HandleClient()
    {
        // Obsługa połączenia z klientem
        NetworkStream stream = client.GetStream();
        byte[] data = new byte[256];
        string? responseData = null;

        int bytes = stream.Read(data, 0, data.Length);
        responseData = Encoding.ASCII.GetString(data, 0, bytes);
        Console.WriteLine("Otrzymano: {0}", responseData);

        // Odpowiedź do klienta
        byte[] msg = Encoding.ASCII.GetBytes("Otrzymano twoją wiadomość.");
        stream.Write(msg, 0, msg.Length);

        // Zamknięcie połączenia
        client.Close();
    }
}
