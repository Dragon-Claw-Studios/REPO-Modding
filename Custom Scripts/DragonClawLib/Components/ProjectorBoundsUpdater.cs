using UnityEngine;

[RequireComponent(typeof(Projector))]
public class ProjectorBoundsUpdater : MonoBehaviour
{
    void Awake()
    {
        Projector projector = GetComponent<Projector>();

        projector.material = new Material(projector.material);

        Vector3 boundsCenter = transform.parent.position;
        projector.material.SetFloat("_OffsetX", boundsCenter.x);
        projector.material.SetFloat("_OffsetZ", boundsCenter.z);
    }
}