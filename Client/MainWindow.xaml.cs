using System;
using System.Net.Sockets;
using System.Reflection.Emit;
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

        private TcpClient? tcpClient;
        private NetworkStream? stream;

        private char PlayerChar;
        private char OpponentChar;

        public MainWindow()
        {
            InitializeComponent();
            ConnectToServer();
        }

        private async void ConnectToServer()
        {
            try
            {
                tcpClient = new TcpClient();
                await tcpClient.ConnectAsync(serverIp, port);
                stream = tcpClient.GetStream();

               
                _ = Task.Run(MessageReceiverAsync);
   
               
                await GetPlayerSignAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to connect to server: {ex.Message}");
            }
        }

        private async Task GetPlayerSignAsync()
        {
            if (stream == null) return;

            var buffer = new byte[1024];
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            if (bytesRead > 0)
            {
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                if (message.StartsWith("SIGN:"))
                {
                    PlayerChar = message[5]; 
                    OpponentChar = PlayerChar == 'X' ? 'O' : 'X';
                }
                lblPlayerTurn.Content = "Connected to server! ";
                if (OpponentChar == 'X')
                {
                    lblPlayerTurn.Content += "Opponent's Turn";
                    FreezeBoard();
                } else
                {
                    lblPlayerTurn.Content += "Your Turn";
                }
            }
        }

        private async Task MessageReceiverAsync()
        {
            if (stream == null) return;

            var buffer = new byte[1024];
            while (tcpClient?.Connected == true)
            {
                try
                {
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;
           
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Dispatcher.Invoke(() => ProcessMessage(message));
                    Dispatcher.Invoke(CheckResult);

                    

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
            if (message.StartsWith("move:"))
            {
                if (int.TryParse(message.Substring(5), out int move))
                {
                    UpdateBoard(move, OpponentChar);
                    lblPlayerTurn.Content = "Your Turn!";
                    UnfreezeBoard();
                }
            }
        }

        private void UpdateBoard(int move, char playerChar)
        {
            Button? button = GetButtonByMove(move);
            if (button != null)
            {
                button.Content = playerChar.ToString();
            }         
        }

        private Button? GetButtonByMove(int move)
        {
            return move switch
            {
                1 => btn1,
                2 => btn2,
                3 => btn3,
                4 => btn4,
                5 => btn5,
                6 => btn6,
                7 => btn7,
                8 => btn8,
                9 => btn9,
                _ => null
            };
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && int.TryParse(button.Tag.ToString(), out int move))
            {
                UpdateBoard(move, PlayerChar);
                FreezeBoard();
                lblPlayerTurn.Content = "Opponent's Turn!";
                SendMove(move);
                CheckResult();
            }
        }

        private async void SendMove(int move)
        {
            if (tcpClient?.Connected == true)
            {
                try
                {
                    string message = $"move:{move}";
                    var buffer = Encoding.UTF8.GetBytes(message);
                    await stream.WriteAsync(buffer, 0, buffer.Length);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to send move: {ex.Message}");
                }
            }
        }

        private void FreezeBoard()
        {
            SetButtonsEnabled(false);
        }

        private void UnfreezeBoard()
        {
            foreach (var button in new[] { btn1, btn2, btn3, btn4, btn5, btn6, btn7, btn8, btn9 })
            {
                if (button.Content == null)
                {
                    button.IsEnabled = true;
                }
            }
        }

        private void SetButtonsEnabled(bool enabled)
        {
            foreach (var button in new[] { btn1, btn2, btn3, btn4, btn5, btn6, btn7, btn8, btn9 })
            {
                button.IsEnabled = enabled;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            stream?.Close();
            tcpClient?.Close();
        }
        private void CheckResult()
        {
        
            char GetButtonContent(Button button) =>
                button.Content != null ? button.Content.ToString()[0] : ' ';

            // Helper method to check if a line has the same char
            bool CheckLine(Button b1, Button b2, Button b3)
            {
                char c1 = GetButtonContent(b1);
                char c2 = GetButtonContent(b2);
                char c3 = GetButtonContent(b3);
                return c1 == c2 && c2 == c3 && c1 != ' ';
            }

            
            if (CheckLine(btn1, btn2, btn3))
            {
                lblPlayerTurn.Content = GetButtonContent(btn1) == PlayerChar ? "You Won!" : "Opponent Won";               
                return;
            }
            if (CheckLine(btn4, btn5, btn6))
            {
                lblPlayerTurn.Content = GetButtonContent(btn4) == PlayerChar ? "You Won!" : "Opponent Won";
              
                return;
            }

            if (CheckLine(btn7, btn8, btn9))
            {
                lblPlayerTurn.Content = GetButtonContent(btn7) == PlayerChar ? "You Won!" : "Opponent Won";
                return;
            }


            if (CheckLine(btn1, btn4, btn7))
            {
                lblPlayerTurn.Content = GetButtonContent(btn1) == PlayerChar ? "You Won!" : "Opponent Won";
                return;
            }
            if (CheckLine(btn2, btn5, btn8))
            {
                lblPlayerTurn.Content = GetButtonContent(btn2) == PlayerChar ? "You Won!" : "Opponent Won";
                return;
            }
            if (CheckLine(btn3, btn6, btn9))
            {
                lblPlayerTurn.Content = GetButtonContent(btn3) == PlayerChar ? "You Won!" : "Opponent Won";
                return;
            }


            if (CheckLine(btn1, btn5, btn9))
            {
                lblPlayerTurn.Content = GetButtonContent(btn1) == PlayerChar ? "You Won!" : "Opponent Won";
                return;
            }

            if (CheckLine(btn3, btn5, btn7))
            {
                lblPlayerTurn.Content = GetButtonContent(btn3) == PlayerChar ? "You Won!" : "Opponent Won";
                return;
            }


            bool IsDraw() =>
                !new[] { btn1, btn2, btn3, btn4, btn5, btn6, btn7, btn8, btn9 }
                    .Any(b => b.Content == null);

            if (IsDraw())
            {
                MessageBox.Show("It's a draw!");
            }
        }
      
    }
}
