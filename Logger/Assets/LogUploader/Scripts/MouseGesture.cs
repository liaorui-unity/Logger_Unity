/// <summary>
/// 实现日志上传到服务器的功能
/// </summary>

using System.Collections.Generic;
using UnityEngine;

namespace Sailfish.Log
{
    public class MouseGesture
    {
        public int _islock;
        public int IsLock
        {
            get
            {
                _islock = PlayerPrefs.GetInt($"{Application.productName}_MouseGesture", 0);
                return _islock;
            }
            set
            {

                PlayerPrefs.SetInt($"{Application.productName}_MouseGesture", value);
                _islock = value;
            }
        }


        private bool isOpenCameraBack = false;
        public System.Action<bool> Call;
        private List<Vector2> mousePositions = new List<Vector2>();

        private float time = 2.0f;
        private float runTime = 0;
        public void Check()
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (Input.GetKeyDown(KeyCode.L))
                {
                    Debug.Log("Lock MouseGesture:"+ IsLock);
                    var value = IsLock;
                    IsLock = value== 1 ? 0 : 1;
                }
            }

            if (IsLock==1)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    mousePositions.Clear();
                }

                if (Input.GetMouseButton(0))
                {
                    runTime = Time.time;
                    mousePositions.Add(Input.mousePosition);
                }

                if (Input.GetMouseButtonUp(0))
                {
                    if (Time.time - runTime < time)
                    {
                        if (IsCircleGesture(mousePositions))
                        {
                            isOpenCameraBack = !isOpenCameraBack;
                            Call?.Invoke(isOpenCameraBack);
                        }
                    }
                }
            }
        }

        private bool IsCircleGesture(List<Vector2> positions)
        {
            if (positions.Count < 20) // 增加点数要求
                return false;

            Vector2 start = positions[0];
            Vector2 middle = positions[positions.Count / 2];
            Vector2 end = positions[positions.Count - 1];

            float dis1 = Vector2.Distance(start, middle);
            float dis2 = Vector2.Distance(start, end);

            if (dis2 > dis1 )
                return false;

            if (IsAcuteAngle(start, middle, end))
                return false; // 如果夹角小于 90°

            return true;
        }

        private bool IsAcuteAngle(Vector2 start, Vector2 middle, Vector2 end)
        {
            Vector2 startToMiddle = start - middle;
            Vector2 middleToEnd = end - middle;

            float dotProduct = Vector2.Dot(startToMiddle.normalized, middleToEnd.normalized);
            float angle = Mathf.Acos(dotProduct) * Mathf.Rad2Deg;

            return angle >= 90f;
        }
    }
}

