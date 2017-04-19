using UnityEngine;
using System.Collections.Generic;
using Common.Util;

namespace Logic.NPC.Model
{
	public class NPCShowData
	{
		private static Dictionary<string, NPCShowData> _npcShowDataDic;
		public static Dictionary<string, NPCShowData> NPCShowDataDic
		{
			get
			{
				if (_npcShowDataDic == null)
				{
					_npcShowDataDic = CSVUtil.Parse<string, NPCShowData>("config/csv/npc_show", "id");
				}
				return _npcShowDataDic;
			}
		}

		public static NPCShowData GetNPCShowData (string id)
		{
			NPCShowData npcShowData = null;
			NPCShowDataDic.TryGetValue(id, out npcShowData);
			return npcShowData;
		}

		[CSVElement("id")]
		public string id;

		[CSVElement("npc_name")]
		public string npc_name;

		public Dictionary<string, string> faceDic = new Dictionary<string, string>();
		[CSVElement("face1")]
		public string face1
		{
			set
			{
				faceDic.Add("face1", value);
			}
		}

		[CSVElement("face2")]
		public string face2
		{
			set
			{
				faceDic.Add("face2", value);
			}
		}

		[CSVElement("face3")]
		public string face3
		{
			set
			{
				faceDic.Add("face3", value);
			}
		}

		[CSVElement("face4")]
		public string face4
		{
			set
			{
				faceDic.Add("face4", value);
			}
		}

		[CSVElement("face5")]
		public string face5
		{
			set
			{
				faceDic.Add("face5", value);
			}
		}

		public Vector2 facePosition;
		[CSVElement("face_position")]
		public string facePostionStr
		{
			set
			{
				facePosition = value.ToVector2(';');
			}
		}
	}
}
