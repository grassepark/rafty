using UnityEngine;
using UnityEngine.UI;

public class SettingsScript : MonoBehaviour
{
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public Slider alphaSlider;

    public AudioSource musicSource;
    public AudioSource sfxSource;

    public Material[] materials;

    private void Start()
    {
        musicVolumeSlider.value = musicSource.volume;
        sfxVolumeSlider.value = sfxSource.volume;
        musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
        alphaSlider.onValueChanged.AddListener(SetAlpha);
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }

    public void SetAlpha(float alpha)
    {
        foreach (Material mat in materials)
        {
            if (mat != null)
            {
                mat.SetFloat("_Alpha", alpha);
            }
        }
    }

}
