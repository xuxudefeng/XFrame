using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "UISystem/CopyTemplateSettings", fileName = "CopyTemplateSettings")]
public class CopyTemplateSettings : ScriptableObject
{
    public string ClassName => className;
    [SerializeField] private string className = DefaultClassName;
    public static readonly string DefaultClassName = "{0}UI";

    public string[] PickupComponentNames => pickupComponentNames;
    [SerializeField] private string[] pickupComponentNames = DefaultPickupComponentNames;
    public static readonly string[] DefaultPickupComponentNames = { nameof(Button), nameof(InputField), nameof(Text), nameof(ScrollRect) };

    public string[] RemoveText => removeText;
    [SerializeField] private string[] removeText = DefaultRemoveText;
    public static readonly string[] DefaultRemoveText = { };
}
