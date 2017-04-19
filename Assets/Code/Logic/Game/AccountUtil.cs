using UnityEngine;
using Logic.Game.Model;

namespace Logic.Game
{
	public class AccountUtil : MonoBehaviour
	{
		public static float GetAccountExpPercentToNextLevel (int accountLevel)
		{
			float accountExpPercentToNextLevel = 0;
			AccountExpData data = AccountExpData.GetAccountExpDataByLv(accountLevel);
			if (data != null && data.exp > 0)
			{
				accountExpPercentToNextLevel = (float)GameProxy.instance.AccountExp / AccountExpData.GetAccountExpDataByLv(accountLevel).exp;
			}
			return accountExpPercentToNextLevel;
		}
	}
}
