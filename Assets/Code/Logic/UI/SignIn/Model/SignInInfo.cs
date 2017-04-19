using UnityEngine;
using System.Collections;

namespace Logic.UI.SignIn.Model
{
	public class SignInInfo 
	{
		
		public int id;
		public SignInData signData;
		public bool isSign;

		public SignInInfo( int id,bool isSign)
		{
			this.id = id;
			this.signData = SignInData.GetSignInDataByID(id);
			this.isSign = isSign;
		}
	}
}

