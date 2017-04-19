using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Logic.Skill.Model;
using Common.ResMgr;


namespace Logic.UI.Description.View
{
	public class CommonRoleDesSkillView : MonoBehaviour 
	{
		
		#region ui component
		public Text textTitle;
		public Image skillPrefab;
		public Transform skillRoot;
		#endregion

		public void Set(string title,List<SkillInfo> skillList)
		{
			textTitle.text = title;
			for(int i = 0,count = skillList.Count;i<count;i++)
			{
				Image img = Instantiate<Image>(skillPrefab);
				img.SetSprite( ResMgr.instance.Load<Sprite>( ResPath.GetSkillIconPath(skillList[i].skillData.skillIcon)));
				img.transform.SetParent(skillRoot,false);
				SkillDesButton skillBtn = img.gameObject.AddComponent<SkillDesButton>();
				skillBtn.SetSkillInfo(skillList[i],0,0);
				Image combo = img.transform.Find("combo").GetComponent<Image>();
				Sprite sp = ResMgr.instance.LoadSprite(Skill.SkillUtil.GetDesTypeIcon(skillList[i].skillData));
				if (sp != null){
					combo.SetSprite(sp);
				}else{
					combo.gameObject.SetActive(false);
				}
			}
			skillPrefab.gameObject.SetActive(false);
		}
		public void Set(string title,List<int> skillDataIdList)
		{
			List<SkillInfo> skillInfoList = new List<SkillInfo>();
			for(int i = 0,count = skillDataIdList.Count;i<count;i++)
			{
				SkillInfo skillInfo = new SkillInfo((uint)skillDataIdList[i]);
				skillInfoList.Add(skillInfo);
			}
			Set(title,skillInfoList);
		}
	}
}

