using UnityEngine;

[CreateAssetMenu(fileName = "GoldengooseSettings", menuName = "Scriptable Objects/GoldengooseSettings")]
public class GoldengooseSettings : ScriptableObject
{
    public float gameLength;
    public float secondsPerPoint;
    public float pickUpDelay;
    public float minBisonUptime;
    public float maxBisonUptime;

}
