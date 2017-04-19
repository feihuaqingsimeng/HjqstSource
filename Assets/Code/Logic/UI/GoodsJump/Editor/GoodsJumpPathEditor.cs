using UnityEngine;
using System.Collections;
using UnityEditor;
using Logic.Enums;
using Logic.Game.Model;
using Logic.Skill.Model;
using Logic.UI.GoodsJump.Model;
using Logic.Dungeon.Model;
using System.Collections.Generic;
using Common.Localization;
using Logic.Drop.Model;
using System.Text;
using System.IO;
using Logic.Skill;
using Logic.Hero.Model;

namespace Logic.UI.GoodsJump.Editor
{
	public class GoodsJumpPathEditor : EditorWindow
	{

        [MenuItem("Tools/查找器-副本物品", false, 200)]
		 public static void OpenGoodsJumpPathEditor()
		 {
			EditorWindow.GetWindow<GoodsJumpPathEditor>(true,"查找器-副本物品");
		 }
		private BaseResType selectType;
		private int selectId;
		private int selectStar;
		private string testSkillId;
		private Dictionary<int,DungeonData> lootDungeonDictionay = new Dictionary<int,DungeonData>();
		private Dictionary<int,DungeonData> lootViewDisplayDungeonDictionay = new Dictionary<int,DungeonData>();
		private string resultNameTip = string.Empty;
		private string resultNameTip2 = string.Empty;
		Vector2 scrollPos = Vector2.zero;
		Vector2 scrollPos2 = Vector2.zero;

		void OnGUI()
		{
			EditorGUILayout.Space();
			GUILayout.Label("备注：\n" +
			                "1、只针对副本掉落查找\n" +
			                "2、星级为0: 表示忽略星级查找",GUILayout.Width(300),GUILayout.Height(50));

			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();

			GUILayout.Label("物品类型：",GUILayout.Width(100));
			selectType = (BaseResType)EditorGUILayout.EnumPopup(selectType,GUILayout.Width(200));

			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			
			GUILayout.Label("测试技能(heroid,skillid)：",GUILayout.Width(100));
			selectId = EditorGUILayout.IntField(selectId,GUILayout.Width(200),GUILayout.Height(20));
			
			EditorGUILayout.EndHorizontal();


			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();
			if(selectType == BaseResType.Hero || selectType == BaseResType.Equipment || selectType == BaseResType.Item)
			{
				GUILayout.Label("物品id：",GUILayout.Width(100));
				selectId = EditorGUILayout.IntField(selectId,GUILayout.Width(200),GUILayout.Height(20));
			}else
			{
				selectId = 0;
			}

			EditorGUILayout.EndHorizontal();

			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();
			if(selectType == BaseResType.Hero)
			{
				GUILayout.Label("物品星级：",GUILayout.Width(100));
				selectStar = EditorGUILayout.IntField(selectStar,GUILayout.Width(200),GUILayout.Height(20));

			}else
			{
				selectStar = 0;
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();
			if(GUILayout.Button("查找",GUILayout.Width(100),GUILayout.Height(25)))
			{
				Clear();
				if (!Application.isPlaying)
				{
					resultNameTip = "运行游戏才能查哟！！！";
					resultNameTip2 = string.Empty;
					return;
				}


				Dictionary<int,DungeonData> dungeonDataDic = DungeonData.DungeonDataDictionary;
				DungeonData dungeon;

				foreach(var value in dungeonDataDic)
				{
					dungeon = value.Value;

					CheckViewDisplayData(dungeon);
					CheckLootData(dungeon);

				}
				if(lootDungeonDictionay.Count == 0)
				{
					resultNameTip = "未找到哟！！！";
				}
				if(lootViewDisplayDungeonDictionay.Count == 0)
				{
					resultNameTip2 = "未找到哟！！！";
				}
				//----------------------打印技能伤害数据------------------
				TestSkillDescription();
				
				//---------------------test 屏蔽词--------------------
				//				string[] blockWords = Common.Util.BlackListWordUtil.blockWords;
				//				List<string> blockList = new List<string>();
				//				StringBuilder builder = new StringBuilder();
				//				for(int i = 0,count = blockWords.Length;i<count;i++)
				//				{
				//					if(!blockList.Contains(blockWords[i]))
				//					{
				//						blockList.Add(blockWords[i]);
				//						builder.Append(blockWords[i]).Append("\r\n");
				//					}
				//				}
				//				builder.AppendFormat("屏蔽词个数：{0}",blockList.Count);
				//				resultNameTip = builder.ToString();
				//				return ;
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.Space();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("副本列表(掉落包中的掉落)：",GUILayout.Width(310));
			EditorGUILayout.LabelField("副本列表(界面上显示的掉落)：");
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();

			scrollPos = EditorGUILayout.BeginScrollView(scrollPos,false,false,GUILayout.Width(700),GUILayout.Height(500));
			EditorGUILayout.TextArea(resultNameTip);
			EditorGUILayout.EndScrollView();

			EditorGUILayout.LabelField("",GUILayout.Width(10));

			scrollPos2 = EditorGUILayout.BeginScrollView(scrollPos2,false,false,GUILayout.Width(300),GUILayout.Height(500));
			EditorGUILayout.TextArea(resultNameTip2);
			EditorGUILayout.EndScrollView();
			EditorGUILayout.EndHorizontal();
		}

		private void Clear()
		{
			lootDungeonDictionay.Clear();
			lootViewDisplayDungeonDictionay.Clear();
			resultNameTip = string.Empty;
			resultNameTip2 = string.Empty;
		}
		private void CheckViewDisplayData(DungeonData dungeon)
		{
			List<GameResData> lootList = dungeon.eachLootPresent;
			GameResData resData;
			for(int i = 0,count = lootList.Count;i<count;i++)
			{
				resData = lootList[i];
				if(selectType == resData.type && selectId == resData.id)
				{
					if(selectStar == 0 || selectStar == resData.star)
					{
						FoundViewDisplayDungeon(dungeon);
					}
				}
			}

		}
		private void CheckLootData(DungeonData dungeon)
		{
			int[] loot_id = dungeon.each_loot_id;
			int id;
			PrizeData prizeData;
			PrizeElementData elementData;
			List<PrizeElementData> elementDataList;
			if(loot_id != null)
			{
				for(int i = 0,count = loot_id.Length;i<count;i++)
				{
					id = loot_id[i];
					prizeData = PrizeData.GetPrizeDataByID(id);
					if(prizeData != null)
					{
						int[] elementid = prizeData.prize_element_id;
						for(int j = 0,count2 = elementid.Length;j<count2;j++)
						{
							elementDataList = PrizeElementData.GetPrizeElementDataByID(elementid[j]);
							for(int k = 0,count3 = elementDataList.Count;k<count3;k++)
							{
								elementData = elementDataList[k];
								if(selectType == (BaseResType)elementData.type && selectId == elementData.item_id && elementData.count_min>0)
								{
									if(selectStar == 0 || selectStar == elementData.star)
									{
										FoundLootDungeon(dungeon);
									}
								}
							}
						}
					}
				}
			}
		}
		private void FoundLootDungeon(DungeonData data)
		{
			if(lootDungeonDictionay.ContainsKey(data.dungeonID))
				return;
			lootDungeonDictionay[data.dungeonID] = data;
		
			resultNameTip +=string.Format(" id:{0} ，类型：{1}  副本名：{2}\n",data.dungeonID,data.dungeonType, Localization.Get(data.name));
		}
		private void FoundViewDisplayDungeon(DungeonData data)
		{
			if(lootViewDisplayDungeonDictionay.ContainsKey(data.dungeonID))
				return;
			lootViewDisplayDungeonDictionay[data.dungeonID] = data;
			
			resultNameTip2 +=string.Format(" id:{0} ，类型：{1}  副本名：{2}\n",data.dungeonID,data.dungeonType, Localization.Get(data.name));
		}
		private void TestSkillDescription()
		{
			string name = "";
			float cd = 0;
			string des = "";
			resultNameTip = "";

			Dictionary<int,HeroData> hdDic =HeroData.HeroDataDictionary;
			List<uint> skillIdList = new List<uint>();
			foreach(var value in hdDic)
			{
				skillIdList.Clear();

				skillIdList.Add( value.Value.skillId1);
				skillIdList.Add(value.Value.skillId2);
				for(int k = 0,count2 = skillIdList.Count;k<count2;k++)
				{
					SkillData skillData = SkillData.GetSkillDataById(skillIdList[k]);
					if(skillData != null)
					{
						name = "heroId:{5},skillId:{6},( Time：{0}，Probabiblity：{1}，FixedValue：{2}，PercentValue：{3}，total:{4})\n";
						cd = skillData.CD;

						List<SkillDesInfo> desInfoList = SkillUtil.GetMechanicsValueType((int)skillIdList[k]);



                        List<KeyValuePair<MechanicsValueType, float>> skillValueList = null;// SkillUtil.GetMechanicsDesInfo(skillData);

						float[] skillValues = new float[5];//每组数保留2份
						
						KeyValuePair<MechanicsValueType, float> v;
						for(int i = 0,count = skillValueList.Count;i<count;i++)
						{
							v = skillValueList[i];
							int index = (int)v.Key-1;
							while(true)
							{
								if(index >= skillValues.Length)
									break;
								if(skillValues[index] == 0)
								{
									skillValues[index] = Mathf.Abs( v.Value);
									if(index % 5 == 1 || index%5 == 3)
										skillValues[index] *= 100;
									
									break;
								}
								index += 5;
							}
						}
						//第四个参数的攻击总和
						float totoalValue = Mathf.Abs(skillValues[3])*skillData.timeline.Count;
						skillValues[4] = totoalValue;
						
//						des = Localization.Get(skillData.skillDesc);
//						for(int i = 1;i<15;i++)
//						{
//							if(i %5 != 0)
//								des = des.Replace("{"+i+"}","{"+i+":f1}");
//						}
						
						
						//des = string.Format(des,skillValues[0],skillValues[1],skillValues[2],skillValues[3],skillValues[4],skillValues[5],skillValues[6],skillValues[7],skillValues[8],skillValues[9]);
						des = string.Format(name,skillValues[0],skillValues[1],skillValues[2],skillValues[3],skillValues[4],value.Value.id,skillIdList[k]);
						resultNameTip += des;
					}
				}

			}

		}
	}
}

