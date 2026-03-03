using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HealthAttribute", menuName = "Attributes/Health Attribute")]
public class HealthAttributeData : ScriptableObject
{
    [NonReorderable] public List<AttributeData> attributes;
}