using System.Collections;
using System.Collections.Generic;
using System.IO;
using GCTextureTools;
using UnityEngine;
using UnityEditor;

// with help from : https://github.com/KellanHiggins/AsciiFBXExporterForUnity/blob/master/Assets/com.8bitgoose.asciifbxexporter/Editor/ExporterMenu.cs
public class GladiusGCExporter : Editor
{
// Dropdown
    [MenuItem("GameObject/GladiusGCExporter/Export", false, 10)]
    public static void ExportDropdownGameObjectToGladiusGC()
    {
        ExportCurrentGameObject(false, false);
    }


// Assets
    [MenuItem("Assets/GladiusGCExporter/Export", false, 30)]
    public static void ExportGameObjectToGladiusGC()
    {
        ExportCurrentGameObject(false, false);
    }

    private static void ExportCurrentGameObject(bool copyMaterials, bool copyTextures)
    {
        if (Selection.activeGameObject == null)
        {
            EditorUtility.DisplayDialog("No Object Selected", "Please select any GameObject to Export to FBX", "Okay");
            return;
        }

        GameObject currentGameObject = Selection.activeObject as GameObject;

        if (currentGameObject == null)
        {
            EditorUtility.DisplayDialog("Warning", "Item selected is not a GameObject", "Okay");
            return;
        }

        ExportGameObject(currentGameObject, copyMaterials, copyTextures);
    }

    /// <summary>
    /// Exports ANY Game Object given to it. Will provide a dialog and return the path of the newly exported file
    /// </summary>
    /// <returns>The path of the newly exported FBX file</returns>
    /// <param name="gameObject">Game object to be exported</param>
    /// <param name="copyMaterials">If set to <c>true</c> copy materials.</param>
    /// <param name="copyTextures">If set to <c>true</c> copy textures.</param>
    /// <param name="oldPath">Old path.</param>
    public static string ExportGameObject(GameObject gameObject, bool copyMaterials, bool copyTextures,
        string oldPath = null)
    {
        if (gameObject == null)
        {
            EditorUtility.DisplayDialog("Object is null", "Please select any GameObject to Export to GladiusGC",
                "Okay");
            return null;
        }

        string newPath = GetNewPath(gameObject, oldPath);

        if (newPath != null && newPath.Length != 0)
        {
            //string testPath = newPath.Substring(0,(newPath.Length-newPath.LastIndexOf("/"))+1);
            string testPath = newPath.Substring(0,(newPath.LastIndexOf("/"))+1);
            //testPath += "/";
            
            GCModel model = GCModel.CreateFromGameObject(gameObject);
            if (model != null)
            {
                using (BinaryWriter bw = new BinaryWriter(File.Create(newPath)))
                {
                    model.WriteData(bw);
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Warning", "Failed to create GCModel data from gameobj.","Okay");
            }
            
            Renderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
            if (meshRenderer == null)
            {
                meshRenderer = gameObject.GetComponent<SkinnedMeshRenderer>();
            }
            Material m = meshRenderer.sharedMaterial;

            string textureName = m.mainTexture.name.ToLower();
            string modelName = gameObject.name;

            ImageExtractor.EncodeFile((Texture2D)m.mainTexture,textureName+".tga", testPath+modelName+".ptx");
        }

        return null;
    }

    /// <summary>
    /// Creates save dialog window depending on old path or right to the /Assets folder no old path is given
    /// </summary>
    /// <returns>The new path.</returns>
    /// <param name="gameObject">Item to be exported</param>
    /// <param name="oldPath">The old path that this object was original at.</param>
    private static string GetNewPath(GameObject gameObject, string oldPath = null)
    {
        // NOTE: This must return a path with the starting "Assets/" or else textures won't copy right

        string name = gameObject.name;

        string newPath = null;
        if (oldPath == null)
            newPath = EditorUtility.SaveFilePanelInProject("Export GladiusGC File", name + ".pax", "pax",
                "Export " + name + " GameObject to a GladiusGC file");
        else
        {
            if (oldPath.StartsWith("/Assets"))
            {
                oldPath = Application.dataPath.Remove(Application.dataPath.LastIndexOf("/Assets"), 7) + oldPath;
                oldPath = oldPath.Remove(oldPath.LastIndexOf('/'), oldPath.Length - oldPath.LastIndexOf('/'));
            }

            newPath = EditorUtility.SaveFilePanel("Export GladiusGC File", oldPath, name + ".pax", "pax");
        }

        int assetsIndex = newPath.IndexOf("Assets");

        if (assetsIndex < 0)
            return null;

        if (assetsIndex > 0)
            newPath = newPath.Remove(0, assetsIndex);

        return newPath;
    }
}