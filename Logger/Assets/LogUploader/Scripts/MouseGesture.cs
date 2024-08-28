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

        public System.Action<bool> Call;

        Point downPoint;
        Point topPoint;

        //屏幕左边中心点
        Bounds boundsZero = new Bounds(new Vector3(0, Screen.height / 2, 0), Vector3.one * 200);

        //屏幕右边中心点
        Bounds boundsTop  = new Bounds(new Vector3(Screen.width,Screen.height/2,0), Vector3.one * 200);

        public class Point
        {
            public Bounds bouns;
            public bool  isActive;

            int   localCount = 0;
            float localTime  = 1.0f;

            public Point(Bounds bounds)
            {
                this.bouns = bounds;
            }

            public void Complete()
            {
                isActive = false;
                localCount = 0;
            }

            public void Check(float time)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (bouns.Contains(Input.mousePosition))
                    {
                        if (localCount == 0 || localCount == 1)
                        {
                            localCount++;
                            localTime = time;
                        }
                        else if (localCount == 2)
                        {
                            localTime = time;
                            isActive = true;
                        }
                    }
                }

                if (isActive)
                {
                    if (Time.time - localTime > 2.5f)
                    {
                        isActive = false;
                    }
                }
                else 
                {
                    if (localCount > 0)
                    {
                        if (Time.time - localTime > 1.0f)
                        {
                            localCount = 0;
                        }
                    }
                }
            }
        }

        public MouseGesture()
        {
            topPoint  = new Point(boundsTop);
            downPoint = new Point(boundsZero);
        }




        public void Check()
        {
            topPoint .Check(Time.time);
            downPoint.Check(Time.time);

            if (topPoint.isActive && downPoint.isActive)
            {
                topPoint .Complete();
                downPoint.Complete();
                isOpenCameraBack = !isOpenCameraBack;
                Call?.Invoke(isOpenCameraBack);
            }
        }
    }
}

