using UnityEngine;

[CreateAssetMenu(fileName = "GeneralSettings", menuName = "Scriptable Objects/GeneralSettings")]
public class GeneralSettings : ScriptableObject
{
    public float dashCooldown;
    public float attackCooldown;   
    public float blockCooldown;
    public float playermovementSpeed;
}
