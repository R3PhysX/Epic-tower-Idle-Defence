using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackAttribute", menuName = "Attributes/Attack Attribute")]
public class AttackAttributeData : ScriptableObject
{
    [NonReorderable] public List<AttributeData> attributes;
}
