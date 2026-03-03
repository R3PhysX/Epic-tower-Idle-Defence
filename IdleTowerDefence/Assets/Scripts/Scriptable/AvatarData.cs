using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AvatarInfo", menuName = "Avatar/AvatarInfo")]
public class AvatarData : ScriptableObject
{
    [NonReorderable] public List<AvatarInfo> avatarInfo;
}

[System.Serializable]
public class AvatarInfo
{
    public int id;
    public Sprite avatar;
}