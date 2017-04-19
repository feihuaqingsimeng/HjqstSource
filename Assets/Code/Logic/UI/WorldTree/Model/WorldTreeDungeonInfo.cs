using Logic.Dungeon.Model;
using Logic.Enums;

namespace Logic.UI.WorldTree.Model
{
	public class WorldTreeDungeonInfo
	{
		public int dungeonID;
		public DungeonData dungeonData;
		public int orderNumber;
		public WorldTreeDungeonStatus worldTreeDungeonStatus;

		public WorldTreeDungeonInfo (DungeonData dungeonData, int orderNumber)
		{
			dungeonID = dungeonData.dungeonID;
			this.dungeonData = dungeonData;
			this.orderNumber = orderNumber;
			worldTreeDungeonStatus = WorldTreeDungeonStatus.Locked;
		}
	}
}