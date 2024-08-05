using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string serverIp = "127.0.0.1";
        int port = 13000;
        TcpClient? tcppClient;
        NetworkStream? stream;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            tcppClient = new TcpClient(serverIp, port);
            stream = tcppClient.GetStream();
        }

        private void btnSendMessage_Click(object sender, RoutedEventArgs e)
        {
            if (tcppClient is null || stream is null)
            {
                MessageBox.Show("Najpierw należy połączyć się z serwerem!");
                return;
            }
                
            string message = txtMessage.Text;
            byte[] data = Encoding.ASCII.GetBytes(message);
            stream.Write(data, 0, data.Length);

        }

        private void btnDisconnect_Click(object sender, RoutedEventArgs e)
        {
            stream?.Close();
            tcppClient?.Close();
        }
    }
}