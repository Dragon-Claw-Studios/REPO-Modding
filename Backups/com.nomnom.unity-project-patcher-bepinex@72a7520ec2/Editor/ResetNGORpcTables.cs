using System;
using System.Collections;
using System.Linq;
using Nomnom.UnityProjectPatcher.Editor;
using UnityEngine;

namespace Nomnom.BepInEx.Editor {
    internal static class ResetNGORpcTables {
        public static bool DidReset;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void OnBeforeSceneLoadRuntimeMethod() {
            if (DidReset) return;
            Reset();
        }

        public static void Reset() {
            if (DidReset) return;
            if (!PatcherUtility.HasDomainReloadingDisabled()) return;
            
            var networkManagerType = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .FirstOrDefault(x => x.FullName == "Unity.Netcode.NetworkManager");
            var rpcFuncTableField = networkManagerType.GetField("__rpc_func_table");
            var rpcNameTableField = networkManagerType.GetField("__rpc_name_table");
            var rpcFuncTable = (IDictionary)rpcFuncTableField.GetValue(null);
            var rpcNameTable = (IDictionary)rpcNameTableField.GetValue(null);
            rpcFuncTable.Clear();
            rpcNameTable.Clear();
            rpcFuncTableField.SetValue(null, rpcFuncTable);
            rpcNameTableField.SetValue(null, rpcNameTable);
            Debug.Log("Reset rpc_func_table and rpc_name_table.");
            DidReset = true;
        }
    }
}