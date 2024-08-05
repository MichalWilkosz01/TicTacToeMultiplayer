using System.ComponentModel;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string serverIp = "127.0.0.1";
        private const int port = 13000;

        private BackgroundWorker MessageReceiver = new BackgroundWorker();
        private TcpClient? tcpClient;
        private Socket? sock;

        private char PlayerChar;
        private char OpponentChar;

        public MainWindow()
        {
            InitializeComponent();
            MessageReceiver.DoWork += MessageReceiver_DoWork;
        }

        private void MessageReceiver_DoWork(object sender, DoWorkEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                if (CheckState())
                    return;
                FreezeBoard();
                lblPlayerTurn.Content = "Opponent's Turn!";
                ReceiveMove();
                lblPlayerTurn.Content = "Your Turn!";
                if (!CheckState())
                    UnfreezeBoard();
            });
        }


        private bool CheckState()
        {
            //Horizontals
            if (btn1.Content == btn2.Content && btn2.Content == btn3.Content && btn3.Content.ToString() != "")
            {
                if (btn1.Content.ToString()?.ElementAt(0) == PlayerChar)
                {
                    lblPlayerTurn.Content = "You Won!";
                    MessageBox.Show("You Won!");
                }
                else
                {
                    lblPlayerTurn.Content = "You Lost!";
                    MessageBox.Show("You Lost!");
                }
                return true;
            }

            else if (btn4.Content == btn5.Content && btn5.Content == btn6.Content && btn6.Content.ToString() != "")
            {
                if (btn4.Content.ToString()?.ElementAt(0) == PlayerChar)
                {
                    lblPlayerTurn.Content = "You Won!";
                    MessageBox.Show("You Won!");
                }
                else
                {
                    lblPlayerTurn.Content = "You Lost!";
                    MessageBox.Show("You Lost!");
                }
                return true;
            }

            else if (btn7.Content == btn8.Content && btn8.Content == btn9.Content && btn9.Content.ToString() != "")
            {
                if (btn7.Content.ToString()?.ElementAt(0) == PlayerChar)
                {
                    lblPlayerTurn.Content = "You Won!";
                    MessageBox.Show("You Won!");
                }
                else
                {
                    lblPlayerTurn.Content = "You Lost!";
                    MessageBox.Show("You Lost!");
                }
                return true;
            }

            //Verticals
            else if (btn1.Content == btn4.Content && btn4.Content == btn7.Content && btn7.Content.ToString() != "")
            {
                if (btn1.Content.ToString()?.ElementAt(0) == PlayerChar)
                {
                    lblPlayerTurn.Content = "You Won!";
                    MessageBox.Show("You Won!");
                }
                else
                {
                    lblPlayerTurn.Content = "You Lost!";
                    MessageBox.Show("You Lost!");
                }
                return true;
            }

            else if (btn2.Content == btn5.Content && btn5.Content == btn8.Content && btn8.Content.ToString() != "")
            {
                if (btn2.Content.ToString()?.ElementAt(0) == PlayerChar)
                {
                    lblPlayerTurn.Content = "You Won!";
                    MessageBox.Show("You Won!");
                }
                else
                {
                    lblPlayerTurn.Content = "You Lost!";
                    MessageBox.Show("You Lost!");
                }
                return true;
            }

            else if (btn3.Content == btn6.Content && btn6.Content == btn9.Content && btn9.Content.ToString() != "")
            {
                if (btn3.Content.ToString()?.ElementAt(0) == PlayerChar)
                {
                    lblPlayerTurn.Content = "You Won!";
                    MessageBox.Show("You Won!");
                }
                else
                {
                    lblPlayerTurn.Content = "You Lost!";
                    MessageBox.Show("You Lost!");
                }
                return true;
            }

            //Diagonals
            else if (btn1.Content == btn5.Content && btn5.Content == btn9.Content && btn9.Content.ToString() != "")
            {
                if (btn1.Content.ToString()?.ElementAt(0) == PlayerChar)
                {
                    lblPlayerTurn.Content = "You Won!";
                    MessageBox.Show("You Won!");
                }
                else
                {
                    lblPlayerTurn.Content = "You Lost!";
                    MessageBox.Show("You Lost!");
                }
                return true;
            }

            else if (btn3.Content == btn5.Content && btn5.Content == btn7.Content && btn7.Content.ToString() != "")
            {
                if (btn3.Content.ToString()?.ElementAt(0) == PlayerChar)
                {
                    lblPlayerTurn.Content = "You Won!";
                    MessageBox.Show("You Won!");
                }
                else
                {
                    lblPlayerTurn.Content = "You Lost!";
                    MessageBox.Show("You Lost!");
                }
                return true;
            }

            //Draw
            else if (btn1.Content.ToString() != "" && btn2.Content.ToString() != "" && btn3.Content.ToString() != "" &&
                     btn4.Content.ToString() != "" && btn5.Content.ToString() != "" && btn6.Content.ToString() != "" &&
                     btn7.Content.ToString() != "" && btn8.Content.ToString() != "" && btn9.Content.ToString() != "")
            {
                lblPlayerTurn.Content = "It's a draw!";
                MessageBox.Show("It's a draw!");
                return true;
            }
            return false;
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

        private void ReceiveMove()
        {
            byte[] buffer = new byte[1];
            sock?.Receive(buffer);
            switch (buffer[0])
            {
                case 1:
                    btn1.Content = OpponentChar.ToString();
                    break;
                case 2:
                    btn2.Content = OpponentChar.ToString();
                    break;
                case 3:
                    btn3.Content = OpponentChar.ToString();
                    break;
                case 4:
                    btn4.Content = OpponentChar.ToString();
                    break;
                case 5:
                    btn5.Content = OpponentChar.ToString();
                    break;
                case 6:
                    btn6.Content = OpponentChar.ToString();
                    break;
                case 7:
                    btn7.Content = OpponentChar.ToString();
                    break;
                case 8:
                    btn8.Content = OpponentChar.ToString();
                    break;
                case 9:
                    btn9.Content = OpponentChar.ToString();
                    break;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            byte move = byte.Parse(button.Tag.ToString());
            byte[] num = { move };
            sock?.Send(num);
            button.Content = PlayerChar.ToString();
            button.IsEnabled = false;
            MessageReceiver.RunWorkerAsync();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            MessageReceiver.WorkerSupportsCancellation = true;
            MessageReceiver.CancelAsync();
            sock?.Close();
            tcpClient?.Close();
        }
    }
}
