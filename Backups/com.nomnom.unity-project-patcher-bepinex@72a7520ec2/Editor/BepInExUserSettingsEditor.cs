using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Nomnom.UnityProjectPatcher.Editor;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace Nomnom.BepInEx.Editor {
    [CustomEditor(typeof(BepinexUserSettings))]
    public sealed class BepInExUserSettingsEditor: UnityEditor.Editor {
        [NonSerialized]
        private bool _isLoading = true;

        [NonSerialized] private string[] _internalPlugins;
        [NonSerialized] private string[] _externalPlugins;
        
#if ENABLE_BEPINEX
        private void OnEnable() {
            FindPlugins();
            BepInExPreloader.InitPaths();
        }
#endif

        private void ChangeFlag(bool enabled) {
            if (!enabled) {
                EditorUtility.DisplayProgressBar("Disabling BepInEx", "Disabling BepInEx", 0.5f);
                EditorApplication.delayCall += () => {
                    PatcherUtility.SetScriptingDefineSymbols(PatcherUtility.GetScriptingDefineSymbols().Replace("ENABLE_BEPINEX", ""));
                    EditorUtility.ClearProgressBar();
                };
            } else {
                EditorUtility.DisplayProgressBar("Enabling BepInEx", "Enabling BepInEx", 0.5f);
                EditorApplication.delayCall += () => {
                    PatcherUtility.SetScriptingDefineSymbols(PatcherUtility.GetScriptingDefineSymbols() + ";ENABLE_BEPINEX");
                    EditorUtility.ClearProgressBar();
                };
            }
        }

        
#if ENABLE_BEPINEX
        private void FindPlugins() {
            _isLoading = true;
            EditorApplication.delayCall += () => {
                _internalPlugins = BepInExPreloader.GetInternalPluginAssemblyNames(PatcherUtility.GetSettings()).ToArray();
                _externalPlugins = BepInExPreloader.GetExternalPluginAssemblyNames(BepInExPreloader.GetBepInExUserSettings()).ToArray();
                _isLoading = false;
            };
        }
#endif

        public override void OnInspectorGUI() {
            GUI.enabled = !EditorApplication.isCompiling;
            
            using (var change = new EditorGUI.ChangeCheckScope()) {
#if ENABLE_BEPINEX
                var toggle = EditorGUILayout.Toggle("Enabled", true);
#else
                var toggle = EditorGUILayout.Toggle("Enabled", false);
#endif   
                
                if (change.changed) {
                    ChangeFlag(toggle);
                }
            }
            
            base.OnInspectorGUI();
            
            EditorGUILayout.Space();
            
#if ENABLE_BEPINEX
            GUI.enabled = !_isLoading;
            if (GUILayout.Button("Reload")) {
                FindPlugins();
            }

            if (GUILayout.Button("Open BepInEx Folder")) {
                var settings = BepInExPreloader.GetBepInExUserSettings();
                BepInExPreloader.InitPaths();
                EditorUtility.RevealInFinder(settings.RootFolder);
            }

            GUI.enabled = true;
            
            EditorGUILayout.Space();
            ShowInternalPlugins();
            EditorGUILayout.Space();
            ShowExternalPlugins();
            EditorGUILayout.Space();
#endif
        }

        private void ShowInternalPlugins() {
            EditorGUILayout.LabelField("Possible Plugins in the Project", EditorStyles.miniBoldLabel);
            
            if (_isLoading) {
                EditorGUILayout.LabelField(" - loading...");
            } else if (_internalPlugins == null || _internalPlugins.Length == 0) {
                EditorGUILayout.LabelField(" - no plugins found");
            } else {
                var i = -1;
                foreach (var name in _internalPlugins) {
                    i++;
                    EditorGUILayout.LabelField($" - [{i}] {name}");
                }
            }
        }

        private void ShowExternalPlugins() {
            EditorGUILayout.LabelField("External Plugins", EditorStyles.miniBoldLabel);
            
            if (_isLoading) {
                EditorGUILayout.LabelField(" - loading...");
            } else if (_externalPlugins == null || _externalPlugins.Length == 0) {
                EditorGUILayout.LabelField(" - no plugins found");
            } else {
                var i = -1;
                foreach (var name in _externalPlugins) {
                    i++;
                    EditorGUILayout.LabelField($" - [{i}] {name}");
                }
            }
        }
    }
}