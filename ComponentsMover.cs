using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class ComponentsMover : MonoBehaviour
{
    private string jsonFilePath; // JSON文件路径
    private Dictionary<string, Transform> components = new Dictionary<string, Transform>(); // 存储场景中的组件

    private void Start()
    {
        // 设置JSON文件路径
        jsonFilePath = Path.Combine(Application.streamingAssetsPath, "generate.json");

        if (!File.Exists(jsonFilePath))
        {
            Debug.LogWarning("JSON 文件未找到：" + jsonFilePath);
        }

        // 收集场景中的所有对象
        foreach (GameObject obj in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            if (obj.hideFlags == HideFlags.None) // 排除隐藏对象
            {
                components[obj.name] = obj.transform;
            }
        }

        Debug.Log($"初始化完成，已找到 {components.Count} 个组件。");
        foreach (var kvp in components)
        {
            Debug.Log($"找到组件：{kvp.Key}, 当前位置：{kvp.Value.position}");
        }
    }

    private void LateUpdate()
    {
        if (File.Exists(jsonFilePath))
        {
            Debug.Log("读取 JSON 文件并更新组件位置。");
            UpdateComponentsPositions();
        }
        else
        {
            Debug.LogWarning("JSON 文件未找到：" + jsonFilePath);
        }
    }

    private void UpdateComponentsPositions()
    {
        // 读取 JSON 文件内容
        string jsonData = File.ReadAllText(jsonFilePath);
        Debug.Log("读取的 JSON 数据：" + jsonData);

        try
        {
            // 解析 JSON 数据
            ObjectDataList objectDataList = JsonUtility.FromJson<ObjectDataList>(jsonData);

            if (objectDataList == null || objectDataList.objects == null)
            {
                Debug.LogWarning("JSON 数据解析失败或为空！");
                return;
            }

            Debug.Log($"成功解析 JSON 数据，组件数量：{objectDataList.objects.Count}");

            // 更新或实例化组件位置
            foreach (ObjectData data in objectDataList.objects)
            {
                Debug.Log($"JSON 数据 - 组件名称：{data.name}, 位置：({data.position.x}, {data.position.y}, {data.position.z})");

                if (components.ContainsKey(data.name))
                {
                    // 更新已有组件的位置
                    Transform target = components[data.name];
                    target.position = new Vector3(data.position.x, data.position.y, data.position.z);
                    Debug.Log($"组件 {data.name} 的位置更新为：({target.position.x}, {target.position.y}, {target.position.z})");
                }
                else
                {
                    // 尝试实例化 Prefab
                    Debug.LogWarning($"未找到名为 {data.name} 的组件，尝试实例化 Prefab。");

                    GameObject prefab = Resources.Load<GameObject>("Prefabs/" + data.name);
                    if (prefab != null)
                    {
                        GameObject instance = Instantiate(prefab);
                        instance.name = data.name; // 确保名称与 JSON 中一致
                        instance.transform.position = new Vector3(data.position.x, data.position.y, data.position.z);
                        components[data.name] = instance.transform; // 添加到字典
                        Debug.Log($"实例化 Prefab {data.name} 并设置位置到 ({data.position.x}, {data.position.y}, {data.position.z})");
                    }
                    else
                    {
                        Debug.LogWarning($"未找到名为 {data.name} 的 Prefab！");
                    }
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"解析 JSON 数据时出错：{ex.Message}");
        }
    }
}

// 用于表示 JSON 中单个对象的数据
[System.Serializable]
public class ObjectData
{
    public string name;
    public PositionData position;
}

// 用于表示 JSON 中的对象列表
[System.Serializable]
public class ObjectDataList
{
    public List<ObjectData> objects;
}

// 用于表示位置数据
[System.Serializable]
public class PositionData
{
    public float x;
    public float y;
    public float z;
}
