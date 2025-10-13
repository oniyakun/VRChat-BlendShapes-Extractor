using System;
using System.Collections.Generic;
using UnityEngine;

namespace VRChat.BlendShapesExtractor
{
    /// <summary>
    /// 表示单个BlendShape的数据结构
    /// </summary>
    [Serializable]
    public class BlendShapeInfo
    {
        [SerializeField]
        public string name;
        
        [SerializeField]
        public List<BlendShapeFrame> frames;
        
        public BlendShapeInfo()
        {
            frames = new List<BlendShapeFrame>();
        }
        
        public BlendShapeInfo(string shapeName)
        {
            this.name = shapeName;
            this.frames = new List<BlendShapeFrame>();
        }
    }
    
    /// <summary>
    /// 表示BlendShape帧的数据结构
    /// </summary>
    [Serializable]
    public class BlendShapeFrame
    {
        [SerializeField]
        public float weight;
        
        public BlendShapeFrame(float weight)
        {
            this.weight = weight;
        }
    }
    
    /// <summary>
    /// 表示整个Mesh的BlendShape数据集合
    /// </summary>
    [Serializable]
    public class MeshBlendShapeData
    {
        [SerializeField]
        public string meshName;
        
        [SerializeField]
        public string meshPath;
        
        [SerializeField]
        public int totalBlendShapeCount;
        
        [SerializeField]
        public List<BlendShapeInfo> blendShapes;
        
        [SerializeField]
        public string extractionDate;
        
        public MeshBlendShapeData()
        {
            blendShapes = new List<BlendShapeInfo>();
            extractionDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
        
        public MeshBlendShapeData(string meshName, string meshPath)
        {
            this.meshName = meshName;
            this.meshPath = meshPath;
            this.blendShapes = new List<BlendShapeInfo>();
            this.extractionDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}