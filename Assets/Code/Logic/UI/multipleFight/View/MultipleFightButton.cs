using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using Logic.Enums;
using Common.Localization;
using Logic.FunctionOpen.Model;
using Logic.Game.Model;
using Logic.Dungeon.Model;

namespace Logic.UI.MultipleFight.View
{
	public class MultipleFightButton : MonoBehaviour 
	{
		#region ui component
		public Text titleText;
		public Text unlockLevelText;
		public GameObject unlockGo;
		public GameObject unlockTipsFrameGO;
		public Text openTimeText;

		public Transform openRoot;
		public Text worldBossEndCountDownText;
		#endregion

		public MultipleFightCenterType centerType;
		public FunctionOpenType openType;

		private bool isCountingDown = false;
		void Start()
		{
			titleText.text = Localization.Get("ui.multple_fight_view.center"+(int)centerType);
			FunctionData data = FunctionData.GetFunctionDataByID(openType);
			if (data == null)
			{
				unlockLevelText.gameObject.SetActive(false);
				unlockGo.SetActive(false);
				unlockTipsFrameGO.SetActive(false);
			}else
			{
				if (data.player_level > 0 )
				{
					unlockLevelText.text = string.Format(Localization.Get("ui.multple_fight_view.unlock"),data.player_level);
				}else
				{
					DungeonInfo info =  DungeonProxy.instance.GetDungeonInfo(data.dungeon_pass);
					if (info == null)
					{
						unlockLevelText.text = "副本不存在该";
					}else{
						if(info.star <= 0 ){
							unlockLevelText.text = string.Format(Localization.Get("ui.function_open.dungeon_not_pass_cshap"),info.dungeonData.GetDungeonTypeName(),info.dungeonData.GetOrderName(),info.dungeonData.GetDungeonName());
						}
					}
				}

				bool isOpen = FunctionOpenProxy.instance.IsFunctionOpen(openType);
				unlockGo.SetActive(!isOpen);
				unlockLevelText.gameObject.SetActive(!isOpen);
				unlockTipsFrameGO.SetActive(!isOpen);
			}
			if (openTimeText != null)
			{
				if (centerType == MultipleFightCenterType.FightCenter_WorldBoss)
				{
					System.DateTime time1 = GlobalData.GetGlobalData().boss_time1_fight_began;
					System.DateTime time1End = time1.AddSeconds(GlobalData.GetGlobalData().bossFightLastTime);
					string startTime1 = string.Format(Localization.Get("ui.multple_fight_view.dailyTime"),time1.ToString("HH:mm"),time1End.ToString("HH:mm"));
					System.DateTime time2 = GlobalData.GetGlobalData().boss_time2_fight_began;
					System.DateTime time2End = time2.AddSeconds(GlobalData.GetGlobalData().bossFightLastTime);
					string startTime2 = string.Format(Localization.Get("ui.multple_fight_view.dailyTime"),time2.ToString("HH:mm"),time2End.ToString("HH:mm"));
					openTimeText.text = startTime1 + "\n"+startTime2;
				}
                else if (centerType == MultipleFightCenterType.FightCenter_PvpRace)
                {
					System.DateTime time1 = GlobalData.GetGlobalData().point_pvp_start_time;
					System.DateTime time1End = time1.AddSeconds(GlobalData.GetGlobalData().point_pvp_keep_time);
                    string startTime = string.Format(Localization.Get("ui.multple_fight_view.specailOpenTime"), time1.ToString("HH:mm"), time1End.ToString("HH:mm"));
                    openTimeText.text = startTime;
                }
			}


			if (centerType == MultipleFightCenterType.FightCenter_WorldBoss)
			{
				if (Logic.WorldBoss.Model.WorldBossProxy.instance.IsOpen)
				{
					StartWorldBossCountDown();
				}
				else
				{
					StopWorldBossCountDown();
				}
			}
		}

		public void BindDelegate ()
		{
			Logic.WorldBoss.Model.WorldBossProxy.instance.onWorldBossStatusChangedDelegate += OnWorldBossStatusChangedHandler;
			Logic.WorldBoss.Model.WorldBossProxy.instance.onWorldBossActivityEndDelegate += OnWorldBossActivityEndHandler;
		}

		public void UnbindDelegate ()
		{
			Logic.WorldBoss.Model.WorldBossProxy.instance.onWorldBossStatusChangedDelegate -= OnWorldBossStatusChangedHandler;
			Logic.WorldBoss.Model.WorldBossProxy.instance.onWorldBossActivityEndDelegate -= OnWorldBossActivityEndHandler;
		}

		private void OnWorldBossStatusChangedHandler()
		{
			if (Logic.WorldBoss.Model.WorldBossProxy.instance.IsOpen)
			{
				StartWorldBossCountDown();
			}
			else
			{
				StopWorldBossCountDown();
			}
		}

		public void OnWorldBossActivityEndHandler ()
		{
			StopWorldBossCountDown();
		}

		public void CountDownWorldBossTime()
		{
			if (Logic.WorldBoss.Model.WorldBossProxy.instance.IsOpen)
			{
				TimeSpan timeSpan = TimeSpan.FromSeconds(Logic.WorldBoss.Model.WorldBossProxy.instance.OverDiffTimeWithServerTimeInSecond);
				worldBossEndCountDownText.text = timeSpan.ToString();
				LeanTween.delayedCall(gameObject, 1f, CountDownWorldBossTime).setIgnoreTimeScale(true);
			}
		}

		public void StartWorldBossCountDown ()
		{
			CountDownWorldBossTime();
			openRoot.gameObject.SetActive(false);
			worldBossEndCountDownText.gameObject.SetActive(true);
			isCountingDown = true;
		}
		
		public void StopWorldBossCountDown ()
		{
			LeanTween.cancel(gameObject);
			openRoot.gameObject.SetActive(true);
			worldBossEndCountDownText.gameObject.SetActive(false);
			isCountingDown = false;
		}

		public void OnClickButtonHandler()
		{
			FunctionOpen.Model.FunctionOpenProxy.instance.OpenFunction(openType,null,false,true);
		}
	}
}

