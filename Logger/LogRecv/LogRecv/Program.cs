using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;


namespace LogRecv
{
    class Program
    {
        public static string serverIp   = "127.0.0.1";//设置服务端IP
        public static int    serverPort = 8765;//服务端端口

   
        public static Socket socketServer;//定义socket
        public static Thread threadWatch; //定义线程


        private static List<RecvLog> recvLogs = new List<RecvLog>();

        [STAThread]
        static void Main(string[] args)
        {
            SocketServer();
        }

        public static void SocketServer()
        {
            serverIp = IP.Pv4;
            serverPort = IP.Port;
            socketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//创建一个socket的对象
            IPAddress ip = IPAddress.Parse(serverIp);//获取服务器IP地址
            IPEndPoint point = new IPEndPoint(ip, serverPort);//获取端口
            try
            {
                socketServer.Bind(point);//绑定IP地址及端口
            }
            catch (Exception ex)
            {
                Console.WriteLine("绑定IP时出现异常：" + ex.Message);
                Console.ReadLine();
                return;
            }
            socketServer.Listen(100);//开启监听并设定最多10个排队连接请求

            threadWatch = new Thread(WatchConnect);//创建一个监听进程
            threadWatch.IsBackground = true;//后台启动
            threadWatch.Start();//运行
            Console.WriteLine("服务器{0}监听启动成功！", socketServer.LocalEndPoint.ToString());
            Console.WriteLine("输入 [port=xxxx] 改变监听端口");


            ThreadPool.SetMinThreads(2, 2);
            ThreadPool.SetMaxThreads(5, 5);

            WaitInput();
        }



        static void WaitInput()
        {
            var recv = Console.ReadLine();
            if (recv == ("cmd"))
            {
                var cmd = Console.ReadLine();
                if (cmd.Contains("port"))
                {             
                    serverPort = int.Parse(cmd.Split('=').Last());
                    Replace();
                }
            }
        }


        public static void Replace()
        {
            Console.WriteLine("停止侦听,重启！");
            threadWatch.Abort();
            socketServer.Close();

            IP.Port = serverPort;
            SocketServer();
        }




        public static void WatchConnect()
        {
            while (true)
            {
                Socket watchConnect = socketServer.Accept();//接收连接并返回一个新的Socket
                if (watchConnect != null)
                {
                    RecvLog recvLog = new RecvLog(watchConnect);
                    recvLogs.Add(recvLog);
                    watchConnect.Send(Encoding.UTF8.GetBytes("服务器连接成功"));//在客户端显示"服务器连接成功"提示
                }

                for (int i = (recvLogs.Count) - (1); i >= 0; i--)
                {
                    if (recvLogs[i].isRuning == false)
                    {
                        recvLogs[i].thisSocket.Shutdown(SocketShutdown.Both);
                        recvLogs[i].thisSocket.Close();
                        recvLogs[i] = null;
                        recvLogs.RemoveAt(i);
                    }
                }

                Thread.Sleep(200);
            }
        }


        public static DateTime GetTime()
        {
            DateTime now = new DateTime();
            now = DateTime.Now;
            return now;
        }
    }
}
