using UnityEngine;

namespace DragonClawLib;
public static class MeshGenerator
{
    static Mesh frustumMesh;

    public static Mesh GetFrustumMesh(float fov, float aspect, float near, float far)
    {
        if (frustumMesh != null) return frustumMesh;

        frustumMesh = new Mesh();
        frustumMesh.name = "ProjectorFrustum";

        float halfFov = Mathf.Tan(Mathf.Deg2Rad * fov * 0.5f);
        float nearHeight = halfFov * near * 2f;
        float nearWidth = nearHeight * aspect;
        float farHeight = halfFov * far * 2f;
        float farWidth = farHeight * aspect;

        Vector3[] verts = new Vector3[8]
        {
            new Vector3(-nearWidth/2, -nearHeight/2, -near), // 0
            new Vector3( nearWidth/2, -nearHeight/2, -near), // 1
            new Vector3( nearWidth/2,  nearHeight/2, -near), // 2
            new Vector3(-nearWidth/2,  nearHeight/2, -near), // 3
            new Vector3(-farWidth/2,  -farHeight/2,  -far),  // 4
            new Vector3( farWidth/2,  -farHeight/2,  -far),  // 5
            new Vector3( farWidth/2,   farHeight/2,  -far),  // 6
            new Vector3(-farWidth/2,   farHeight/2,  -far)   // 7
        };

        int[] tris = new int[]
        {
            // connect near to far (6 faces)
            0,2,1, 0,3,2, // near
            4,5,6, 4,6,7, // far
            0,1,5, 0,5,4, // bottom
            2,3,7, 2,7,6, // top
            1,2,6, 1,6,5, // right
            3,0,4, 3,4,7  // left
        };

        frustumMesh.vertices = verts;
        frustumMesh.triangles = tris;
        frustumMesh.RecalculateNormals();
        return frustumMesh;
    }
}
