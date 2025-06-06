using Photon.Pun;
using UnityEngine;

public class CastingPot : MonoBehaviour, IPunObservable
{
    public Transform pivot; // Rotating part of the pot
    public float pourThresholdAngle = 60f; // degrees
    public bool hasPoured = false;

    public CastingTray tray; // Reference to the tray
    public MoltenMetal moltenMetalPreset;


    private float initialRotation;

    public PhotonView photonView;
        
        void Awake()
    {
            photonView = GetComponent<PhotonView>();
            initialRotation = pivot.localEulerAngles.x;
    }

    void Update()
    {
        float currentRotation = pivot.localEulerAngles.x;
        float delta = Mathf.DeltaAngle(initialRotation, currentRotation);

        if (!hasPoured && Mathf.Abs(delta) >= pourThresholdAngle && tray.containedValuables.Count == 1)
        {

            if (SemiFunc.IsMultiplayer())
            {
                photonView.RPC("TriggerCasting", RpcTarget.All);
            }
            else
            {
                TriggerCasting();
            }
        }
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
        if (hasPoured)
        {
            return;
        }

        hasPoured = true;

        tray.ApplyCastingToAll(moltenMetalPreset);  // Must be deterministic
        tray.UpdateIndicatorColor();
        tray.PlayPouringVisuals();
        tray.DisableHurtCollider();
    }
}
