using UnityEngine;
// using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
namespace Common.Components
{
    public enum TextureSize { S_16 = 16, S_32 = 32, S_64 = 64, S_128 = 128, S_256 = 256, S_512 = 512, S_1024 = 1024, S_2048 = 2048 }
    public enum DepthBuffer { BIT_0 = 0, BIT_16 = 16, BIT_24 = 24 }
    public class RunTimeRender : MonoBehaviour
    {

        public Camera cam;
        //public UITexture uiTexture;

        public TextureSize sizeX = TextureSize.S_512, sizeY = TextureSize.S_512;
        public DepthBuffer depthBuffer = DepthBuffer.BIT_16;

        public float scaleX = 1, scaleY = 1;
        private RenderTexture _renderTexture;
        public RenderTexture renderTexture { get { return _renderTexture; } }
        // Use this for initialization
        void Awake()
        {
            _renderTexture = new RenderTexture((int)sizeX, (int)sizeY, (int)depthBuffer, RenderTextureFormat.ARGB32);
            cam.targetTexture = _renderTexture;
            //uiTexture.mainTexture = _renderTexture;
            var matrix = cam.projectionMatrix;
            matrix[0, 0] = scaleX;
            matrix[1, 1] = scaleY;
            cam.projectionMatrix = matrix;
        }

        void OnDestroy()
        {
            //uiTexture.mainTexture = null;
            cam.targetTexture = null;

            if (_renderTexture != null)
                Destroy(_renderTexture);
        }
    }
}
