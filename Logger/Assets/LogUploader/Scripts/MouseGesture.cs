/// <summary>
/// 实现日志上传到服务器的功能
/// </summary>

using System.Collections.Generic;
using UnityEngine;

namespace Sailfish.Log
{
    public class MouseGesture
    {
        bool isOpenCameraBack = false;

        Vector3 startPos, endPos, middlePos;
        List<Vector3> middles = new List<Vector3>();

        float time = 0;

        public System.Action<bool> Call;

        public void Check()
        {
            if (Input.GetMouseButtonDown(0))
            {
                startPos = Input.mousePosition;
                middles.Clear();
                time = Time.time;
            }

            if (Input.GetMouseButton(0))
            {
                middles.Add(Input.mousePosition);
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (middles.Count < 2) return;

                endPos = Input.mousePosition;
                middlePos = middles[middles.Count / 2];

                if (Vector3.Distance(middlePos, startPos) > 540 && Vector3.Distance(middlePos, endPos)> 540)
                {

                    if (Time.time - time < 1.5f)
                    {
                        if (Angle(middlePos, startPos, endPos) < 60)
                        {
                            isOpenCameraBack = !isOpenCameraBack;
                            Call?.Invoke(isOpenCameraBack);
                        }
                    }
                }

                middles.Clear();
            }
        }



        double Angle(Vector3 cen, Vector3 first, Vector3 second)
        {
            const float M_PI = 3.1415926535897f;

            float ma_x = first.x - cen.x;
            float ma_y = first.y - cen.y;
            float mb_x = second.x - cen.x;
            float mb_y = second.y - cen.y;
            float v1 = (ma_x * mb_x) + (ma_y * mb_y);
            float ma_val = Mathf.Sqrt(ma_x * ma_x + ma_y * ma_y);
            float mb_val = Mathf.Sqrt(mb_x * mb_x + mb_y * mb_y);
            float cosM = v1 / (ma_val * mb_val);
            float angleAMB = Mathf.Acos(cosM) * 180.0f / M_PI;

            return angleAMB;
        }
    }
}

