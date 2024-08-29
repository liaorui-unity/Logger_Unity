using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Timers;

namespace LogRecv
{
    internal class RecvLog
    {
        public bool isRuning = false;

        public Socket thisSocket;

        public RecvLog(Socket socket)
        {
            thisSocket = socket;

            isRuning = true;

            ThreadPool.QueueUserWorkItem(ReceiveMsg, thisSocket);
        }

     

        public void ShowProgress(float proc)
        {
            Console.WriteLine("Process.recv -> [ {0} ]", (proc * 100).ToString() + "%");
        }

        public void ReceiveMsg(object watchConnect)
        {
            Socket socketServer = watchConnect as Socket;
            long fileLength = 0;//文件长度
            string recFileStr = null;//文件名
            string recFolderStr = null;//文件名

            while (socketServer != null)
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
                            Console.WriteLine("客户端接收到信息：" + socketServer.LocalEndPoint.ToString() + "\r\n" + Program.GetTime() + "\r\n" + recMsg + "\r\n");
                        }

                        if (buffer[0] == 1)//1对应文件信息
                        {
                            RecvFile(socketServer, fileLength, recFileStr, recFolderStr, firstRcv, buffer);
                        }


                        if (buffer[0] == 2)//2对应文件名字和长度
                        {
                            RecvFileData(out fileLength, out recFileStr, out recFolderStr, firstRcv, buffer);
                        }

                        if (buffer[0] == 3)//3对应接收结束
                        {
                            RecvFinish(socketServer, firstRcv, buffer);
                            isRuning = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    isRuning = false;
                    Console.WriteLine("系统异常..." + ex.Message);
                    break;
                }
            }
        }

        private  void RecvFile(Socket socketServer, long fileLength, string recFileStr, string recFolderStr, int firstRcv, byte[] buffer)
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
            }
            Console.WriteLine("保存成功！！！！");
        }

        private  void RecvFileData(out long fileLength, out string recFileStr, out string recFolderStr, int firstRcv, byte[] buffer)
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


        private  void RecvFinish(Socket client, int firstRcv, byte[] buffer)
        {
            string verify = Encoding.UTF8.GetString(buffer, 1, firstRcv - 1);
            Console.WriteLine("接收到：" + verify);

            if (verify == "upload finish")
            {
                isRuning = false;
            }
        }
    }
}
