using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using VRChat.BlendShapesExtractor;

namespace VRChat.BlendShapesExtractor.Editor
{
    /// <summary>
    /// BlendShape提取器的编辑器窗口
    /// </summary>
    public class BlendShapeExtractorWindow : EditorWindow
    {
        private GameObject targetGameObject;
        private string outputPath = "Assets/BlendShapeData/";
        private string fileName = "BlendShapeData";
        private Vector2 scrollPosition;
        private bool showAdvancedOptions = false;
        private bool includeFrameDetails = true;
        
        // 缓存变量，避免重复提取
        private GameObject cachedGameObject;
        private List<MeshBlendShapeData> cachedMeshDataList;
        
        // 导入功能相关变量
        private bool showImportSection = false;
        private GameObject importTargetGameObject;
        private TextAsset importJsonAsset;
        private MeshBlendShapeData importData;
        private Dictionary<string, bool> blendShapeSelections = new Dictionary<string, bool>();
        private Vector2 importScrollPosition;
        private bool allSelected = false;
        
        [MenuItem("VRChat Tools/BlendShape Extractor")]
        public static void ShowWindow()
        {
            var window = GetWindow<BlendShapeExtractorWindow>();
            window.titleContent = new GUIContent(Localization.GetString("window_title"));
            window.minSize = new Vector2(400, 300);
        }
        
        private void OnGUI()
        {
            // 语言选择
            DrawLanguageSelector();
            EditorGUILayout.Space(5);
            
            EditorGUILayout.LabelField(Localization.GetString("window_title"), EditorStyles.boldLabel);
            EditorGUILayout.Space(10);
            
            DrawInputSection();
            EditorGUILayout.Space(10);
            
            DrawOutputSection();
            EditorGUILayout.Space(10);
            
            DrawAdvancedOptions();
            EditorGUILayout.Space(10);
            
            DrawActionButtons();
            EditorGUILayout.Space(10);
            
            DrawImportSection();
            EditorGUILayout.Space(10);
            
            DrawPreviewSection();
        }
        
        private void DrawLanguageSelector()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(Localization.GetString("language"), GUILayout.Width(60));
            
            var currentLanguage = Localization.CurrentLanguage;
            var languages = System.Enum.GetValues(typeof(Language)) as Language[];
            var languageNames = new string[languages.Length];
            var currentIndex = 0;
            
            for (int i = 0; i < languages.Length; i++)
            {
                languageNames[i] = Localization.GetLanguageDisplayName(languages[i]);
                if (languages[i] == currentLanguage)
                    currentIndex = i;
            }
            
            var newIndex = EditorGUILayout.Popup(currentIndex, languageNames, GUILayout.Width(100));
            
            if (newIndex != currentIndex)
            {
                Localization.CurrentLanguage = languages[newIndex];
                titleContent = new GUIContent(Localization.GetString("window_title"));
                Repaint();
            }
            
            EditorGUILayout.EndHorizontal();
        }
        
        private void DrawInputSection()
        {
            EditorGUILayout.LabelField(Localization.GetString("input_settings"), EditorStyles.boldLabel);
            
            EditorGUI.BeginChangeCheck();
            targetGameObject = (GameObject)EditorGUILayout.ObjectField(
                Localization.GetString("target_gameobject"), 
                targetGameObject, 
                typeof(GameObject), 
                true
            );

            
            if (EditorGUI.EndChangeCheck())
            {
                // 当选择改变时，自动更新文件名
                if (targetGameObject != null)
                {
                    fileName = targetGameObject.name + "_BlendShapes";
                }
            }
            
            // 显示信息
            if (targetGameObject != null)
            {
                var renderers = targetGameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
                var filters = targetGameObject.GetComponentsInChildren<MeshFilter>();
                int totalBlendShapes = 0;
                
                foreach (var renderer in renderers)
                {
                    if (renderer.sharedMesh != null)
                        totalBlendShapes += renderer.sharedMesh.blendShapeCount;
                }
                
                foreach (var filter in filters)
                {
                    if (filter.sharedMesh != null)
                        totalBlendShapes += filter.sharedMesh.blendShapeCount;
                }
                
                EditorGUILayout.HelpBox(Localization.GetString("mesh_components_found", renderers.Length + filters.Length, totalBlendShapes), MessageType.Info);
            }
        }
        
        private void DrawOutputSection()
        {
            EditorGUILayout.LabelField(Localization.GetString("output_settings"), EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            outputPath = EditorGUILayout.TextField(Localization.GetString("output_path"), outputPath);
            if (GUILayout.Button(Localization.GetString("browse"), GUILayout.Width(60)))
            {
                string selectedPath = EditorUtility.SaveFolderPanel(Localization.GetString("select_output_folder"), outputPath, "");
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    // 转换为相对于Assets的路径
                    if (selectedPath.StartsWith(Application.dataPath))
                    {
                        outputPath = "Assets" + selectedPath.Substring(Application.dataPath.Length);
                    }
                    else
                    {
                        outputPath = selectedPath;
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
            
            fileName = EditorGUILayout.TextField(Localization.GetString("file_name"), fileName);
        }
        
        private void DrawAdvancedOptions()
        {
            showAdvancedOptions = EditorGUILayout.Foldout(showAdvancedOptions, Localization.GetString("advanced_options"));
            
            if (showAdvancedOptions)
            {
                EditorGUI.indentLevel++;
                includeFrameDetails = EditorGUILayout.Toggle(Localization.GetString("include_frame_details"), includeFrameDetails);
                EditorGUI.indentLevel--;
            }
        }
        
        private void DrawActionButtons()
        {
            EditorGUILayout.BeginHorizontal();
            
            GUI.enabled = (targetGameObject != null) && !string.IsNullOrEmpty(fileName);
            
            if (GUILayout.Button(Localization.GetString("extract_and_export"), GUILayout.Height(30)))
            {
                ExtractAndExport();
            }
            
            GUI.enabled = true;
            
            if (GUILayout.Button(Localization.GetString("clear"), GUILayout.Width(60), GUILayout.Height(30)))
            {
                ClearAll();
            }
            
            EditorGUILayout.EndHorizontal();
        }
        
        private void DrawPreviewSection()
        {
            if (targetGameObject != null)
            {
                EditorGUILayout.LabelField(Localization.GetString("preview"), EditorStyles.boldLabel);
                
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(150));
                
                // 检查是否需要重新提取数据
                if (cachedGameObject != targetGameObject)
                {
                    cachedGameObject = targetGameObject;
                    cachedMeshDataList = BlendShapeExtractor.ExtractBlendShapesFromGameObject(targetGameObject);
                }
                
                if (cachedMeshDataList != null)
                {
                    DrawMeshDataPreview(cachedMeshDataList);
                }
                
                EditorGUILayout.EndScrollView();
            }
        }
        
        private void DrawMeshDataPreview(List<MeshBlendShapeData> meshDataList)
        {
            foreach (var meshData in meshDataList)
            {
                DrawSingleMeshDataPreview(meshData);
                EditorGUILayout.Space(5);
            }
        }
        
        private void DrawSingleMeshDataPreview(MeshBlendShapeData meshData)
        {
            EditorGUILayout.LabelField(Localization.GetString("mesh_name", meshData.meshName), EditorStyles.boldLabel);
            EditorGUILayout.LabelField(Localization.GetString("blendshape_count", meshData.totalBlendShapeCount));
            
            EditorGUI.indentLevel++;
            foreach (var blendShape in meshData.blendShapes)
            {
                float weight = blendShape.frames.Count > 0 ? blendShape.frames[0].weight : 0f;
                EditorGUILayout.LabelField($"• {blendShape.name} ({Localization.GetString("weight", weight.ToString("F1"))})");
            }
            EditorGUI.indentLevel--;
        }
        
        private void ExtractAndExport()
        {
            try
            {
                // 确保输出目录存在
                if (!Directory.Exists(outputPath))
                {
                    Directory.CreateDirectory(outputPath);
                    AssetDatabase.Refresh();
                }
                
                string fullPath = Path.Combine(outputPath, fileName + ".json");
                bool success = false;
                
                if (targetGameObject != null)
                {
                    var meshDataList = BlendShapeExtractor.ExtractBlendShapesFromGameObject(targetGameObject);
                    if (meshDataList.Count > 0)
                    {
                        success = BlendShapeExtractor.ExportMultipleToJson(meshDataList, fullPath);
                    }
                    else
                    {
                        EditorUtility.DisplayDialog(Localization.GetString("error"), Localization.GetString("no_blendshapes_found"), Localization.GetString("ok"));
                        return;
                    }
                }
                
                if (success)
                {
                    AssetDatabase.Refresh();
                    
                    EditorUtility.DisplayDialog(Localization.GetString("success"), Localization.GetString("export_success", fullPath), Localization.GetString("ok"));
                    
                    // 在Project窗口中高亮显示文件
                    var asset = AssetDatabase.LoadAssetAtPath<TextAsset>(fullPath);
                    if (asset != null)
                    {
                        EditorGUIUtility.PingObject(asset);
                    }
                }
                else
                {
                    EditorUtility.DisplayDialog(Localization.GetString("error"), Localization.GetString("export_failed"), Localization.GetString("ok"));
                }
            }
            catch (System.Exception e)
            {
                EditorUtility.DisplayDialog(Localization.GetString("error"), Localization.GetString("export_error", e.Message), Localization.GetString("ok"));
                Debug.LogError($"BlendShapeExtractor Export Error: {e}");
            }
        }
        
        private void ClearAll()
        {
            targetGameObject = null;
            fileName = "BlendShapeData";
            
            // 清除缓存
            cachedGameObject = null;
            cachedMeshDataList = null;
        }
        
        private void DrawImportSection()
        {
            // 导入功能折叠面板
            showImportSection = EditorGUILayout.Foldout(showImportSection, Localization.GetString("import_function"), true, EditorStyles.foldoutHeader);
            
            if (showImportSection)
            {
                EditorGUILayout.BeginVertical("box");
                
                // 目标GameObject选择
                EditorGUILayout.LabelField(Localization.GetString("import_settings"), EditorStyles.boldLabel);
                importTargetGameObject = (GameObject)EditorGUILayout.ObjectField(
                    Localization.GetString("target_gameobject_import"), 
                    importTargetGameObject, 
                    typeof(GameObject), 
                    true
                );
                
                EditorGUILayout.Space(5);
                
                // JSON文件选择（拖拽式）
                TextAsset newJsonAsset = (TextAsset)EditorGUILayout.ObjectField(
                    Localization.GetString("json_file"), 
                    importJsonAsset, 
                    typeof(TextAsset), 
                    false
                );
                
                // 检测到新的JSON文件时自动加载
                if (newJsonAsset != importJsonAsset)
                {
                    importJsonAsset = newJsonAsset;
                    if (importJsonAsset != null)
                    {
                        LoadJsonFile();
                    }
                    else
                    {
                        // 清空数据
                        importData = null;
                        blendShapeSelections.Clear();
                    }
                }
                
                EditorGUILayout.Space(10);
                
                // 如果已加载JSON文件，显示BlendShape选择
                if (importData != null && importData.blendShapes != null && importData.blendShapes.Count > 0)
                {
                    DrawBlendShapeSelection();
                    EditorGUILayout.Space(10);
                    DrawImportButtons();
                }
                else if (importJsonAsset != null)
                {
                    EditorGUILayout.HelpBox(Localization.GetString("json_load_error"), MessageType.Warning);
                }
                
                EditorGUILayout.EndVertical();
            }
        }
        
        private void DrawBlendShapeSelection()
        {
            EditorGUILayout.LabelField(Localization.GetString("blendshape_selection", importData.blendShapes.Count), EditorStyles.boldLabel);
            
            // 全选/反选按钮
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(Localization.GetString("select_all")))
            {
                SelectAllBlendShapes(true);
            }
            if (GUILayout.Button(Localization.GetString("select_none")))
            {
                SelectAllBlendShapes(false);
            }
            if (GUILayout.Button(Localization.GetString("invert_selection")))
            {
                InvertBlendShapeSelection();
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space(5);
            
            // BlendShape列表
            importScrollPosition = EditorGUILayout.BeginScrollView(importScrollPosition, GUILayout.MaxHeight(200));
            
            foreach (var blendShape in importData.blendShapes)
            {
                if (!blendShapeSelections.ContainsKey(blendShape.name))
                {
                    blendShapeSelections[blendShape.name] = false;
                }
                
                EditorGUILayout.BeginHorizontal();
                blendShapeSelections[blendShape.name] = EditorGUILayout.Toggle(blendShapeSelections[blendShape.name], GUILayout.Width(20));
                EditorGUILayout.LabelField(blendShape.name);
                
                // 显示权重值
                if (blendShape.frames != null && blendShape.frames.Count > 0)
                {
                    EditorGUILayout.LabelField($"权重: {blendShape.frames[0].weight:F1}", GUILayout.Width(80));
                }
                
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.EndScrollView();
        }
        
        private void DrawImportButtons()
        {
            GUI.enabled = importTargetGameObject != null && importJsonAsset != null && importData != null && GetSelectedBlendShapes().Count > 0;
            
            if (GUILayout.Button(Localization.GetString("import_blendshapes")))
            {
                PerformImport(false);
            }
            
            GUI.enabled = true;
            
            // 显示选中数量
            int selectedCount = GetSelectedBlendShapes().Count;
            EditorGUILayout.LabelField(Localization.GetString("selected_count", selectedCount));
        }
        
        private void LoadJsonFile()
        {
            try
            {
                if (importJsonAsset != null && !string.IsNullOrEmpty(importJsonAsset.text))
                {
                    string jsonContent = importJsonAsset.text;
                    blendShapeSelections.Clear();
                    
                    // 首先尝试作为单个mesh数据加载
                    try
                    {
                        importData = JsonUtility.FromJson<MeshBlendShapeData>(jsonContent);
                        if (importData != null && importData.blendShapes != null && importData.blendShapes.Count > 0)
                        {
                            foreach (var blendShape in importData.blendShapes)
                            {
                                blendShapeSelections[blendShape.name] = true; // 自动选择所有BlendShape
                            }
                            return;
                        }
                    }
                    catch (System.Exception)
                    {
                        // 忽略解析异常
                    }
                    
                    // 尝试作为多mesh数据加载
                    try
                    {
                        var collection = JsonUtility.FromJson<MeshBlendShapeDataCollection>(jsonContent);
                        if (collection != null && collection.meshes != null && collection.meshes.Count > 0)
                        {
                            // 如果有多个mesh，使用第一个包含BlendShape的mesh
                            foreach (var mesh in collection.meshes)
                            {
                                if (mesh.blendShapes != null && mesh.blendShapes.Count > 0)
                                {
                                    importData = mesh;
                                    foreach (var blendShape in mesh.blendShapes)
                                    {
                                        blendShapeSelections[blendShape.name] = true; // 自动选择所有BlendShape
                                    }
                                    return;
                                }
                            }
                        }
                    }
                    catch (System.Exception)
                    {
                        // 忽略解析异常
                    }
                    
                    importData = null;
                    blendShapeSelections.Clear();
                }
                else
                {
                    importData = null;
                    blendShapeSelections.Clear();
                }
            }
            catch (System.Exception)
            {
                importData = null;
                blendShapeSelections.Clear();
            }
        }
        
        private void SelectAllBlendShapes(bool selected)
        {
            var keys = new List<string>(blendShapeSelections.Keys);
            foreach (string key in keys)
            {
                blendShapeSelections[key] = selected;
            }
        }
        
        private void InvertBlendShapeSelection()
        {
            var keys = new List<string>(blendShapeSelections.Keys);
            foreach (string key in keys)
            {
                blendShapeSelections[key] = !blendShapeSelections[key];
            }
        }
        
        private List<string> GetSelectedBlendShapes()
        {
            var selected = new List<string>();
            foreach (var kvp in blendShapeSelections)
            {
                if (kvp.Value)
                {
                    selected.Add(kvp.Key);
                }
            }
            return selected;
        }
        
        private void PerformImport(bool forceImport)
        {
            try
            {
                var selectedBlendShapes = GetSelectedBlendShapes();
                

                
                if (selectedBlendShapes.Count == 0)
                {
                    EditorUtility.DisplayDialog(Localization.GetString("import_error"), Localization.GetString("no_blendshapes_selected"), Localization.GetString("confirm"));
                    return;
                }
                
                var result = BlendShapeExtractor.ImportFromJson(importJsonAsset.text, importTargetGameObject, selectedBlendShapes, forceImport);
                
                if (result.needsUserConfirmation && !forceImport)
                {
                    // 显示警告对话框
                    bool userConfirmed = EditorUtility.DisplayDialog(
                        Localization.GetString("blendshape_mismatch_warning"), 
                        result.warningMessage, 
                        Localization.GetString("continue_import"), 
                        Localization.GetString("cancel")
                    );
                    
                    if (userConfirmed)
                    {
                        // 用户确认后直接强制导入，避免重复调用
                        var forceResult = BlendShapeExtractor.ImportFromJson(importJsonAsset.text, importTargetGameObject, selectedBlendShapes, true);
                        
                        if (forceResult.success)
                        {
                            EditorUtility.DisplayDialog(Localization.GetString("import_success"), forceResult.successMessage, Localization.GetString("confirm"));
                            EditorUtility.SetDirty(importTargetGameObject);
                        }
                        else
                        {
                            EditorUtility.DisplayDialog(Localization.GetString("import_error"), forceResult.errorMessage, Localization.GetString("confirm"));
                        }
                    }
                }
                else if (result.success)
                {
                    EditorUtility.DisplayDialog(Localization.GetString("import_success"), result.successMessage, Localization.GetString("confirm"));
                    
                    // 标记场景为已修改
                    EditorUtility.SetDirty(importTargetGameObject);
                }
                else
                {
                    EditorUtility.DisplayDialog(Localization.GetString("import_error"), result.errorMessage, Localization.GetString("confirm"));
                }
            }
            catch (System.Exception e)
            {
                EditorUtility.DisplayDialog(Localization.GetString("import_error"), Localization.GetString("import_error") + ": " + e.Message, Localization.GetString("confirm"));
            }
        }
    }
}