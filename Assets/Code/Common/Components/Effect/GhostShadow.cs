using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Common.Components.Effect
{
    public class GhostShadow : MonoBehaviour
    {

        //残影拷贝临时gameobject引用存储 角色被destroy后清除该表
        private List<GameObject> mrTempList = new List<GameObject>();
        float previous = 0;
        public bool ghost
        {
            get { return _ghost; }
            set
            {
                _ghost = value;
                if (!value)
                    CharacterDestroyGameObject();
            }
        }
        private bool _ghost;
#if UNITY_EDITOR
        public bool play = false;
#endif
        public float interval = 0.05f;
        public float lifeTime = 0.25f;
        //bool boolParentDestroy = false;

        void Awake()
        {
            _ghost = false;
        }
        void FixedUpdate()
        {
#if UNITY_EDITOR
            if (play)
#endif
                if (_ghost)
                    if (Time.fixedTime - previous > interval)
                    {
                        foreach (SkinnedMeshRenderer smr in GetComponentsInChildren<SkinnedMeshRenderer>())
                        {
                            Mesh m = new Mesh();
                            smr.BakeMesh(m);
                            GameObject go = new GameObject();
                            Transform goTrans = go.transform;
                            goTrans.position = smr.transform.position;
                            goTrans.localRotation = smr.transform.rotation;
                            MeshRenderer mr = go.AddComponent<MeshRenderer>();
                            MeshFilter mf = go.AddComponent<MeshFilter>();
                            mf.mesh = m;
                            mr.materials = smr.materials;
                            mrTempList.Add(go);
                            for (int i = 0; i < mr.materials.Length; i++)
                            {
                                mr.materials[i].shader = Shader.Find("Particles/Additive");
                            }
                            //StartCoroutine(FadeAndDestory(m, mr, 0.25f));
                            Ghost g = go.AddComponent<Ghost>();
                            g.m = m;
                            g.mr = mr;
                            g.time = lifeTime;
                        }
                        previous = Time.fixedTime;
                    }
        }

        //角色死亡或被删除
        private void CharacterDestroyGameObject()
        {
            ClearMeshCopyRender();
        }

        private void ClearMeshCopyRender()
        {
            for (int i = 0; i < mrTempList.Count; i++)
                Destroy(mrTempList[i]);
            mrTempList.Clear();
        }
    }
}
