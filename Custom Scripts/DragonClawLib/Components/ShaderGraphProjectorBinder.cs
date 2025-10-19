using UnityEngine;

namespace DragonClawLib;

[ExecuteAlways]
[RequireComponent(typeof(Projector))]
public class ShaderGraphProjectorBinder : MonoBehaviour
{
    Projector projector;
    Material mat;

    void OnEnable()
    {
        projector = GetComponent<Projector>();
        if (projector.material != null)
            mat = projector.material;
    }

    void LateUpdate()
    {
        if (!projector || !mat) return;

        // 1️⃣ Recreate the same matrices Unity uses internally
        // World → Projector space
        Matrix4x4 worldToProj = projector.transform.worldToLocalMatrix;

        // 2️⃣ Build the projection matrix based on perspective or ortho mode
        Matrix4x4 projMatrix;
        if (!projector.orthographic)
            projMatrix = Matrix4x4.Perspective(projector.fieldOfView, projector.aspectRatio, projector.nearClipPlane, projector.farClipPlane);
        else
            projMatrix = Matrix4x4.Ortho(-projector.orthographicSize * projector.aspectRatio, projector.orthographicSize * projector.aspectRatio,
                                         -projector.orthographicSize, projector.orthographicSize,
                                         projector.nearClipPlane, projector.farClipPlane);

        // 3️⃣ Convert to GPU projection space (flips Y if needed)
        Matrix4x4 clipMatrix = GL.GetGPUProjectionMatrix(projMatrix, false);

        // 4️⃣ Combine like the legacy projector does:
        //     unity_Projector = projection * worldToLocal
        //     unity_ProjectorClip = same but with extra clip adjustment
        Matrix4x4 unity_Projector = clipMatrix * worldToProj;
        Matrix4x4 unity_ProjectorClip = unity_Projector; // identical for most shaders

        // 5️⃣ Send to material
        mat.SetMatrix("_Projector", unity_Projector);
        mat.SetMatrix("_ProjectorClip", unity_ProjectorClip);
    }
}
