using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsController : MonoBehaviour
{
    [SerializeField] AudioMixer mixer;
    [SerializeField] Slider masterSlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider masterSFXSlider;

    private const string masterVolumeFloatName = "MasterVolume";
    private const string musicVolumeFloatName = "MusicVolume";
    private const string masterSFXVolumeFloatName = "SFXVolume";
    private const float volumeCoefficient = 20f;
    private const float minSliderValue = 0.0001f;
    private const float maxSliderValue = 1f;

    private void Awake()
    {
        masterSlider.onValueChanged.AddListener(MasterVolumeChangeValue);
        musicSlider.onValueChanged.AddListener(MusicVolumeChangeValue);
        masterSFXSlider.onValueChanged.AddListener(MasterSFXVolumeChangeValue);
    }

    private void MasterVolumeChangeValue(float value)
    {
        mixer.SetFloat(masterVolumeFloatName, CalculateVolume(value));
    }

    private void MusicVolumeChangeValue(float value)
    {
        mixer.SetFloat(musicVolumeFloatName, CalculateVolume(value));
    }

    private void MasterSFXVolumeChangeValue(float value)
    {
        mixer.SetFloat(masterSFXVolumeFloatName, CalculateVolume(value));
    }

    private float CalculateVolume(float value)
    {
        float clampedValue = Mathf.Clamp(value, minSliderValue, maxSliderValue);
        return Mathf.Log10(clampedValue) * volumeCoefficient;
    }
}
