using UnityEngine;
using System.Collections;
using Common.Components.Meshs;
using System.Collections.Generic;
namespace Common.Components.Shadow
{
    public class DynamicShadow : SingletonMono<DynamicShadow>
    {
        public MeshFilter mf;
        public float size;
        private Dictionary<Transform, Vector2> _targetDic;
        private List<Transform> _targets;
        Mesh shadow;
        void Awake()
        {
            instance = this;
            _targets = new List<Transform>();
            _targetDic = new Dictionary<Transform, Vector2>();
            enabled = false;
        }

        void OnEnable()
        {
            Logic.Cameras.Controller.CameraController.instance.mainCameraEvent.onPreRenderHandler = null;
            Logic.Cameras.Controller.CameraController.instance.mainCameraEvent.onPreRenderHandler += OnPreRenderHandler;
        }

        public void AddTarget(Transform trans, Vector2 size)
        {
            if (!enabled)
                enabled = true;
            if (!_targetDic.ContainsKey(trans))
            {
                _targets.Add(trans);
                _targetDic.Add(trans, size);
            }
        }

        public void RemoveTarget(Transform trans)
        {
            if (_targetDic.ContainsKey(trans))
            {
                _targets.Remove(trans);
                _targetDic.Remove(trans);
            }
        }

        public void ClearTargets()
        {
            _targets.Clear();
            _targetDic.Clear();
            if (mf)
                UnityEngine.Object.Destroy(mf.sharedMesh);
            enabled = false;
            Logic.Cameras.Controller.CameraController.instance.mainCameraEvent.onPreRenderHandler -= OnPreRenderHandler;
        }

        //void OnBecameVisible()
        //{
        //    enabled = true;
        //}

        //void OnBecameInvisible()
        //{
        //    enabled = false;
        //}

        void OnPreRenderHandler()
        {
            //Debugger.Log("onPreRender");
            List<KeyValuePair<Vector3, Vector2>> posAndSize = new List<KeyValuePair<Vector3, Vector2>>();
            for (int i = 0, count = _targets.Count; i < count; i++)
            {
                Transform tran = _targets[i];
                if (tran && tran.gameObject.activeInHierarchy)
                {
                    Vector2 size = _targetDic[tran];
                    posAndSize.Add(new KeyValuePair<Vector3, Vector2>(tran.position, size));
                }
            }
            if (posAndSize.Count <= 0) return;
            shadow = GenerateMesh.instance.Generate(shadow, posAndSize);
            mf.sharedMesh = shadow;
        }
    }
}