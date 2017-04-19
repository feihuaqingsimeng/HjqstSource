using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using Logic.Item.Model;
using Common.ResMgr;
using Logic.UI.Tips.View;
using Common.Localization;
using Logic.Enums;
using Logic.Game.Model;

namespace Logic.UI.Role.View
{
	public class MedicineAutoUseButton : MonoBehaviour ,IPointerDownHandler,IPointerUpHandler,IPointerExitHandler
	{
		public delegate bool OnAutoAddExpDelegate(int addExp);

		public OnAutoAddExpDelegate onAutoAddExpDelegate;
		public System.Action onMedicineCompleteDelegate ;
		public System.Action<int,int> onSendProtocolDelegate;//<itemid,count>
		public int itemId;
		
		public Image itemImage;
		public Text itemCountText;

		private ItemInfo _itemInfo;
		private bool _isPressUp = false;
		private int _useTotalNum = 0;
		public void Set(int itemId)
		{
			this.itemId = itemId;
			Refresh();
		}
		
		void Awake()
		{
			Refresh();
		}
		void Start () 
		{
		}
		
		public void Refresh()
		{
			_itemInfo = ItemProxy.instance.GetItemInfoByItemID(itemId);
			if(_itemInfo == null)
				_itemInfo = new ItemInfo(0,itemId,0);
			Sprite sprite =  ResMgr.instance.Load<Sprite>( ResPath.GetItemIconPath( _itemInfo.itemData.icon));
			if(sprite != null)
				itemImage.SetSprite(sprite);
			RefreshCount();
		}
		private void RefreshCount()
		{
			itemCountText.text = _itemInfo.count == 0 ? UIUtil.FormatToRedText(_itemInfo.count.ToString()) :UIUtil.FormatToGreenText(_itemInfo.count.ToString());
		}
		public void OnPointerDown(PointerEventData data)
		{
			if(_itemInfo.count == 0)
			{
				CommonAutoDestroyTipsView.Open(string.Format(Localization.Get("ui.goodsJumpPathView.notEnough"),Localization.Get(_itemInfo.itemData.name)));
				return;
			}
			if(!AutoAddExp(0))
			{
				CommonAutoDestroyTipsView.Open(Localization.Get("ui.goodsJumpPathView.levelMax"));
				return;
			}
			_isPressUp = false;
			_useTotalNum = 0;
			StartCoroutine(MedicineUseCoroutine());
			Debug.Log("medicine down itemid:"+itemId);
		}
		public void OnPointerExit(PointerEventData data)
		{
			_isPressUp = true;
		}
		public void OnPointerUp(PointerEventData data)
		{
			_isPressUp = true;
			SendProtocol();

			if (onMedicineCompleteDelegate!= null )
			{
				onMedicineCompleteDelegate();
			}
		}

		private bool AutoAddExp(int addExp)
		{
			if(onAutoAddExpDelegate != null)
				return onAutoAddExpDelegate(addExp);
			return false;
		}
		private IEnumerator MedicineUseCoroutine()
		{
			int index = 0;
			int[] addExps = GlobalData.GetGlobalData().expSolutions;
			int addExp = itemId == (int)ITEM_ID.bigExpMedicine ? addExps[0] : itemId == (int)ITEM_ID.midExpMedicine ? addExps[1] : itemId == (int)ITEM_ID.smallExpMedicine ? addExps[2]:0;
			while(true)
			{
				if(_isPressUp)
					break;
				_useTotalNum++;
				_itemInfo.count --;
				RefreshCount();
				if(!AutoAddExp(addExp))
				{
					CommonAutoDestroyTipsView.Open(Localization.Get("ui.goodsJumpPathView.levelMax"));
					break;
				}
				if(_itemInfo.count <= 0)
				{
					CommonAutoDestroyTipsView.Open(string.Format(Localization.Get("ui.goodsJumpPathView.notEnough"),Localization.Get(_itemInfo.itemData.name)));
					break;
				}
				if(index == 0)
					yield return new WaitForSeconds(0.5f);
				else
					yield return new WaitForSeconds(0.15f);
				index ++;
			}
		}
		private void SendProtocol()
		{
			if(_useTotalNum != 0)
			{
				if(onSendProtocolDelegate != null)
					onSendProtocolDelegate(itemId,_useTotalNum);
			}
			_useTotalNum = 0;
		}
	}
}

