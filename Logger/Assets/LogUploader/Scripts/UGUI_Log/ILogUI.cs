using System.Collections;
using System.Collections.Generic;
using Sailfish.Log;
using UnityEngine;
using UnityEngine.Accessibility;

public interface ILogUI 
{
    void Init(Transform panel);
    void AddLog(LogData data);
    void SelectLog(LogData data);

    void Set(bool isOpen);
    
}
