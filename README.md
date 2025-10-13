# VRChat BlendShapes Extractor

一个Unity插件，用于从指定的mesh文件中提取所有BlendShapes名字和对应参数，并生成结构化的JSON数据文件。

## 功能特性

- 🎯 **精确提取**: 从Mesh或GameObject中提取所有BlendShape数据
- 📊 **详细信息**: 包含BlendShape名称、帧数、权重等完整信息
- 📁 **JSON导出**: 生成结构化的JSON文件，便于数据处理和分享
- 🖥️ **友好界面**: 提供直观的Unity编辑器窗口界面
- 🌐 **多语言支持**: 支持中文、英文、日文三种语言界面，默认为英文
- 🔍 **实时预览**: 在导出前预览将要提取的BlendShape数据
- ⚡ **批量处理**: 支持从GameObject中批量提取多个Mesh的BlendShape数据
- 📊 **精确数值**: 所有权重值精确到小数点后一位

## 安装方法

### 方法1: 直接复制文件
1. 将整个插件文件夹复制到你的Unity项目的 `Assets` 目录下
2. Unity会自动编译插件

### 方法2: Unity Package Manager (推荐)
1. 在Unity中打开 `Window > Package Manager`
2. 点击 `+` 按钮，选择 `Add package from disk...`
3. 选择插件根目录下的 `package.json` 文件

## 使用方法

### 通过编辑器窗口使用

1. 在Unity菜单栏中选择 `VRChat Tools > BlendShape Extractor`
2. 在打开的窗口中：
   - **语言选择**: 在窗口顶部选择界面语言（中文/English/日本語）
   - **目标GameObject**: 选择包含SkinnedMeshRenderer的GameObject
   - **输出路径**: 设置JSON文件的保存路径
   - **文件名**: 设置输出文件的名称

3. 点击 `提取并导出JSON` 按钮
4. 插件会自动提取BlendShape数据并保存为JSON文件，所有权重值精确到小数点后一位

### 通过代码使用

```csharp
using VRChat.BlendShapesExtractor;

// 从GameObject提取BlendShape数据
GameObject targetObject = // 你的GameObject
List<MeshBlendShapeData> meshDataList = BlendShapeExtractor.ExtractBlendShapesFromGameObject(targetObject);

// 导出为JSON文件
string outputPath = "Assets/BlendShapeData/output.json";
bool success = BlendShapeExtractor.ExportMultipleToJson(meshDataList, outputPath);

// 如果需要从单个Mesh提取（高级用法）
Mesh targetMesh = // 你的Mesh对象
MeshBlendShapeData meshData = BlendShapeExtractor.ExtractBlendShapes(targetMesh);
bool singleSuccess = BlendShapeExtractor.ExportToJson(meshData, outputPath);
```

## JSON输出格式

生成的JSON文件包含以下结构：

```json
{
  "meshName": "角色头部",
  "meshPath": "Assets/Models/Character/Head.fbx",
  "totalBlendShapeCount": 52,
  "blendShapes": [
    {
      "name": "eye_blink_left",
      "frames": [
        {
          "weight": 45.5
        }
      ]
    },
    {
      "name": "mouth_smile",
      "frames": [
        {
          "weight": 78.3
        }
      ]
    }
  ],
  "extractionDate": "2024-01-15 14:30:25"
}
```

### 字段说明

- `meshName`: Mesh的名称
- `meshPath`: Mesh资源在项目中的路径
- `totalBlendShapeCount`: BlendShape的总数量
- `blendShapes`: BlendShape数据数组
  - `name`: BlendShape的名称
  - `frames`: 帧数据数组
    - `weight`: 当前BlendShape的权重值（0-100）
- `extractionDate`: 数据提取的时间戳

## 系统要求

- Unity 2022.3.22f1 或更高版本
- 支持所有Unity支持的平台
- 兼容Unity 2022.3 LTS及以上版本

## 注意事项

1. **Mesh要求**: 只有包含BlendShape数据的Mesh才能被提取
2. **权限要求**: 确保有足够的文件写入权限到指定的输出目录
3. **性能考虑**: 对于包含大量BlendShape的复杂模型，提取过程可能需要一些时间
4. **路径格式**: 输出路径建议使用相对于Assets文件夹的路径

## 故障排除

### 常见问题

**Q: 为什么提取的BlendShape数量为0？**
A: 请确认选择的Mesh确实包含BlendShape数据。可以在Inspector中查看Mesh的BlendShape信息。

**Q: 导出的JSON文件为空或格式错误？**
A: 检查输出路径是否有写入权限，以及文件名是否包含非法字符。

**Q: 编辑器窗口无法打开？**
A: 确保插件已正确安装，并且Unity已完成编译。尝试重启Unity编辑器。

**Q: 在运行时无法使用？**
A: 某些功能（如获取资源路径）仅在编辑器模式下可用。运行时使用请参考代码示例。

## 许可证

本插件遵循MIT许可证。详情请参见LICENSE文件。

## 贡献

欢迎提交Issue和Pull Request来改进这个插件！

## 更新日志

### v1.5.0
- 🌐 新增多语言支持：中文、英文、日文三种界面语言
- 🌍 默认界面语言设置为英文，提供更好的国际化体验
- 📊 数值格式化：所有权重值统一精确到小数点后一位
- 🎨 优化用户界面，在窗口顶部添加语言选择器

### v1.4.0
- 简化用户界面，移除目标Mesh选项，专注于GameObject工作流
- 优化编辑器窗口，减少界面复杂度
- 更新使用文档，突出GameObject为主要使用方式

### v1.3.0
- 修复BlendShape权重值获取问题，现在能正确获取SkinnedMeshRenderer上设置的实际权重值
- 移除JSON导出中的frameCount字段，简化数据结构
- 优化数据提取逻辑，现在直接从SkinnedMeshRenderer获取当前权重值
- 更新示例数据，显示真实的权重值而非固定的100

### v1.2.0
- 修复拖入物体后控制台重复打印日志的问题
- 移除JSON导出中的vertexCount字段，简化数据结构
- 移除导出后自动打开文件的功能
- 优化数据缓存机制，提升编辑器性能

### v1.1.0
- 更新Unity兼容性至2022.3.22f1
- 修复EditorGUILayout.Space()在Unity 2022.3中的弃用警告
- 优化编辑器界面间距显示
- 确保与Unity 2022.3 LTS完全兼容

### v1.0.0
- 初始版本发布
- 支持从Mesh和GameObject提取BlendShape数据
- 提供Unity编辑器窗口界面
- 支持JSON格式导出
- 包含完整的错误处理和验证逻辑