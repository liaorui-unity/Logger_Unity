
//=======================================================
// 作者：liusumei
// 公司：广州纷享科技发展有限公司
// 描述：线程安全的列表
// 创建时间：2019-07-16 18:07:24
//=======================================================
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace fs
{
	public sealed class SafeList<T> : IEnumerable<T>, IEnumerable
    {
        private List<T> m_List = new List<T>();
        private object m_LockObj = new object();

        public T this[int index] { get {return m_List[index]; } set { lock (m_LockObj) m_List[index] = value; } }

        public int Count { get { return m_List.Count; } }
        public int Capacity { get { return m_List.Capacity; } set { m_List.Capacity = value; } }

        public void Add(T item) { lock (m_LockObj) m_List.Add(item); }
        public void AddRange(IEnumerable<T> collection) { lock (m_LockObj) m_List.AddRange(collection); }
        public int BinarySearch(int index, int count, T item, IComparer<T> comparer) { return m_List.BinarySearch(index, count, item, comparer); }
        public int BinarySearch(T item) { return m_List.BinarySearch(item); }
        public int BinarySearch(T item, IComparer<T> comparer) { return m_List.BinarySearch(item); }
        public void Clear() { lock (m_LockObj) m_List.Clear(); }
        public bool Contains(T item) { return m_List.Contains(item); }
        public void CopyTo(T[] array, int arrayIndex) { m_List.CopyTo(array, arrayIndex); }
        public void CopyTo(T[] array) { m_List.CopyTo(array); }
        public void CopyTo(int index, T[] array, int arrayIndex, int count) { m_List.CopyTo(index, array, arrayIndex, count); }
        public bool Exists(Predicate<T> match) { return m_List.Exists(match); }
        public T Find(Predicate<T> match) { return m_List.Find(match); }
        public List<T> FindAll(Predicate<T> match) { return m_List.FindAll(match); }
        public int FindIndex(int startIndex, int count, Predicate<T> match) { return m_List.FindIndex(startIndex, count, match); }
        public int FindIndex(int startIndex, Predicate<T> match) { return m_List.FindIndex(startIndex, match); }
        public int FindIndex(Predicate<T> match) { return m_List.FindIndex(match); }
        public T FindLast(Predicate<T> match) { return m_List.FindLast(match); }
        public int FindLastIndex(int startIndex, int count, Predicate<T> match) { return m_List.FindLastIndex(startIndex, count,  match); }
        public int FindLastIndex(int startIndex, Predicate<T> match) { return m_List.FindLastIndex(startIndex, match); }
        public int FindLastIndex(Predicate<T> match) { return m_List.FindLastIndex(match); }
        public void ForEach(Action<T> action) { m_List.ForEach(action); }
        public List<T> GetRange(int index, int count) { return m_List.GetRange(index, count); }
        public int IndexOf(T item, int index, int count) { return m_List.IndexOf(item, index, count); }
        public int IndexOf(T item, int index) { return m_List.IndexOf(item, index); }
        public int IndexOf(T item) { return m_List.IndexOf(item); }
        public void Insert(int index, T item) { lock (m_LockObj) m_List.Insert(index, item); }
        public void InsertRange(int index, IEnumerable<T> collection) { lock (m_LockObj) m_List.InsertRange(index, collection); }
        public int LastIndexOf(T item) { return m_List.LastIndexOf(item); }
        public int LastIndexOf(T item, int index) { return m_List.LastIndexOf(item, index); }
        public int LastIndexOf(T item, int index, int count) { return m_List.LastIndexOf(item, index, count); }
        public bool Remove(T item) { lock (m_LockObj) return m_List.Remove(item); }
        public int RemoveAll(Predicate<T> match) { lock (m_LockObj) return m_List.RemoveAll(match); }
        public void RemoveAt(int index) { lock (m_LockObj) m_List.RemoveAt(index); }
        public void RemoveRange(int index, int count) { lock (m_LockObj) m_List.RemoveRange(index, count); }
        public void Reverse(int index, int count) { lock (m_LockObj) m_List.Reverse(index, count); }
        public void Reverse() { lock (m_LockObj) m_List.Reverse(); }
        public void Sort(Comparison<T> comparison) { lock (m_LockObj) m_List.Sort(comparison); }
        public void Sort(int index, int count, IComparer<T> comparer) { lock (m_LockObj) m_List.Sort(index, count, comparer); }
        public void Sort() { lock (m_LockObj) m_List.Sort(); }
        public void Sort(IComparer<T> comparer) {lock(m_LockObj) m_List.Sort(comparer); }
        public T[] ToArray() { return m_List.ToArray(); }

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
        public struct NodeEnumerator : IEnumerator<T>, IEnumerator
        {
            private SafeList<T> list;
            private int index;
            private T current;

            internal NodeEnumerator(SafeList<T> list)
            {
                this.list = list;
                index = list.Count - 1;
                current = default(T);
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (index >= 0)
                {
                    current = list.m_List[index];
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

            bool IEnumerator.MoveNext()
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
