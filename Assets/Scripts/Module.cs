using UnityEngine;

public class Module : MonoBehaviour
{
    public enum Type
    {
        Health,
        Armor,
        Recovery,
        Cooldown
    }
    
    public Type ModuleType;
    public int Value;

    public string GetInfo() => ModuleType switch
    {
        Type.Health => $"{ModuleType} +{Value}",
        Type.Armor => $"{ModuleType} +{Value}",
        Type.Recovery => $"{ModuleType} +{Value}%",
        Type.Cooldown => $"{ModuleType} -{Value}%",
        _ => string.Empty
    };
}
