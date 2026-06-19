using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

public class OperatorVolumeChange : MonoBehaviour
{
    [FormerlySerializedAs("MasterVolumeSlider")] [SerializeField]
    public Slider masterVolumeSlider;
    [FormerlySerializedAs("MusicVolumeSlider")] public Slider musicVolumeSlider;
    [FormerlySerializedAs("SFXVolumeSlider")] public Slider sfxVolumeSlider;
    [FormerlySerializedAs("MasterMixer")] public AudioMixer masterMixer;
    [FormerlySerializedAs("MasterText")] public TextMeshProUGUI masterText;
    [FormerlySerializedAs("MusicText")] public TextMeshProUGUI musicText;
    [FormerlySerializedAs("SFXText")] public TextMeshProUGUI sfxText;

    public void Awake()
    {
        masterMixer.SetFloat("MasterVolumeParam", -40f*(1f-SettingsManager.Instance.settings.masterVolume / 100f));
        masterMixer.SetFloat("MusicVolumeParam", -40f*(1f-SettingsManager.Instance.settings.musicVolume / 100f));
        masterMixer.SetFloat("SFXVolumeParam", -40f*(1f-SettingsManager.Instance.settings.sfxVolume / 100f));
        masterVolumeSlider.value = SettingsManager.Instance.settings.masterVolume;
        musicVolumeSlider.value = SettingsManager.Instance.settings.musicVolume;
        sfxVolumeSlider.value = SettingsManager.Instance.settings.sfxVolume;
        masterText.text = masterVolumeSlider.value.ToString("0");
        musicText.text = musicVolumeSlider.value.ToString("0");
        sfxText.text = sfxVolumeSlider.value.ToString("0");
    }

    public void OnMasterVolumeChanged(float value)
    {
        masterMixer.SetFloat("MasterVolumeParam", -40f*(1f-value / 100f));
        SettingsManager.Instance.settings.masterVolume = value;
        masterText.text = masterVolumeSlider.value.ToString("0");
    }

    public void OnMusicVolumeChanged(float value)
    {
        masterMixer.SetFloat("MusicVolumeParam", -40f * (1f - value / 100f));
        SettingsManager.Instance.settings.musicVolume = value;
        musicText.text = musicVolumeSlider.value.ToString("0");
    }

    public void OnSFXVolumeChanged(float value)
    {
        masterMixer.SetFloat("SFXVolumeParam", -40f * (1f - value / 100f));
        SettingsManager.Instance.settings.sfxVolume = value;
        sfxText.text = sfxVolumeSlider.value.ToString("0");
    }
}
