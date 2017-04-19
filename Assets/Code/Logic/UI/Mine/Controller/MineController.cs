using UnityEngine;
using System.Collections;
using Logic.Protocol.Model;
using Logic.Fight.Controller;
using Logic.Protocol;
namespace Logic.UI.Mine.Controller
{
    public class MineController : SingletonMono<MineController>
    {
        public System.Action onMineFightOverHandler;
        public int inCome { get; set; }

        void Awake()
        {
            instance = this;
        }

        void Start()
        {
            Observers.Facade.Instance.RegisterObserver(((int)MSG.RobMineResp).ToString(), LOBBY2CLIENT_RobMineResp_handler);
            Observers.Facade.Instance.RegisterObserver(((int)MSG.PlunderMineResp).ToString(), LOBBY2CLIENT_PlunderMineResp_handler);
            Observers.Facade.Instance.RegisterObserver(((int)MSG.MineFightOverResp).ToString(), LOBBY2CLIENT_MineFightOverResp_handler);
        }

        public void LOBBY2SERVER_MineFightOverReq(int result)
        {
            MineFightOverReq req = new MineFightOverReq();
            req.result = result;
            ProtocolProxy.instance.SendProtocol(req);
        }

        private bool LOBBY2CLIENT_PlunderMineResp_handler(Observers.Interfaces.INotification note)
        {
            PlunderMineResp resp = note.Body as PlunderMineResp;
            Logic.Fight.Model.FightProxy.instance.SetData(resp.fightData.selfTeamData, resp.fightData.opponentTeamData);

            FightController.instance.fightType = Logic.Enums.FightType.MineFight;
            FightController.instance.PreReadyFight();
            return true;
        }

        private bool LOBBY2CLIENT_RobMineResp_handler(Observers.Interfaces.INotification note)
        {
            RobMineResp resp = note.Body as RobMineResp;
            Logic.Fight.Model.FightProxy.instance.SetData(resp.fightData.selfTeamData, resp.fightData.opponentTeamData);

            FightController.instance.fightType = Logic.Enums.FightType.MineFight;
            FightController.instance.PreReadyFight();
            return true;
        }

        private bool LOBBY2CLIENT_MineFightOverResp_handler(Observers.Interfaces.INotification note)
        {
            inCome = 0;
            MineFightOverResp resp = note.Body as MineFightOverResp;
            inCome = resp.inCome;
            if (onMineFightOverHandler != null)
                onMineFightOverHandler();
            return true;
        }

        void OnDestroy()
        {
            if (Observers.Facade.Instance != null)
            {
                Observers.Facade.Instance.RemoveObserver(((int)MSG.RobMineResp).ToString(), LOBBY2CLIENT_RobMineResp_handler);
                Observers.Facade.Instance.RemoveObserver(((int)MSG.PlunderMineResp).ToString(), LOBBY2CLIENT_PlunderMineResp_handler);
                Observers.Facade.Instance.RemoveObserver(((int)MSG.MineFightOverResp).ToString(), LOBBY2CLIENT_MineFightOverResp_handler);
            }
        }
    }
}