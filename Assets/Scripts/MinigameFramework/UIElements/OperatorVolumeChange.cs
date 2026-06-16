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

        MasterVolumeSlider.value = SettingsManager.Instance.Settings.MasterVolume;
        MusicVolumeSlider.value = SettingsManager.Instance.Settings.MusicVolume;
        SFXVolumeSlider.value = SettingsManager.Instance.Settings.SfxVolume;
    }


    public void OnMasterVolumeChanged(float value)
    {
        MasterMixer.SetFloat("MasterVolumeParam", -40f*(1f-value));
        SettingsManager.Instance.Settings.MasterVolume = value;
    }

    public void OnMusicVolumeChanged(float value)
    {
        MasterMixer.SetFloat("MusicVolumeParam", -40f * (1f - value));
        SettingsManager.Instance.Settings.MusicVolume = value;
    }

    public void OnSFXVolumeChanged(float value)
    {
        MasterMixer.SetFloat("SFXVolumeParam", -40f * (1f - value));
        SettingsManager.Instance.Settings.SfxVolume = value;
    }
}
