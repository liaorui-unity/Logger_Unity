//=======================================================
// 作者：LR
// 公司：广州旗博士科技有限公司
// 描述：工具人
// 创建时间：#CreateTime#
//=======================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace fs
{
	public class UIMsg
	{
		public UIMsg(params object[] args)
		{
			m_Args = args;
		}
		public object[] m_Args;
	}

	

	public struct TypeMsg
	{
		public Dictionary<string, System.Action<UIMsg>> m_Msgs;
	}


	public class UIMessage  
	{

		static Dictionary<System.Type, TypeMsg> m_Types = new Dictionary<System.Type, TypeMsg>();


		internal static TypeMsg FindType<T>()
		{
			TypeMsg typeMsg;
			if (m_Types.TryGetValue(typeof(T), out typeMsg))
			{
				return typeMsg;
			}
			else
			{
				typeMsg.m_Msgs = new Dictionary<string, System.Action<UIMsg>>();
				m_Types[typeof(T)] = typeMsg;
			}
			return typeMsg;
		}

		internal void AddTypeMsg<T>( string msgKey, System.Action<UIMsg> msgCall)
		{
			var typeMsg = FindType<T>();
			typeMsg.m_Msgs[msgKey] = msgCall;
		}

		internal static void RemoveTypeMsg<T>(string msgKey)
		{
			var typeMsg = FindType<T>();

			if (typeMsg.m_Msgs.ContainsKey(msgKey))
				typeMsg.m_Msgs.Remove(msgKey);
		}

		internal static void RemoveAllTypeMsg<T>()
		{
			var typeMsg = FindType<T>();
			typeMsg.m_Msgs.Clear();
			m_Types.Remove(typeof(T));
		}

	}

	public static class UIMessageExtension
    {

		public static void AddTypeMsg<T>(this T target, string msgKey, System.Action<UIMsg> msgCall)
		{
			var typeMsg = UIMessage.FindType<T>();
			typeMsg.m_Msgs[msgKey] = msgCall;
		}

		public static void RemoveTypeMsg<T>(this T target, string msgKey)
		{
			UIMessage.RemoveTypeMsg<T>(msgKey);
		}

		public static void RemoveAllTypeMsg<T>(this T target)
		{
			UIMessage.RemoveAllTypeMsg<T>();
		}

		public static void TriggerUIMsg<T>(this T target, string msgKey, UIMsg msg)
		{
			var msgType = UIMessage.FindType<T>();

			System.Action<UIMsg> msgCall;
			if (msgType.m_Msgs.TryGetValue(msgKey, out msgCall))
			{
				msgCall?.Invoke(msg);
			}
		}
	}
}
