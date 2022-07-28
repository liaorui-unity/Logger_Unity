using UnityEngine;
using System.Collections;

namespace fs
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        protected static T m_instance = null;

        public static T instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = FindObjectOfType<T>();

                    if (FindObjectsOfType<T>().Length > 1)
                    {
                        Debuger.LogError("存在多个单例:" + typeof(T).Name);
                        return m_instance;
                    }

                    if (m_instance == null)
                    {
                        string instanceName = typeof(T).Name;
                        GameObject instanceGO = GameObject.Find(instanceName);
                        if (instanceGO == null)
                            instanceGO = new GameObject(instanceName);
                        m_instance = instanceGO.AddComponent<T>();
                    }
                }
                return m_instance;
            }
        }

        protected virtual void Awake()
        {
            if (m_instance != null)
            {
                Debuger.LogWarning(string.Format("存在多个单例:{0}，移除旧的", typeof(T).Name));
                GameObject.Destroy(m_instance.GetComponent(typeof(T)));
            }
            m_instance = this as T;
        }
    }
    public abstract class DontMonoSingleton<T> : MonoBehaviour where T : DontMonoSingleton<T>
    {
        protected static T m_instance = null;

        public static T instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = FindObjectOfType<T>();

                    if (FindObjectsOfType<T>().Length > 1)
                    {
                        Debuger.LogError("存在多个单例:" + typeof(T).Name);
                        return m_instance;
                    }

                    if (m_instance == null)
                    {
                        string instanceName = typeof(T).Name;
                        GameObject instanceGO = GameObject.Find(instanceName);
                        if (instanceGO == null)
                            instanceGO = new GameObject(instanceName);
                        m_instance = instanceGO.AddComponent<T>();
                        DontDestroyOnLoad(instanceGO);
                    }
                }
                return m_instance;
            }
        }
        protected virtual void Awake()
        {
            if (typeof(T).Name != gameObject.name)
            {
                Debuger.LogError(string.Format("DnotMonoSingleton({0}) 不能直接挂到对象上", typeof(T).Name));
            }
        }
    }
}