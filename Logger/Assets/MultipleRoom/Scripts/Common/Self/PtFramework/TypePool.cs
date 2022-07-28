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

namespace Sailfish
{


	public class TypePool :MonoBehaviour
	{


		private void Start()
		{


		}




        private void Update()
        {

			if (Input.GetKeyDown(KeyCode.H))
			{
				UpdateTimer.Creater.PtDelayTimer(1.0f).OnComplete((_) =>
				{
					Debug.Log(_);
				});
			}
		}
    }


}
