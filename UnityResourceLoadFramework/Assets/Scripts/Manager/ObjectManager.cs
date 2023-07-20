using System;
using System.Collections.Generic;

public class ObjectManager : Singleton<ObjectManager>
{
    protected Dictionary<Type, object> m_ClassPoolDic = new Dictionary<Type, object>();

    public ClassObjectPool<T> GetOrCreateClassPool<T>(int maxCount ) where T:class,new()
    {
        Type t = typeof(T);
        if (!m_ClassPoolDic.TryGetValue(t, out var outObj)|| outObj == null)
        {
            ClassObjectPool<T> newPool = new ClassObjectPool<T>(maxCount);
            m_ClassPoolDic.Add(t,newPool);
            return newPool;
        }

        return outObj as ClassObjectPool<T>;
    }
}
