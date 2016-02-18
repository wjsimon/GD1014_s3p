using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class AnimImporter : AssetPostprocessor
{

    void OnPreprocessAnimation()
    {
        var modelImporter = assetImporter as ModelImporter;
        //modelImporter.clipAnimations = modelImporter.defaultClipAnimations;

        Debug.Log("IMP:" + modelImporter.assetPath);
    }
}
