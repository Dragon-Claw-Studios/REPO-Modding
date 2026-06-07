using UnityEngine;

namespace DragonClawLib;
public class RotatorComponent : MonoBehaviour
{
    [Tooltip("Rotation speed in degrees per second")]
    public float RotationSpeedX = 0f;
    public float RotationSpeedY = 0f;
    public float RotationSpeedZ = 0f;

    void Update()
    {
        transform.Rotate(RotationSpeedX * Time.deltaTime, RotationSpeedY * Time.deltaTime, RotationSpeedZ * Time.deltaTime);
    }
}
