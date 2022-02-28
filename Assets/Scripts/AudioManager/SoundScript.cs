using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class SoundScript
{
    public string name;
    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume;
    [Range(0f, 1f)]
	public float volumeVariance = .1f;
    [Range(.1f, 3f)]
    public float pitch;
    [Range(0f, 1f)]
	public float pitchVariance = .1f;
    public bool loop = false;

    [HideInInspector]
    public AudioSource source;
}
