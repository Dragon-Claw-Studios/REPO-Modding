using UnityEngine;

namespace DragonClawLib;
public class ValueModifierPart : MonoBehaviour
{
    [Tooltip("Percentage that this part adds to the base value of the item.")]
    public float valueModifier = 0f;
}
