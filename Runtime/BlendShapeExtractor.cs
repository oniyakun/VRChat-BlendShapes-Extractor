using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace VRChat.BlendShapesExtractor
{
    /// <summary>
    /// BlendShape提取器核心类
    /// </summary>
    public static class BlendShapeExtractor
    {
        /// <summary>
        /// 从指定的Mesh中提取所有BlendShape数据
        /// </summary>
        /// <param name="mesh">要提取的Mesh对象</param>
        /// <param name="meshName">Mesh名称</param>
        /// <param name="meshPath">Mesh文件路径</param>
        /// <returns>提取的BlendShape数据</returns>
        public static MeshBlendShapeData ExtractBlendShapes(Mesh mesh, string meshName = "", string meshPath = "")
        {
            if (mesh == null)
            {
                Debug.LogError("BlendShapeExtractor: Mesh is null");
                return null;
            }
            
            var meshData = new MeshBlendShapeData(
                string.IsNullOrEmpty(meshName) ? mesh.name : meshName,
                meshPath
            );
            
            meshData.totalBlendShapeCount = mesh.blendShapeCount;
            
            // 遍历所有BlendShape
            for (int i = 0; i < mesh.blendShapeCount; i++)
            {
                string blendShapeName = mesh.GetBlendShapeName(i);
                int frameCount = mesh.GetBlendShapeFrameCount(i);
                
                var blendShapeInfo = new BlendShapeInfo(blendShapeName);
                
                // 遍历BlendShape的所有帧
                for (int frameIndex = 0; frameIndex < frameCount; frameIndex++)
                {
                    float frameWeight = mesh.GetBlendShapeFrameWeight(i, frameIndex);
                    
                    // 获取顶点数据来计算顶点数量
                    Vector3[] deltaVertices = new Vector3[mesh.vertexCount];
                    Vector3[] deltaNormals = new Vector3[mesh.vertexCount];
                    Vector3[] deltaTangents = new Vector3[mesh.vertexCount];
                    
                    mesh.GetBlendShapeFrameVertices(i, frameIndex, deltaVertices, deltaNormals, deltaTangents);
                    
                    var frame = new BlendShapeFrame(frameWeight);
                    blendShapeInfo.frames.Add(frame);
                }
                
                meshData.blendShapes.Add(blendShapeInfo);
            }
            
            Debug.Log($"BlendShapeExtractor: Successfully extracted {meshData.totalBlendShapeCount} blend shapes from mesh '{meshData.meshName}'");
            
            return meshData;
        }
        
        /// <summary>
        /// 从SkinnedMeshRenderer中提取BlendShape数据（包含实际权重值）
        /// </summary>
        /// <param name="renderer">SkinnedMeshRenderer组件</param>
        /// <param name="meshName">Mesh名称</param>
        /// <param name="meshPath">Mesh文件路径</param>
        /// <returns>提取的BlendShape数据</returns>
        public static MeshBlendShapeData ExtractBlendShapesFromRenderer(SkinnedMeshRenderer renderer, string meshName = "", string meshPath = "")
        {
            if (renderer == null || renderer.sharedMesh == null)
            {
                Debug.LogError("BlendShapeExtractor: SkinnedMeshRenderer or its mesh is null");
                return null;
            }
            
            var mesh = renderer.sharedMesh;
            var meshData = new MeshBlendShapeData(
                string.IsNullOrEmpty(meshName) ? mesh.name : meshName,
                meshPath
            );
            
            // 遍历所有BlendShape
            for (int i = 0; i < mesh.blendShapeCount; i++)
            {
                string blendShapeName = mesh.GetBlendShapeName(i);
                int frameCount = mesh.GetBlendShapeFrameCount(i);
                
                var blendShapeInfo = new BlendShapeInfo(blendShapeName);
                
                // 获取SkinnedMeshRenderer上设置的实际权重值
                float currentWeight = renderer.GetBlendShapeWeight(i);
                
                // 只添加一个帧，包含当前的实际权重值
                var frame = new BlendShapeFrame(currentWeight);
                blendShapeInfo.frames.Add(frame);
                
                meshData.blendShapes.Add(blendShapeInfo);
            }
            
            Debug.Log($"BlendShapeExtractor: Successfully extracted {meshData.totalBlendShapeCount} blend shapes with current weights from renderer '{meshData.meshName}'");
            
            return meshData;
        }
        
        /// <summary>
        /// 从GameObject中提取BlendShape数据
        /// </summary>
        /// <param name="gameObject">包含SkinnedMeshRenderer的GameObject</param>
        /// <returns>提取的BlendShape数据列表</returns>
        public static List<MeshBlendShapeData> ExtractBlendShapesFromGameObject(GameObject gameObject)
        {
            var results = new List<MeshBlendShapeData>();
            
            if (gameObject == null)
            {
                Debug.LogError("BlendShapeExtractor: GameObject is null");
                return results;
            }
            
            // 获取所有SkinnedMeshRenderer组件
            var skinnedMeshRenderers = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
            
            foreach (var renderer in skinnedMeshRenderers)
            {
                if (renderer.sharedMesh != null && renderer.sharedMesh.blendShapeCount > 0)
                {
                    string meshPath = GetMeshAssetPath(renderer.sharedMesh);
                    var meshData = ExtractBlendShapesFromRenderer(renderer, renderer.sharedMesh.name, meshPath);
                    
                    if (meshData != null)
                    {
                        results.Add(meshData);
                    }
                }
            }
            
            // 也检查MeshRenderer组件（虽然不常见，但可能存在）
            var meshFilters = gameObject.GetComponentsInChildren<MeshFilter>();
            
            foreach (var filter in meshFilters)
            {
                if (filter.sharedMesh != null && filter.sharedMesh.blendShapeCount > 0)
                {
                    string meshPath = GetMeshAssetPath(filter.sharedMesh);
                    var meshData = ExtractBlendShapes(filter.sharedMesh, filter.sharedMesh.name, meshPath);
                    
                    if (meshData != null)
                    {
                        results.Add(meshData);
                    }
                }
            }
            
            return results;
        }
        
        /// <summary>
        /// 获取Mesh资源的路径
        /// </summary>
        /// <param name="mesh">Mesh对象</param>
        /// <returns>资源路径</returns>
        private static string GetMeshAssetPath(Mesh mesh)
        {
#if UNITY_EDITOR
            return UnityEditor.AssetDatabase.GetAssetPath(mesh);
#else
            return "Runtime - Path not available";
#endif
        }
        
        /// <summary>
        /// 将BlendShape数据导出为JSON文件
        /// </summary>
        /// <param name="meshData">要导出的数据</param>
        /// <param name="filePath">导出文件路径</param>
        /// <returns>是否导出成功</returns>
        public static bool ExportToJson(MeshBlendShapeData meshData, string filePath)
        {
            try
            {
                if (meshData == null)
                {
                    Debug.LogError("BlendShapeExtractor: MeshBlendShapeData is null");
                    return false;
                }
                
                string json = JsonUtility.ToJson(meshData, true);
                
                // 格式化JSON中的权重值为一位小数
                json = FormatWeightValuesInJson(json);
                
                // 确保目录存在
                string directory = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                
                File.WriteAllText(filePath, json);
                
                Debug.Log($"BlendShapeExtractor: Successfully exported blend shape data to '{filePath}'");
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"BlendShapeExtractor: Failed to export JSON file. Error: {e.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// 格式化JSON字符串中的权重值为一位小数
        /// </summary>
        /// <param name="json">原始JSON字符串</param>
        /// <returns>格式化后的JSON字符串</returns>
        private static string FormatWeightValuesInJson(string json)
        {
            // 使用正则表达式匹配 "weight": 数字 的模式，包括科学计数法
            var regex = new Regex(@"""weight"":\s*([+-]?(?:\d+\.?\d*|\.\d+)(?:[eE][+-]?\d+)?)");
            
            return regex.Replace(json, match =>
            {
                string weightStr = match.Groups[1].Value;
                if (float.TryParse(weightStr, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float weight))
                {
                    return $"\"weight\": {weight.ToString("F1", System.Globalization.CultureInfo.InvariantCulture)}";
                }
                return match.Value; // 如果解析失败，返回原值
            });
        }
        
        /// <summary>
        /// 将多个BlendShape数据导出为JSON文件
        /// </summary>
        /// <param name="meshDataList">要导出的数据列表</param>
        /// <param name="filePath">导出文件路径</param>
        /// <returns>是否导出成功</returns>
        public static bool ExportMultipleToJson(List<MeshBlendShapeData> meshDataList, string filePath)
        {
            try
            {
                if (meshDataList == null || meshDataList.Count == 0)
                {
                    Debug.LogError("BlendShapeExtractor: MeshBlendShapeData list is null or empty");
                    return false;
                }
                
                var wrapper = new MeshBlendShapeDataCollection { meshes = meshDataList };
                string json = JsonUtility.ToJson(wrapper, true);
                
                // 格式化JSON中的权重值为一位小数
                json = FormatWeightValuesInJson(json);
                
                // 确保目录存在
                string directory = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                
                File.WriteAllText(filePath, json);
                
                Debug.Log($"BlendShapeExtractor: Successfully exported {meshDataList.Count} mesh blend shape data to '{filePath}'");
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"BlendShapeExtractor: Failed to export JSON file. Error: {e.Message}");
                return false;
            }
        }
    }
    
    /// <summary>
    /// 用于包装多个MeshBlendShapeData的容器类
    /// </summary>
    [System.Serializable]
    public class MeshBlendShapeDataCollection
    {
        public List<MeshBlendShapeData> meshes;
    }
}