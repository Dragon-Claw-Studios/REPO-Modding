using UnityEngine;

public class PotionWobble : MonoBehaviour
{
    [SerializeField] Renderer rend;
    [SerializeField] Rigidbody rb;

    [Header("Spring")]

    [Tooltip("How strongly the system pulls the wobble back toward its rest position. Higher values make the liquid respond faster and feel more rigid; lower values make it feel heavier and more sluggish.")]
    [SerializeField] float stiffness = 35f;

    [Tooltip("How quickly motion energy is lost over time. Higher values dampen oscillations faster (less slosh), lower values allow longer, more fluid-like wobble.")]
    [SerializeField] float damping = 6f;

    [Tooltip("Scales how much velocity/rotation is converted into wobble energy. Higher values make small movements cause stronger sloshing; lower values make the system more subtle.")]
    [SerializeField] float inputStrength = 0.02f;

    [Tooltip("Maximum allowed wobble amplitude. Prevents extreme motion and keeps the effect visually stable and contained.")]
    [SerializeField] float maxWobble = 0.05f;

    Vector2 wobble;
    Vector2 wobbleVelocity;
    Vector2 target;

    Vector3 lastPos;
    Quaternion lastRot;

    MaterialPropertyBlock mpb;
    static readonly int WobbleID = Shader.PropertyToID("_Wobble");

    void Awake()
    {
        mpb = new MaterialPropertyBlock();
        lastPos = transform.position;
        lastRot = transform.rotation;
    }

    void Update()
    {
        float dt = Time.deltaTime;
        if (dt <= 0f) return;

        // --- motion ---
        Vector3 vel;
        Vector3 angVel;

        if (rb)
        {
            vel = rb.velocity;
            angVel = rb.angularVelocity * Mathf.Rad2Deg;
        }
        else
        {
            vel = (transform.position - lastPos) / dt;

            Quaternion delta = transform.rotation * Quaternion.Inverse(lastRot);
            delta.ToAngleAxis(out float angle, out Vector3 axis);
            angVel = axis * angle / dt;

            lastPos = transform.position;
            lastRot = transform.rotation;
        }

        // --- inject ENERGY into target (important) ---
        target += new Vector2(
            vel.x + angVel.z * 0.2f,
            vel.z + angVel.x * 0.2f
        ) * inputStrength;

        target = Vector2.ClampMagnitude(target, maxWobble);

        // --- spring physics (THIS creates wobble) ---
        Vector2 force = (target - wobble) * stiffness;
        wobbleVelocity += force * dt;

        wobbleVelocity *= Mathf.Exp(-damping * dt);

        wobble += wobbleVelocity * dt;

        // --- decay target slowly so motion stops but wobble persists briefly ---
        target = Vector2.Lerp(target, Vector2.zero, 1.5f * dt);

        // --- send to shader ---
        rend.GetPropertyBlock(mpb);
        mpb.SetVector(WobbleID, wobble);
        rend.SetPropertyBlock(mpb);
    }
}