//=======================================================
// 作者：LR
// 公司：广州旗博士科技有限公司
// 描述：工具人
// 创建时间：#CreateTime#
//=======================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
//using Mirror;

namespace NET
{
    public class SyncPacketMsg : MonoBehaviour
    {
    //    static Queue<SyncMessage> msgQues = new Queue<SyncMessage>();
    //    static Dictionary<ushort, Action<SyncMsg>> packets = new Dictionary<ushort, Action<SyncMsg>>();

    //    public static void AddMsg(SyncMessage msg)
    //    {
    //        msgQues.Enqueue(msg);
    //    }


    //    public static void Register<T>(Action<T> action) where T : SyncMsg
    //    {
    //        var header = SyncPacketPools.GetID<T>();

    //        if (!packets.ContainsKey(header))
    //        {
    //            packets.Add(header, (_) => { action?.Invoke(_ as T); });
    //        }
    //    }

    //    public static ushort RegisterUshort<T>(Action<T> action) where T : SyncMsg
    //    {
    //        var header = SyncPacketPools.GetID<T>();

    //        if (!packets.ContainsKey(header))
    //        {
    //            packets.Add(header, (_) => { action?.Invoke(_ as T); });
    //        }
    //        return header;
    //    }


    //    static SyncMessage syncMsg;
    //    public static void SendSyncRoom<T>(T msg, SyncMode mode = SyncMode.Other) where T : SyncMsg
    //    {
    //        if (RoomClient.instance.currentRoom != null)
    //        {
    //            if (!string.IsNullOrEmpty(RoomClient.instance.currentRoom.OwnerUID))
    //            {
    //                sendMsgBytes.Clear();
    //                msg.Write(sendMsgBytes);

    //                syncMsg.header = msg.header;
    //                syncMsg.modeID = mode;
    //                syncMsg.msgBytes = sendMsgBytes.Buffer;

    //                syncMsg.roomID = RoomClient.instance.currentRoom.OwnerUID;
    //                NetworkClient.Send(syncMsg);
    //            }
    //        }
    //    }



    //    public static void SendSyncRoom<T>(SyncValue<T> msg, SyncMode mode = SyncMode.Other)
    //    {
    //        if (RoomClient.instance.currentRoom != null)
    //        {
    //            if (!string.IsNullOrEmpty(RoomClient.instance.currentRoom.OwnerUID))
    //            {
    //                sendMsgBytes.Clear();
    //                msg.Write(sendMsgBytes);

    //                syncMsg.header = msg.header;
    //                syncMsg.modeID = mode;
    //                syncMsg.msgBytes = sendMsgBytes.Buffer;
    //                syncMsg.roomID = RoomClient.instance.currentRoom.OwnerUID;
    //                NetworkClient.Send(syncMsg);
    //            }
    //        }
    //    }



    //    private void Update()
    //    {
    //        while (msgQues.Count > 0)
    //        {
    //            var msg = msgQues.Dequeue();
    //            Analysis(msg);
    //        }
    //    }


    //    static ByteArray analysisBytes = new ByteArray(0, 4096);
    //    static ByteArray sendMsgBytes = new ByteArray(0, 4096);
    //    public void Analysis(SyncMessage msg)
    //    {
    //        Action<SyncMsg> action = null;

    //        if (packets.TryGetValue(msg.header, out action))
    //        {
    //            analysisBytes.Clear();
    //            analysisBytes.WriteBytes(msg.msgBytes, msg.msgBytes.Length);

    //            var packet = SyncPacketPools.Get(msg.header);
    //            action?.Invoke(packet);
    //            SyncPacketPools.Recover(packet);
    //        }
    //    }

    //    private void OnDestroy()
    //    {
    //        packets.Clear();
    //        msgQues.Clear();
    //    }
    }
}



