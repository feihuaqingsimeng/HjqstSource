using UnityEngine;
using Common.Localization;
using Logic.Enums;
using Logic.Game.Model;

namespace Logic.Game
{
	public static class GameResUtil
	{
		public static bool IsBaseRes (BaseResType baseResType)
		{
			switch (baseResType)
			{
				case BaseResType.Account_Exp:
				case BaseResType.ArenaPoint:
				case BaseResType.Crystal:
				case BaseResType.Diamond:
				case BaseResType.Gold:
				case BaseResType.Hero_Exp:
				case BaseResType.Honor:
				case BaseResType.PveAction:
				case BaseResType.PvpAction:
				case BaseResType.TowerAction:
					return true;
				default:
					return false;
			}
		}

		public static string GetBaseResName (BaseResType baseResType)
		{
			string name = string.Empty;
			switch (baseResType)
			{
				case BaseResType.Account_Exp:
					name = Localization.Get("game.base_resource.account_exp");
					break;
				case BaseResType.ArenaPoint:
					name = Localization.Get("game.base_resource.arena_point");
					break;
				case BaseResType.Crystal:
					name = Localization.Get("game.base_resource.crystal");
					break;
				case BaseResType.Diamond:
					name = Localization.Get("game.base_resource.diamond");
					break;
				case BaseResType.Gold:
					name = Localization.Get("game.base_resource.gold");
					break;
				case BaseResType.Hero_Exp:
					name = Localization.Get("game.base_resource.hero_exp");
					break;
				case BaseResType.Honor:
					name = Localization.Get("game.base_resource.honor");
					break;
				case BaseResType.PveAction:
					name = Localization.Get("game.base_resource.pve_action");
					break;
				case BaseResType.PvpAction:
					name = Localization.Get("game.base_resource.pvp_actio");
					break;
				case BaseResType.TowerAction:
					name = Localization.Get("game.base_resource.tower_action");
					break;
				default:
					name = string.Empty;
					break;
			}
			return name;
		}
	}
}
