using UnityEngine;
using System.Collections;
using Logic.UI.FightTips.View;
using Logic.Game;
using Logic.Enums;

namespace Logic.UI.FightTips.Controller
{
    public class FightTipsController : SingletonMono<FightTipsController>
    {
        public delegate void FightOverHandler();
        public FightOverHandler fightOverHandler;

        public delegate void ComboWaitingOverHandler(FightStatus fightStatus);
        public ComboWaitingOverHandler comboWaitingOverHandler;
        void Awake()
        {
            instance = this;
        }

        public FightTipsView OpenFightTipsView()
        {
            return UI.UIMgr.instance.Open<Logic.UI.FightTips.View.FightTipsView>(Logic.UI.FightTips.View.FightTipsView.PREFAB_PATH, EUISortingLayer.MainUI, UIOpenMode.Overlay);
        }

        public FightTipsView GetFightTipsView()
        {
            return UI.UIMgr.instance.Get<Logic.UI.FightTips.View.FightTipsView>(Logic.UI.FightTips.View.FightTipsView.PREFAB_PATH);
        }

        public void CloseFightTipsView()
        {
            UI.UIMgr.instance.Close(Logic.UI.FightTips.View.FightTipsView.PREFAB_PATH);
        }

        public GameSpeedMode gameSpeedMode
        {
            get
            {
                FightTipsView fightTipsView = GetFightTipsView();
                if (fightTipsView)
                    return fightTipsView.gameSpeedMode;
                return GameSpeedMode.Normal;
            }
            set
            {
                FightTipsView fightTipsView = GetFightTipsView();
                if (fightTipsView)
                    fightTipsView.gameSpeedMode = value;
            }
        }

        public float CostTime
        {
            get
            {
                FightTipsView fightTipsView = GetFightTipsView();
                if (fightTipsView)
                    return fightTipsView.costTime;
                return 0;
            }
        }

        public void NextTeam()
        {
            FightTipsView fightTipsView = GetFightTipsView();
            if (fightTipsView)
                fightTipsView.NextTeam();
        }

        public void FightStart()
        {
            FightTipsView fightTipsView = GetFightTipsView();
            if (fightTipsView)
                fightTipsView.FightStart();
        }

        public void FightOver()
        {
            FightTipsView fightTipsView = GetFightTipsView();
            if (fightTipsView)
                fightTipsView.FightOver();
        }

        public bool ComboWating(float comboWaitTime, FightStatus fightStatus, bool isPlayer)
        {
            FightTipsView fightTipsView = GetFightTipsView();
            if (fightTipsView)
            {
                fightTipsView.ComboWating(comboWaitTime, fightStatus, isPlayer);
                return true;
            }
            return false;
        }

        public void SetPause(bool pause)
        {
            FightTipsView fightTipsView = GetFightTipsView();
            if (fightTipsView)
                fightTipsView.pause = pause;
        }
    }
}
