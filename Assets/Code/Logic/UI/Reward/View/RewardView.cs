using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Common.Util;
namespace Logic.UI.Reward.View
{
    public class RewardView : MonoBehaviour
    {
        public const string PREFAB_PATH = "ui/reward/reward_view";
        #region ui
        public GameObject core;
        public Image[] moneyImages;
        public Image[] boxImages;
        #endregion

        void Start() 
        {
			ImageUtil.SetNumberImages(moneyImages, "sprite/temporary_resources/num2_", 1000);
			ImageUtil.SetNumberImages(boxImages, "sprite/temporary_resources/num2_", 320);
        }
        
    }
}