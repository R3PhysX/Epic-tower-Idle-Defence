using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CoinAttribute", menuName = "Attributes/Coin Attribute")]
public class CoinAttributeData : ScriptableObject
{
    [NonReorderable] public List<AttributeData> attributes;
}