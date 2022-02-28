using UnityEngine;
using UnityEngine.Audio;
using System;

public class AudioManagerScript : MonoBehaviour
{
    public SoundScript[] sounds;
    public static AudioManagerScript instance;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);;
        foreach (SoundScript sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;

            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
        }
    }

    public void Play(string name)
    {
        SoundScript sound = Array.Find(sounds, s => s.name == name);
        if (sound == null)
        {
            Debug.Log("Sound: " + name + " not found!");
            return;
        }
        sound.source.Play();
    }

    public void Stop(string name)
    {
        SoundScript sound = Array.Find(sounds, s => s.name == name);
                if (sound == null)
        {
            Debug.Log("Sound: " + name + " not found!");
            return;
        }

        sound.source.volume = sound.volume * (1f + UnityEngine.Random.Range(-sound.volumeVariance / 2f, sound.volumeVariance / 2f));
        sound.source.pitch = sound.pitch * (1f + UnityEngine.Random.Range(-sound.pitchVariance / 2f, sound.pitchVariance / 2f));

        sound.source.Stop ();

    }
}
