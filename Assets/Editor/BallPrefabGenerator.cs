using UnityEngine;
using UnityEditor;
using System.IO;

public class BallPrefabGenerator
{
    [MenuItem("Tools/Ball/Generate Ball Prefabs")]
    static void GenerateBallPrefabs()
    {
        string modelPath = "Assets/Ball/Models/Sport_Balls/";
        string materialPath = "Assets/Ball/Models/Sport_Balls/Materials/";
        string savePath = "Assets/Prefabs/";

        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
            AssetDatabase.Refresh();
        }

        string[] guids = AssetDatabase.FindAssets("t:Model", new[] { modelPath });

        foreach (string guid in guids)
        {
            string modelAssetPath = AssetDatabase.GUIDToAssetPath(guid);
            GameObject model = AssetDatabase.LoadAssetAtPath<GameObject>(modelAssetPath);

            if (model == null) continue;

            // 尝试加载材质
            string matName = Path.GetFileNameWithoutExtension(modelAssetPath);
            string matPath = materialPath + matName + ".mat";
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);

            if (mat == null)
            {
                Debug.LogWarning($"材质没找到: {matPath}");
            }

            // 创建实例对象
            GameObject instance = Object.Instantiate(model);
            instance.name = "Ball_" + matName;

            MeshRenderer renderer = instance.GetComponentInChildren<MeshRenderer>();
            if (renderer && mat != null)
            {
                renderer.sharedMaterial = mat;
            }

            // 添加刚体和碰撞器
            if (instance.GetComponent<Rigidbody>() == null)
                instance.AddComponent<Rigidbody>();

            if (instance.GetComponent<Collider>() == null)
                instance.AddComponent<SphereCollider>();

            // 保存为 prefab
            string prefabPath = savePath + instance.name + ".prefab";
            PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
            Object.DestroyImmediate(instance);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("✅ 球体预制件生成完成！");
    }
}

