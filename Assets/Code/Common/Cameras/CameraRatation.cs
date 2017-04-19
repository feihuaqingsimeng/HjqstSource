using UnityEngine;
using System.Collections;
namespace Common.Cameras
{
    public class CameraRatation : SingletonMono<CameraRatation>
    {

        public AnimationCurve xAxisCurve;
        public AnimationCurve yAxisCurve;
        public AnimationCurve zAxisCurve;
        private float startTime;
        private float length;
        private float nowTime;
        //float angle;

        void Awake()
        {
            instance = this;
        }

        // Use this for initialization
        void Start()
        {
            startTime = Time.realtimeSinceStartup;
            length = xAxisCurve.length;
            //angle = Mathf.PI * 2 / length;
        }

        // Update is called once per frame
        void Update()
        {
            nowTime = Time.realtimeSinceStartup;
            float normalizedTime = nowTime - startTime;
            if (normalizedTime <= length)
            {
                startTime = nowTime;
                transform.RotateAround(Vector3.zero, Vector3.up, 100 * Time.deltaTime);//在原点按Y轴旋转100度
                this.transform.localPosition = new Vector3(xAxisCurve.Evaluate(normalizedTime / 5), yAxisCurve.Evaluate(normalizedTime / 5), -10);//zAxisCurve.Evaluate(normalizedTime / 5));
                //this.transform.LookAt(Vector3.zero);
            }
        }
    }
}