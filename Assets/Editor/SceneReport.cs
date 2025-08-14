// Assets/Editor/SceneReport.cs
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.Text;

public static class SceneReport
{
    [MenuItem("Tools/Report/Print Scene Scripts")]
    public static void PrintSceneScripts()
    {
        var sb = new StringBuilder();
        var scene = SceneManager.GetActiveScene();
        sb.AppendLine($"--- Scene: {scene.name} ---");

        foreach (var root in scene.GetRootGameObjects())
            DumpGO(root.transform, sb, 0);

        Debug.Log(sb.ToString());
    }

    static void DumpGO(Transform t, StringBuilder sb, int depth)
    {
        string indent = new string(' ', depth * 2);
        sb.AppendLine($"{indent}- {t.name}");

        var mbs = t.GetComponents<MonoBehaviour>();
        foreach (var mb in mbs)
        {
            if (mb == null) continue; // missing script
            sb.AppendLine($"{indent}    * {mb.GetType().Name}");
        }

        for (int i = 0; i < t.childCount; i++)
            DumpGO(t.GetChild(i), sb, depth + 1);
    }
}
#endif
