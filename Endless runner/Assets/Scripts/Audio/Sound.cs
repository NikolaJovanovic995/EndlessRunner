using UnityEngine;

[CreateAssetMenu(menuName = "My Assets/Sound")]
public class Sound : ScriptableObject
{
    [SerializeField] string soundName;
    [SerializeField] AudioClip audioClip;
    [Range(0f, 1f)]
    [SerializeField] float volume;
    [SerializeField] MixerGroup mixerGroup;

    public string Name => soundName;
    public AudioClip AudioClip => audioClip;
    public float Volume => volume;
    public MixerGroup MixerGroup => mixerGroup;
}

public enum MixerGroup
{
    MUSIC,
    SFX
}
