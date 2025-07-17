using UnityEngine;
using System.IO;

public static class ChartIO
{
    private static string folderPath = Path.Combine(Application.dataPath, "Resources", "Charts");

    public static void SaveChart(ChartData chart, string fileName)
    {
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string json = JsonUtility.ToJson(chart, true);
        string path = Path.Combine(folderPath, fileName + ".json");
        File.WriteAllText(path, json);

        Debug.Log("Chart sauvegard¨¦ : " + path);
    }

    public static ChartData LoadChart(string fileName)
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("Charts/" + fileName);
        if (jsonFile == null)
        {
            Debug.LogError("Fichier introuvable dans Resources/Charts : " + fileName);
            return null;
        }

        return JsonUtility.FromJson<ChartData>(jsonFile.text);
    }
}
