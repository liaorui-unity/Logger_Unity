using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sailfish.Log
{
    public class LogUploader
    {
        Socket socketClient;//定义socket
        long mb = 1024 * 1024 * 5;

        public LogUploader()
        {
            ThreadPool.SetMinThreads(1, 1);
            ThreadPool.SetMaxThreads(3, 3);
        }



        public void Upload()
        {
            if (SocketClient())
            {
                SendFile();
            }
        }


        public void Clear()
        {
            socketClient.Shutdown(SocketShutdown.Both);//禁止Socket上的发送和接受
            socketClient.Close();//关闭Socket并释放资源
        }

        private bool SocketClient()
        {
            socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//创建一个socket的对象
            IPAddress ip = IPAddress.Parse(Debug.debugTypeLAN ? Debug.uploadLAN : Debug.uploadNET);//获取服务器IP地址
            IPEndPoint point = new IPEndPoint(ip, Debug.uploadPort);//获取端口
            try
            {
                socketClient.Connect(point);//链接服务器IP与端口
                Debug.Log("连接服务器中.....");
            }
            catch (Exception ex)
            {
                Debug.Log("与服务器链接失败！！！" + ex);
                return false;
            }

            Debug.Log("与服务器链接成功！:"+ ip);

            return true;
        }


        private async void SendFile()
        {
            Debug.isUploadDebug = true;
            try
            {
                using (FileStream fsRead = new FileStream(Debug.logFileSavePath, FileMode.OpenOrCreate, FileAccess.Read))
                {
                    //1. 第一步：发送一个文件，表示文件名和长度，让客户端知道后续要接收几个包来重新组织成一个文件
                    await Task.Run(() => { SendFileInfo(Debug.logFileSavePath, fsRead); });

                    //2第二步：每次发送一个1MB的包，如果文件较大，则会拆分为多个包
                    await Task.Run(() => { SendFileBytes(fsRead); });

                    //3第三步：发送关闭消息
                    await Task.Run(() => { SendFinish(); });
                }
                Debug.Log("发送完成");
            }
            catch
            {
                Debug.Log("发送失败");
            }
         
            Debug.isUploadDebug = false;
            Clear();
        }


        private void SendFinish()
        {
            byte[] buffer = Encoding.UTF8.GetBytes("upload finish");
            byte[] newBuffer = new byte[buffer.Length + 1];

            newBuffer[0] = 3;
            Buffer.BlockCopy(buffer, 0, newBuffer, 1, buffer.Length);
            socketClient.Send(newBuffer);//完成后，发送完成消息过去
        }


        private void SendFileInfo(string filePath, FileStream fsRead)
        {
            string fileName = Debug.debugFileDate;
        
            string folder = string.Format("{0}/{1}/{2}", Debug.debugName, Debug.debugIp, Debug.debugDate);
          //Debug.Log("发送的文件夹是：" + folder);

            long fileLength = fsRead.Length;//文件长度
          //Debug.Log("发送的文件长度为：" + fileLength);

            string totalMsg = string.Format("{0}*{1}*{2}", fileName, folder, fileLength);

            byte[] buffer = Encoding.UTF8.GetBytes(totalMsg);
            byte[] newBuffer = new byte[buffer.Length + 1];

            newBuffer[0] = 2;
            Buffer.BlockCopy(buffer, 0, newBuffer, 1, buffer.Length);
            socketClient.Send(newBuffer);//发送文件前，将文件名和长度发过去
        }



        private void SendFileBytes(FileStream fsRead)
        {

            int    readLength = 0;  //定义读取的长度
            bool   firstRead = true;
            byte[] Filebuffer = new byte[mb];//定义1MB的缓存空间

            long   sendProc = 0;
            long   sendFileLength = 0;
            long   sendAllLength = fsRead.Length;
            long   sendCycle = fsRead.Length / 100;

            while ((readLength = fsRead.Read(Filebuffer, 0, Filebuffer.Length)) > 0)
            {
                if (sendFileLength >= sendAllLength)
                    break;

                sendFileLength += readLength;
                // 第一次发送的字节流上加个前缀1
                if (firstRead)
                {
                    firstRead = false;

                    byte[] firstBuffer = new byte[readLength + 1];
                    firstBuffer[0] = 1;//标记1，代表为文件

                    Buffer.BlockCopy(Filebuffer, 0, firstBuffer, 1, readLength);
                    socketClient.Send(firstBuffer, 0, readLength + 1, SocketFlags.None);

                    continue;
                }

                socketClient.Send(Filebuffer, 0, readLength, SocketFlags.None);

                sendProc += readLength;
                if (sendProc >= sendCycle)
                {
                    sendProc = 0;
                    ShowProgress(sendFileLength / sendAllLength);
                }
            }

            ShowProgress(1);
        }
        void ShowProgress(float proc)
        {
             Debug.Log(string.Format("Process.recv -> [  {0}%  ]", (proc * 100).ToString()));
        }
    }
}
