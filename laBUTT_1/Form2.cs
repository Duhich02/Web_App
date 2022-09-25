using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace laBUTT_1
{
    enum Команда
    {
        ShowStatistics
    }
    public partial class Form2 : Form
    {
        private Socket tcpSocket;
        public Form2(Socket _s)
        {
            tcpSocket = _s;
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

            byte[] buf = new byte[1];
            buf[0] = (byte)Команда.ShowStatistics;
            tcpSocket.Send(buf);

            //получаем все вычисленные данные, что мы отправили с сервера и загружаем их в график
            double stddev = Serialization.ReceiveDouble(tcpSocket);
                chart1.Series[0].Points.Add(stddev);

            double sum = Serialization.ReceiveDouble(tcpSocket);
                chart1.Series[0].Points.Add(sum);

            double determ = Serialization.ReceiveDouble(tcpSocket);
                chart1.Series[0].Points.Add(determ);

            int numOFexps = Serialization.ReceiveInt(tcpSocket);
            for (int i = 0; i < numOFexps; i++)
            {
                double suck = Serialization.ReceiveDouble(tcpSocket);
                double fuck = Serialization.ReceiveDouble(tcpSocket);
                double huck = Serialization.ReceiveDouble(tcpSocket);
                chart2.Series[i + 1].Points.Add(suck);
                chart2.Series[i + 1].Points.Add(fuck);
                chart2.Series[i + 1].Points.Add(huck);
            }

            
        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }
    }
}
