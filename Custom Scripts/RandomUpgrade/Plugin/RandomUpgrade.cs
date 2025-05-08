using BepInEx;

namespace RandomUpgrade;

[BepInPlugin("DragonClaw.RandomUpgrade", "RandomUpgrade", "1.0")]
[BepInDependency(REPOLib.MyPluginInfo.PLUGIN_GUID, BepInDependency.DependencyFlags.HardDependency)]
public class RandomUpgrade : BaseUnityPlugin
{
    public static RandomUpgrade Instance;

    private void Awake()
    {
        Instance = this;
        Logger.LogInfo("RandomUpgrade plugin has been loaded!");
    }
}