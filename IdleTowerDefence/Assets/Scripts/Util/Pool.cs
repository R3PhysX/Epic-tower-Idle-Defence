using System.Collections.Generic;

internal class Pool<T> : System.IDisposable where T : class
{
    protected List<T> _pool;
    private int _maxCapacity;

    public Pool(int maxCapacity)
    {
        _pool = new List<T>(maxCapacity);
        _maxCapacity = maxCapacity;
    }

    internal virtual void Add(T obj)
    {
        if (obj != null)
        {
            _pool?.Add(obj);
        }
    }

    internal T Get()
    {
        var v_obj = default(T);
        if (_pool.Count == 0)
        {
            v_obj = CreateObject();
        }
        else
        {
            v_obj = _pool[0];
            _pool.RemoveAt(0);
        }
        return v_obj;
    }

    internal T Get(System.Type type)
    {
        var v_obj = default(T);
        foreach (var v_item in _pool)
        {
            if (v_item.GetType() == type)
            {
                v_obj = v_item;
                _pool.Remove(v_item);
                break;
            }
        }
        if (v_obj == null)
        {
            v_obj = CreateObject(type);
        }
        return v_obj;
    }

    private T CreateObject(System.Type type)
    {
        return (T)System.Activator.CreateInstance(type);
    }

    protected virtual T CreateObject()
    {
        return CreateObject(typeof(T));
    }

    public virtual void Dispose()
    {
        _pool?.Clear();
        _pool = null;
    }
}
