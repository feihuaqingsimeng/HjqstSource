using UnityEngine;
using System.Collections;
namespace Common.Tools
{
    public class FPS : MonoBehaviour
    {

        // A FPS counter.
        // It calculates frames/second over each updateInterval,
        // so the display does not keep changing wildly.

        float updateInterval = 0.5f;
        private float lastInterval; // Last interval end time
        private int frames = 0; // Frames over current interval
        private float fps; // Current FPS
        private float sum = 0.0f;
        private float num = 0.0f;

        void Start()
        {
            lastInterval = Time.realtimeSinceStartup;
            frames = 0;
        }

        void OnGUI()
        {
            GUILayout.Label("fps:" + fps.ToString("f0") + "      avg:" + (sum / num).ToString("f0"));
        }

        void Update()
        {
            //transform.RotateAround(Vector3.zero, Vector3.up, 100 * Time.deltaTime);//在原点按Y轴旋转100度
            ++frames;
            float timeNow = Time.realtimeSinceStartup;
            if (timeNow > lastInterval + updateInterval)
            {
                fps = frames / (timeNow - lastInterval);
                frames = 0;
                lastInterval = timeNow;
                sum += fps;
                num++;
            }
        }
    }
}