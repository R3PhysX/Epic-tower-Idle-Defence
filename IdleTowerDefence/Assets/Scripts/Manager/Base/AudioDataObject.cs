using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Audio/AudioDataObject")]
public class AudioDataObject : ScriptableObject
{
    [NonReorderable]public List<AudioData> audiosDataList;
}

[System.Serializable]
public class AudioData
{
    public AudioClipsType clipType;
    public AudioClip audioClip;
}