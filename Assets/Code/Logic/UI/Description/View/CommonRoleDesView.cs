using UnityEngine;
using System.Collections;
using Logic.Hero.Model;
using UnityEngine.UI;
using Common.Localization;
using Logic.UI.CommonHeroIcon.View;
using System.Collections.Generic;
using Logic.UI.Hero.View;
using Logic.Hero;
using Logic.Skill.Model;
using Logic.Player.Model;
using Logic.Player;
using Common.Util;
using Logic.UI.GoodsJump.Model;
using Logic.Role.Model;
using Logic.Enums;
using Logic.Role;

namespace Logic.UI.Description.View
{
	public class CommonRoleDesView : MonoBehaviour 
	{
		
		public const string PREFAB_PATH = "ui/description/common_role_description_view";

		public static CommonRoleDesView Open(RoleInfo data,Vector3 pos,Vector2 sizeDelta)
		{
			CommonRoleDesView view = UIMgr.instance.Open<CommonRoleDesView>(PREFAB_PATH, EUISortingLayer.Tips);
			view.SetData(data,pos,sizeDelta);
			
			return view;
		}
		public Text textTitle;
		public Transform iconRoot;
		public Text textStory;
		public RoleAttributeView attrViewPrefab;
		public Transform attrViewRoot;
		public CommonRoleDesSkillView skillViewPrefab;
		public Transform skillViewRoot;
		public Text textFrom;
		public RectTransform rootPanel;

		private float _defaultContentLineHeight = 23f;

		private int _defaultBorderX = 10;

		private RoleInfo _roleInfo;
		private Vector3 _worldPos;
		private Vector2 _sizeDelta ;
		private Vector2 _originSizeDelta;
		
		void Awake()
		{
			_originSizeDelta = rootPanel.sizeDelta;
			_defaultContentLineHeight = textStory.preferredHeight;
		}

		public void SetData(RoleInfo data,Vector3 worldPos,Vector2 sizeDelta)
		{

			_roleInfo = data;
			_worldPos = worldPos;
			_sizeDelta = sizeDelta;
			StartCoroutine(RefreshCoroutine());

		}
		public void SetPivot(Vector2 pivot)
		{
			rootPanel.pivot = pivot;
		}
		public IEnumerator RefreshCoroutine()
		{
			rootPanel.gameObject.SetActive(false);
			yield return null;
			int strengthenAddShow = RoleUtil.GetStrengthenAddShowValue(_roleInfo.strengthenLevel);
			textTitle.text = Localization.Get(_roleInfo.heroData.name) + (strengthenAddShow == 0 ? "" : string.Format("+{0}",strengthenAddShow.ToString()));
			textTitle.color = UIUtil.GetRoleNameColor(_roleInfo);
			textStory.text = Localization.Get(_roleInfo.heroData.description);
			DropMessageData data = DropMessageData.GetDropMsgDataByResData((int)BaseResType.Hero,_roleInfo.modelDataId,0,_roleInfo.advanceLevel);
			if(data == null)
				data = DropMessageData.GetDropMsgDataByResData((int)BaseResType.Hero,_roleInfo.modelDataId);
			textFrom.text = data == null ? "" : Localization.Get( data.des);
			TransformUtil.ClearChildren(iconRoot,true);
			CommonHeroIcon.View.CommonHeroIcon icon =  CommonHeroIcon.View.CommonHeroIcon.Create(iconRoot);
			icon.SetRoleInfo(_roleInfo);
			icon.HideLevel();
			RefreshAttr();
			RefreshSkill();
			RefreshSize();
		}
		private void RefreshSize()
		{
			float length = skillViewRoot.GetComponent<GridLayoutGroup>().cellSize.y;
			float skillDeltaHeight = (skillViewRoot.childCount-3)*length;//默认三行高度
			
			float storyDeltaHeight = textStory.preferredHeight-_defaultContentLineHeight;//默认有一行高度了
			float fromDeltaHeight = (string.IsNullOrEmpty(textFrom.text) ? 0 : textFrom.preferredHeight)-_defaultContentLineHeight;//默认有一行高度了
			Vector3 localPos = skillViewRoot.parent.localPosition;
			skillViewRoot.parent.localPosition = new Vector3(localPos.x,localPos.y-storyDeltaHeight,localPos.z);
			rootPanel.sizeDelta = new Vector2(_originSizeDelta.x,_originSizeDelta.y+skillDeltaHeight+storyDeltaHeight+fromDeltaHeight);
			
			
			float screenHalfHeight = UIMgr.instance.designResolution.y/2;
			Vector3 localPosition = transform.InverseTransformPoint(_worldPos);
			float x = 0f;
			float y = localPosition.y;
			if(localPosition.x>0)
			{
				x = localPosition.x-_sizeDelta.x/2-rootPanel.sizeDelta.x/2-_defaultBorderX;
			}else{
				x = localPosition.x+_sizeDelta.x/2+rootPanel.sizeDelta.x/2+_defaultBorderX;
			}
			if(localPosition.y<rootPanel.sizeDelta.y/2-screenHalfHeight)
			{
				y = rootPanel.sizeDelta.y/2-screenHalfHeight;
			}
			if(localPosition.y>screenHalfHeight-rootPanel.sizeDelta.y/2)
			{
				y = screenHalfHeight - rootPanel.sizeDelta.y/2;
			}
			localPosition = new Vector3(x,y);
			rootPanel.localPosition = localPosition;
			rootPanel.gameObject.SetActive(true);
		}
		private void RefreshAttr()
		{
			TransformUtil.ClearChildren(attrViewRoot,true);
			//attr
			List<RoleAttribute> attrList = RoleUtil.CalcRoleMainAttributesList(_roleInfo);
			int count = attrList.Count;
			for(int i = 0;i<count;i++)
			{
				RoleAttributeView view = Instantiate<RoleAttributeView>(attrViewPrefab);
				view.transform.SetParent(attrViewRoot,false);
				view.Set(attrList[i]);
            }
            attrViewPrefab.gameObject.SetActive(false);
            
		}

		private void RefreshSkill()
		{
			TransformUtil.ClearChildren(skillViewRoot,true);
			skillViewPrefab.gameObject.SetActive(true);
			List<int> skillIdList = new List<int>();

//			PlayerInfo player = _roleInfo as PlayerInfo;
//			if(player!= null)
//			{
//				//召唤技能
//				SkillData summonData = SkillData.GetSkillDataById(player.playerData.summonSkillId);
//				if(summonData!= null)
//					skillIdList.Add((int)summonData.skillId);
//				if(skillIdList.Count!= 0)
//				{
//					CommonRoleDesSkillView view = Instantiate<CommonRoleDesSkillView>(skillViewPrefab);
//					view.transform.SetParent(skillViewRoot,false);
//					view.Set(Localization.Get("ui.common_des_view.summonSkill"),skillIdList);
//				}
//			}
			skillIdList.Clear();
			//主动技能

			SkillData skillData1 = SkillData.GetSkillDataById(_roleInfo.heroData.skillId1);
			if(skillData1!= null)
				skillIdList.Add((int)skillData1.skillId);
			SkillData skillData2 = SkillData.GetSkillDataById(_roleInfo.heroData.skillId2);
			if(skillData2!= null)
				skillIdList.Add((int)skillData2.skillId);

			if(skillIdList.Count!= 0)
			{
				CommonRoleDesSkillView view = Instantiate<CommonRoleDesSkillView>(skillViewPrefab);
				view.transform.SetParent(skillViewRoot,false);
				view.Set(Localization.Get("ui.common_des_view.activeSkill"),skillIdList);
			}

			//被动
			skillIdList.Clear();
			SkillData passiveSkillData = SkillData.GetSkillDataById(_roleInfo.heroData.passiveId1);
			if(passiveSkillData!= null)
			{
				skillIdList.Add((int)passiveSkillData.skillId);
				CommonRoleDesSkillView view = Instantiate<CommonRoleDesSkillView>(skillViewPrefab);
				view.transform.SetParent(skillViewRoot,false);
				view.Set(Localization.Get("ui.common_des_view.passiveSkill"),skillIdList);
			}

			skillViewPrefab.gameObject.SetActive(false);
		}
		public void Close()
		{
			UIMgr.instance.Close(PREFAB_PATH);
		}
	}
}

