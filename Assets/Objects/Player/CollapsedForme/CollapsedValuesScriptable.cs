using UnityEngine;

[CreateAssetMenu(fileName = "CollapsedValuesScriptable", menuName = "Scriptable Objects/CollapsedValuesScriptable")]
public class CollapsedValuesScriptable : ScriptableObject
{
    public float strength = 5f;
    public float transitionStrength = 2f;
    public float jumpStrength = 1f;
    public float torqueStrength = .2f;
    public float jumpCooldown = 1.5f;
    public float attractionMultiplier = 1.5f;
}
