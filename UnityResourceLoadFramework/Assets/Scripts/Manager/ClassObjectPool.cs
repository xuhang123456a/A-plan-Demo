using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassObjectPool<T> where T : class, new()
{
    //池
    protected Stack<T> Pool = new Stack<T>();

    // 最大对象个数，0表示不限个数
    protected int MaxCount = 0;

    // 没回收的对象
    protected int NoRecycleCount = 0;

    public ClassObjectPool(int maxCount)
    {
        this.MaxCount = maxCount;
        for (int i = 0; i < maxCount; i++)
        {
            Pool.Push(new T());
        }
    }

    /// <summary>
    /// 从池子里取类对象
    /// </summary>
    /// <param name="isPoolEmpty"></param>
    /// <returns></returns>
    public T Spawn(bool isPoolEmpty)
    {
        if (Pool.Count > 0)
        {
            T rtn = Pool.Pop();
            if (rtn == null)
            {
                if (isPoolEmpty)
                {
                    rtn = new T();
                }
            }

            NoRecycleCount++;
            return rtn;
        }
        else
        {
            if (isPoolEmpty)
            {
                T rtn = new T();
                NoRecycleCount++;
                return rtn;
            }
        }

        return null;
    }

    /// <summary>
    /// 回收类对象
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public bool Recycle(T obj)
    {
        if (obj == null)
            return false;

        NoRecycleCount--;
        if (Pool.Count > MaxCount && MaxCount > 0)
        {
            obj = null;
            return false;
        }

        Pool.Push(obj);
        return true;
    }
}