using UnityEngine;
using System.Collections;
using Logic.UI.Pvp.Model;
using UnityEngine.UI;
using Common.Util;
using System.Collections.Generic;
using Logic.UI.Pvp.Controller;

namespace Logic.UI.Pvp.View
{
	public class PvpBattleReportView : MonoBehaviour 
	{
		public const string PREFAB_PATH = "ui/pvp/pvp_battle_report_view";

		public static PvpBattleReportView Open()
		{
			PvpBattleReportView view = UIMgr.instance.Open<PvpBattleReportView>(PREFAB_PATH,EUISortingLayer.MainUI,UIOpenMode.Overlay);
			return view;
		}

		#region ui component
		public Text textTitle;
		public Text textReportPrefab;
		public Transform textReportRoot;
		#endregion

		void Start()
		{
			BindDelegate();
			Init();

		}
		void OnDestroy()
		{
			UnbindDelegate();
		}
		private void Init()
		{
			textReportPrefab.gameObject.SetActive(false);
			Refresh();
			//PvpController.instance.CLIENT2LOBBY_GET_RANK_ARENA_REPORT_REQ();
		}
		private void BindDelegate()
		{
			PvpBattleReportProxy.instance.OnUpdateBattleReportDelegate += Refresh;
		}
		private void UnbindDelegate()
		{
			PvpBattleReportProxy.instance.OnUpdateBattleReportDelegate -= Refresh;
		}
		public void Refresh()
		{
			TransformUtil.ClearChildren(textReportRoot,true);
			List<PvpBattleReportInfo> battleReportList = PvpBattleReportProxy.instance.BattleReportList;

			textReportPrefab.gameObject.SetActive(true);
			int count = battleReportList.Count;
			for(int i = count-1;i>=0;i--)
			{
				PvpBattleReportInfo info = battleReportList[i];
				Text reportText = Instantiate<Text>(textReportPrefab);
				reportText.transform.SetParent(textReportRoot,false);
				reportText.text = info.ToString();
			}
			textReportPrefab.gameObject.SetActive(false);

		}
		public void OnClickCloseBtnHandler()
		{
			UIMgr.instance.Close(PREFAB_PATH);
		}
	}
}

