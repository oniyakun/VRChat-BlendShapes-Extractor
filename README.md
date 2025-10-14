# VRChat BlendShapes Extractor

*中文 | [English](README.en.md) | [日本語](README.ja.md)*

一个用于从Unity Mesh文件中提取BlendShape数据并生成结构化JSON文件的Unity插件。

## 功能特性

- 从Mesh对象中提取所有BlendShape数据
- 从SkinnedMeshRenderer组件中提取实际权重值
- 生成结构化的JSON数据文件
- 支持批量处理多个Mesh
- 提供直观的编辑器窗口界面
- 支持中日英三语界面
- 支持BlendShape数据的导入和应用

## 系统要求

- Unity 2022.3.22f1 或更高版本
- 适用于VRChat Avatar开发

## 安装方法

1. 将插件文件夹复制到项目的`Assets`目录下
2. Unity会自动编译并加载插件

## 使用方法

### 提取BlendShape数据

1. 在Unity编辑器中，打开菜单 `Oniya Tools > BlendShape Extractor`
2. 在打开的窗口中，将包含BlendShape的GameObject拖拽到"目标对象"字段
3. 设置输出路径和文件名
4. 点击"提取BlendShape数据"按钮
5. 生成的JSON文件将保存到指定路径

### 导入BlendShape数据

1. 在编辑器窗口中展开"导入BlendShape数据"部分
2. 选择要应用数据的目标GameObject
3. 选择包含BlendShape数据的JSON文件
4. 选择要导入的BlendShape项目
5. 点击"应用选中的BlendShape"按钮

## 输出格式

生成的JSON文件包含以下结构：

```json
{
  "meshName": "Mesh名称",
  "meshPath": "Mesh文件路径",
  "totalBlendShapeCount": 数量,
  "extractionDate": "提取日期",
  "blendShapes": [
    {
      "name": "BlendShape名称",
      "frames": [
        {
          "weight": 权重值
        }
      ]
    }
  ]
}
```

## 高级选项

- **包含帧详细信息**: 控制是否在输出中包含BlendShape帧的详细数据
- **自定义输出路径**: 可以指定JSON文件的保存位置
- **批量处理**: 支持一次性处理GameObject下的所有Mesh

## 许可证

本项目遵循MIT许可证。

## 贡献

欢迎提交Issue和Pull Request来改进这个项目。