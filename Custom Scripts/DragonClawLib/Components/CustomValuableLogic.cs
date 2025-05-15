using Photon.Pun;
using UnityEngine;

namespace DragonClawLib;

public class CustomValuableLogic : MonoBehaviour
{
    public Material defaultMaterial;
    public Material heldMaterial;

    private Renderer objectRenderer;
    private PhysGrabObject physGrabObject;
    private PhotonView photonView;

    private bool isHeld = false;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        physGrabObject = GetComponent<PhysGrabObject>();
        objectRenderer = GetComponentInChildren<Renderer>(); // Or adjust if not in child
        ApplyMaterial(defaultMaterial);
    }

    private void Update()
    {
        if (!SemiFunc.IsMultiplayer())
        {
            UpdateHeldStateLocal();
        }
        else
        {
            if (photonView.IsMine)
            {
                CheckHeldStateMultiplayer();
            }
        }
    }

    private void UpdateHeldStateLocal()
    {
        bool currentlyHeld = PhysGrabber.instance.grabbed && PhysGrabber.instance.grabbedPhysGrabObject == physGrabObject;

        if (currentlyHeld != isHeld)
        {
            isHeld = currentlyHeld;
            ApplyMaterial(isHeld ? heldMaterial : defaultMaterial);
        }
    }

    private void CheckHeldStateMultiplayer()
    {
        bool currentlyHeld = PhysGrabber.instance.grabbed && PhysGrabber.instance.grabbedPhysGrabObject == physGrabObject;

        if (currentlyHeld != isHeld)
        {
            isHeld = currentlyHeld;
            photonView.RPC("SyncHeldMaterial", RpcTarget.All, isHeld);
        }
    }

    [PunRPC]
    public void SyncHeldMaterial(bool held)
    {
        ApplyMaterial(held ? heldMaterial : defaultMaterial);
    }

    private void ApplyMaterial(Material mat)
    {
        if (objectRenderer != null && mat != null)
        {
            objectRenderer.material = mat;
        }
    }
}
