using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class OperatorVolumeChange : MonoBehaviour
{
    [SerializeField]
    public Slider MasterVolumeSlider;
    public Slider MusicVolumeSlider;
    public Slider SFXVolumeSlider;
    public AudioMixer MasterMixer;
    public TextMeshProUGUI MasterText;
    public TextMeshProUGUI MusicText;
    public TextMeshProUGUI SFXText;


    public void Awake()
    {
        MasterMixer.SetFloat("MasterVolumeParam", SettingsManager.Instance.Settings.MasterVolume);
        MasterMixer.SetFloat("MusicVolumeParam", SettingsManager.Instance.Settings.MusicVolume);
        MasterMixer.SetFloat("SFXVolumeParam", SettingsManager.Instance.Settings.SfxVolume);
        MasterVolumeSlider.value = SettingsManager.Instance.Settings.MasterVolume;
        MusicVolumeSlider.value = SettingsManager.Instance.Settings.MusicVolume;
        SFXVolumeSlider.value = SettingsManager.Instance.Settings.SfxVolume;
        MasterText.text = MasterVolumeSlider.value.ToString("0");
        MusicText.text = MusicVolumeSlider.value.ToString("0");
        SFXText.text = SFXVolumeSlider.value.ToString("0");
    }

    public void OnMasterVolumeChanged(float value)
    {
        MasterText.text = MasterVolumeSlider.value.ToString("0");
        MasterMixer.SetFloat("MasterVolumeParam", -40f*(1f-value / 100f));
        SettingsManager.Instance.Settings.MasterVolume = value;
    }

    public void OnMusicVolumeChanged(float value)
    {
        MusicText.text = MusicVolumeSlider.value.ToString("0");
        MasterMixer.SetFloat("MusicVolumeParam", -40f * (1f - value / 100f));
        SettingsManager.Instance.Settings.MusicVolume = value;
    }

    public void OnSFXVolumeChanged(float value)
    {
        SFXText.text = SFXVolumeSlider.value.ToString("0");
        MasterMixer.SetFloat("SFXVolumeParam", -40f * (1f - value / 100f));
        SettingsManager.Instance.Settings.SfxVolume = value;
    }
}
