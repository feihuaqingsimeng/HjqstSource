using UnityEngine;
using System.Collections;
using UnityEngine.UI;
namespace Logic.UI.CommonPopupView{
	public class CommonPopup : MonoBehaviour {
		
		
		public delegate void ChangeValueDelegat(int index);
		public ChangeValueDelegat onChangeValueDelegate;
		
		#region ui
		public Button originButton;
		public Text originText;
		public int space = 50;
		public int curSelectIndex;
		public POPUP_POSITION pop_pos = POPUP_POSITION.bottom;
		public string[] choice;
		
		#endregion
		
		private bool _isClick;
		private GameObject _subViewGO;
		
		private int _id = -1;
		
		public int ID{
			get{
				return _id;
			}
			set{
				_id = value;
			}
		}
		
		public enum POPUP_POSITION{
			bottom,
			top
			
		}
		void Awake(){
			
		}
		void Start(){
				
		}
	
		public void Init(){
			refreshText();
		}
		public void Init(int curSelectIndex,string[] choice,ChangeValueDelegat changeDelegate){
			this.curSelectIndex = curSelectIndex;
			this.choice = choice;
			this.onChangeValueDelegate = changeDelegate;
			refreshText();
		}
		public void DestroySubView(){
			Destroy (_subViewGO);
			_subViewGO = null;
			_isClick = false;
		}
		public void refreshText(){
			originText.text = choice [curSelectIndex];
		}
		public void refreshText(int selectIndex){
			if(selectIndex<0)
				selectIndex = 0;
			if(selectIndex>=choice.Length)
				selectIndex = choice.Length-1;
			curSelectIndex = selectIndex;
			refreshText ();
		}
		#region ui event handle
		public void ClickOriginButton(){
			//Debugger.Log ("[commonPopup][DoClickOriginButton] ID:"+ID);
			
			if (CheckClickSubView ())
				return;
			DoClickOriginButton ();
			
		}
		#endregion
		
		private bool CheckClickSubView(){
			if (ID != -1) {
				if(onChangeValueDelegate!= null){
					onChangeValueDelegate(ID);
				}
				CommonPopup rootPopup = transform.parent.parent.GetComponent<CommonPopup>();
				rootPopup.DestroySubView();
				rootPopup.refreshText(ID);
				return true;
			}
			return false;
		}
		
		private void DoClickOriginButton(){
			
			_isClick = !_isClick;
			if (!_isClick) {//hide
				DestroySubView();
			} else {
				Transform originButtonTran = originButton.transform;
				_subViewGO = new GameObject("subView");
				Transform subViewTran = _subViewGO.transform;
				
				//clone
				GameObject cloneGO;
				Transform cloneTran;
				CommonPopup popup;
				for(int i = 0,count = choice.Length;i<count;i++){
					cloneGO = Instantiate<GameObject>(originButton.gameObject);
					cloneTran = cloneGO.transform;
					cloneTran.SetParent(subViewTran,false);
					cloneGO.name = i.ToString();
					cloneTran.localScale = originButtonTran.localScale;
					if(pop_pos == POPUP_POSITION.top){
						cloneTran.localPosition = new Vector3(0, (i+1)*space,0);
					}else{
						cloneTran.localPosition = new Vector3(0, -(i+1)*space,0);
					}
					
					popup = cloneTran.GetComponent<CommonPopup>();
					popup.originText.text = choice[i];
					popup.ID = i;
					popup.onChangeValueDelegate = onChangeValueDelegate;
				}
				subViewTran.SetParent(originButton.transform,false);
			}
			
		}
	}
}

