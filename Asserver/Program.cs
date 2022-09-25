using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;

namespace Asserver
{
    class Experiment //класс с переменными для вызова на сервере и на клиенте
    {
        public double[][] chisla;
        public double stddev;
        public double mean;
        public double determ;

    }
    enum Команда  //енум - тип перечисления для удобства
    {
        ShowStatistics
    }

    class Program
    {
        static List<Experiment> exps = new List<Experiment>(); //лист со всеми экспериментами (ср знач, среднекв отклон, определитель)

        public static void Main(string[] args)
        {
            const string ip = "127.0.0.1"; //подключимся к станд ip
            const int port = 12334; //выбрем рандомный порт

            var tcpEndPoint = new IPEndPoint(IPAddress.Parse(ip), port); //эндпоинт для сетевого подключения

            var tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); //создаем сокет для связи
            tcpSocket.Bind(tcpEndPoint); //говорим сокету где ему слушать 
            tcpSocket.Listen(10); //говорим сокету сколько клентов ему слушать

            while (true)
            {
                var listener = tcpSocket.Accept(); //новый сокет для подключения конкретного клиента
                var buffer = new byte[256]; //куда будут приходить запросы
                var data = new StringBuilder(); //собирает полученные данные
                int size;

                    while (true)
                    {
                        do
                        {
                            size = listener.Receive(buffer);//получаем данные
                            data.Append(Encoding.UTF8.GetString(buffer, 0, size));//для использования строк в качестве сбора данных
                        }

                        while (listener.Available > 0);

                        string d = Encoding.UTF8.GetString(buffer, 0, size);

                        string[] S = d.Split("\r\n").Where(цифры => цифры != "").ToArray();//сплит-для разделения элдементов массива

                        double[][] chizla = S.Select(строка => строка.Split(',')//селекты для выделения цифр из массива
                              .Where(цифры => цифры != "")
                              .Select(цифры => double.Parse(цифры.Replace('.', ',')))
                              .ToArray())
                        .ToArray();
                        double sum = chizla.Select(mASS => mASS.Sum()).Sum(); //сложим все жлементы
                        sum = sum / (chizla.Length * chizla[0].Length); //находим среднее значение

                        //среднекв отклонение
                        double[][] otklon = chizla.Select(newmASS => newmASS.Select(elem => ((elem - sum) * (elem - sum))).ToArray()).ToArray();
                        double newsum = otklon.Select(newmASS => newmASS.Sum()).Sum();
                        newsum = Math.Sqrt(newsum);
                        newsum = newsum / Math.Sqrt(otklon.Length * otklon[0].Length);

                        DET(chizla.Length, chizla);//определитель

                        Experiment newExp = new Experiment(); //новый лист экспериментов для нахождения других похожих экспериментов
                        newExp.chisla = chizla;//новые пеерменные для вычислений
                        newExp.stddev = newsum;
                        newExp.mean = sum;
                        newExp.determ = DET(chizla.Length, chizla);

                        int a = exps.Count();//счетчик экспериментов
                        Experiment[] самыеПохожие;

                        if (a >= 3)//цикл для второй кнопки
                        {
                            byte[] buf = new byte[1];
                            buf[0] = (byte)Команда.ShowStatistics;
                            listener.Receive(buf);//получаем данные


                            самыеПохожие = exps.OrderBy(exp => Похожесть(exp, newExp)).Take(a).ToArray();//сортируем их по порядку
                            SendDouble(listener, newExp.stddev);//отправляем данные первого эксперимента
                            SendDouble(listener, newExp.mean = sum);
                            SendDouble(listener, newExp.determ);

                            SendInt(listener, самыеПохожие.Length);//отправляем количество загруженных экспериментов

                            foreach (Experiment suck in самыеПохожие)//отправляем вычисления новых экспериментов
                            {
                                SendDouble(listener, suck.stddev);
                                SendDouble(listener, suck.mean);
                                SendDouble(listener, suck.determ);
                            }
                        }




                        else
                        {
                            Console.WriteLine("Load more csv's");//если меньше 3х
                        }

                        exps.Add(newExp);
                    }
                


            }

        }

        public static void SendDouble(Socket sock, double x)//методы для отправки разных типов данных
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

        private static double Похожесть(Experiment exp, Experiment newExp)//класс для выявления похожих экспериментов
        {
            double sameshit = exp.chisla.Zip(newExp.chisla, (first, second) => first.Zip(second, (p, q) => Math.Sqrt((p - q) * (p - q))).Sum()).Sum();
            //зип-метод для обЪединения последовательностей, тут мы сравниваем каждый последуемый загруженный файл с предыдущим
            return sameshit;
        }
        
        public static double DET(int n, double[][] Mat)//цикл для определителя
        {
            double d = 0;
            int k, i, j, subi, subj;
            double[][] SUBMat = new double[n][];
            SUBMat = SUBMat.Select(x => new double[n]).ToArray();

            if (n == 2)
            {
                return ((Mat[0][0] * Mat[1][1]) - (Mat[1][0] * Mat[0][1]));
            }

            else
            {
                for (k = 0; k < n; k++)
                {
                    subi = 0;
                    for (i = 1; i < n; i++)
                    {
                        subj = 0;
                        for (j = 0; j < n; j++)
                        {
                            if (j == k)
                            {
                                continue;
                            }
                            SUBMat[subi][subj] = Mat[i][j];
                            subj++;
                        }
                        subi++;
                    }
                    d = d + (Math.Pow(-1, k) * Mat[0][k] * DET(n - 1, SUBMat));
                }
            }
            return d;

        }
    }
}
