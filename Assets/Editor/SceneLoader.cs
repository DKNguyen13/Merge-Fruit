using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class SceneLoader : EditorWindow
{
    [MenuItem("Tools/Scene Loader")]
    public static void ShowWindow()
    {
        GetWindow<SceneLoader>("Scene Loader");
    }

    private void OnGUI()
    {
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            string name = System.IO.Path.GetFileNameWithoutExtension(scene.path);
            if (GUILayout.Button(name))
            {
                EditorSceneManager.OpenScene(scene.path);
            }
        }
    }
}
