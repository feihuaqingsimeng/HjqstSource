using UnityEngine;
using System.Collections;
namespace Logic.UI.Mask.View{
	public class BlockView : MonoBehaviour {

		public const string PREFAB_PATH = "ui/mask/block_view";
		public static void Open()
		{
			UIMgr.instance.Open(PREFAB_PATH,EUISortingLayer.Tips);
		}
		public static void Close()
		{
			UIMgr.instance.Close(PREFAB_PATH);
		}
	}
}
