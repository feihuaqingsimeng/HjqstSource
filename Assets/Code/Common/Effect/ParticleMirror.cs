using UnityEngine;
using System.Collections;
using Logic.Cameras.Controller;
namespace Common.Effect
{
    public class ParticleMirror : MonoBehaviour
    {
#if UNITY_EDITOR
		public bool test;
#endif
        /// <summary>
        /// 1,original -1,mirro
        /// </summary>
        public int mirror = 1;
        Material _mat;
        void Awake()
        {
            _mat = GetComponent<Renderer>().material;
        }

        void OnWillRenderObject()
        {
#if UNITY_EDITOR
			if(test)
				_mat.SetVector("_Position", Camera.main.worldToCameraMatrix.MultiplyPoint(transform.position));
			else
#endif
            _mat.SetVector("_Position", CameraController.instance.mainCamera.worldToCameraMatrix.MultiplyPoint(transform.position));
            _mat.SetVector("_Scale", new Vector3(transform.localScale.x * mirror, transform.localScale.y, transform.localScale.z));
        }
    }
}
