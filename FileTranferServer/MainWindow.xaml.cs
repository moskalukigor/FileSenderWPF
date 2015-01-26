using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Net;
using System.Net.Sockets;
using System.IO;

namespace FileTranferServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            FTServerCode.receivedPath = "D:/moskaluk/testSendFile";
        }

        private void btnStart_Click_1(object sender, RoutedEventArgs e)
        {
            FTServerCode obj = new FTServerCode();
            obj.StartServer();
            //obj.StopServer();
        }

        class FTServerCode
        {
            IPEndPoint ipEnd;
            Socket sock;
            public FTServerCode()
            {
                ipEnd = new IPEndPoint(IPAddress.Any, 5656);
                sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                sock.Bind(ipEnd);
            }
            public static string receivedPath;
            public void StartServer()
            {
                try
                {
                    sock.Listen(100);
                    Socket clientSock = sock.Accept();
                    byte[] clientData = new byte[1024];
                    int receivedBytesLen = clientSock.Receive(clientData);
                    Console.WriteLine("Receiving data...");
                    int fileNameLen = BitConverter.ToInt32(clientData, 0);
                    string fileName = Encoding.ASCII.GetString(clientData, 4, fileNameLen);
                    BinaryWriter bWrite = new BinaryWriter(File.Open
                        (receivedPath + "/" + fileName, FileMode.Append));
                    bWrite.Write(clientData, 4 + fileNameLen, receivedBytesLen - 4 - fileNameLen);
                    do
                    {
                        receivedBytesLen = clientSock.Receive(clientData);
                        bWrite.Write(clientData, 0, receivedBytesLen);
                    } while (receivedBytesLen > 0);
                    clientSock.Close();
                    bWrite.Close();
                    sock.Close();
                    MessageBox.Show("Received & Saved file.\n Server Stopped.");
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "File Receiving error.");
                }
            }
        } 
    }
}
