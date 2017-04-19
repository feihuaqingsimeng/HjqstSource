using UnityEngine;
using System.Collections;
using Logic.Skill.Model;
using UnityEngine.UI;
using Common.Localization;
using Logic.Player.Model;
using Logic.Skill;
using System.Collections.Generic;
using Logic.Enums;
using System;

namespace Logic.UI.Description.View
{
	public class CommonSkillDesView : MonoBehaviour {
		
		public const string PREFAB_PATH = "ui/description/common_skill_description_view";
		
		public static CommonSkillDesView Open(SkillInfo data,int curStar,int starMin, Vector3 pos,Vector2 sizeDelta)
		{
			CommonSkillDesView view = UIMgr.instance.Open<CommonSkillDesView>(PREFAB_PATH, EUISortingLayer.Tips);
			view.SetData(data, curStar, starMin,pos,sizeDelta);

			return view;
		}
		public static CommonSkillDesView Open(PlayerSkillTalentInfo data, Vector3 pos,Vector2 sizeDelta)
		{
			CommonSkillDesView view = UIMgr.instance.Open<CommonSkillDesView>(PREFAB_PATH, EUISortingLayer.Tips);
			view.SetData(data,pos,sizeDelta);
			
			return view;
		}
		public Text textTitle;
		public Text textCD;
		public Text textContent;
		public RectTransform rootPanel;
		public Image imgCombo;
		private float _defaultContentLineHeight = 23f;

		private int _defaultBorderX = 10;

		private SkillInfo _skillInfo;
		private PlayerSkillTalentInfo _talentSkillInfo;
		private Vector3 _worldPos;
		private Vector2 _sizeDelta ;
		private Vector2 _originSizeDelta;
		private int _curStar;
		private int _starMin ;
		void Awake()
		{
			_originSizeDelta = rootPanel.sizeDelta;
			_defaultContentLineHeight = textContent.preferredHeight;

		}
		public void SetData(PlayerSkillTalentInfo data,Vector3 worldPos,Vector2 sizeDelta)
		{
			_talentSkillInfo = data;
			_worldPos = worldPos;
			_sizeDelta = sizeDelta;
			StartCoroutine(RefreshCoroutine());
		}
		public void SetData(SkillInfo data,int curStar,int starMin,Vector3 worldPos,Vector2 sizeDelta)
		{

			_skillInfo = data;
			_worldPos = worldPos;
			_sizeDelta = sizeDelta;
			_curStar = curStar;
			_starMin = starMin;
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
			string name = "";
			float cd = 0;
            string des = "";
			string oldDes = "";
			string newDes = "";
			if(_skillInfo != null)
			{
                name = Localization.Get(_skillInfo.skillData.skillName);
                cd = _skillInfo.skillData.CD;
				des = Localization.Get(_skillInfo.skillData.skillDesc);
				//六星遍历（处理格式：攻击力增加[1,100][2,200]持续时间[1,5][2,6]s）变为（攻击力增加100持续时间5秒）
				int level = 0;
				for(int i = 0;i<6;i++)
				{
					while(true)
					{
						int start = des.IndexOf("{"+i+",");
						if(start >= 0)
						{
							int end = des.IndexOf("}",start);
							int length = end-start+1;
							oldDes = des.Substring(start,length);
							newDes = des.Substring(start+3,length-4);
							if (i == level )
							{
								des = des.Replace(oldDes,newDes);
							}
						}else
						{
							break;
						}
					}

				}
//				List<KeyValuePair<MechanicsValueType, float>> skillValueList = SkillUtil.GetMechanicsValueType(_skillInfo.skillData);
//				float[] skillValues = new float[10];//每组数保留2份
//
//				KeyValuePair<MechanicsValueType, float> v;
//				for(int i = 0,count = skillValueList.Count;i<count;i++)
//				{
//					v = skillValueList[i];
//					int index = (int)v.Key-1;
//					while(true)
//					{
//						if(index >= skillValues.Length)
//							break;
//						if(skillValues[index] == 0)
//						{
//							skillValues[index] = Mathf.Abs( v.Value);
//							if(index % 5 == 1 || index%5 == 3)
//								skillValues[index] *= 100;
//
//							break;
//						}
//						index += 5;
//					}
//				}
//				//第四个参数的攻击总和
//				float totoalValue = Mathf.Abs(skillValues[3])*_skillInfo.skillData.timeline.Count;
//				skillValues[4] = totoalValue;
//
//				des = Localization.Get(_skillInfo.skillData.skillDesc);
//				for(int i = 1;i<15;i++)
//				{
//					if(i %5 != 0)
//						des = des.Replace("{"+i+"}","{"+i+":f1}");
//				}
//
//
//				des = string.Format(des,skillValues[0],skillValues[1],skillValues[2],skillValues[3],skillValues[4],skillValues[5],skillValues[6],skillValues[7],skillValues[8],skillValues[9]);
            }
            else if (_talentSkillInfo != null) 
            {
                name = Localization.Get(_talentSkillInfo.name);
                des = Localization.Get(_talentSkillInfo.des);
            }
            textTitle.text = name;
			textCD.text = cd <= 0 ? string.Empty : string.Format("{0}s",_skillInfo.skillData.CD);
            textContent.text = des;
			//RefreshContent();
			float desDeltaHeight = textContent.preferredHeight-_defaultContentLineHeight;//默认有一行高度了

			rootPanel.sizeDelta = new Vector2(_originSizeDelta.x,_originSizeDelta.y+desDeltaHeight);


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
			string path = "";
            if (_skillInfo != null)
            {
                path = Logic.Skill.SkillUtil.GetDesTypeIcon(_skillInfo);
            }
			if(string.IsNullOrEmpty(path))
			{
				imgCombo.gameObject.SetActive(false);
			}else{
				imgCombo.gameObject.SetActive(true);
				imgCombo.SetSprite(Common.ResMgr.ResMgr.instance.Load<Sprite>(path));
                imgCombo.SetNativeSize();
			}
			rootPanel.gameObject.SetActive(true);
		}
        //private void RefreshContent()
        //{
        //    string content = Localization.Get(_skillInfo.skillData.skillDesc);
//			int firstIndex = content.IndexOf("{");
//			int lastIndex = content.LastIndexOf("}");
//			if(firstIndex >= 0 && lastIndex >= 0)
//			{
//				string oldDamageContent = content.Substring(firstIndex,lastIndex-firstIndex+1);
//				int tempLv = 1;
//				int index = 0;
//				int currentLv =(int)( _skillInfo.currentLevel < 1 ? 1:_skillInfo.currentLevel);
//				
//				string newDamageContent = oldDamageContent;
//				while(true)
//				{
//					index = oldDamageContent.IndexOf("{",index);
//					if(index == -1)
//						break;
//					
//					if(tempLv == currentLv)
//					{
//						newDamageContent = newDamageContent.Insert(index,"<color=#00ff00>");
//						int tempIndex = newDamageContent.IndexOf("}",index);
//						newDamageContent = newDamageContent.Insert(tempIndex+1,"</color>");
//						break;
//					}else
//					{
//						index++;
//						tempLv++;
//					}
//				}
//				content = content.Replace(oldDamageContent,newDamageContent);
//			}

        //    textContent.text = content;
        //}
		public void Close()
		{
			UIMgr.instance.Close(PREFAB_PATH);
		}
	}
}

