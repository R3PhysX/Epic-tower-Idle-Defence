using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public enum AudioChannel { Master, Sfx, Music };

public enum AudioClipsType
{
    UITap = 1,
    BGM = 2,
    CloudIn = 3,
    CloudOut = 4,
    DeadEffect = 5,
    BattleStart = 6,
    WarEnd = 7,
    Bullet = 8,
    BasicEnemy = 9,
    MidEnemy = 10,
    BulletEnemy = 11,
    BossEnemy = 12,
    TownDamage = 13,
    CoinCollectSound = 14,
    BombSound,
    InfernoSound,
    RewardMultiplierTick,
    ChestUnlock,
    CardUpgrade,
    W2_BasicEnemy,
    W2_MidEnemy,
    W2_BulletEnemy,
    W2_BossEnemy,
    W2_DeadEffect,
    DiceButtonPressed,
    PlayerMoveDice
}

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;
    public static AudioManager Instance
    {
        get
        {
            return _instance;
        }
    }

    [SerializeField] private AudioSource uiTapAudioSource;
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioSource[] sfxAudioSource;
    [SerializeField] private AudioDataObject audioDataObject;

    public float masterVolumePercent { get; private set; }
    public float sfxVolumePercent { get; private set; }
    public float musicVolumePercent { get; private set; }

    private Dictionary<AudioClipsType, AudioData> audioDictionary = new Dictionary<AudioClipsType, AudioData>();

    private void Awake()
    {
        _instance = this;
        masterVolumePercent = PlayerPrefs.GetFloat("master vol");
        sfxVolumePercent = PlayerPrefs.GetFloat("sfx vol");
        musicVolumePercent = PlayerPrefs.GetFloat("music vol");
    }

    private void Start()
    {
        for (int i = 0; i < audioDataObject.audiosDataList.Count; i++)
        {
            if (!audioDictionary.ContainsKey(audioDataObject.audiosDataList[i].clipType))
            {
                audioDictionary.Add(audioDataObject.audiosDataList[i].clipType, audioDataObject.audiosDataList[i]);
            }
        }
    }

    internal void RefreshSoundState()
    {
        //Set data from user state -------------
        //isSfxOn = PlayerData.Instance.isSfxOn;
        //isMusicOn = PlayerData.Instance.isMusicOn;
    }

    internal void PlayUITapSound()
    {
        if (ActiveGameData.Instance.saveData.SoundEffect == 0)
            return;

        if (uiTapAudioSource.isPlaying)
            uiTapAudioSource.Stop();

        if(uiTapAudioSource.clip == null)
        {
            AudioData audioData = GetAudioClip(AudioClipsType.UITap);

            if (audioData == null)
                return;

            uiTapAudioSource.clip = audioData.audioClip;
          // uiTapAudioSource.volume = sfxVolumePercent;
        }

        uiTapAudioSource.Play();
    }

    private AudioData GetAudioClip(AudioClipsType audioClipData)
    {
        AudioData audioData = null;
        if (audioDictionary.ContainsKey(audioClipData))
            audioData = audioDictionary[audioClipData];

        return audioData;
    }

    internal void PlaySFXSound(AudioClipsType audioClipType, Vector3 sourcePosition = default)
    {
        if (ActiveGameData.Instance.saveData.SoundEffect == 0)
            return;

        int audioSourceIndex = -1;
        for(int i = 0; i < sfxAudioSource.Length; i++)
        {
            if (!sfxAudioSource[i].isPlaying)
            {
                audioSourceIndex = i;
                break;
            }
        }

        if (audioSourceIndex == -1)
        {
            audioSourceIndex = 0;
            sfxAudioSource[audioSourceIndex].Stop();
        }

        AudioData audioData = null;
        if (audioDictionary.ContainsKey(audioClipType))
            audioData = audioDictionary[audioClipType];

        if (audioData == null)
            return;

        sfxAudioSource[audioSourceIndex].transform.position = sourcePosition;
        sfxAudioSource[audioSourceIndex].clip = audioData.audioClip;
      //  sfxAudioSource[audioSourceIndex].volume = sfxVolumePercent;
        sfxAudioSource[audioSourceIndex].Play();
    }

    #region BGMUSIC

    internal void PlayBackGroundMusic(AudioClipsType audioClipsType)
    {
        if (ActiveGameData.Instance.saveData.MusicEffect == 0)
            return;

        AudioData audioData = null;
        if (audioDictionary.ContainsKey(audioClipsType))
            audioData = audioDictionary[audioClipsType];

        if (audioData == null)
            return;

        musicAudioSource.clip = audioData.audioClip;
        //  musicAudioSource.volume = musicVolumePercent;
        musicAudioSource.Play();

    }

    internal void StopBackGroundMusic()
    {
        musicAudioSource?.Stop();
    }

    #endregion
    internal void SetVolume(float volumePercent, AudioChannel channel)
    {
        switch (channel)
        {
            case AudioChannel.Master:
                masterVolumePercent = volumePercent;
                break;
            case AudioChannel.Sfx:
                sfxVolumePercent = volumePercent;
                break;
            case AudioChannel.Music:
                musicVolumePercent = volumePercent;
                break;
        }

        PlayerPrefs.SetFloat("master vol", masterVolumePercent);
        PlayerPrefs.SetFloat("sfx vol", sfxVolumePercent);
        PlayerPrefs.SetFloat("music vol", musicVolumePercent);
        PlayerPrefs.Save();
    }
}