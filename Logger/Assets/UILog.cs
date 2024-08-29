using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class LogItem 
{
    public LogType logType;
    public string content;
    public string trace;
    public string timer;

    public LogItem(string trace, string content, LogType logType)
    {
        this.trace   = trace;
        this.content = content;
        this.logType = logType;
        this.timer = $"[<color=#BC331A>{DateTime.Now.ToString("yy/MM/dd  HH:mm:ss:ffff")}</color>]";
    }

    public string Value()
    {
        return $"{timer} {content}";
    }

    public override string ToString()
    { 
      return $"{timer} {content} \r\n {trace}" ;
    }
}

public class UILog : MonoBehaviour
{
    private Label titleElement;

    private VisualElement traceElement;

    private Label traceValue;

    Button selectElement;

    Color startColor = new Color(0.7058824f, 0.7058824f, 0.7058824f);

    private ScrollView contentElement;

    private Dictionary<Button, LogItem> itemButtons = new Dictionary<Button, LogItem>();

    public void AddLog(string log,string trace ,LogType logType)
    {   
        // 创建一个 VisualElement，用于显示日志信息
        var logItem    = new LogItem(trace, log, logType);
        var logElement = new Button();
        logElement.AddToClassList("item");
        logElement.text = logItem.Value();

        logElement.clicked += () =>
        {
            if (selectElement != null)
            {
                selectElement.style.backgroundColor = startColor;
            }
            logElement.style.backgroundColor = Color.white;
            Show(logItem);

            selectElement = logElement;
        };

        // 将日志信息添加到内容容器中
        contentElement.Add(logElement);

        itemButtons[logElement] = logItem;

        // 滚动到最底部
        contentElement.scrollOffset = Vector2.zero;
    }



    public void Show(LogItem logItem)
    {
        if (traceElement.style.display == DisplayStyle.None)
        {
            traceElement.style.display = DisplayStyle.Flex;
            traceValue.text = logItem.ToString();
        }
        else
        {
            traceValue.text = logItem.ToString();
        }
    }


    void OnDisable()
    {
        Application.logMessageReceivedThreaded -= AddLog;
    }


void Update()
{
        if (Input.GetKey(KeyCode.G))
        {
            Debug.Log("-----:"+Time.time);
        }
}

    void Awake()
    {
        Application.logMessageReceivedThreaded += AddLog;

        // 获取 UI 文档的根 VisualElement
        var root = GetComponent<UIDocument>().rootVisualElement;

        titleElement = root.Q<Label>("IP");
        titleElement.text = "IP: " + "_" +  $"{Application.productName}";

        contentElement = root.Q<ScrollView>("Scroll");
        contentElement.horizontalScrollerVisibility = ScrollerVisibility.Hidden;

        traceElement = root.Q<VisualElement>("Trace");
   


        traceValue = root.Q<Label>("tValue");
    //    uploadBtn  = root.Q<Button>("Upload");
     //   logElement = root.Q<VisualElement>("logItem");


        traceElement.style.display = DisplayStyle.None;
    }
}
