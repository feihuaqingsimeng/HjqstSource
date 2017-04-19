using UnityEngine;
using System.Collections;
namespace Common.Cameras
{
    public class CameraEvent : MonoBehaviour
    {
        public delegate void OnPreRenderHandler();
        public OnPreRenderHandler onPreRenderHandler;

        void OnPreRender()
        {
            if (onPreRenderHandler != null)
                onPreRenderHandler();
        }
    }
}
