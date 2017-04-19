using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Logic.Game.Model;
using Common.Util;
using Common.Localization;

namespace Logic.UI.Pvp.View
{
	public class PvpActionPointView : MonoBehaviour 
	{
		
		#region ui component
		public Text textPvpPoint;
		public Text textRecover;
		public Transform imgPvpPointPrefab;
		public Transform pvpPointRoot;
		#endregion
		
		private int _pvpPointCount = -1;
		
		void Awake()
		{
			Init();

			BindDelegate();
		}
		void OnDestroy()
		{
			UnbindDelegate();	
		}
		private void BindDelegate()
		{
			GameProxy.instance.onPvpActionInfoUpdateDelegate += OnPvpActionUpdateHandler;
			//GameProxy.instance.onPvpActionNextRecoverTimeUpdateDelegate += OnPvpActionNextRecoverTimeUpdateHandler;
		}
		private void UnbindDelegate()
		{
			GameProxy.instance.onPvpActionInfoUpdateDelegate -= OnPvpActionUpdateHandler;
			//GameProxy.instance.onPvpActionNextRecoverTimeUpdateDelegate -= OnPvpActionNextRecoverTimeUpdateHandler;
			
		}
		private void Init()
		{

			OnPvpActionUpdateHandler();
			//OnPvpActionNextRecoverTimeUpdateHandler();
		}
		
		private void OnPvpActionUpdateHandler()
		{
			TransformUtil.ClearChildren(pvpPointRoot,true);
			int count = GameProxy.instance.PvpAction;
			int max = GameProxy.instance.PvpActionMax;
			imgPvpPointPrefab.gameObject.SetActive(true);
			for(int i = 0;i<1;i++)
			{
				Transform tran = Instantiate<Transform>(imgPvpPointPrefab);
				tran.SetParent(pvpPointRoot,false);
//				if(i >= count)
//				{
//					tran.GetComponent<Image>().SetGray(true);
//				}

			}
			imgPvpPointPrefab.gameObject.SetActive(false);
			textPvpPoint.text = string.Format(Localization.Get("common.x_count"),GameProxy.instance.PvpAction);
		}
//		private void OnPvpActionNextRecoverTimeUpdateHandler()
//		{
//
//
//
//			textPvpPoint.text = string.Format(Localization.Get("common.x_count"),GameProxy.instance.PvpAction);
//			textRecover.gameObject.SetActive(false);
////			if (GameProxy.instance.PvpAction < GameProxy.instance.PvpActionMax)
////			{
////				textRecover.gameObject.SetActive(true);
////				textRecover.text = TimeUtil.FormatSecondToHour((int)(GameProxy.instance.PvpActionNextRecoverTime/1000));
////				
////			}else
////			{
////				textRecover.gameObject.SetActive(false);
////			}

//		}
	}
}

