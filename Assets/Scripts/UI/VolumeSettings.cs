using UnityEngine;

[CreateAssetMenu(fileName = "VolumeSettings", menuName = "Scriptable Objects/VolumeSettings")]
public class VolumeSettings : ScriptableObject
{
    public float masterVolume;
    public float musicVolume;
    public float sfxVolume;
}
