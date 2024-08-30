using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FPSDisplay : MonoBehaviour
{
    private float deltaTime = 0.0f;
    private float updateInterval = 0.5f;
    private float timeSinceLastUpdate = 0.0f;

    public Color showColor = Color.white;

    public UnityAction<string> OnUpdateFPS;
    void Update()
    {
        // 计算帧时间
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        timeSinceLastUpdate += Time.unscaledDeltaTime;

        // 每 0.5 秒更新一次 FPS 显示
        if (timeSinceLastUpdate >= updateInterval)
        {
            UpdateFPSLabel();
            timeSinceLastUpdate = 0.0f;
        }
    }

    void UpdateFPSLabel()
    {
        // 计算 FPS
        float fps = 1.0f / deltaTime;

        // 根据 FPS 值设置颜色
        if (fps >= 60)
        {
            showColor = Color.green;
        }
        else if (fps < 30)
        {
            showColor = Color.yellow;
        }
        else if (fps < 24)
        {
            showColor = Color.red;
        }

        OnUpdateFPS?.Invoke(fps.ToString("0.0"));
    }
}
