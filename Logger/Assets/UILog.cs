using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UILog : MonoBehaviour
{
    private Button myButton;

    private VisualElement logElement;

private VisualElement titleElement;

private VisualElement contentElement;



    void OnEnable()
    {
        // 获取 UI 文档的根 VisualElement
        var root = GetComponent<UIDocument>().rootVisualElement;


        contentElement = root.Q<ScrollView>("Scroll");

        Debug.Log("contentElement: " + contentElement);

    }


   

}
