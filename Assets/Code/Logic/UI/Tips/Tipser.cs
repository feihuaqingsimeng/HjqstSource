using UnityEngine;
using System.Collections;
namespace Logic.UI.Tips
{
    public static class Tipser
    {
        public static void ShowTips(TipsType type)
        {
            switch(type)
            {
                case TipsType.UnopenFunction:
                    UI.UIMgr.instance.Open(View.TipsUnopenFunctionView.PREFAB_PATH);
                    break;
            }
        }
    }

    public enum TipsType
    {
        None = 0,
        UnopenFunction = 1,//功能未开启

    }
}