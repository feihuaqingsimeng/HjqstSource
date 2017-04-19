using UnityEngine;
using System.Collections;
using Logic.Enums;
using UnityEngine.UI;
using Common.Localization;

namespace Logic.UI.MultipleFight.View
{
	public class MultipleFightView : MonoBehaviour 
	{

		public const string PREFAB_PATH = "ui/multiple_fight/multiple_fight_view";
		public static MultipleFightView Open()
		{
			MultipleFightView view = UIMgr.instance.Open<MultipleFightView>(PREFAB_PATH);
			return view;
		}

		#region ui component
		public Transform core;
		#endregion
		void Awake()
		{
			Init();
		}
		private void Init()
		{
			CommonTopBar.View.CommonTopBarView commonTopBarView = CommonTopBar.View.CommonTopBarView.CreateNewAndAttachTo(core.transform);
			commonTopBarView.SetAsCommonStyle(Localization.Get("ui.multple_fight_view.title"), OnClickCloseBtnHandler, true, true, true, false);
		}
		#region ui event handler

		public void OnClickCloseBtnHandler()
		{
			UIMgr.instance.Close(PREFAB_PATH);
		}
		#endregion
	}
}