using BepInEx;

namespace DragonClawLib;

[BepInPlugin("DragonClaw.DragonClawLib", "DragonClawLib", "1.0")]
[BepInDependency(REPOLib.MyPluginInfo.PLUGIN_GUID, BepInDependency.DependencyFlags.HardDependency)]
public class DragonClawLib : BaseUnityPlugin
{
    public static DragonClawLib Instance;

    private void Awake()
    {
        Instance = this;
        Logger.LogInfo("DragonClawLib plugin has been loaded!");
    }
}