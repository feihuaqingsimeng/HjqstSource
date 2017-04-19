using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Common.Tools
{
    public class StopUintyOnRuntimeWhenComplie : MonoBehaviour
    {
        StopUintyOnRuntimeWhenComplie instance;
        void Awake()
        {
            instance = this;
#if !UNITY_EDITOR
        UnityEngine.Object.Destroy(instance);
#endif
        }

#if UNITY_EDITOR
        bool isComplie = false;
        void Update()
        {
            if (isComplie)
                return;
            if (EditorApplication.isCompiling && !isComplie)
            {
                Debugger.Log("recomplie,stop play mode!");
                isComplie = true;
                EditorApplication.ExecuteMenuItem("Edit/Play");
            }
        }
#endif
    }
}