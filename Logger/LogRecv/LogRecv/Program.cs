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
        public static string serverIp = "127.0.0.1";//设置服务端IP
        public static int serverPort = 8765;//服务端端口

        [STAThread]
        static void Main(string[] args)
        {
            SocketServer();
        }


        public static Socket socketServer;//定义socket
        public static Thread threadWatch;//定义线程
        public static byte[] result = new byte[1024 * 1024 * 2];//定义缓存
        public static string filePath = "";//存储保存文件的路径


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
                    watchConnect.Send(Encoding.UTF8.GetBytes("服务器连接成功"));//在客户端显示"服务器连接成功"提示
                    Thread threadwhat = new Thread(ReceiveMsg);//创建一个接受信息的进程
                    threadwhat.IsBackground = true;//后台启动
                    threadwhat.Start(watchConnect);//有传入参数的线程
                }
                Thread.Sleep(100);
            }
        }


        public static DateTime GetTime()
        {
            DateTime now = new DateTime();
            now = DateTime.Now;
            return now;
        }


        public static void ShowProgress(float proc)
        {
            Console.WriteLine("Process.recv -> [ {0} ]", (proc * 100).ToString() + "%");
        }

        public static void ReceiveMsg(object watchConnect)
        {
            Socket socketServer = watchConnect as Socket;
            long fileLength = 0;//文件长度
            string recFileStr = null;//文件名
            string recFolderStr = null;//文件名

            while (socketServer!=null)
            {
                int firstRcv = 0;
                byte[] buffer = new byte[1024 * 1024 * 5];
                try
                {
                    //获取接受数据的长度，存入内存缓冲区，返回一个字节数组的长度
                    firstRcv = socketServer.Receive(buffer);

                    if (firstRcv > 0)//大于0，说明有东西传过来
                    {
                        if (buffer[0] == 0)//0对应文字信息
                        {
                            string recMsg = Encoding.UTF8.GetString(buffer, 1, firstRcv - 1);
                            Console.WriteLine("客户端接收到信息：" + socketServer.LocalEndPoint.ToString() + "\r\n" + GetTime() + "\r\n" + recMsg + "\r\n");
                        }

                        if (buffer[0] == 1)//1对应文件信息
                        {
                            RecvFile(socketServer, fileLength, recFileStr, recFolderStr, firstRcv, buffer);
                        }


                        if (buffer[0] == 2)//2对应文件名字和长度
                        {
                            RecvFileData(out fileLength, out recFileStr,out recFolderStr, firstRcv, buffer);
                        }

                        if (buffer[0] == 3)//3对应验证信息
                        {
                            RecvVerify(socketServer, firstRcv,buffer);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("系统异常..." + ex.Message);
                    break;
                }
            }
        }

        private static void RecvFile(Socket socketServer, long fileLength, string recFileStr,string recFolderStr, int firstRcv, byte[] buffer)
        {
            bool firstWrite = true;

            int rec = 0;
            long recProc = 0;
            long recFileLength = 0;
            long recCycle = fileLength / 20;

            string savePath = SaveFile.GetSaveFolder(recFileStr, recFolderStr);

            Console.WriteLine(recFileStr);

            using (FileStream fs = new FileStream(savePath, FileMode.Create, FileAccess.Write))
            {
                while (recFileLength < fileLength)
                {
                    if (firstWrite)
                    {
                        firstWrite = false;
                        fs.Write(buffer, 1, firstRcv - 1);
                        fs.Flush();
                        recFileLength += firstRcv - 1;
                    }
                    else
                    {
                        rec = socketServer.Receive(buffer);
                        fs.Write(buffer, 0, rec);
                        fs.Flush();
                        recFileLength += rec;
                    }

                    recProc += rec;
                    if (recProc >= recCycle)
                    {
                        recProc = 0;
                        ShowProgress(recFileLength / fileLength);
                    }
                }
                fs.Close();
            }

            Console.WriteLine("保存成功！！！！");
        }

        private static void RecvFileData(out long fileLength, out string recFileStr, out string recFolderStr, int firstRcv, byte[] buffer)
        {
            string fileNameWithLength = Encoding.UTF8.GetString(buffer, 1, firstRcv - 1);
            Console.WriteLine("接收到：" + fileNameWithLength);

            string[] recStrs = fileNameWithLength.Split('*');

            recFileStr = recStrs.First();//获取文件名
            recFolderStr = recStrs[1];
            fileLength = Convert.ToInt64(recStrs.Last());//获取文件长度

            Console.WriteLine("接收到的文件夹：" + recFolderStr);
            Console.WriteLine("接收到的文件名为：" + recFileStr);
            Console.WriteLine("接收到的文件长度为：" + fileLength);
        }


        private static void RecvVerify(Socket client, int firstRcv, byte[] buffer)
        {
            string verify = Encoding.UTF8.GetString(buffer, 1, firstRcv - 1);
            Console.WriteLine("接收到：" + verify);

            if (verify == "Logger")
            {
                Console.WriteLine("返回验证：" + verify);

                byte[] newBuffer = Encoding.UTF8.GetBytes("验证成功");
                byte[] sendBuffer = new byte[newBuffer.Length + 1];

                sendBuffer[0] = 1;
                Buffer.BlockCopy(newBuffer, 0, sendBuffer, 1, newBuffer.Length);
                client.Send(sendBuffer);//发送验证信息
            }
        }
    }
}
