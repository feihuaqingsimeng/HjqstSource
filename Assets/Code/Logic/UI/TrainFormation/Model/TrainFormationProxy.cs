using UnityEngine;
using System.Collections;
using Common.GameTime.Controller;
using Logic.Game.Model;
using Logic.VIP.Model;
using System.Collections.Generic;
using Logic.UI.RedPoint.Model;
using Logic.Formation.Model;
using Logic.Enums;
using Logic.Item.Model;
using LuaInterface;

namespace Logic.UI.TrainFormation.Model
{
	public class TrainFormationProxy : SingletonMono<TrainFormationProxy> 
	{
		
		void Awake()
		{
			instance = this;
		}

		public System.Action onRecoverTimeUpdateDelegate;
		/// <summary>
		/// 阵型升级更新
		/// </summary>
		public System.Action onUpgradeTrainFormationDelegate;
		public System.Action onBuyTrainPointDelegate;
		/// <summary>
		/// 阵型改变更新
		/// </summary>
		public System.Action onFormationChangedDelegate;

        public System.Action onAdditionFormationAttrActiveDelegate;

		//培养点
		public int trainPoint;
		///回复上限
		public int trainPointRecoverMax;
		///培养点已购买次数
		public int trainPointPurcasedTimes
		{
			get
			{
				LuaTable table = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel","formation_model")[0];
				return table["bringUpPointPurcasedTimes"].ToString().ToInt32();
			}
			set
			{
				LuaTable table = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel","formation_model")[0];
				table.GetLuaFunction("SetBringUpPointPurcasedTimes").Call(value);
				
			}

		}
		///下次恢复时间（时间戳）(-1回复已满)
		private long trainPointNextRecoverTime;
		//回复倒计时（ms）
		public int trainPointNextRecoverCountDown;

		//购买培养点价格
		public int trainPointPurcase
		{
			get
			{
				return GlobalData.GetGlobalData().formationTrainPointBuyA * (trainPointPurcasedTimes+1)+GlobalData.GetGlobalData().formationTrainPointBuyB;
			}
		}
		public int TrainPointPurcasedMaxTimes
		{
			get
			{
				return VIPProxy.instance.VIPData.formation_add;
			}
		}

		private List<int> _newFormationListByRedPoint = new List<int>();
		public void AddNewFormationTip(int id)
		{
			if (!_newFormationListByRedPoint.Contains(id))
				_newFormationListByRedPoint.Add(id);
		}
		public void ClearNewFormationTip()
		{
			_newFormationListByRedPoint.Clear();
			_isFormationPointRecoverFullTip = false;
			RedPointProxy.instance.Refresh();
		}
		public void RemoveNewFormationTip(int id)
		{
			_newFormationListByRedPoint.Remove(id);
			RedPointProxy.instance.Refresh();
		}

		private long _serverTime = 0;
		private long _lastTime = 0;
		private bool _isFormationPointRecoverFullTip = false;
		void Update()
		{
//			_serverTime = TimeController.instance.ServerTimeTicksSecond;
//			if(_serverTime - _lastTime >= 1)
//			{
//				_lastTime = _serverTime;
//				if (trainPoint < trainPointRecoverMax)
//				{
//					if (trainPointNextRecoverCountDown > 0)
//					{
//						trainPointNextRecoverCountDown = (int)Mathf.Max(trainPointNextRecoverTime - _serverTime*1000, 0);
//						if (trainPointNextRecoverCountDown <= 0)
//						{
//							trainPointNextRecoverCountDown = 0;
//							FormationController.instance.CLIENT2LOBBY_LineupPointSyn_REQ();
//						}
//                    }
//                }
//                if(onRecoverTimeUpdateDelegate != null)
//                {
//                    onRecoverTimeUpdateDelegate();
//                }
//            }
            
        }
		//单个阵型红点提示
		public int GetNewFormationTipByRedPoint(int id)
		{
			if (!FunctionOpen.Model.FunctionOpenProxy.instance.IsFunctionOpen(FunctionOpenType.FormationTraining))
			{
				return 0;
			}
			if(_newFormationListByRedPoint.Contains(id))
				return 1;
			return 0;
		}
		//有新阵型and培养点满
		public int GetNewFormationAndPointFullTipByRedPoint()
		{
			if (!FunctionOpen.Model.FunctionOpenProxy.instance.IsFunctionOpen(FunctionOpenType.FormationTraining))
			{
				return 0;
			}
			if(_newFormationListByRedPoint.Count > 0 || _isFormationPointRecoverFullTip)
			{
				int needTip = 0;
				Dictionary<int, FormationInfo> dic = FormationProxy.instance.FormationInfoDictionary;
				foreach(var data in dic)
				{
					if(data.Value.formationState != FormationState.Locked)
					{
						GameResData resData = data.Value.UpgradeResCost;
						int ownCount = 0;
						if(resData.type == BaseResType.Item)
						{
							ownCount = ItemProxy.instance.GetItemCountByItemID(resData.id);
						}else
						{
							ownCount = GameProxy.instance.BaseResourceDictionary.GetValue(resData.type);
						}
						if(ownCount >= resData.count && trainPoint >= data.Value.UpgradeTrainPointCost)
						{
							needTip = 1;
							break;
						}

					}
				}
				return needTip;
			}
			return 0;
		}

        public void UpdateFormationChangedByProtocol()
		{
			if(onFormationChangedDelegate!= null)
				onFormationChangedDelegate();
		}
		public void UpdateTrainPointByProtocol(int trainPoint,int recoverLimit,long recoverTime)
		{
			this.trainPoint = trainPoint;
			trainPointRecoverMax = recoverLimit;
			trainPointNextRecoverTime = recoverTime;
			if(trainPointNextRecoverTime == -1)
			{
				trainPointNextRecoverCountDown = -1;
				_isFormationPointRecoverFullTip = true;
			}else{
				trainPointNextRecoverCountDown = (int)Mathf.Max(trainPointNextRecoverTime - (TimeController.instance.ServerTimeTicksSecond-1)*1000, 0);
			}
			if(onRecoverTimeUpdateDelegate != null)
			{
				onRecoverTimeUpdateDelegate();
			}
		}
		public void UpdateUpgradeFormationByProtocol()
		{
			if(onUpgradeTrainFormationDelegate!= null)
				onUpgradeTrainFormationDelegate();
			LuaTable formationModelLua = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel","formation_model")[0];
			formationModelLua.GetLuaFunction("FormationUpgradeDelegateHandler").Call();
		}
		public void UpdateBuyTrainPointByProtocol()
		{
			if(onBuyTrainPointDelegate!= null)
				onBuyTrainPointDelegate();
		}

        public void UpdateAdditionFormationAttrActiveByProtocol()
        {
            if (onAdditionFormationAttrActiveDelegate != null)
                onAdditionFormationAttrActiveDelegate();
        }
	}
}

