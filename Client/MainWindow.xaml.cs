using System;
using System.ComponentModel;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Client
{
    public partial class MainWindow : Window
    {
        private const string serverIp = "127.0.0.1";
        private const int port = 13000;

        private BackgroundWorker MessageReceiver = new BackgroundWorker();
        private TcpClient? tcpClient;
        private NetworkStream? stream;

        private char PlayerChar = 'X';
        private char OpponentChar = 'O';

        public MainWindow()
        {
            InitializeComponent();
            MessageReceiver.DoWork += MessageReceiver_DoWork;
            ConnectToServer();
        }

        private async void ConnectToServer()
        {
            try
            {
                tcpClient = new TcpClient();
                await tcpClient.ConnectAsync(serverIp, port);
                stream = tcpClient.GetStream();
                MessageReceiver.RunWorkerAsync();
                lblPlayerTurn.Content = "Connected to server!";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to connect to server: {ex.Message}");
            }
        }

        private void MessageReceiver_DoWork(object sender, DoWorkEventArgs e)
        {
            while (tcpClient?.Connected == true)
            {
                try
                {
                    var buffer = new byte[1024];
                    int bytesRead = stream!.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Dispatcher.Invoke(() => ProcessMessage(message));
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(() => MessageBox.Show($"Error receiving message: {ex.Message}"));
                    break;
                }
            }
        }

        private void ProcessMessage(string message)
        {
            // Assume message format is "move:1" where 1 is the button number
            if (message.StartsWith("move:"))
            {
                int move = int.Parse(message.Substring(5));
                UpdateBoard(move, OpponentChar);
                lblPlayerTurn.Content = "Your Turn!";
                UnfreezeBoard();
            }
        }

        private void UpdateBoard(int move, char playerChar)
        {
            switch (move)
            {
                case 1:
                    btn1.Content = playerChar.ToString();
                    break;
                case 2:
                    btn2.Content = playerChar.ToString();
                    break;
                case 3:
                    btn3.Content = playerChar.ToString();
                    break;
                case 4:
                    btn4.Content = playerChar.ToString();
                    break;
                case 5:
                    btn5.Content = playerChar.ToString();
                    break;
                case 6:
                    btn6.Content = playerChar.ToString();
                    break;
                case 7:
                    btn7.Content = playerChar.ToString();
                    break;
                case 8:
                    btn8.Content = playerChar.ToString();
                    break;
                case 9:
                    btn9.Content = playerChar.ToString();
                    break;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            int move = int.Parse(button.Tag.ToString());
            UpdateBoard(move, PlayerChar);
            FreezeBoard();
            lblPlayerTurn.Content = "Opponent's Turn!";

            SendMove(move);
        }

        private async void SendMove(int move)
        {
            if (tcpClient?.Connected == true)
            {
                try
                {
                    string message = $"move:{move}";
                    var buffer = Encoding.UTF8.GetBytes(message);
                    await stream!.WriteAsync(buffer, 0, buffer.Length);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to send move: {ex.Message}");
                }
            }
        }

        private void FreezeBoard()
        {
            btn1.IsEnabled = false;
            btn2.IsEnabled = false;
            btn3.IsEnabled = false;
            btn4.IsEnabled = false;
            btn5.IsEnabled = false;
            btn6.IsEnabled = false;
            btn7.IsEnabled = false;
            btn8.IsEnabled = false;
            btn9.IsEnabled = false;
        }

        private void UnfreezeBoard()
        {
            if (btn1.Content.ToString() == "")
                btn1.IsEnabled = true;
            if (btn2.Content.ToString() == "")
                btn2.IsEnabled = true;
            if (btn3.Content.ToString() == "")
                btn3.IsEnabled = true;
            if (btn4.Content.ToString() == "")
                btn4.IsEnabled = true;
            if (btn5.Content.ToString() == "")
                btn5.IsEnabled = true;
            if (btn6.Content.ToString() == "")
                btn6.IsEnabled = true;
            if (btn7.Content.ToString() == "")
                btn7.IsEnabled = true;
            if (btn8.Content.ToString() == "")
                btn8.IsEnabled = true;
            if (btn9.Content.ToString() == "")
                btn9.IsEnabled = true;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            MessageReceiver.WorkerSupportsCancellation = true;
            MessageReceiver.CancelAsync();
            stream?.Close();
            tcpClient?.Close();
        }
    }
}
