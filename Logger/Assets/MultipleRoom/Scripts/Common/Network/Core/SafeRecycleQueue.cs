using UnityEngine;

namespace fs
{
    /// <summary>
    /// 定长、线程安全、可重复使用的循环队列
    /// @author hannibal
    /// @time 2019-4-9
    /// </summary>
    public class SafeRecycleQueue<T>
    {
        private object m_lock_obj = new object();       
        private int m_start, m_end;                     //循环队列的头与尾

        private T[] values;                             //存放队列中的所有数据
        
        public int Count { get; private set; }          //队列中元素的个数


        /// <summary>
        /// 初始化循环队列
        /// </summary>
        /// <param name="max">队列的长度</param>
        public SafeRecycleQueue(int max)
        {
            Clear();
            values = new T[max];
        }

        /// <summary>
        /// 入队
        /// </summary>
        /// <param name="cur">插入的数据</param>
        public void Enqueue(T cur)
        {
            lock (m_lock_obj)
            {
                values[m_end] = cur;

                m_end++;
                m_end %= values.Length;
                //如果达到头部，覆盖
                if (m_end == m_start)
                {
                    m_start++;
                    m_start %= values.Length;
                }

                Count = Mathf.Clamp(++Count, 0, values.Length);
            }
        }

        /// <summary>
        /// 删除队列中的第一个数据
        /// </summary>
        /// <returns>删除的数据信息</returns>
        public T Dequeue()
        {
            if(Count <= 0)
            {
                throw new System.Exception("queue is empty");
            }

            T cur = default(T);
            lock (m_lock_obj)
            {
                cur = values[m_start];
                m_start++;
                m_start %= values.Length;

                Count = Mathf.Clamp(--Count, 0, values.Length);
            }
            return cur;
        }

        /// <summary>
        /// 返回循环队列的第一个元素
        /// </summary>
        public T Peek()
        {
            T cur = values[m_start];
            return cur;
        }

        /// <summary>
        /// 返回循环队列的最后一个元素
        /// </summary>
        public T PeekEnd()
        {
            T cur = values[m_end];
            return cur;
        }

        /// <summary>
        /// 清空循环队列
        /// </summary>
        public void Clear()
        {
            lock(m_lock_obj)
            {
                m_start = m_end = 0;
                Count = 0;
            }
        }
    }
}