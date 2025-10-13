using System.Collections.Generic;
using UnityEngine;

namespace VRChat.BlendShapesExtractor.Editor
{
    /// <summary>
    /// 支持的语言类型
    /// </summary>
    public enum Language
    {
        English,
        Chinese,
        Japanese
    }

    /// <summary>
    /// 多语言支持类
    /// </summary>
    public static class Localization
    {
        private static Language _currentLanguage = Language.English;
        
        public static Language CurrentLanguage
        {
            get => _currentLanguage;
            set => _currentLanguage = value;
        }

        private static readonly Dictionary<string, Dictionary<Language, string>> LocalizedStrings = 
            new Dictionary<string, Dictionary<Language, string>>
            {
                ["language"] = new Dictionary<Language, string>
                {
                    [Language.English] = "Language",
                    [Language.Chinese] = "语言",
                    [Language.Japanese] = "言語"
                },
                ["window_title"] = new Dictionary<Language, string>
                {
                    [Language.English] = "VRChat BlendShape Extractor",
                    [Language.Chinese] = "VRChat BlendShape 提取器",
                    [Language.Japanese] = "VRChat BlendShape エクストラクター"
                },
                ["input_settings"] = new Dictionary<Language, string>
                {
                    [Language.English] = "Input Settings",
                    [Language.Chinese] = "输入设置",
                    [Language.Japanese] = "入力設定"
                },
                ["target_gameobject"] = new Dictionary<Language, string>
                {
                    [Language.English] = "Target GameObject",
                    [Language.Chinese] = "目标GameObject",
                    [Language.Japanese] = "ターゲットGameObject"
                },
                ["mesh_components_found"] = new Dictionary<Language, string>
                {
                    [Language.English] = "Found {0} Mesh components with {1} BlendShapes total",
                    [Language.Chinese] = "发现 {0} 个Mesh组件，总共 {1} 个BlendShape",
                    [Language.Japanese] = "{0}個のMeshコンポーネントで合計{1}個のBlendShapeを発見"
                },
                ["output_settings"] = new Dictionary<Language, string>
                {
                    [Language.English] = "Output Settings",
                    [Language.Chinese] = "输出设置",
                    [Language.Japanese] = "出力設定"
                },
                ["output_path"] = new Dictionary<Language, string>
                {
                    [Language.English] = "Output Path",
                    [Language.Chinese] = "输出路径",
                    [Language.Japanese] = "出力パス"
                },
                ["browse"] = new Dictionary<Language, string>
                {
                    [Language.English] = "Browse",
                    [Language.Chinese] = "浏览",
                    [Language.Japanese] = "参照"
                },
                ["select_output_folder"] = new Dictionary<Language, string>
                {
                    [Language.English] = "Select Output Folder",
                    [Language.Chinese] = "选择输出文件夹",
                    [Language.Japanese] = "出力フォルダを選択"
                },
                ["file_name"] = new Dictionary<Language, string>
                {
                    [Language.English] = "File Name",
                    [Language.Chinese] = "文件名",
                    [Language.Japanese] = "ファイル名"
                },
                ["advanced_options"] = new Dictionary<Language, string>
                {
                    [Language.English] = "Advanced Options",
                    [Language.Chinese] = "高级选项",
                    [Language.Japanese] = "詳細オプション"
                },
                ["include_frame_details"] = new Dictionary<Language, string>
                {
                    [Language.English] = "Include Frame Details",
                    [Language.Chinese] = "包含帧详细信息",
                    [Language.Japanese] = "フレーム詳細を含む"
                },
                ["extract_and_export"] = new Dictionary<Language, string>
                {
                    [Language.English] = "Extract and Export JSON",
                    [Language.Chinese] = "提取并导出JSON",
                    [Language.Japanese] = "抽出してJSONエクスポート"
                },
                ["clear"] = new Dictionary<Language, string>
                {
                    [Language.English] = "Clear",
                    [Language.Chinese] = "清除",
                    [Language.Japanese] = "クリア"
                },
                ["preview"] = new Dictionary<Language, string>
                {
                    [Language.English] = "Preview",
                    [Language.Chinese] = "预览",
                    [Language.Japanese] = "プレビュー"
                },
                ["mesh_name"] = new Dictionary<Language, string>
                {
                    [Language.English] = "Mesh: {0}",
                    [Language.Chinese] = "Mesh: {0}",
                    [Language.Japanese] = "Mesh: {0}"
                },
                ["blendshape_count"] = new Dictionary<Language, string>
                {
                    [Language.English] = "BlendShape Count: {0}",
                    [Language.Chinese] = "BlendShape数量: {0}",
                    [Language.Japanese] = "BlendShape数: {0}"
                },
                ["weight"] = new Dictionary<Language, string>
                {
                    [Language.English] = "Weight: {0}",
                    [Language.Chinese] = "权重: {0}",
                    [Language.Japanese] = "ウェイト: {0}"
                },
                ["success"] = new Dictionary<Language, string>
                {
                    [Language.English] = "Success",
                    [Language.Chinese] = "成功",
                    [Language.Japanese] = "成功"
                },
                ["error"] = new Dictionary<Language, string>
                {
                    [Language.English] = "Error",
                    [Language.Chinese] = "错误",
                    [Language.Japanese] = "エラー"
                },
                ["ok"] = new Dictionary<Language, string>
                {
                    [Language.English] = "OK",
                    [Language.Chinese] = "确定",
                    [Language.Japanese] = "OK"
                },
                ["no_blendshapes_found"] = new Dictionary<Language, string>
                {
                    [Language.English] = "No BlendShapes found in the specified GameObject",
                    [Language.Chinese] = "在指定的GameObject中未找到包含BlendShape的Mesh",
                    [Language.Japanese] = "指定されたGameObjectでBlendShapeが見つかりません"
                },
                ["export_success"] = new Dictionary<Language, string>
                {
                    [Language.English] = "BlendShape data successfully exported to:\n{0}",
                    [Language.Chinese] = "BlendShape数据已成功导出到:\n{0}",
                    [Language.Japanese] = "BlendShapeデータが正常にエクスポートされました:\n{0}"
                },
                ["export_failed"] = new Dictionary<Language, string>
                {
                    [Language.English] = "Export failed, please check console for error messages",
                    [Language.Chinese] = "导出失败，请检查控制台错误信息",
                    [Language.Japanese] = "エクスポートに失敗しました。コンソールでエラーメッセージを確認してください"
                },
                ["export_error"] = new Dictionary<Language, string>
                {
                    [Language.English] = "An error occurred during export:\n{0}",
                    [Language.Chinese] = "导出过程中发生错误:\n{0}",
                    [Language.Japanese] = "エクスポート中にエラーが発生しました:\n{0}"
                }
            };

        public static string GetString(string key)
        {
            if (LocalizedStrings.TryGetValue(key, out var translations))
            {
                if (translations.TryGetValue(_currentLanguage, out var translation))
                {
                    return translation;
                }
            }
            return key; // 如果找不到翻译，返回键名
        }

        public static string GetString(string key, params object[] args)
        {
            string format = GetString(key);
            try
            {
                return string.Format(format, args);
            }
            catch
            {
                return format;
            }
        }

        public static string GetLanguageDisplayName(Language language)
        {
            switch (language)
            {
                case Language.English: return "English";
                case Language.Chinese: return "中文";
                case Language.Japanese: return "日本語";
                default: return language.ToString();
            }
        }
    }
}