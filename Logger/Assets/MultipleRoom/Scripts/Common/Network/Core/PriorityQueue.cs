using System;
using System.Collections;
using System.Collections.Generic;

namespace fs
{
    /// <summary>
    /// 优先级队列
    /// NOTE:此类效率不高，数据量大时慎用
    /// 使用方式:
    /**
        class Que
        {
            public int priority;
            public int value;
            public Que(int p, int v)
            {
                this.priority = p;
                this.value = v;
            }
        }
        class QueComparer : IComparer<Que>
        {
            int IComparer<Que>.Compare(Que x, Que y)
            {
                return x.priority > y.priority ? 1 : ((x.priority < y.priority) ? -1 : 0);
            }
        }

        PriorityQueue<Que> que = new PriorityQueue<Que>(new QueComparer());
        que.Push(new Que(1, 1));
        que.Push(new Que(4, 4));
        que.Push(new Que(3, 3));
        que.Push(new Que(2, 2));
        foreach(var obj in que)
        {
            Debuger.Log(obj.priority.ToString());
        }
     */
    /// @author hannibal
    /// @time 2018-10-16
    /// </summary>
    public sealed class PriorityQueue<T> : IEnumerable<T>, IEnumerable
    {
        IComparer<T> comparer;              //比较
        List<T> heap = new List<T>();       //存放所有数据

        /// <summary>
        /// 初始化优先级队列
        /// </summary>
        /// <param name="comparer"></param>
        public PriorityQueue(IComparer<T> comparer)
        {
            this.comparer = (comparer == null) ? Comparer<T>.Default : comparer;
        }


        public void Push(T v)
        {
            int i = heap.BinarySearch(v, comparer);
            heap.Insert((i < 0) ? ~i : i, v);
        }

        /// <summary>
        /// 删除队列的最后一个数据，并返回最后一个数据
        /// </summary>
        /// <returns>最后一个数据</returns>
        public T Pop()
        {
            T v = heap[heap.Count - 1];
            heap.RemoveAt(heap.Count - 1);
            return v;
        }

        /// <summary>
        /// 返回最后一个数据
        /// </summary>
        public T Top()
        {
            if (Count > 0) return heap[heap.Count - 1];
            throw new InvalidOperationException("优先队列为空");
        }

        /// <summary>
        /// 队列的大小
        /// </summary>
        public int Count 
        {
            get { return heap.Count; } 
        }

        public NodeEnumerator GetEnumerator()
        {
            return new NodeEnumerator(this);
        }
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new NodeEnumerator(this);
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new NodeEnumerator(this);
        }

        /// <summary>
        /// 定义优先级队列的结构体
        /// </summary>
        public struct NodeEnumerator : IEnumerator<T>, IEnumerator
        {
            private PriorityQueue<T> list;              //优先级队列
            private int index;                          //队列最大索引
            private T current;                          

            /// <summary>
            /// 初始化优先级队列
            /// </summary>
            /// <param name="list"></param>
            internal NodeEnumerator(PriorityQueue<T> list)
            {
                this.list = list;
                index = list.Count - 1;
                current = default(T);
            }

            public void Dispose()
            {
            }

            /// <summary>
            /// 判断优先级队列是否为空
            /// </summary>
            /// <returns></returns>
            public bool MoveNext()
            {
                if (index >= 0)
                {
                    current = list.heap[index];
                    index--;
                    return true;
                }
                else
                {
                    index = -1;
                    current = default(T);
                    return false;
                }
            }

            public T Current
            {
                get
                {
                    return current;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }

            void IEnumerator.Reset()
            {
                index = list.Count - 1;
                current = default(T);
            }
        }
    }
}

