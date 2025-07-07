using System.IO;
using Nomnom.UnityProjectPatcher;
using UnityEngine;
using UnityEngine.Serialization;

namespace Nomnom {
    [CreateAssetMenu(fileName = "UPPatcherBepInExUserSettings", menuName = "Unity Project Patcher/BepInEx User Settings")]
    public sealed class BepinexUserSettings: ScriptableObject {
#if UNITY_EDITOR
        public bool LoadProjectPlugins => _loadProjectPlugins;
        
        public string RootFolder
        {
            get {
                var settings = PatcherUtility.GetSettings();
                switch (_bepinexLocation) {
                    case Nomnom.BepinexLocation.Local:
                        return Path.GetFullPath(Path.Combine(Path.GetDirectoryName(LocalExePath), "BepInEx"));
                    case Nomnom.BepinexLocation.Game:
                        return Path.GetFullPath(Path.Combine(settings.GameFolderPath, "BepInEx"));
                    case Nomnom.BepinexLocation.Custom:
                        return Path.GetFullPath(Path.Combine(Path.GetFullPath(_bepinexCustomLocation.ToOSPath()), "BepInEx"));
                    default:
                        return null;
                }
            }
        }
        
        public string CoreFolder => Path.GetFullPath(Path.Combine(RootFolder, "core"));

        public string GameExePath => Path.GetFullPath(PatcherUtility.GetSettings().GameExePath);

        public string LocalExePath
        {
            get {
                var settings = PatcherUtility.GetSettings();
                return Path.GetFullPath(Path.Combine(Application.dataPath, "..", settings.GameName, $"{settings.GameName}.exenot"));
            }
        }

        public string PluginsPath => Path.GetFullPath(Path.Combine(RootFolder, "plugins"));
        
        public BepinexLocation BepinexLocation => _bepinexLocation;
        
        public string RouterGameExePath
        {
            get {
                switch (_bepinexLocation) {
                    case BepinexLocation.Local:
                    case BepinexLocation.Custom:
                        if (!File.Exists(LocalExePath)) {
                            using (var fs = File.Create(LocalExePath)) { }
                        }
                        return LocalExePath;
                    case BepinexLocation.Game:
                        return GameExePath;
                    default:
                        return null;
                }
            }
        }
#endif
        
        [Header("Project plugins are located in \"[Game Name]/Mods/\"")]
        [SerializeField] private bool _loadProjectPlugins = true;
        [SerializeField] private BepinexLocation _bepinexLocation;
        [SerializeField] private string _bepinexCustomLocation;
    }
    
    public enum BepinexLocation {
        Local,
        Game,
        Custom
    }
}