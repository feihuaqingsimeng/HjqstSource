using UnityEngine;
using System.Collections.Generic;
using Logic.Game.Model;
using Common.Util;

namespace Logic.Item.Model
{
	public class PieceData
	{
		private static Dictionary<int, PieceData> _pieceDataDictionary;
		
		public static Dictionary<int, PieceData> GetPieceDatas()
		{
			if (_pieceDataDictionary == null)
			{
				_pieceDataDictionary = CSVUtil.Parse<int, PieceData>("config/csv/piece", "id");
			}
			return _pieceDataDictionary;
		}
		
		public static Dictionary<int, PieceData> PieceDataDictionary
		{
			get
			{
				if (_pieceDataDictionary == null)
				{
					GetPieceDatas();
				}
				return _pieceDataDictionary;
			}
		}

		public static PieceData GetPieceDataByID (int id)
		{
			PieceData heroPieceData = null;
			if (PieceDataDictionary.ContainsKey(id))
			{
				heroPieceData = PieceDataDictionary[id];
			}
			return heroPieceData;
		}

		[CSVElement("id")]
		public int id;

		public GameResData pieceGameResData;
		[CSVElement("piece_id")]
		public string pieceID
		{
			set
			{
				pieceGameResData = new GameResData(value);
			}
		}

		public GameResData heroGameResData;
		[CSVElement("hero_id")]
		public string heroID
		{
			set
			{
				heroGameResData = new GameResData(value);
			}
		}
	}
}
