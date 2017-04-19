using UnityEngine;
using Logic.Enums;
using System.Collections;
using Logic.UI.DungeonDetail.Model;
using Logic.UI.FightResult.Model;

namespace Logic.UI.FightResult.View
{
    public abstract class FightResultBasePanel
    {
        public const string PREFAB_PATH = "ui/fight_result/fight_result_view";
        public static FightResultBasePanel Create(FightType fightType, FightResultView view)
        {
            FightResultBasePanel panel = null;
            switch (fightType)
            {
                case FightType.PVE:        //普通副本
                    panel = new PveFightResultPanel();
                    break;
                case FightType.Arena:       //竞技场
                    panel = new PvpFightResultPanel();
                    break;
                case FightType.DailyPVE:    //每日副本
                    panel = new DailyDungeonFightResultPanel();
                    break;
                case FightType.Expedition:	 //远征
                    panel = new ExpeditionFightResultPanel();
                    break;
                case FightType.WorldTree:   //世界树
                    panel = new WorldTreeFightResultPanel();
                    break;
                case FightType.WorldBoss:   //世界Boss
                    break;
                case FightType.PVP:         //PVP
                    panel = new PvpRaceFightResultPanel();
                    break;
                case FightType.FirstFight:  //第一场战斗
                case FightType.SkillDisplay://技能展示
                    break;
                case FightType.ConsortiaFight://公会战
                    panel = new PvpFightResultPanel();
                    break;
                case FightType.FriendFight://好友战
                    panel = new FriendFightResultPanel();
                    break;
                case FightType.MineFight://矿战
                    panel = new MineFightResultPanel();
                    break;
            }
            if (panel != null)
                panel.view = view;
            else
                Debugger.LogError("[FightResultBasePanel]  create panel is null,please check this FightType is implement!!! fightType:" + fightType);
            return panel;
        }
        public FightResultView view;



        public abstract void Init();

        public virtual void OnDestroy() { }
		public virtual void OnAutoGoMainViewHandler()
		{
			Debugger.LogError("not implement this virtual method");
		}
        public Coroutine StartCoroutine(IEnumerator routine)
        {
            return view.StartCoroutine(routine);
        }
	
    }
}