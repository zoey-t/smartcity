using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class ExtractComponentsPosition : MonoBehaviour
{
    // 输出 JSON 文件路径
    private string outputFilePath = Path.Combine(Application.streamingAssetsPath, "components.json");

    private void Start()
    {
        // 确保目标目录存在
        string directory = Path.GetDirectoryName(outputFilePath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
            Debug.Log("创建目录：" + directory);
        }

        // 提取组件位置并保存为 JSON 文件
        ExtractAndSavePositions();
    }

    private void ExtractAndSavePositions()
    {
        Debug.Log("开始提取组件位置...");
        Debug.Log("输出文件路径：" + outputFilePath);

        // 创建一个列表存储组件数据
        List<ComponentData> components = new List<ComponentData>();

        // 遍历当前物体的所有子物体
        foreach (Transform child in transform)
        {
            Debug.Log($"发现子物体：{child.name}，位置：{child.position}");

            // 获取组件名称和位置
            ComponentData data = new ComponentData
            {
                name = child.name,
                x = child.position.x,
                y = child.position.y,
                z = child.position.z
            };

            // 添加到列表中
            components.Add(data);
        }

        if (components.Count == 0)
        {
            Debug.LogWarning("没有找到任何子物体，未生成 JSON 数据。");
            return;
        }

        // 将数据转换为 JSON 格式
        ComponentDataList componentDataList = new ComponentDataList
        {
            components = components
        };

        string jsonData = JsonUtility.ToJson(componentDataList, true);
        Debug.Log("生成的 JSON 数据：\n" + jsonData);

        // 写入 JSON 文件
        try
        {
            File.WriteAllText(outputFilePath, jsonData);
            Debug.Log("组件位置信息已保存到：" + outputFilePath);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("写入文件时出错：" + ex.Message);
        }
    }
}

// 用于存储单个组件的数据
[System.Serializable]
public class ComponentData
{
    public string name;
    public float x;
    public float y;
    public float z;
}

// 用于存储所有组件数据的列表
[System.Serializable]
public class ComponentDataList
{
    public List<ComponentData> components;
}
