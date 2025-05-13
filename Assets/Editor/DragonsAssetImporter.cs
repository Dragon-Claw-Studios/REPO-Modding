using UnityEngine;
using UnityEditor;

public class DragonsAssetImporter : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        TextureImporter textureimporter = assetImporter as TextureImporter;
        Object texture = AssetDatabase.LoadAssetAtPath(textureimporter.assetPath, typeof(Texture2D));
        if (!texture)
        {
            //textureimporter.textureFormat = TextureImporterFormat.AutomaticTruecolor; // deprecated
            textureimporter.maxTextureSize = 1024;
            textureimporter.compressionQuality = 100;
        }
    }
    public void OnPreprocessModel()
    {
        //ModelImporter modelImporter = (ModelImporter)assetImporter;
        ModelImporter modelimporter = assetImporter as ModelImporter;

        Object mesh = AssetDatabase.LoadAssetAtPath(modelimporter.assetPath, typeof(Mesh));
        if (!mesh)
        {
            modelimporter.importLights = false;
            modelimporter.importCameras = false;
            modelimporter.importVisibility = false;
            modelimporter.generateSecondaryUV = true;
            //modelimporter.importMaterials = true;
            modelimporter.materialImportMode = ModelImporterMaterialImportMode.ImportStandard;
            modelimporter.materialName = ModelImporterMaterialName.BasedOnMaterialName;
        }
    }
}