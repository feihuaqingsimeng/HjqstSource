using UnityEngine;
using System.Collections;
namespace Logic.UI.Mask.Contorller
{
    public class MaskController : SingletonMono<MaskController>
    {
        void Awake()
        {
            instance = this;
        }

        [ContextMenu("Show Mask")]
        public void ShowMask()
        {
            UIMgr.instance.Open<Mask.View.MaskView>(Mask.View.MaskView.PREFAB_PATH, EUISortingLayer.Loading);
        }

        [ContextMenu("Hide Mask")]
        public void HideMask()
        {
            UIMgr.instance.Close(Mask.View.MaskView.PREFAB_PATH);
        }
    }
}