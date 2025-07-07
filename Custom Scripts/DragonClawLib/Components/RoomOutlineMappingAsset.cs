using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "REPO Mod/Room Outline Mapping", fileName = "RoomOutlineMappingAsset")]
public class RoomOutlineMappingAsset : ScriptableObject
{
    public List<Map.RoomVolumeOutlineCustom> customOutlines = new();
}
