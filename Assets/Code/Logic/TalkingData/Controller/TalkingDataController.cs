using System.Collections;
using Logic.Enums;
using LuaInterface;
using Logic.Hero.Model;
using Common.Localization;
using Logic.Equipment.Model;
using Logic.Item.Model;


namespace Logic.TalkingData.Controller
{
    public class TalkingDataController : SingletonPersistent<TalkingDataController>
    {
        private TDGAAccount _tdGAAccount;
		[NoToLua]
        public TDGAAccount tdGAAccount
        {
            set { _tdGAAccount = value; }
            private get { return _tdGAAccount; }
        }
		[NoToLua]
        public TDGAAccount TDGAAccountSetAccount(string accountId)
        {
            _tdGAAccount = TDGAAccount.SetAccount(accountId);
            return _tdGAAccount;
        }
		[NoToLua]
        public void SetAccountName(string accountName)
        {
            if (_tdGAAccount != null)
                _tdGAAccount.SetAccountName(accountName);
        }
		[NoToLua]
        public void TDGAAccountSetLevel(int level)
        {
            if (_tdGAAccount != null)
                tdGAAccount.SetLevel(level);
        }
		[NoToLua]
        public void TDGAVirtualCurrencyOnChargeRequest(string orderId, string iapId, double currencyAmount,
            string currencyType, double virtualCurrencyAmount, string paymentType)
        {
            TDGAVirtualCurrency.OnChargeRequest(orderId, iapId, currencyAmount, currencyType, virtualCurrencyAmount, paymentType);
        }
		[NoToLua]
        public void TalkingDataGAOnStart(string appID, string channelId)
        {
            TalkingDataGA.OnStart(appID, channelId);
        }
		[NoToLua]
        public void TalkingDataGAOnEnd()
        {
            TalkingDataGA.OnEnd();
        }
		[NoToLua]
        public void TDGAVirtualCurrencyOnChargeSuccess(string orderId)
        {
            TDGAVirtualCurrency.OnChargeSuccess(orderId);
        }
		[NoToLua]
        public static string GenerateMissionId(string missId, TalkDataMissionType talkDataMissionType)
        {
            string result = string.Empty;
            switch (talkDataMissionType)
            {
                case TalkDataMissionType.Task:
                    result = "task-" + missId;
                    break;
                case TalkDataMissionType.Level:
                    result = "level-" + missId;
                    break;
                case TalkDataMissionType.Tutorial:
                    result = "tutorial-" + missId;
                    break;
            }
            return result;
        }
		[NoToLua]
        public void TDGAMissionOnBegin(string missId, TalkDataMissionType talkDataMissionType)
        {
            TDGAMission.OnBegin(GenerateMissionId(missId, talkDataMissionType));
        }
		[NoToLua]
        public void TDGAMissionOnCompleted(string missId, TalkDataMissionType talkDataMissionType)
        {
            TDGAMission.OnCompleted(GenerateMissionId(missId, talkDataMissionType));
        }
		[NoToLua]
        public static string GenerateItemId(int itemId, BaseResType baseResType)
        {
			string result = "";
            switch (baseResType)
            {
			case BaseResType.Hero:
			{
				HeroData data = HeroData.GetHeroDataByID(itemId);
				if (data == null)
					Debugger.LogError("[TalkingDataController]GenerateItemId, can not find HeroData,id:"+itemId);
				else
					result = Localization.Get( data.name);
			}

                    break;
            case BaseResType.Equipment:
			{
				EquipmentData data = EquipmentData.GetEquipmentDataByID(itemId);
				if (data == null)
					Debugger.LogError("[TalkingDataController]GenerateItemId, can not find EquipData,id:"+itemId);
				else
					result = Localization.Get( data.name);
			}
                    break;
			case BaseResType.Item:
			{
				ItemData data = ItemData.GetItemDataByID(itemId);
				if (data == null)
					Debugger.LogError("[TalkingDataController]GenerateItemId, can not find ItemData,id:"+itemId);
				else
					result = Localization.Get( data.name);
			}
				break;
			default:
			{
				ItemData data = ItemData.GetBasicResItemByType(baseResType);
				if (data == null)
					Debugger.LogError("[TalkingDataController]GenerateItemId, can not find ItemData by baseResType:"+baseResType);
				else
					result = Localization.Get( data.name);
			}
                    break;
                
            }
            return result;
        }
		[NoToLua]
		public void TDGAItemOnPurchase(string functionName,BaseResType baseResType,int itemId, int itemNumber, double priceInVirtualCurrency)
        {
			TDGAItem.OnPurchase(functionName+":"+GenerateItemId(itemId, baseResType), itemNumber, priceInVirtualCurrency);
        }
		public void TDGAItemOnPurchase(string functionName, int baseResType ,int itemId, int itemNumber, double priceInVirtualCurrency)
		{
			TDGAItemOnPurchase(functionName,(BaseResType)baseResType,itemId,itemNumber,priceInVirtualCurrency);
		}
		public void TDGAItemOnPurchaseByCount(string functionName,int count,double priceInVirtualCurrency)
		{
			TDGAItem.OnPurchase(functionName, count, priceInVirtualCurrency);
        }
		[NoToLua]
        public void TDGAItemOnUse(int itemId, int itemNumber, BaseResType baseResType)
        {
            TDGAItem.OnUse(GenerateItemId(itemId, baseResType), itemNumber);
        }
		public void TDGAItemOnUse(int itemId, int itemNumber, int baseResType)
		{
			TDGAItemOnUse(itemId,itemNumber,(BaseResType)baseResType);
		}
    }

    public enum TalkDataMissionType
    {
        None = 0,
        Task = 1,    //任务
        Level = 2,   //关卡
        Tutorial = 3 //新手引导
    }
}
