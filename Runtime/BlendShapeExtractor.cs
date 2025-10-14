using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        
        /// <summary>
        /// 从JSON文件导入BlendShape数据到指定的GameObject
        /// </summary>
        /// <param name="jsonFilePath">JSON文件路径</param>
        /// <param name="targetGameObject">目标GameObject</param>
        /// <param name="selectedBlendShapes">用户选择要导入的BlendShape名称列表</param>
        /// <param name="forceImport">是否强制导入（忽略不匹配警告）</param>
        /// <returns>导入结果信息</returns>
        public static ImportResult ImportFromJson(string jsonContent, GameObject targetGameObject, List<string> selectedBlendShapes, bool forceImport = false)
        {
            var result = new ImportResult();
            
            try
            {
                // 验证JSON内容
                if (string.IsNullOrEmpty(jsonContent))
                {
                    result.success = false;
                    result.errorMessage = "JSON内容为空";
                    return result;
                }
                
                MeshBlendShapeData importData = null;
                
                // 首先尝试作为单个mesh数据加载
                try
                {
                    importData = JsonUtility.FromJson<MeshBlendShapeData>(jsonContent);
                    if (importData == null || importData.blendShapes == null || importData.blendShapes.Count == 0)
                    {
                        importData = null;
                    }
                }
                catch (System.Exception)
                {
                    importData = null;
                }
                
                // 如果单个mesh格式失败，尝试多mesh格式
                if (importData == null)
                {
                    try
                    {
                        var collection = JsonUtility.FromJson<MeshBlendShapeDataCollection>(jsonContent);
                        if (collection != null && collection.meshes != null && collection.meshes.Count > 0)
                        {
                            // 使用第一个包含BlendShape的mesh
                            foreach (var mesh in collection.meshes)
                            {
                                if (mesh.blendShapes != null && mesh.blendShapes.Count > 0)
                                {
                                    importData = mesh;
                                    break;
                                }
                            }
                        }
                    }
                    catch (System.Exception)
                    {
                        // 忽略解析异常
                    }
                }
                
                if (importData == null)
                {
                    result.success = false;
                    result.errorMessage = "JSON文件格式无效或不包含有效的BlendShape数据";
                    return result;
                }
                
                // 获取目标GameObject的所有SkinnedMeshRenderer
                var skinnedMeshRenderers = targetGameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
                if (skinnedMeshRenderers.Length == 0)
                {
                    result.success = false;
                    result.errorMessage = "目标GameObject中没有找到SkinnedMeshRenderer组件";
                    return result;
                }
                
                // 分析BlendShape匹配情况
                result.matchAnalysis = AnalyzeBlendShapeMatching(importData, skinnedMeshRenderers, selectedBlendShapes);
                
                // 如果有不匹配的情况且用户没有强制导入，返回警告
                if (!result.matchAnalysis.isFullMatch && !forceImport)
                {
                    result.success = false;
                    result.needsUserConfirmation = true;
                    result.warningMessage = GenerateMatchingWarningMessage(result.matchAnalysis);
                    return result;
                }
                
                // 执行导入
                int importedCount = 0;
                foreach (var renderer in skinnedMeshRenderers)
                {
                    if (renderer.sharedMesh == null) continue;
                    
                    var mesh = renderer.sharedMesh;
                    for (int i = 0; i < mesh.blendShapeCount; i++)
                    {
                        string blendShapeName = mesh.GetBlendShapeName(i);
                        
                        // 检查是否在选择列表中
                        if (!selectedBlendShapes.Contains(blendShapeName)) continue;
                        
                        // 在导入数据中查找匹配的BlendShape
                        var matchingBlendShape = importData.blendShapes.Find(bs => bs.name == blendShapeName);
                        if (matchingBlendShape != null && matchingBlendShape.frames.Count > 0)
                        {
                            // 应用权重值（使用第一帧的权重）
                            renderer.SetBlendShapeWeight(i, matchingBlendShape.frames[0].weight);
                            importedCount++;
                        }
                    }
                }
                
                result.success = true;
                result.importedCount = importedCount;
                result.successMessage = $"成功导入 {importedCount} 个BlendShape权重值";
                
                return result;
            }
            catch (System.Exception e)
            {
                result.success = false;
                result.errorMessage = $"导入过程中发生错误: {e.Message}";
                return result;
            }
        }
        
        /// <summary>
        /// 分析BlendShape匹配情况
        /// </summary>
        private static BlendShapeMatchAnalysis AnalyzeBlendShapeMatching(MeshBlendShapeData importData, SkinnedMeshRenderer[] renderers, List<string> selectedBlendShapes)
        {
            var analysis = new BlendShapeMatchAnalysis();
            
            // 收集目标Mesh中的所有BlendShape名称
            var targetBlendShapes = new HashSet<string>();
            foreach (var renderer in renderers)
            {
                if (renderer.sharedMesh == null) continue;
                
                var mesh = renderer.sharedMesh;
                for (int i = 0; i < mesh.blendShapeCount; i++)
                {
                    string blendShapeName = mesh.GetBlendShapeName(i);
                    targetBlendShapes.Add(blendShapeName);
                }
            }
            
            // 收集导入数据中的BlendShape名称
            var importBlendShapes = new HashSet<string>();
            foreach (var blendShape in importData.blendShapes)
            {
                importBlendShapes.Add(blendShape.name);
            }
            
            // 分析匹配情况
            analysis.targetBlendShapes = new List<string>(targetBlendShapes);
            analysis.importBlendShapes = new List<string>(importBlendShapes);
            analysis.matchingBlendShapes = new List<string>();
            analysis.missingInTarget = new List<string>();
            analysis.missingInImport = new List<string>();
            
            // 只分析选中的BlendShape的匹配情况
            foreach (string selectedName in selectedBlendShapes)
            {
                // 详细检查目标中的匹配
                bool foundInTarget = false;
                foreach (var targetName in targetBlendShapes)
                {
                    if (targetName == selectedName)
                    {
                        foundInTarget = true;
                        break;
                    }
                    else if (targetName.Trim() == selectedName.Trim())
                    {
                        foundInTarget = true;
                        break;
                    }
                }
                
                // 详细检查JSON中的匹配
                bool foundInImport = false;
                foreach (var importName in importBlendShapes)
                {
                    if (importName == selectedName)
                    {
                        foundInImport = true;
                        break;
                    }
                    else if (importName.Trim() == selectedName.Trim())
                    {
                        foundInImport = true;
                        break;
                    }
                }
                
                // 显示匹配结果
                if (foundInTarget && foundInImport)
                {
                    analysis.matchingBlendShapes.Add(selectedName);
                }
                else if (!foundInTarget)
                {
                    analysis.missingInTarget.Add(selectedName);
                }
                else if (!foundInImport)
                {
                    analysis.missingInImport.Add(selectedName);
                }
            }
            
            // 判断是否完全匹配：只要选中的BlendShape都能匹配就算完全匹配
            analysis.isFullMatch = selectedBlendShapes.Count > 0 && analysis.matchingBlendShapes.Count == selectedBlendShapes.Count;
            
            return analysis;
        }
        
        /// <summary>
        /// 生成匹配警告消息
        /// </summary>
        private static string GenerateMatchingWarningMessage(BlendShapeMatchAnalysis analysis)
        {
            var message = "BlendShape匹配检查结果:\n\n";
            
            if (analysis.matchingBlendShapes.Count > 0)
            {
                message += $"✓ 可以导入的BlendShape ({analysis.matchingBlendShapes.Count}个):\n";
                foreach (string name in analysis.matchingBlendShapes)
                {
                    message += $"  • {name}\n";
                }
                message += "\n";
            }
            
            if (analysis.missingInTarget.Count > 0)
            {
                message += $"⚠ 目标Mesh中不存在的BlendShape ({analysis.missingInTarget.Count}个):\n";
                foreach (string name in analysis.missingInTarget)
                {
                    message += $"  • {name}\n";
                }
                message += "\n";
            }
            
            if (analysis.missingInImport.Count > 0)
            {
                message += $"⚠ JSON文件中不存在的BlendShape ({analysis.missingInImport.Count}个):\n";
                foreach (string name in analysis.missingInImport)
                {
                    message += $"  • {name}\n";
                }
                message += "\n";
            }
            
            if (analysis.matchingBlendShapes.Count > 0)
            {
                message += "是否继续导入匹配的BlendShape？";
            }
            else
            {
                message += "没有可以导入的BlendShape，请检查选择或文件内容。";
            }
            
            return message;
        }
    }
    
    /// <summary>
    /// 导入结果类
    /// </summary>
    [System.Serializable]
    public class ImportResult
    {
        public bool success;
        public bool needsUserConfirmation;
        public string errorMessage;
        public string warningMessage;
        public string successMessage;
        public int importedCount;
        public BlendShapeMatchAnalysis matchAnalysis;
    }
    
    /// <summary>
    /// BlendShape匹配分析结果
    /// </summary>
    [System.Serializable]
    public class BlendShapeMatchAnalysis
    {
        public bool isFullMatch;
        public List<string> targetBlendShapes;
        public List<string> importBlendShapes;
        public List<string> matchingBlendShapes;
        public List<string> missingInTarget;
        public List<string> missingInImport;
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