using System.Collections;
using System.Collections.Generic;
using System;

using GameObject = UnityEngine.GameObject;

public class ListPool<T> : IDisposable where T : UnityEngine.Component
{
    T prefab;
    CObjectList<T> _objects;

    public T GetComponent { get { return _objects.GetObject(); } }

    public ListPool(T _component)
    {
        this.prefab =_component;
        _objects = new CObjectList<T>(
            0,
            () =>
            {
                T c = GameObject.Instantiate(prefab) as T;
                c.gameObject.SetActive(false);
                return c;
            },
            (T _obj) => {
                return !_obj.gameObject.activeInHierarchy;
            }
            );
    }

    public void Dispose()
    {
        _objects.Destroy();
        prefab = null;
        // Suppress finalization.
        GC.SuppressFinalize(this);
    }
}

public class CObjectList<T> where T : class
{
    public delegate T FuncT();
    public delegate bool FuncBOOL(T temp);

    FuncT create_fn;
    FuncBOOL check_fn;

    BetterList<T> objects_;

    public CObjectList(short count, FuncT fn_create, FuncBOOL fn_check)
    {
        this.create_fn = fn_create;
        this.check_fn = fn_check;
        this.objects_ = new BetterList<T>();
        InitObject(count);
    }

    ~ CObjectList()
    {
        Destroy();
        GC.SuppressFinalize(this);
    }

    void InitObject(short InitCount)
    {
        for (int i = 0; i < InitCount; ++i)
        {
            this.objects_.Add(this.create_fn());
        }
    }

    public T GetObject(int _idx)
    {
        if (_idx >= objects_.size) return null;
        return objects_[_idx];
    }

    public T GetObject()
    {
        //foreach(T tmp in this.objects_)
        for (int i = 0; i < objects_.size; ++i)
        {
            T tmp = objects_[i];
            if (check_fn(tmp) == true)
            {
                return tmp;
            }
        }

        return AddPool();
    }

    public T AddPool()
    {
        T temp = this.create_fn();
        this.objects_.Add(temp);
        return temp;
    }

    public void Destroy()
    {
        if (objects_ != null && objects_.size > 0)
        {
            objects_.Clear(); 
        }
        objects_ = null;
        create_fn = null;
        check_fn = null;
    }

    public int count
    {
        get { return this.objects_.size; }
    }
}

public class CObjectListToDic<U, T> where T : class
{
    public delegate T FuncT(U iType);
    public delegate bool FuncBOOL(T temp);
    public Dictionary<U, List<T>> dictionary
    {
        get { return objects_; }
    }

    FuncT create_fn;
    FuncBOOL check_fn;
    Dictionary<U, List<T>> objects_;

    public CObjectListToDic(FuncT fn_create, FuncBOOL fn_check)
    {
        this.create_fn = fn_create;
        this.check_fn = fn_check;

        this.objects_ = new Dictionary<U, List<T>>();
    }

    public int Count
    {
        get { return this.objects_.Count; }
    }

    public bool ContainKey(U iKey)
    {
        return !this.objects_.ContainsKey(iKey);
    }

    public T getObject(U iKey)
    {
        //foreach(KeyValuePair<int, List<T>> pair in this.objects_)
        if (this.objects_.ContainsKey(iKey) == false)
        {
            List<T> tempList = new List<T>();
            this.objects_.Add(iKey, tempList);

            T tempReturn = this.create_fn(iKey);
            tempList.Add(tempReturn);

            return tempReturn;
        }

        List<T> tempFind = this.objects_[iKey];
        T tmp;

        for (int i = 0; i < tempFind.Count; ++i) //foreach(T tmp in tempFind)
        {
            tmp = tempFind[i];
            if (tmp == null)
            {
                tmp = this.create_fn(iKey);
                tempFind.Add(tmp);
                return tmp;
            }

            if (check_fn(tmp) == true)
            {   return tmp;  }
        }

        T temp = this.create_fn(iKey);
        tempFind.Add(temp);

        return temp;
    }

    public void Destroy()
    {
        if (objects_ != null && objects_.Count > 0)
        { objects_.Clear(); }
        objects_ = null;
        create_fn = null;
        check_fn = null;
    }
}

public class CObjectDictionary<U, T> where T : class
{
    public delegate object FuncT(U iType);
    public delegate bool FuncBOOL(T temp);
    FuncT create_fn;
    Dictionary<U, T> objects_;

    public CObjectDictionary(FuncT fn_create)
    {
        this.create_fn = fn_create;
        this.objects_ = new Dictionary<U, T>();
    }

    public void SetObject(U iKey, T obj)
    {
        if (CheckObject(iKey))
        {
            objects_.Add(iKey, obj);
        }
    }

    public int getObjectCount()
    {
        return this.objects_.Count;
    }

    public bool CheckObject(U iKey)
    {
        return !this.objects_.ContainsKey(iKey);
    }

    public T getObject(U iKey)
    {
        //foreach(KeyValuePair<int, List<T>> pair in this.objects_)

        if (this.objects_.ContainsKey(iKey) == false)
        {
            T tempReturn = (T)this.create_fn(iKey);
            this.objects_.Add(iKey, tempReturn);

            return tempReturn;
        }

        T tmp = null;
        bool bReadValue = this.objects_.TryGetValue(iKey, out tmp);
        if (!bReadValue)
        {
            tmp = (T)this.create_fn(iKey);
            if (CheckObject(iKey))
            { this.objects_.Add(iKey, tmp); }
        }
        return tmp;
    }

    public void Destroy()
    {
        if (objects_ != null)
        {
            if (objects_.Count > 0)
            { objects_.Clear(); }
        }
        
        objects_ = null;
        create_fn = null;
    }
}
