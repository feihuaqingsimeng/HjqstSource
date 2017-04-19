using UnityEngine;
using System.Collections;
namespace Common.Cameras
{
    public class CameraRotateAround : MonoBehaviour
    {
        private float _distance;
        private Transform _target;
        public Transform target
        {
            set
            {
                _target = value;
                if (value)
                {
#if UNITY_EDITOR
                    _traces.Clear();
#endif
                    _distance = Vector3.Distance(_target.position, _trans.position);
                }
            }
        }
        private Transform _trans;
        private float _eulerAngles_x;
        private float _eulerAngles_y;

        public float distanceMax = 100;//摄像机与目标的最大距离
        public float distanceMin = 10;//摄像机与目标的最小距离

        public float yMaxLimit = 60f;//最大y(单位是角度)
        public float yMinLimit = -10f;//最小y(单位是角度)

        public float speed = 0.1f;//摄像机拉近目标的速度
        public float xSpeed = 30f;//摄像机水平旋转速度
        public float ySpeed = 10;//摄像机垂直旋转速度
        public Vector3 offset = new Vector3(0, 1.5f, 1);
        // Use this for initialization
        void Start()
        {
            _trans = transform;
        }

        // Update is called once per frame
        void LateUpdate()
        {
            if (_target)
            {
                _eulerAngles_x = _trans.eulerAngles.y;//水平按y旋转
                _eulerAngles_y = _trans.eulerAngles.x;//垂直按x旋转
                _eulerAngles_x += xSpeed;
                _eulerAngles_y += ySpeed;
                _eulerAngles_y = ClampAngle(_eulerAngles_y, yMinLimit, yMaxLimit);
                Quaternion quaternion = Quaternion.Euler(_eulerAngles_y, _eulerAngles_x, 0f);
                _distance = Mathf.Clamp(_distance - speed, distanceMin, distanceMax);
                Vector3 pos = (Vector3)(quaternion * new Vector3(0f, 0f, -_distance)) + _target.position + offset;
                _trans.position = pos;
                _trans.rotation = quaternion;
#if UNITY_EDITOR
                _traces.Add(pos);
#endif
            }
        }

        private float ClampAngle(float angle, float min, float max)
        {
            while (angle < -360)
                angle += 360;
            while (angle > 360)
                angle -= 360;
            return Mathf.Clamp(angle, min, max);
        }

#if UNITY_EDITOR
        private System.Collections.Generic.List<Vector3> _traces = new System.Collections.Generic.List<Vector3>();

        void OnDrawGizmos()
        {
            Gizmos.color = Color.black;
            foreach (var v in _traces)
            {
                Gizmos.DrawSphere(v, 0.2f);
            }
        }
#endif
    }
}
