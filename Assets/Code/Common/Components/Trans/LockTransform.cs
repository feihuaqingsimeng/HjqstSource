using UnityEngine;
using System.Collections;

namespace Common.Components.Trans
{
    public class LockTransform : MonoBehaviour
    {
        public Transform trans;
        public Vector3 offset;
        public bool ignoreYaxis = false;
        public float delayRate = 0f;
        public bool isRotate = false;
        private Transform _transform;
        private Vector3 _eulerAngle;

        void Start()
        {
            _transform = transform;
            _eulerAngle = _transform.eulerAngles;
        }

        void Update()
        {
            //if (!trans || !trans.gameObject.activeInHierarchy)
            //    UnityEngine.Object.Destroy(gameObject);
            if (trans && trans.position != _transform.position)
            {
                Vector3 pos = Vector3.zero;
                if (ignoreYaxis)
                    pos = new Vector3(trans.position.x, 0, trans.position.z) + offset;
                else
                    pos = trans.position + offset;
                if (delayRate > 0)
                    _transform.position = Vector3.Slerp(_transform.position, pos, delayRate);
                else
                    _transform.position = pos;
            }
            if (isRotate)
            {
                if (trans && trans.eulerAngles != _transform.eulerAngles)
                {
                    Vector3 rotate = trans.eulerAngles + _eulerAngle;
                    if (delayRate > 0)
                        _transform.eulerAngles = Vector3.Slerp(_transform.eulerAngles, rotate, delayRate);
                    else
                        _transform.eulerAngles = rotate;
                }
            }
        }
    }
}
