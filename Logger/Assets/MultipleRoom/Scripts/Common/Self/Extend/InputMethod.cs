//=======================================================
// 作者：LR
// 公司：广州纷享科技发展有限公司
// 描述：
// 创建时间：2021-06-21 14:12:20
//=======================================================
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace fs
{
    public enum KeyType
    {
        KeyDown,
        KeyStay,
        KeyExit
    }

  
    public class InputKey
    {
        bool inDown;
        bool inStay;
        bool inExit;

        public bool isPass = false;


        KeyCode key;

        InputData inputData;

        /// <summary>
        /// info事件对应的数组
        /// </summary>
        List<MethodInfo> downMethods = new List<MethodInfo>();
        List<MethodInfo> stayMethods = new List<MethodInfo>();
        List<MethodInfo> exitMethods = new List<MethodInfo>();


        public InputKey(InputData inputData, KeyCode key)
        {
            this.key = key;
            this.inputData = inputData;

            CallUnit.updateCall.AddListener(Update);
        }

        bool CheckCondition()
        {
            return (Input.GetKey(key) && inputData.playerID == InputMethod.m_PlayerID) || isPass;
        }


        void DownEvents()
        {
            for (int i = 0; i < downMethods.Count; i++)
            {
                downMethods[i].Invoke(inputData.instance, null);
            }
        }

        void StayEvents()
        {
            for (int i = 0; i < stayMethods.Count; i++)
            {
                stayMethods[i].Invoke(inputData.instance, null);
            }
        }

        void UpEvents()
        {
            for (int i = 0; i < exitMethods.Count; i++)
            {
                exitMethods[i].Invoke(inputData.instance, null);
            }
        }

        bool IsHas()
        {
            if (exitMethods.Count <= 0 && stayMethods.Count <= 0)
                return false;
            return true;
        }

        public void AddMethodInfo(KeyType type, MethodInfo info)
        {
            if (type == KeyType.KeyDown)
                downMethods.Add(info);
            else if (type == KeyType.KeyStay)
                stayMethods.Add(info);
            else if(type == KeyType.KeyExit)
                exitMethods.Add(info);
        }



        public void Update()
        {
            if (CheckCondition())
            {
                if (inStay)
                {
                    StayEvents();
                }

                if (!inDown)
                {
                    inDown = true;
                    inStay = true;

                    DownEvents();

                    if (!IsHas()) Replace();
                }
            }
            else
            {
                if (inDown && !inExit)
                {
                    inExit = true;
                    UpEvents();

                    Replace();
                }
            }
        }


        public void Replace()
        {
            inDown = false;
            inStay = false;
            inExit = false;

            isPass = false;
        }

        public void RemoveAll()
        {
            for (int i = 0; i < downMethods.Count; i++)
            {
                downMethods[i] = null;
            }
            downMethods.Clear();

            for (int i = 0; i < stayMethods.Count; i++)
            {
                stayMethods[i] = null;
            }
            stayMethods.Clear();

            for (int i = 0; i < exitMethods.Count; i++)
            {
                exitMethods[i] = null;
            }
            exitMethods.Clear();

            CallUnit.updateCall.RemoveListener(Update);
        }
    }


    public class InputMethod 
    {
        public static List<int> playerIDs = new List<int>();
        public static List<InputKey> keyInputs = new List<InputKey>();

        public static Dictionary<int, InputData> intInputs = new Dictionary<int, InputData>();
        public static Dictionary<KeyCode, List<int>> keyCodeInputs = new Dictionary<KeyCode, List<int>>();

        /// <summary>
        /// 按不同的KEY，触发对用事件
        /// </summary>
        /// <param name="keyCode">按键</param>
        public static void InputDownTrigger(KeyCode keyCode, int playerID = 0)
        {
            if (keyCodeInputs.ContainsKey(keyCode))
            {
                for (int i = 0; i < keyCodeInputs[keyCode].Count; i++)
                {
                    if (intInputs.ContainsKey(keyCodeInputs[keyCode][i]))
                    {
                        if (intInputs[keyCodeInputs[keyCode][i]].playerID == playerID)
                            intInputs[keyCodeInputs[keyCode][i]].TriggerInfos(keyCode);
                    }
                }
            }
        }


        /// <summary>
        /// 按不同的KEY，触发对用事件
        /// </summary>
        /// <param name="keyCode">按键</param>
        public static void InputExitTrigger(KeyCode keyCode, int playerID = 0)
        {
            if (keyCodeInputs.ContainsKey(keyCode))
            {
                for (int i = 0; i < keyCodeInputs[keyCode].Count; i++)
                {
                    if (intInputs.ContainsKey(keyCodeInputs[keyCode][i]))
                    {
                        if (intInputs[keyCodeInputs[keyCode][i]].playerID == playerID)
                            intInputs[keyCodeInputs[keyCode][i]].ExitInfos(keyCode);
                    }
                }
            }
        }


        public static int m_PlayerID = 0;

    }

    /// <summary>
    /// Input数据
    /// </summary>
    public class InputData
    {
        public Dictionary<KeyCode, InputKey> keyInfos = new Dictionary<KeyCode, InputKey>();

        internal int playerID;// 脚本分配的玩家ID
        internal object instance;// 脚本的实例

        /// <summary>
        /// 缓存脚本实例
        /// </summary>
        public InputData(object target, int playerID)
        {
            instance = target;
            this.playerID = playerID;
        }



        /// <summary>
        /// 添加input控制事件
        /// </summary>
        public void AddInfos(KeyCode key, KeyType keyType, MethodInfo info)
        {
            if (keyInfos.ContainsKey(key) == false)
            {
                InputKey temp = new InputKey(this, key);
                keyInfos.Add(key, temp);
                InputMethod.keyInputs.Add(temp);
            }
            keyInfos[key].AddMethodInfo(keyType, info);
        }


        /// <summary>
        /// 触发对应key的函数
        /// </summary>
        /// <param name="key"></param>
        public void TriggerInfos(KeyCode key)
        {
            if (keyInfos.ContainsKey(key))
                keyInfos[key].isPass = true;
        }



        /// <summary>
        /// 退出对应key的函数
        /// </summary>
        /// <param name="key"></param>
        public void ExitInfos(KeyCode key)
        {
            if (keyInfos.ContainsKey(key))
                keyInfos[key].isPass = false;
        }


        /// <summary>
        /// 删除所有的info函数
        /// </summary>
        public void RemoveAllInfos()
        {
            foreach (var item in keyInfos.Values)
            {
                item.RemoveAll();
            }
            keyInfos.Clear();
            instance = null;
        }
    }

    /// <summary>
    /// 输出输入函数特性
    /// </summary>
    public class InputAttribute : System.Attribute
    {
        /// <summary>
        /// 触发的KEY
        /// </summary>
        public KeyCode keyCode;

        /// <summary>
        /// 触发KEY的状态
        /// </summary>
        public KeyType keyType;


        #region 构造函数
        /// <summary>
        /// 构造函数不同的用法
        /// </summary>
        public InputAttribute() 
        {
            keyType = KeyType.KeyDown;
        }

        public InputAttribute(KeyCode keyCode) : this(keyCode, KeyType.KeyDown)
        {
            this.keyCode = keyCode;
        }


        public InputAttribute(KeyCode keyCode, KeyType keyType)
        {
            this.keyCode = keyCode;
            this.keyType = keyType;
        }
        #endregion
    }

    /// <summary>
    /// 特性数据处理
    /// </summary>
    public static class InputAttributeManager
    {
        /// <summary>
        /// 加入需要检测按钮事件的脚本
        /// </summary>
        /// <param name="target"></param>
        /// <param name="playerID"></param>
        public static void AddInputEvents(this object target, int playerID = 0)
        {
            var fields = target.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            InputData tempEvent = new InputData(target, playerID);

            for (int i = 0; i < fields.Length; i++)
            {
                InputAttribute[] attrs = fields[i].GetCustomAttributes(typeof(InputAttribute), false) as InputAttribute[];
                if (attrs != null && attrs.Length > 0)
                {
                    tempEvent.AddInfos(attrs[0].keyCode, attrs[0].keyType, fields[i]);

                    Debug.Log("Input实例化按键：" + attrs[0].keyCode.ToString());

                    if (InputMethod.keyCodeInputs.ContainsKey(attrs[0].keyCode))
                        InputMethod.keyCodeInputs[attrs[0].keyCode].Add(target.GetHashCode());
                    else
                    {
                        List<int> temp = new List<int>();
                        temp.Add(target.GetHashCode());
                        InputMethod.keyCodeInputs.Add(attrs[0].keyCode, temp);
                    }
                }
            }

            if (!InputMethod.playerIDs.Contains(playerID))
                InputMethod.playerIDs.Add(playerID);


            InputMethod.intInputs.Add(target.GetHashCode(), tempEvent);
        }


        /// <summary>
        /// 移除检测的脚本
        /// </summary>
        /// <param name="target"></param>
        public static void RemoveInputEvents(this object target)
        {
            if (InputMethod.intInputs.ContainsKey(target.GetHashCode()))
            {
                InputMethod.intInputs[target.GetHashCode()].RemoveAllInfos();
                InputMethod.intInputs.Remove(target.GetHashCode());
            }

            foreach (var item in InputMethod.keyCodeInputs.Values)
            {
                if (item.Contains(target.GetHashCode()))
                {
                    item.Remove(target.GetHashCode());
                }
            }
        }
    }
    //-----------------Input 特性---------------end
}
