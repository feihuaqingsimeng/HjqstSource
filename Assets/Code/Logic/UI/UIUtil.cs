using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;
using Common.Localization;
using Common.ResMgr;
using Logic.Enums;
using Logic.Role.Model;

namespace Logic.UI
{
	public class UIUtil : MonoBehaviour
	{
		private static Material _grayMat;
		public static Material GrayMat
		{
			get
			{
				if (!_grayMat)
				{
					_grayMat = Common.ResMgr.ResMgr.instance.Load<Material>("material/gray");
				}
				return _grayMat;
			}
		}

		public const string BASE_RES_ICON_PATH = "sprite/main_ui/";
		public const string ROLE_TYPE_SMALL_ICON_PATH = "sprite/main_ui/";
		public const string ROLE_TYPE_BIG_ICON_PATH = "sprite/main_ui/";
		public const string ROLE_STRENGTHEN_STAGE_FRAME_PATH = "sprite/main_ui/";
		public const string ROLE_STRENGTHEN_STAGE_CORNER_PATH = "sprite/main_ui/";
		public const string ROLE_STAR_BIG_SPRITE_PATH = "sprite/main_ui/";

		public const string ROLE_QUALITY_FRAME_SPRITE_PATH = "sprite/main_ui/";
		public const string ITEM_QUALITY_FRAME_SPRITE_PATH = "sprite/main_ui/";

		public static string GetBaseResIconPath (BaseResType baseResType)
		{
			string baseResIconPath = string.Empty;

			switch (baseResType)
			{
				case BaseResType.PveAction:
					baseResIconPath = Path.Combine(BASE_RES_ICON_PATH, "icon_key_small");
					break;
				case BaseResType.TowerAction:
					baseResIconPath = Path.Combine(BASE_RES_ICON_PATH, "world_tree_fruit");
					break;
				case BaseResType.PvpAction:
					baseResIconPath = Path.Combine(BASE_RES_ICON_PATH, "icon_key_pvp_small");
					break;
				case BaseResType.Gold:
					baseResIconPath = Path.Combine(BASE_RES_ICON_PATH, "icon_coin_small");
					break;
				case BaseResType.Crystal:
					baseResIconPath = Path.Combine(BASE_RES_ICON_PATH, "");
					break;
				case BaseResType.Diamond:
					baseResIconPath = Path.Combine(BASE_RES_ICON_PATH, "icon_gem_small");
					break;
				case BaseResType.Honor:
					baseResIconPath = Path.Combine(BASE_RES_ICON_PATH, "icon_honor_small");
					break;
				case BaseResType.RMB:
					baseResIconPath = Path.Combine(BASE_RES_ICON_PATH, "icon_rmb_sml");
					break;
				case BaseResType.Hero_Exp:        //伙伴经验
					baseResIconPath = Path.Combine(BASE_RES_ICON_PATH, "icon_exp_green_sml");
					break;
				case BaseResType.Account_Exp:     //账号经验
					baseResIconPath = Path.Combine(BASE_RES_ICON_PATH, "icon_exp_sml");
					break;
				case BaseResType.ExpeditionPoint://远征币(类似荣誉)
					baseResIconPath = Path.Combine(BASE_RES_ICON_PATH, "icon_expedition_small");
					break;
				case BaseResType.WorldBossResource://世界Boss资源
					baseResIconPath = Path.Combine(BASE_RES_ICON_PATH, "icon_type_sml_06");
					break;
				case BaseResType.FromationTrainPoint: //阵型培养点
					baseResIconPath = Path.Combine(BASE_RES_ICON_PATH, "ui_team_3");
					break;
				default:
					break;
			}
			return baseResIconPath;
		}
		public static string GetRoleTypeName(RoleType roleType)
		{
			string name = roleType.ToString();
			switch (roleType)
			{
			case RoleType.Defence:
				name = Localization.Get("hero_type_name_def");
				break;
			case RoleType.Offence:
				name = Localization.Get("hero_type_name_atk");
				break;
			case RoleType.Mage:
				name = Localization.Get("hero_type_name_magic");
				break;
			case RoleType.Mighty:
				name = Localization.Get("hero_type_name_all");
				break;
			case RoleType.Support:
				name = Localization.Get("hero_type_name_assist");
				break;
			default:
				break;
			}
			return name;
		}
		public static Sprite GetRoleTypeSmallIconSprite (RoleType roleType)
		{
			string roleTypeSmallIconPath = string.Empty;
			switch (roleType)
			{
				case RoleType.Defence:
					roleTypeSmallIconPath = Path.Combine(ROLE_TYPE_SMALL_ICON_PATH, "icon_small_role_type_defence_2");
					break;
				case RoleType.Mage:
					roleTypeSmallIconPath = Path.Combine(ROLE_TYPE_SMALL_ICON_PATH, "icon_small_role_type_mage_2");
					break;
				case RoleType.Mighty:
					roleTypeSmallIconPath = Path.Combine(ROLE_TYPE_SMALL_ICON_PATH, "icon_small_role_type_mighty_2");
					break;
				case RoleType.Offence:
					roleTypeSmallIconPath = Path.Combine(ROLE_TYPE_SMALL_ICON_PATH, "icon_small_role_type_offence_2");
					break;
				case RoleType.Support:
					roleTypeSmallIconPath = Path.Combine(ROLE_TYPE_SMALL_ICON_PATH, "icon_small_role_type_support_2");
					break;
				default:
					break;
			}
			return ResMgr.instance.Load<Sprite>(roleTypeSmallIconPath);
		}

		public static Sprite GetRoleTypeBigIconSprite (RoleType roleType)
		{
			string roleTypeBigIconPath = string.Empty;
			switch (roleType)
			{
				case RoleType.Defence:
					roleTypeBigIconPath = Path.Combine(ROLE_TYPE_BIG_ICON_PATH, "icon_big_role_type_defence");
					break;
				case RoleType.Mage:
					roleTypeBigIconPath = Path.Combine(ROLE_TYPE_BIG_ICON_PATH, "icon_big_role_type_mage");
					break;
				case RoleType.Mighty:
					roleTypeBigIconPath = Path.Combine(ROLE_TYPE_BIG_ICON_PATH, "icon_big_role_type_mighty");
					break;
				case RoleType.Offence:
					roleTypeBigIconPath = Path.Combine(ROLE_TYPE_BIG_ICON_PATH, "icon_big_role_type_offence");
					break;
				case RoleType.Support:
					roleTypeBigIconPath = Path.Combine(ROLE_TYPE_BIG_ICON_PATH, "icon_big_role_type_support");
					break;
				default:
				break;
			}
			return ResMgr.instance.Load<Sprite>(roleTypeBigIconPath);
		}

		public static Sprite GetRoleQualityFrameSprite (RoleQuality roleQuality)
		{
			string roleQualityFrameSpritePath = string.Empty;
			switch (roleQuality)
			{
				case RoleQuality.White:
					roleQualityFrameSpritePath = Path.Combine(ROLE_QUALITY_FRAME_SPRITE_PATH, "ui_items_02");
					break;
				case RoleQuality.Green:
					roleQualityFrameSpritePath = Path.Combine(ROLE_QUALITY_FRAME_SPRITE_PATH, "ui_items_02_lv1");
					break;
				case RoleQuality.Blue:
					roleQualityFrameSpritePath = Path.Combine(ROLE_QUALITY_FRAME_SPRITE_PATH, "ui_items_02_lv2");
					break;
				case RoleQuality.Purple:
					roleQualityFrameSpritePath = Path.Combine(ROLE_QUALITY_FRAME_SPRITE_PATH, "ui_items_02_lv3");
					break;
				case RoleQuality.Orange:
					roleQualityFrameSpritePath = Path.Combine(ROLE_QUALITY_FRAME_SPRITE_PATH, "ui_items_02_lv4");
					break;
				case RoleQuality.Red:
					roleQualityFrameSpritePath = Path.Combine(ROLE_QUALITY_FRAME_SPRITE_PATH, "ui_items_02_lv4");
					break;
				default:
					roleQualityFrameSpritePath = Path.Combine(ROLE_QUALITY_FRAME_SPRITE_PATH, "ui_items_02");
					break;
			}
			return ResMgr.instance.Load<Sprite>(roleQualityFrameSpritePath);
		}

		public static Sprite GetItemQualityFrameSprite (ItemQuality itemQuality)
		{
			string itemQualityFrameSpritePath = string.Empty;
			switch (itemQuality)
			{
				case ItemQuality.White:
					itemQualityFrameSpritePath = Path.Combine(ITEM_QUALITY_FRAME_SPRITE_PATH, "ui_items_02");
					break;
				case ItemQuality.Green:
					itemQualityFrameSpritePath = Path.Combine(ITEM_QUALITY_FRAME_SPRITE_PATH, "ui_items_02_lv1");
					break;
				case ItemQuality.Blue:
					itemQualityFrameSpritePath = Path.Combine(ITEM_QUALITY_FRAME_SPRITE_PATH, "ui_items_02_lv2");
					break;
				case ItemQuality.Purple:
					itemQualityFrameSpritePath = Path.Combine(ITEM_QUALITY_FRAME_SPRITE_PATH, "ui_items_02_lv3");
					break;
				case ItemQuality.Orange:
					itemQualityFrameSpritePath = Path.Combine(ITEM_QUALITY_FRAME_SPRITE_PATH, "ui_items_02_lv4");
					break;
				case ItemQuality.Red:
					itemQualityFrameSpritePath = Path.Combine(ITEM_QUALITY_FRAME_SPRITE_PATH, "ui_items_02_lv4");
					break;
				default:
					itemQualityFrameSpritePath = Path.Combine(ITEM_QUALITY_FRAME_SPRITE_PATH, "ui_items_02");
					break;
			}
			return ResMgr.instance.Load<Sprite>(itemQualityFrameSpritePath);
		}

		public static Sprite GetRoleStrengthenStageFrameSprite (RoleInfo roleInfo)
		{
			string roleStrengthenStageFrameSpritePath = string.Empty;
			switch (roleInfo.RoleStrengthenStage)
			{
				case RoleStrengthenStage.White:
					roleStrengthenStageFrameSpritePath = Path.Combine(ROLE_STRENGTHEN_STAGE_FRAME_PATH, "ui_head_colour_white_1");
					break;
				case RoleStrengthenStage.Green:
					roleStrengthenStageFrameSpritePath = Path.Combine(ROLE_STRENGTHEN_STAGE_FRAME_PATH, "ui_head_colour_green_1");
					break;
				case RoleStrengthenStage.Blue:
					roleStrengthenStageFrameSpritePath = Path.Combine(ROLE_STRENGTHEN_STAGE_FRAME_PATH, "ui_head_colour_blue_1");
					break;
				case RoleStrengthenStage.Purple:
					roleStrengthenStageFrameSpritePath = Path.Combine(ROLE_STRENGTHEN_STAGE_FRAME_PATH, "ui_head_colour_purple_1");
					break;
				case RoleStrengthenStage.Orange:
					roleStrengthenStageFrameSpritePath = Path.Combine(ROLE_STRENGTHEN_STAGE_FRAME_PATH, "ui_head_colour_orange_1");
					break;
				default:
					break;
			}
			return ResMgr.instance.Load<Sprite>(roleStrengthenStageFrameSpritePath);
		}

		public static Sprite GetRoleStrengthenStageCornerSprite (RoleInfo roleInfo)
		{
			string roleStrengthenStageCornerSpritePath = string.Empty;
			switch (roleInfo.RoleStrengthenStage)
			{
				case RoleStrengthenStage.White:
					roleStrengthenStageCornerSpritePath = Path.Combine(ROLE_STRENGTHEN_STAGE_CORNER_PATH, "ui_head_colour_white_2");
					break;
				case RoleStrengthenStage.Green:
					roleStrengthenStageCornerSpritePath = Path.Combine(ROLE_STRENGTHEN_STAGE_CORNER_PATH, "ui_head_colour_green_2");
					break;
				case RoleStrengthenStage.Blue:
					roleStrengthenStageCornerSpritePath = Path.Combine(ROLE_STRENGTHEN_STAGE_CORNER_PATH, "ui_head_colour_blue_2");
					break;
				case RoleStrengthenStage.Purple:
					roleStrengthenStageCornerSpritePath = Path.Combine(ROLE_STRENGTHEN_STAGE_CORNER_PATH, "ui_head_colour_purple_2");
					break;
				case RoleStrengthenStage.Orange:
					roleStrengthenStageCornerSpritePath = Path.Combine(ROLE_STRENGTHEN_STAGE_CORNER_PATH, "ui_head_colour_orange_2");
					break;
				default:
					break;
			}
			return ResMgr.instance.Load<Sprite>(roleStrengthenStageCornerSpritePath);
		}

		public static Sprite GetRoleStarBigSprite (RoleInfo roleInfo)
		{
			string roleStarBigSpriteFullPath = string.Empty;
			switch (roleInfo.RoleStrengthenStage)
			{
				case RoleStrengthenStage.White:
					roleStarBigSpriteFullPath = Path.Combine(ROLE_STAR_BIG_SPRITE_PATH, "icon_star_white_big");
					break;
				case RoleStrengthenStage.Green:
					roleStarBigSpriteFullPath = Path.Combine(ROLE_STAR_BIG_SPRITE_PATH, "icon_star_green_big");
					break;
				case RoleStrengthenStage.Blue:
					roleStarBigSpriteFullPath = Path.Combine(ROLE_STAR_BIG_SPRITE_PATH, "icon_star_blue_big");
					break;
				case RoleStrengthenStage.Purple:
					roleStarBigSpriteFullPath = Path.Combine(ROLE_STAR_BIG_SPRITE_PATH, "icon_star_purple_big");
					break;
				case RoleStrengthenStage.Orange:
					roleStarBigSpriteFullPath = Path.Combine(ROLE_STAR_BIG_SPRITE_PATH, "icon_star_orange_big");
					break;
				default:
					break;
			}
			return ResMgr.instance.Load<Sprite>(roleStarBigSpriteFullPath);
		}

		public static Color GetRoleNameColor (RoleInfo roleInfo)
		{
			return GetRoleNameColor(roleInfo.RoleStrengthenStage);
		}
		public static Color GetRoleNameColor (RoleStrengthenStage stage)
		{
			Color roleNameColor = Color.white;
			switch (stage)
			{
			case RoleStrengthenStage.White:
				roleNameColor = new Color(211f / 255, 211f / 255, 211f / 255);
				break;
			case RoleStrengthenStage.Green:
				roleNameColor = new Color(87f / 255, 211f / 255, 19f / 255);
				break;
			case RoleStrengthenStage.Blue:
				roleNameColor = new Color(32f / 255, 142f / 255, 255f / 255);
				break;
			case RoleStrengthenStage.Purple:
				roleNameColor = new Color(220f / 255, 83f / 255, 213f / 255);
				break;
			case RoleStrengthenStage.Orange:
				roleNameColor = new Color(255f / 255, 123f / 255, 6f / 255);
				break;
			default:
				break;
			}
			return roleNameColor;
		}
		public static string FormatToGreenText (string text)
		{
			return string.Format(Localization.Get("common.green_text_template"), text);
		}

		public static string FormatToRedText (string text)
		{
			return string.Format(Localization.Get("common.red_text_template"), text);
		}
		public static string FormatToColorText(string text,string colorString)
		{
			return string.Format("<color=#{0}>{1}</color>",colorString,text);
		}

		// 新品质颜色
		public static string FormatStringWithinQualityColor (RoleQuality roleQuality, string str)
		{
			string formatString = string.Empty;
			switch (roleQuality)
			{
				case RoleQuality.White:
					formatString = Localization.Get("common.quality.csharp_formt_string.white");
					break;
				case RoleQuality.Green:
					formatString = Localization.Get("common.quality.csharp_formt_string.green");
					break;
				case RoleQuality.Blue:
					formatString = Localization.Get("common.quality.csharp_formt_string.blue");
					break;
				case RoleQuality.Purple:
					formatString = Localization.Get("common.quality.csharp_formt_string.purple");
					break;
				case RoleQuality.Orange:
					formatString =Localization.Get("common.quality.csharp_formt_string.orange");
					break;
				case RoleQuality.Red:
					formatString = Localization.Get("common.quality.csharp_formt_string.orange");
					break;
				default:
					formatString = Localization.Get("common.quality.csharp_formt_string.white");
					break;
			}
			return string.Format(formatString, str);
		}
		// 新品质颜色
		public static string FormatStringWithinQualityColor (int quality, string str)
		{
			string formatString = string.Empty;
			switch (quality)
			{
			case 1:
				formatString = Localization.Get("common.quality.csharp_formt_string.white");
				break;
			case 2:
				formatString = Localization.Get("common.quality.csharp_formt_string.green");
				break;
			case 3:
				formatString = Localization.Get("common.quality.csharp_formt_string.blue");
				break;
			case 4:
				formatString = Localization.Get("common.quality.csharp_formt_string.purple");
				break;
			case 5:
				formatString =Localization.Get("common.quality.csharp_formt_string.orange");
				break;
			case 6:
				formatString = Localization.Get("common.quality.csharp_formt_string.orange");
				break;
			default:
				formatString = Localization.Get("common.quality.csharp_formt_string.white");
				break;
			}
			return string.Format(formatString, str);
		}
		// 新品质颜色

		public static string GetChineseNumberString (int number)
		{
			if (number < 1 || number > 20)
			{
				return string.Empty;
			}

			string languageID = "common.num." + number.ToString();
			return Localization.Get(languageID);
		}

		public static string GetWeekdayListString (List<int> weekdayList)
		{
			string weekdayListString = string.Empty;
			int weekdayCount = weekdayList.Count;
			string weekDaySeparator = Localization.Get("common.week__day.separator");
			for (int i = 0; i < weekdayCount; i++)
			{
				int weekday = weekdayList[i];
				string weekdayStringKey = string.Format("common.week_day.{0}", weekday);
				string weekdayStringValue = Localization.Get(weekdayStringKey);
				weekdayListString += weekdayStringValue;
				if (i < weekdayCount - 1)
				{
					weekdayListString += weekDaySeparator;
				}
			}
			return weekdayListString;
		}

		public static void SetGrayExcludeUITextOutline (GameObject go, bool gray)
		{
			List<Image> imageList = new List<Image>(go.GetComponentsInChildren<Image>());
			int imageCount = imageList.Count;
			for (int i = 0; i < imageCount; i++)
			{
				imageList[i].SetGray(gray);
			}
			List<Text> textList = new List<Text>(go.GetComponentsInChildren<Text>());
			int textCount = textList.Count;
			for (int i = 0; i < textCount; i++)
			{
				textList[i].SetGray(gray);
			}
		}

		public static void SetGray (GameObject go, bool gray)
		{
			List<Image> imageList = new List<Image>(go.GetComponentsInChildren<Image>());
			int imageCount = imageList.Count;
			for (int i = 0; i < imageCount; i++)
			{
				imageList[i].SetGray(gray);
			}
			List<Text> textList = new List<Text>(go.GetComponentsInChildren<Text>());
			int textCount = textList.Count;
			for (int i = 0; i < textCount; i++)
			{
				textList[i].SetGray(gray);
			}
			UITextOutline[] outLineList = go.GetComponentsInChildren<UITextOutline>();
			for(int i = 0,count = outLineList.Length;i<count;i++)
			{
				outLineList[i].enabled = !gray;
			}
		}

		public static void CrossFadeAlpha (GameObject rootGameObject, float from, float to, float time)
		{
			List<Image> imageList = new List<Image>(rootGameObject.GetComponentsInChildren<Image>(rootGameObject));
			int imageCount = imageList.Count;
			for (int i = 0; i < imageCount; i++)
			{
				imageList[i].CrossFadeAlpha(from, 0, true);
				imageList[i].CrossFadeAlpha(to, time, true);
			}

			List<Text> textList = new List<Text>(rootGameObject.GetComponentsInChildren<Text>(rootGameObject));
			int textCount = textList.Count;
			for (int i = 0; i < textCount; i++)
			{
				textList[i].CrossFadeAlpha(from, 0, true);
				textList[i].CrossFadeAlpha(to, time, true);
			}
		}
		//解析服务器传过来的头像编号 由heroid + star组成
		public static string ParseHeadIcon(int headNo)
		{
			//headicon

			int headHeroId = headNo/10;
			int headStar = (headNo- headHeroId*10);
			//int isPet = headNo - headHeroId*100 - headStar*10;

			Logic.Hero.Model.HeroData heroData = Logic.Hero.Model.HeroData.GetHeroDataByID(headHeroId);

			if(heroData != null)
			{
//				if(heroData.hero_type == 2 )//主角
//				{
//					Logic.Pet.Model.PetData petdata = Logic.Pet.Model.PetData.GetPetDataByID(Logic.Player.Model.PlayerData.GetPlayerData((uint)headHeroId).pet_id);
//					return Common.ResMgr.ResPath.GetCharacterHeadIconPath(petdata.head_icon);
//				}else if(headStar <= heroData.headIcons.Length)
//				{
					return ResPath.GetCharacterHeadIconPath(heroData.headIcons[headStar-1]);
//				}
			}
			Debugger.LogError(string.Format("头像找不到 headHeroId :{0},star:{0}",headHeroId,headStar));
			return Logic.Game.Model.GameProxy.instance.PlayerInfo.HeadIcon;
		}
		//合并重复的元素
		public static List<Logic.Game.Model.GameResData> CombineGameResList(List<Logic.Game.Model.GameResData> dataList)
		{
			List<Logic.Game.Model.GameResData> tempList = new List<Logic.Game.Model.GameResData>();
			Logic.Game.Model.GameResData data;
			for(int k = 0,count2 = dataList.Count;k<count2;k++)
			{
				data = dataList[k];
				Logic.Game.Model.GameResData gsd;
				bool alreadyAdd = false;
				int count = tempList.Count;
				for(int i = 0;i<count;i++)
				{
					gsd = tempList[i];
					if(gsd.type == BaseResType.Hero)
					{
						if( gsd.type == data.type && gsd.id == data.id && gsd.star == data.star)
						{
							alreadyAdd = true;
							gsd.count += data.count;
							break;
						}
						
					}else if(gsd.type == BaseResType.Equipment)
					{
						if(gsd.type == data.type&& gsd.id == data.id)
						{
							alreadyAdd = true;
							gsd.count += data.count;
							break;
						}
						
					}else if(gsd.type == BaseResType.Item)
					{
						if(gsd.type == data.type&& gsd.id == data.id)
						{
							alreadyAdd = true;
							gsd.count += data.count;
							break;
						}
					}else
					{
						if(gsd.type == data.type)
						{
							gsd.count += data.count;
							alreadyAdd = true;
							break;
						}
					}
				}
				if(!alreadyAdd)
				{
					tempList.Add(new Logic.Game.Model.GameResData(data.type,data.id,data.count,data.star));
				}
			}
			return tempList;
		}
	}
}
