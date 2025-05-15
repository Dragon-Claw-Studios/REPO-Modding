using UnityEngine;

namespace DragonClawLib;
public class RotatorComponent : MonoBehaviour
{
    [Tooltip("Rotation speed in degrees per second")]
    public float RotationSpeed = 45f;

    void Update()
    {
        transform.Rotate(0f, RotationSpeed * Time.deltaTime, 0f);
    }
}
