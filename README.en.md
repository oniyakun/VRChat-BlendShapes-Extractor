# VRChat BlendShapes Extractor

*[中文](README.md) | English | [日本語](README.ja.md)*

A Unity plugin for extracting BlendShape data from Unity Mesh files and generating structured JSON files.

## Features

- Extract all BlendShape data from Mesh objects
- Extract actual weight values from SkinnedMeshRenderer components
- Generate structured JSON data files
- Support batch processing of multiple Meshes
- Intuitive editor window interface
- Support for three languages: Chinese, English, and Japanese
- Support for importing and applying BlendShape data

## System Requirements

- Unity 2022.3.22f1 or higher
- Suitable for VRChat Avatar development

## Installation

### Method 1: Download Pre-built Package (Recommended)

1. Go to the [Releases](https://github.com/oniyakun/VRChat-BlendShapes-Extractor/releases) page
2. Download the latest `.unitypackage` file
3. Double-click the file in Unity or import it via `Assets > Import Package > Custom Package`

### Method 2: Manual Installation

1. Copy the plugin folder to your project's `Assets` directory
2. Unity will automatically compile and load the plugin

## Usage

### Extracting BlendShape Data

1. In Unity Editor, open menu `Oniya Tools > BlendShape Extractor`
2. In the opened window, drag the GameObject containing BlendShapes to the "Target Object" field
3. Set the output path and file name
4. Click the "Extract BlendShape Data" button
5. The generated JSON file will be saved to the specified path

### Importing BlendShape Data

1. Expand the "Import BlendShape Data" section in the editor window
2. Select the target GameObject to apply data to
3. Select the JSON file containing BlendShape data
4. Choose the BlendShape items to import
5. Click the "Apply Selected BlendShapes" button

## Output Format

The generated JSON file contains the following structure:

```json
{
  "meshName": "Mesh Name",
  "meshPath": "Mesh File Path",
  "totalBlendShapeCount": count,
  "extractionDate": "Extraction Date",
  "blendShapes": [
    {
      "name": "BlendShape Name",
      "frames": [
        {
          "weight": weight_value
        }
      ]
    }
  ]
}
```

## Advanced Options

- **Include Frame Details**: Controls whether to include detailed BlendShape frame data in the output
- **Custom Output Path**: Allows specifying the save location for JSON files
- **Batch Processing**: Supports processing all Meshes under a GameObject at once

## License

This project is licensed under the MIT License.

## Contributing

Issues and Pull Requests are welcome to improve this project.