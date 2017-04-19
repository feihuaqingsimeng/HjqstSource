using System;
using System.Collections;
using System.Collections.Generic;
public class CArray<T> : ICollection<T> where T : class
{
    public delegate void EventAdd(int index, T item);
    public delegate void EventUpdate(int index, T item);
    public delegate void EventRemove(int index, T item);
    public delegate void EventClear();
    public delegate void EventResize(int count);
    public delegate void EventUpdateAll();

    public EventUpdateAll eventUpdateAll;
    public EventAdd eventAdd;
    public EventUpdate eventUpDate;
    public EventRemove eventRemove;
    public EventClear eventClear;
    public EventResize eventResize;

    private bool isReadOnly = true;
    //public int place = 0;
    private T[] array;
    private int _count;

    #region constructed function
    public CArray(List<T> list)
    {
        this.array = list.ToArray();
        _count = list.Count;
    }

    public CArray(T[] _array)
    {
        Count = _array.Length;
        Array.Copy(_array, this.array, _array.Length);
    }

    public CArray(int count)
    {
        this.Count = count;
    }

    #endregion

    public T this[int index]
    {
        get
        {
            if (index < 0 || index > _count - 1)
                throw new ArgumentOutOfRangeException();
            else
                return array[index];
        }
        set
        {
            if (index < 0 || index > _count - 1)
                throw new ArgumentOutOfRangeException();
            else
                array[index] = value;
        }
    }

    public void Add(T item)
    {
        int index = FindNullIndex();
        if (index == -1)
            Debugger.LogError("列表已满！");
        this[index] = item;
        if (eventAdd != null) eventAdd(index, item);
    }

    public void Update(T item, int index)
    {
        this[index] = item;
        if (eventUpDate != null) eventUpDate(index, item);
    }

    public T GetAt(int index)
    {
        return this[index];
    }

    public bool Remove(T item)
    {
        int index = array.IndexOf(item);
        this[index] = default(T);
        if (eventRemove != null)
            eventRemove(index, item);
        return true;
    }

    public bool Remove(int index)
    {
        T item = this[index];
        this[index] = default(T);
        if (eventRemove != null)
            eventRemove(index, item);
        return true;
    }

    //删除元素并排序
    public void RemoveResort(int index)
    {
        this[index] = default(T);
        for (int i = index; i < _count - 1; i++)
        {
            this[index] = this[index + 1];
        }
    }

    /// <summary>
    /// 从前往后寻找第一个找空格子
    /// </summary>
    /// <returns></returns>
    public int FindNullIndex()
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (this[i] == null)
            {
                return i;
            }
        }
        return -1;
    }

    //找是否有指定数量空格子
    public bool FindNullNum(int num)
    {
        int num1 = 0;
        for (int i = 0; i < _count; i++)
        {
            if (this[i] == null)
            {
                num1++;
                if (num1 == num) return true;
            }
        }
        return false;
    }

    public void Clear()
    {
        for (int i = 0; i < _count; i++)
        {
            this[i] = default(T);
        }
        _count = 0;
        this.array = null;
        if (eventClear != null)
            eventClear();
    }

    public bool Contains(T item)
    {
        for (int i = 0; i < _count; i++)
        {
            if (this[i] == default(T)) continue;
            if (this[i].Equals(item)) return true;
        }
        return false;
    }

    public void CopyTo(T[] targetArray, int arrayIndex)
    {
        for (int i = arrayIndex; i < this.array.Length; i++)
        {
            targetArray[i] = this[i];
        } 
    }

    public int Count
    {
        get { return _count; }
        private set
        {
            _count = value;
            if (null == array)
                array = new T[_count];
            for (int i = 0; i < _count; i++)
                array[i] = default(T);
        }
    }

    /// <summary>
    /// 重新设置容器的大小。
    /// 提示：该不方法是不被建议频繁使用的，因为实际是重新new一个数组，将原有的数据copy过去，频繁使用的代价会非常高。
    /// </summary>
    /// <param name="size">大小，且size 大于容器当前的大小</param>
    public void Resize(int size)
    {
        if (_count >= size) return;
        if (null == this.array)
            Count = size;
        else
        {
            T[] tempArray = new T[size];
            Array.Copy(this.array, tempArray, _count);
            this.array = tempArray;
            _count = size;
        }
        if (eventResize != null) eventResize(_count);
    }

    public bool IsReadOnly
    {
        get { return isReadOnly; }
    }

    public IEnumerator<T> GetEnumerator()
    {
        if (this.array != null)
        {
            for (int i = 0; i < this.array.Length; i++)
            {
                yield return this[i];
            }
        }
    }

    public List<T> Clone()
    {
        return array.ToList<T>();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
