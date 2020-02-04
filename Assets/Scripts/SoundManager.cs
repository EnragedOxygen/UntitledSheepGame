using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{   
    public AudioMixerGroup mixerGroup;
    private AudioSource audioSource;

    public AudioClip clip;
    public string clipName;  

    [Range (0,1)]
    public float volume=1;
    [Range(0, 1)]
    public float pitch=1;

    public bool loop = false;
    public bool playOnAwake = false;

    public void SetSource(AudioSource m_audioSource)
    {
        this.audioSource = m_audioSource;
        this.audioSource.outputAudioMixerGroup = mixerGroup;       
        this.audioSource.clip = clip;
        this.audioSource.pitch = pitch;
        this.audioSource.volume = volume;
        this.audioSource.playOnAwake = playOnAwake;
        this.audioSource.loop = loop;
    }

    public void Play()
    {
        this.audioSource.Play();
    }



}

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

   [SerializeField]
    Sound[] sounds;

    private void Awake()
    {
        // Простой синглтон
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }


    private void Start()
    {
        for(int i=0; i < sounds.Length; i++)
        {
            GameObject obj = new GameObject("Sound_" + i + "_" + sounds[i].clipName);
            obj.transform.SetParent(transform);
            sounds[i].SetSource(obj.AddComponent<AudioSource>());
        }

        PlaySound("Music");
    }

    public void PlaySound(string soundName)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].clipName == soundName)
            {
                sounds[i].Play();
            }
        }
    }
}
