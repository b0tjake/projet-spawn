using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public class UnityAutoSave
{
    // Configure settings here
    private static int saveIntervalMinutes = 1; 
    private static bool showLog = true;

    private static double nextSaveTime;

    static UnityAutoSave()
    {
        EditorApplication.update += Update;
        nextSaveTime = EditorApplication.timeSinceStartup + saveIntervalMinutes * 60;
    }

    static void Update()
    {
        if (EditorApplication.timeSinceStartup > nextSaveTime)
        {
            Save();
            nextSaveTime = EditorApplication.timeSinceStartup + saveIntervalMinutes * 60;
        }
    }

    private static void Save()
    {
        // Don't save if the game is playing, it causes lag/glitches
        if (EditorApplication.isPlaying) return;

        // Save the Scene
        EditorSceneManager.SaveOpenScenes();
        
        // Save the Assets (Script changes, Prefabs)
        AssetDatabase.SaveAssets();

        if (showLog)
            Debug.Log($"<color=green>[AutoSave]</color> Project saved at {System.DateTime.Now.ToShortTimeString()}");
    }
}