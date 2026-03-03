using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Unity pool will always store the internal pool as Game Object types.
/// </summary>
internal class UnityPool : Pool<Object>
{
    protected List<GameObject> _prefabList;
    protected Transform _parent;
    protected bool _createInParent;

    /// <summary>
    /// Create a new Unity Pool
    /// </summary>
    /// <param name="prefab">The prefab from which new pooled objects will be created</param>
    /// <param name="maxCapacity"></param>
    /// <param name="parent">If defined all pool objects will be created under this parent</param>
    /// <returns></returns>
    internal UnityPool(GameObject prefab, int maxCapacity, Transform parent = default, bool createInParent = false) : base(maxCapacity)
    {
        _prefabList = new List<GameObject>(1);
        _prefabList.Add(prefab);
        _parent = parent;
        _createInParent = createInParent;
    }

    internal UnityPool(List<GameObject> prefabList, int maxCapacity, Transform parent = default) : base(maxCapacity)
    {
        _prefabList = new List<GameObject>(prefabList.Count);
        _prefabList.AddRange(prefabList);
        _parent = parent;
    }

    internal override void Add(Object obj)
    {
        if (obj == null)
        {
            return;
        }
        var v_go = (obj is GameObject) ? ((GameObject)obj) : ((Component)obj).gameObject;
        base.Add(v_go);
        v_go.SetActive(false);
        v_go.transform.SetParent(_parent, true);
    }

    protected override Object CreateObject()
    {
        if (_createInParent)
        {
            var v_objectInParent = (GameObject)Object.Instantiate(_prefabList[0], _parent);
            return v_objectInParent;
        }
        var v_object = (GameObject)Object.Instantiate(_prefabList[0]);
        return v_object;
    }

    /// <summary>
    /// Get a pool object. Either a GameObject or any component attached to G.O can be returned.
    /// </summary>
    /// <param name="newParent">New parent for this pool object</param>
    /// <param name="shouldSearchChildren">If returning component should it be searched in children</param>
    /// <typeparam name="T">Can only either be G.O or Component</typeparam>
    /// <returns></returns>
    internal T Get<T>(Transform newParent = default, bool shouldSearchChildren = false) where T : Object
    {
        var v_object = Get();
        var v_transform = ((GameObject)v_object).transform;
        var v_returnVal = default(T);
        if (typeof(T) != typeof(GameObject))
        {
            //Find a component and return.
            v_returnVal = shouldSearchChildren ? v_transform.GetComponentInChildren<T>() : v_transform.GetComponent<T>();
        }
        else
        {
            //Directly return the type T.
            v_returnVal = (T)v_object;
        }
        ((GameObject)v_object).SetActive(true);
        v_transform.SetParent(newParent, true);
        return v_returnVal;
    }
}
