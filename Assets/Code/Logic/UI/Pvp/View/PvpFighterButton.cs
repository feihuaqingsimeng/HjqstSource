using UnityEngine;
using System.Collections;
using Logic.UI.Pvp.Model;
using Common.Util;
using UnityEngine.UI;
using Logic.Character;
using Logic.Game.Model;
using System.Collections.Generic;
using Logic.Hero.Model;
using Logic.Role.Model;


namespace Logic.UI.Pvp.View
{
    public class PvpFighterButton : MonoBehaviour
    {
        public PvpFighterInfo pvpFighterInfo;
        private CharacterEntity _characterEntity;

        #region ui component
        public Text textName;
        public Text textPower;
        public Text textRank;
		public Transform formationRoot;
        #endregion
		private List<CommonHeroIcon.View.CommonHeroIcon> heroIconList = new List<CommonHeroIcon.View.CommonHeroIcon>();
        public void SetPvpFighterInfo(PvpFighterInfo info)
        {
            pvpFighterInfo = info;
            Refresh();
        }

        public void Refresh()
        {
            textName.text = pvpFighterInfo.name;
            textPower.text = pvpFighterInfo.power.ToString();
            textRank.text = pvpFighterInfo.rank.ToString();
			RefreshFormation();
        }

		private void RefreshFormation()
        {
			//TransformUtil.ClearChildren(formationRoot,true);
			for (int i = 0;i > heroIconList.Count;i++)
			{
				heroIconList[i].transform.gameObject.SetActive(false);
			}
			SortedDictionary<int,RoleInfo> roleDic = pvpFighterInfo.GetRoleInfoDicByPos();
			int tempCount = Mathf.Min(roleDic.Count,heroIconList.Count);
			int index = 0;
			foreach(var value in roleDic)
			{
				if (index >= tempCount)
				{
					CommonHeroIcon.View.CommonHeroIcon icon = CommonHeroIcon.View.CommonHeroIcon.Create(formationRoot);
					icon.SetRoleInfo(value.Value);
					heroIconList.Add(icon);
				}else
				{
					heroIconList[index].SetRoleInfo(value.Value);
				}
				heroIconList[index].transform.gameObject.SetActive(true);
				index ++;
			}

        }
		
    }
}

