using UnityEngine;
using System.Collections;
using Logic.Protocol.Model;

namespace Logic.Item.Model
{
	public class ItemInfo  
	{
		public int instanceId;
		public ItemData itemData;
		public int count;

		public ItemInfo(int instanceID,int itemDataid,int count)
		{
			this.instanceId = instanceID;
			this.itemData = ItemData.GetItemDataByID(itemDataid);
			if(itemData == null)
				Debugger.LogError("itemdata is null,can't find itemId:"+itemDataid);
			this.count = count;
		}
		public ItemInfo(ItemInfo item)
		{
			instanceId = item.instanceId;
			itemData = item.itemData;
			count = item.count;
		}

		public ItemInfo (DrawCardDropProto drawCardDropProto)
		{
			this.instanceId = drawCardDropProto.instanceId;
			this.itemData = ItemData.GetItemDataByID(drawCardDropProto.no);
			this.count = 1;
		}

		public ItemInfo(ItemProtoData data)
		{
			Update(data);
		}
		public void Update(ItemProtoData data)
		{
			instanceId = data.id;
			itemData = ItemData.GetItemDataByID(data.modelId);
			if(itemData == null)
				Debugger.LogError("itemdata not null,can't find itemId:"+data.modelId);
			count = data.num;
		}
	}

}
