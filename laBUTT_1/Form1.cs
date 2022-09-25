using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace laBUTT_1
{
    
    public partial class Form1 : Form
    {
        const string ip = "127.0.0.1";
        const int port = 12334;

        
        EndPoint tcpEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);

        
        Socket tcpSocket;
        public Form1()
        {
            InitializeComponent();
        }

        public void button1_Click(object sender, EventArgs e)//кнопка загрузки файлов
        {
            OpenFileDialog d = new OpenFileDialog();
            d.ShowDialog();
            string path = d.FileName;

            byte[] bytes = File.ReadAllBytes(path);

            tcpSocket.Send(bytes);//отправляем файл на сервер для вычислений


        }

        public void button2_Click(object sender, EventArgs e)//кнопка для выявления похожих экспериментов и показа статистики
        {
            
            this.Hide();

            Form2 f = new Form2(tcpSocket);
            f.ShowDialog();

            this.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            
            tcpSocket.Connect(tcpEndPoint);////подключаемся к сокету при загрузки формы
        }
    }
    class Serialization//класс с методами для отправки различных типов данных
    {
        public static void SendDouble(Socket sock, double x)
        {
            byte[] lenB = BitConverter.GetBytes(x);
            sock.Send(lenB);
        }

        public static double ReceiveDouble(Socket sock)
        {
            byte[] lenB = new byte[8];
            sock.Receive(lenB);
            return BitConverter.ToDouble(lenB, 0);
        }

        public static void SendInt(Socket sock, int x)
        {
            byte[] lenB = BitConverter.GetBytes(x);
            sock.Send(lenB);
        }

        public static int ReceiveInt(Socket sock)
        {
            byte[] lenB = new byte[4];
            sock.Receive(lenB);
            return BitConverter.ToInt32(lenB, 0);
        }
    }
}
