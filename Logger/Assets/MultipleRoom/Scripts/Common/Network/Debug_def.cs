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


public class Debuger : MonoBehaviour
{

	public static void Log(string str)
	{
		Debug.Log(str);
	}
	public static void LogWarning(string str)
	{
		Debug.LogWarning(str);
	}
	public static void LogError(string str)
	{
		Debug.LogError(str);
	}

	public static void LogException(System.Exception exception)
	{
		Debug.LogException(exception);
	}
}

