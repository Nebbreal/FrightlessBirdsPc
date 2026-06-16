using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OperatorVolumeChange : MonoBehaviour
{
    [SerializeField]
    public Slider MasterVolumeSlider;
    public Slider MusicVolumeSlider;
    public Slider SFXVolumeSlider;
    public AudioMixer MasterMixer;


    void Awake()
    {
        
        MasterVolumeSlider.value = SettingsManager.Instance.Settings.Volume.masterVolume;
        MusicVolumeSlider.value = SettingsManager.Instance.Settings.Volume.musicVolume;
        SFXVolumeSlider.value = SettingsManager.Instance.Settings.Volume.sfxVolume;
    }


    public void OnMasterVolumeChanged(float value)
    {
        MasterMixer.SetFloat("MasterVolumeParam", -40f*(1f-value));
        SettingsManager.Instance.Settings.Volume.masterVolume = value;
    }

    public void OnMusicVolumeChanged(float value)
    {
        MasterMixer.SetFloat("MusicVolumeParam", -40f * (1f - value));
        SettingsManager.Instance.Settings.Volume.musicVolume = value;
    }

    public void OnSFXVolumeChanged(float value)
    {
        MasterMixer.SetFloat("SFXVolumeParam", -40f * (1f - value));
        SettingsManager.Instance.Settings.Volume.sfxVolume = value;
    }
}
