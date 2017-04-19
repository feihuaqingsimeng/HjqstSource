using System.Collections.Generic;
using Common.Util;
using Logic.Enums;

namespace Logic.Dialog.Model
{
	public class DialogData
	{
		private static Dictionary<int, DialogData> _dialogDataDictionary;

		public static Dictionary<int, DialogData> DilagDataDictionary
		{
			get
			{
				if (_dialogDataDictionary == null)
				{
					_dialogDataDictionary = CSVUtil.Parse<int, DialogData>("config/csv/dialog", "id");
				}
				return _dialogDataDictionary;
			}
		}

		public static DialogData GetDialogDataByID (int dialogDataID)
		{
			DialogData dialogData = null;
			DilagDataDictionary.TryGetValue(dialogDataID, out dialogData);
			return dialogData;
		}

		public static DialogData GetNextDialogData (DialogData dialogData)
		{
			DialogData nextDialogData = null;
			DilagDataDictionary.TryGetValue(dialogData.nextID, out nextDialogData);
			return nextDialogData;
		}

		[CSVElement("id")]
		public int id;

		[CSVElement("type")]
		public int type;

		[CSVElement("head_icon")]
		public string headIcon;

		public DialogNPCSide dialogNPCSide = DialogNPCSide.Left;
		[CSVElement("side")]
		public int side
		{
			set
			{
				dialogNPCSide = (DialogNPCSide)value;
			}
		}

		[CSVElement("position")]
		public string position;

		[CSVElement("hero_name")]
		public string heroName;

		[CSVElement("message")]
		public string message;

		[CSVElement("pre_id")]
		public int preID;

		[CSVElement("next_id")]
		public int nextID;

		public bool canSkip = false;
		[CSVElement("skip")]
		public int skip
		{
			set
			{
				canSkip = value > 0;
			}
		}
	}
}