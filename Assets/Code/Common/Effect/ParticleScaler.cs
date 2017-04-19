using UnityEngine;
using System.Collections;
using Logic.Cameras.Controller;
namespace Common.Effect
{
    public class ParticleScaler : MonoBehaviour
    {
        public bool isUI;
        Material _mat;
        void Awake()
        {
            _mat = GetComponent<Renderer>().material;
        }

        //测试，lossyscale有问题
        void OnWillRenderObject()
        {
            if (isUI)
            {
                _mat.SetVector("_Position", CameraController.instance.uiCamera.worldToCameraMatrix.MultiplyPoint(transform.position));
                Vector3 scale = new Vector3(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z);
                scale.x *= (transform.lossyScale.x < 0 ? -1.0f : 1.0f);
                scale.y *= (transform.lossyScale.y < 0 ? -1.0f : 1.0f);
                scale.z *= (transform.lossyScale.z < 0 ? -1.0f : 1.0f);
                _mat.SetVector("_Scale", scale);
            }
            else
            {
                _mat.SetVector("_Position", CameraController.instance.mainCamera.worldToCameraMatrix.MultiplyPoint(transform.position));
                _mat.SetVector("_Scale", new Vector3(transform.localScale.x * (transform.lossyScale.x < 0 ? -1.0f : 1.0f), transform.localScale.y * (transform.lossyScale.y < 0 ? -1.0f : 1.0f), transform.localScale.z * (transform.lossyScale.z < 0 ? -1.0f : 1.0f)));
            }
        }
    }
}
