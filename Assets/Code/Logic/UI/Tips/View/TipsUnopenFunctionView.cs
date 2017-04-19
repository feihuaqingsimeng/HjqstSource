using UnityEngine;
using System.Collections;
namespace Logic.UI.Tips.View
{
    public class TipsUnopenFunctionView : MonoBehaviour
    {
        public const string PREFAB_PATH = "ui/tips/tips_unopen_function_view";
        #region ui
        public GameObject core;
        #endregion

        void Start()
        {

        }

        #region  事件
        public void CloseHandler()
        {
            UI.UIMgr.instance.Close(PREFAB_PATH);
        }
        #endregion
    }
}