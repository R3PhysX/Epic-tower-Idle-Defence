using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttributePanel : MonoBehaviour
{
    [SerializeField] protected ScrollRect attributeParent;

    [SerializeField] protected AttributeItem itemPrefab;
    [SerializeField] protected FactoryAttributeItem factoryItemPrefab;

    protected Dictionary<string, IAttribute> attributes = new Dictionary<string, IAttribute>();

    internal List<FactoryAttributeItem> factoryAttributeItems = new List<FactoryAttributeItem>();
    internal List<AttributeItem> attributeItems = new List<AttributeItem>();
    public bool isFactory;

    internal virtual void InitializeAttributes()
    {
        
    }

    protected virtual IAttribute CreateAttributeInstance(AttributeData attributeData)
    {
        string attributeName = attributeData.attributeClassName;

        string fullClassName = typeof(Player).Namespace + "." + attributeName.Replace(" ", "");

        Type attributeType = Type.GetType(fullClassName);
        if (attributeType != null)
        {
            if (typeof(IAttribute).IsAssignableFrom(attributeType))
            {
                return (IAttribute)Activator.CreateInstance(attributeType);
            }
            else
            {
                Debug.LogError(attributeName + " does not implement IAttribute.");
            }
        }
        else
        {
            Debug.LogError("Attribute class not found: " + fullClassName);
        }

        return null;
    }
}
