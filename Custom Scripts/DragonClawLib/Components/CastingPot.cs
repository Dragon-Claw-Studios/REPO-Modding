using Photon.Pun;
using UnityEngine;

public class CastingPot : MonoBehaviour, IPunObservable
{
    [Header("Pot Settings")]
    public Transform pivot; // Rotating part of the pot
    public HingeJoint pivotHinge; // Assign in inspector
    public float pourThresholdAngle = 60f; // degrees
    public bool hasPoured = false;

    [Header("Tray & Metal")]
    public CastingTray tray; // Reference to the tray
    public MoltenMetal moltenMetalPreset;

    private float initialRotation;
    public PhotonView photonView;

    void Awake()
    {
        photonView = GetComponent<PhotonView>();
        if (pivot != null)
            initialRotation = pivot.localEulerAngles.x;
    }

    void Update()
    {

        // Prevent casting if pivot is missing or hinge joint is broken
        if (!CanPour()) return;

        float currentRotation = pivot.localEulerAngles.x;
        float delta = Mathf.DeltaAngle(initialRotation, currentRotation);

        if (!hasPoured && Mathf.Abs(delta) >= pourThresholdAngle && tray.containedValuables.Count == 1)
        {
            if (SemiFunc.IsMultiplayer())
                photonView.RPC("TriggerCasting", RpcTarget.All);
            else
                TriggerCasting();
        }
    }

    // Encapsulated check for whether the pot can pour
    bool CanPour()
    {
        return pivot != null && pivotHinge != null;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
            stream.SendNext(hasPoured);
        else
            hasPoured = (bool)stream.ReceiveNext();
    }

    [PunRPC]
    void TriggerCasting()
    {
        if (hasPoured) return;

        hasPoured = true;

        tray.ApplyCastingToAll(moltenMetalPreset);  // Must be deterministic
        tray.UpdateIndicatorColor();
        tray.PlayPouringVisuals();
        tray.DisableHurtCollider();
    }
}