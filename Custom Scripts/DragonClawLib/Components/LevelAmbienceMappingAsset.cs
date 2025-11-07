using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "REPO Mod/Level Ambience Mapping", fileName = "LevelAmbienceMappingAsset")]
public class LevelAmbienceMappingAsset : ScriptableObject
{
    public List<LevelAmbience> customAmbiences = new();
}
