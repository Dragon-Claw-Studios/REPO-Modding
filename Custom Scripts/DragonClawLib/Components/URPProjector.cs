using UnityEngine;
using UnityEngine.Rendering;

namespace DragonClawLib;

[ExecuteAlways]
public class URPProjector : MonoBehaviour
{
    [Header("Projector Material")]
    public Material material; // like original Projector

    [Header("Projection Settings")]
    public float fieldOfView = 45f;
    public float aspectRatio = 1f;
    public float nearClipPlane = 0.1f;
    public float farClipPlane = 20f;
    public LayerMask affectedLayers = ~0;

    [Header("Gizmos")]
    public bool drawGizmos = true;

    private Matrix4x4 _matrix;

    void LateUpdate()
    {
        if (!material) return;

        // build projector matrix
        float fovRad = fieldOfView * Mathf.Deg2Rad;
        float cot = 1f / Mathf.Tan(fovRad * 0.5f);

        Matrix4x4 proj = new Matrix4x4();
        proj[0, 0] = cot / aspectRatio;
        proj[1, 1] = cot;
        proj[2, 2] = (farClipPlane + nearClipPlane) / (nearClipPlane - farClipPlane);
        proj[2, 3] = (2 * farClipPlane * nearClipPlane) / (nearClipPlane - farClipPlane);
        proj[3, 2] = -1f;
        proj[3, 3] = 0f;

        Matrix4x4 worldToLocal = transform.worldToLocalMatrix;
        _matrix = GL.GetGPUProjectionMatrix(proj, false) * worldToLocal;

        material.SetMatrix("_Projector", _matrix);
        material.SetMatrix("_ProjectorClip", _matrix);
    }

    void OnRenderObject()
    {
        if (!material) return;

        // Draw a full-screen box volume (same as old projector frustum)
        material.SetPass(0);

        Graphics.DrawMeshNow(
            MeshGenerator.GetFrustumMesh(fieldOfView, aspectRatio, nearClipPlane, farClipPlane),
            transform.localToWorldMatrix
        );
    }

    void OnDrawGizmos()
    {
        if (!drawGizmos) return;
        Gizmos.color = Color.yellow;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawFrustum(Vector3.zero, fieldOfView, farClipPlane, nearClipPlane, aspectRatio);
    }
}
