using System;
using System.Collections.Generic;
using UnityEngine;

namespace Nomnom {
    [CreateAssetMenu(fileName = "PluginBlockerUserSettings", menuName = "Unity Project Patcher/Plugin Blocker User Settings")]
    public sealed class PluginBlockerUserSettings: ScriptableObject {
        public IReadOnlyCollection<string> PluginFullTypeNames => _pluginFullTypeNames;
        
        [SerializeField] private string[] _pluginFullTypeNames = Array.Empty<string>();
    }
}