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
                    [Language.English] = "Extract & Export",
                    [Language.Chinese] = "提取并导出",
                    [Language.Japanese] = "抽出＆エクスポート"
                },
                ["copy_to_clipboard"] = new Dictionary<Language, string>
                {
                    [Language.English] = "Copy to Clipboard",
                    [Language.Chinese] = "复制到剪贴板",
                    [Language.Japanese] = "クリップボードにコピー"
                },
                ["copy_success"] = new Dictionary<Language, string>
                {
                    [Language.English] = "Successfully copied BlendShape data from {0} mesh(es) to clipboard!",
                    [Language.Chinese] = "成功将 {0} 个Mesh的BlendShape数据复制到剪贴板！",
                    [Language.Japanese] = "{0}個のMeshのBlendShapeデータをクリップボードにコピーしました！"
                },
                ["copy_error"] = new Dictionary<Language, string>
                {
                    [Language.English] = "Failed to copy to clipboard: {0}",
                    [Language.Chinese] = "复制到剪贴板失败：{0}",
                    [Language.Japanese] = "クリップボードへのコピーに失敗しました：{0}"
                },
                ["no_target_selected"] = new Dictionary<Language, string>
                {
                    [Language.English] = "Please select a target GameObject first.",
                    [Language.Chinese] = "请先选择一个目标GameObject。",
                    [Language.Japanese] = "まずターゲットGameObjectを選択してください。"
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
                },
                ["copy_json_success"] = new Dictionary<Language, string>
                {
                    [Language.English] = "JSON copied to clipboard successfully!",
                    [Language.Chinese] = "JSON已成功复制到剪贴板！",
                    [Language.Japanese] = "JSONがクリップボードにコピーされました！"
                },
                ["copy_json_failed"] = new Dictionary<Language, string>
                {
                    [Language.English] = "Failed to copy JSON to clipboard",
                    [Language.Chinese] = "复制JSON到剪贴板失败",
                    [Language.Japanese] = "JSONのクリップボードへのコピーに失敗しました"
                },
                ["copy_json_no_target"] = new Dictionary<Language, string>
                {
                    [Language.English] = "Please select a target GameObject first",
                    [Language.Chinese] = "请先选择目标GameObject",
                    [Language.Japanese] = "まず対象のGameObjectを選択してください"
                },
                
                // 导入功能的文本框相关
                ["input_method"] = new Dictionary<Language, string>
                {
                    [Language.English] = "Input Method",
                    [Language.Chinese] = "输入方式",
                    [Language.Japanese] = "入力方法"
                },
                ["use_text_input"] = new Dictionary<Language, string>
                {
                    [Language.English] = "Use Text Input",
                    [Language.Chinese] = "使用文本输入",
                    [Language.Japanese] = "テキスト入力を使用"
                },
                ["json_text_input"] = new Dictionary<Language, string>
                {
                    [Language.English] = "JSON Text Input",
                    [Language.Chinese] = "JSON文本输入",
                    [Language.Japanese] = "JSONテキスト入力"
                },
                ["paste_from_clipboard"] = new Dictionary<Language, string>
                {
                    [Language.English] = "Paste from Clipboard",
                    [Language.Chinese] = "从剪贴板粘贴",
                    [Language.Japanese] = "クリップボードから貼り付け"
                },
                ["clear_text"] = new Dictionary<Language, string>
                {
                    [Language.English] = "Clear",
                    [Language.Chinese] = "清空",
                    [Language.Japanese] = "クリア"
                },
                ["json_content_empty"] = new Dictionary<Language, string>
                {
                    [Language.English] = "JSON content is empty",
                    [Language.Chinese] = "JSON内容为空",
                    [Language.Japanese] = "JSONコンテンツが空です"
                },
                ["import_function"] = new Dictionary<Language, string>
                {
                    [Language.English] = "Import BlendShapes via JSON File",
                    [Language.Chinese] = "通过JSON文件导入BlendShapes",
                    [Language.Japanese] = "JSONファイル経由でBlendShapesをインポート"
                },
                ["import_settings"] = new Dictionary<Language, string>
                {
                    [Language.English] = "Import Settings",
                    [Language.Chinese] = "导入设置",
                    [Language.Japanese] = "インポート設定"
                },
                ["target_gameobject_import"] = new Dictionary<Language, string>
                {
                    [Language.English] = "Target GameObject",
                    [Language.Chinese] = "目标GameObject",
                    [Language.Japanese] = "ターゲットGameObject"
                },
                ["json_file"] = new Dictionary<Language, string>
                {
                    [Language.English] = "JSON File",
                    [Language.Chinese] = "JSON文件",
                    [Language.Japanese] = "JSONファイル"
                },
                ["import_blendshapes"] = new Dictionary<Language, string>
                {
                    [Language.English] = "Import BlendShapes",
                    [Language.Chinese] = "导入BlendShape",
                    [Language.Japanese] = "BlendShapeをインポート"
                },
                ["selected_count"] = new Dictionary<Language, string>
                {
                    [Language.English] = "Selected: {0} BlendShapes",
                    [Language.Chinese] = "已选择: {0} 个BlendShape",
                    [Language.Japanese] = "選択済み: {0} BlendShape"
                },
                ["import_success"] = new Dictionary<Language, string>
                {
                    [Language.English] = "Import Successful",
                    [Language.Chinese] = "导入成功",
                    [Language.Japanese] = "インポート成功"
                },
                ["import_error"] = new Dictionary<Language, string>
                {
                    [Language.English] = "Import Error",
                    [Language.Chinese] = "导入错误",
                    [Language.Japanese] = "インポートエラー"
                },
                ["no_blendshapes_selected"] = new Dictionary<Language, string>
                {
                    [Language.English] = "Please select at least one BlendShape",
                    [Language.Chinese] = "请至少选择一个BlendShape",
                    [Language.Japanese] = "少なくとも1つのBlendShapeを選択してください"
                },
                ["confirm"] = new Dictionary<Language, string>
                {
                    [Language.English] = "OK",
                    [Language.Chinese] = "确定",
                    [Language.Japanese] = "OK"
                },
                ["cancel"] = new Dictionary<Language, string>
                {
                    [Language.English] = "Cancel",
                    [Language.Chinese] = "取消",
                    [Language.Japanese] = "キャンセル"
                },
                ["continue_import"] = new Dictionary<Language, string>
                {
                    [Language.English] = "Continue Import",
                    [Language.Chinese] = "继续导入",
                    [Language.Japanese] = "インポートを続行"
                },
                ["blendshape_mismatch_warning"] = new Dictionary<Language, string>
                {
                    [Language.English] = "BlendShape Mismatch Warning",
                    [Language.Chinese] = "BlendShape不匹配警告",
                    [Language.Japanese] = "BlendShape不一致警告"
                },
                ["json_load_error"] = new Dictionary<Language, string>
                {
                    [Language.English] = "Unable to load JSON file or no BlendShape data found",
                    [Language.Chinese] = "无法加载JSON文件或文件中没有BlendShape数据",
                    [Language.Japanese] = "JSONファイルを読み込めないか、BlendShapeデータが見つかりません"
                },
                ["blendshape_selection"] = new Dictionary<Language, string>
                {
                    [Language.English] = "BlendShape Selection (Total: {0})",
                    [Language.Chinese] = "BlendShape选择 (共{0}个)",
                    [Language.Japanese] = "BlendShape選択 (合計: {0}個)"
                },
                ["select_all"] = new Dictionary<Language, string>
                {
                    [Language.English] = "Select All",
                    [Language.Chinese] = "全选",
                    [Language.Japanese] = "すべて選択"
                },
                ["select_none"] = new Dictionary<Language, string>
                {
                    [Language.English] = "Select None",
                    [Language.Chinese] = "全不选",
                    [Language.Japanese] = "すべて解除"
                },
                ["invert_selection"] = new Dictionary<Language, string>
                {
                    [Language.English] = "Invert Selection",
                    [Language.Chinese] = "反选",
                    [Language.Japanese] = "選択を反転"
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