using UnityEngine;

public class ValuableDestructionWatcher : MonoBehaviour
{
    public CastingTray tray;
    public ValuableObject valuable;

    void OnDestroy()
    {
        if (tray != null && valuable != null)
        {
            tray.OnValuableDestroyed(valuable);
        }
    }
}