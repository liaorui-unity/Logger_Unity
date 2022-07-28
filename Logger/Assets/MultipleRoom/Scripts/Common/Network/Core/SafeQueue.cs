
//=======================================================
// 作者：liusumei
// 公司：广州纷享科技发展有限公司
// 描述：线程安全的队列
// 1.ConcurrentQueue需要在.NET Framework 4使用
// 创建时间：2019-07-15 11:56:01
//=======================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace fs
{
	public class SafeQueue<T>
	{
        private Queue<T> m_Queue = new Queue<T>();
        private object m_LockObj = new object();
        
        public int Count { get { return m_Queue.Count; } }

        public void Clear()
        {
            lock(m_LockObj)
            {
                m_Queue.Clear();
            }
        }
        public bool Contains(T item)
        {
            return m_Queue.Contains(item);
        }
        public void CopyTo(T[] array, int idx)
        {
            m_Queue.CopyTo(array, idx);
        }
        public T Dequeue()
        {
            lock (m_LockObj)
            {
                return m_Queue.Dequeue();
            }
        }
        public void Enqueue(T item)
        {
            lock (m_LockObj)
            {
                m_Queue.Enqueue(item);
            }
        }
        public T Peek()
        {
            lock (m_LockObj)
            {
                return m_Queue.Peek();
            }
        }
    }
}
