using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Logic.UI.Pvp.Model;
using Common.ResMgr;
using Logic.Game.Model;

namespace Logic.UI.Pvp.View
{
	public class PvpTopHundredFighterButton : MonoBehaviour 
	{

		#region ui component
		public Image imgRank;
		public Image imgHeadIcon;
		public Text textRank;
		public Text textLevel;
		public Text textName;
		public Text textPowerDes;
		public Text textPowerNum;
		public Text textCheckBtn;
		#endregion

		private PvpFighterInfo _pvpFighterInfo;
		public PvpFighterInfo PvpFighterInfo
		{
			get
			{
				return _pvpFighterInfo;
			}
		}

		public void SetPvpFighterInfo(PvpFighterInfo info)
		{
			_pvpFighterInfo = info;
			Refresh();
		}

		private void Refresh()
		{
			if(_pvpFighterInfo.rank<=3)
			{
				textRank.gameObject.SetActive(false);
				imgRank.gameObject.SetActive(true);
				imgRank.SetSprite( ResMgr.instance.Load<Sprite>(GetTopThreeImgPath(_pvpFighterInfo.rank)));
				imgRank.SetNativeSize();
			}else{
				textRank.gameObject.SetActive(true);
				imgRank.gameObject.SetActive(false);
				textRank.text = _pvpFighterInfo.rank.ToString();
			}
			textLevel.text = _pvpFighterInfo.playerInfo.level.ToString();
//			textName.text = _pvpFighterInfo.playerInfo.name;
			textName.text = GameProxy.instance.AccountName;
			imgHeadIcon.SetSprite( ResMgr.instance.Load<Sprite>(_pvpFighterInfo.headIcon));
			textPowerDes.text = "战力";
			textPowerNum.text = _pvpFighterInfo.power.ToString();
			textCheckBtn.text = "查看";
		}
		private string GetTopThreeImgPath(int rank)
		{
			if(rank == 1)
			{
				return "sprite/main_ui/icon_badge_03";
			}
			if(rank == 2)
			{
				return "sprite/main_ui/icon_badge_02";
			}
			if(rank == 3)
			{
				return "sprite/main_ui/icon_badge_01";
			}
			return string.Empty;
		}
	}
}

