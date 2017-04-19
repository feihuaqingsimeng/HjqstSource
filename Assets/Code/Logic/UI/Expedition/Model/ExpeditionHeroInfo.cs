using UnityEngine;
using System.Collections;
using Logic.Role.Model;
using Logic.Game.Model;

namespace Logic.UI.Expedition.Model
{
	public class ExpeditionHeroInfo  
	{
		public RoleInfo roleInfo;
		public float hpRate;

		public ExpeditionHeroInfo(RoleInfo info,float hpRate)
		{
			this.roleInfo = info;
			this.hpRate = hpRate;
		}

		public bool IsDead
		{
			get
			{
				return hpRate <= 0;
			}
		}
		public bool IsPlayer
		{
			get
			{
				return GameProxy.instance.IsPlayer(roleInfo.instanceID);
			}
		}
	}

}
