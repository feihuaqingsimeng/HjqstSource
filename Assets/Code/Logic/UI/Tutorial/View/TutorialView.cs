using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Tutorial.Model;
using Logic.Tutorial.Controller;
using Common.ResMgr;
using Common.Localization;
using System;
using Logic.NPC.Model;
using UnityEngine.EventSystems;
using Common.UI.Components;

namespace Logic.UI.Tutorial.View
{
	public enum AnchorType
	{
		TopLeft = 0,
		TopRight = 1,
		BottomRight = 2,
		BottomLeft = 3,
	}

	public enum IndicateDirection
	{
		Up = 0,
		Down = 1,
		Left = 2,
		Right = 3,
		UpLeft = 4,
		UpRight = 5,
		DownLeft = 6,
		DownRight = 7,
	}

	public class TutorialView : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
	{
		public const string PREFAB_PATH = "ui/tutorial/tutorial_view";

		private TutorialChapterData _tutorialChapterData;
		private TutorialStepData _tutorialStepData;

		private List<string> _waitMSGIDList;
		private List<string> _forceCompleteMSGIDList;

		private Transform _targetTransform = null;
		private Button _targetButton = null;
		private Toggle _targetToggle = null;
		private EventTrigger _targetEventTrigger = null;
		private EventTriggerDelegate _targetEventTriggerDelegate = null;

		private List<Transform> _maskedTargetTransformList = new List<Transform>();
		private List<Transform> _maskedTransformList = new List<Transform>();

		private Transform _handIndicateUITransform;
		private Transform _arrowIndicateUITransform;

		private Vector3 _handRootOffset = Vector3.zero;
		private Vector3 _arrowRootOffset = new Vector3(50, 0, 0);

		#region UI components
		public Transform highlightGameObjectsRoot;
		public Transform maskedGameObjectsRoot;

		public Image maskImage;
		public TutorialMask tutorialMask;
		public Button nextStepButton;

		public Transform handRoot;
		public ParticleSystem highlightGlowParticleSystem;
		public Image handImage;
		public Transform arrowRoot;
		public Image arrowImage;
		public RawImage illustrateRawImage;
		public RectTransform dialogRootRectTransform;
		public RawImage npcBodyRawImage;
		public RawImage npcFaceRawImage;
		public Text dialogText;
		public Button skipButton;
		#endregion UI components

		private bool _isPointDownInTargetRectTransform = false;
		private bool _isPointUpInTargetRectTransform = false;
		public void OnPointerDown(PointerEventData eventData)
		{
			_isPointDownInTargetRectTransform = false;
			if (_targetTransform != null)
			{
				RectTransform targetRectTransform = _targetTransform.GetComponent<RectTransform>();
				if (targetRectTransform != null)
				{
					if (RectTransformUtility.RectangleContainsScreenPoint(targetRectTransform, eventData.pointerCurrentRaycast.screenPosition, eventData.pressEventCamera))
					{
						if (_targetButton != null)
							_targetButton.OnPointerDown(eventData);
						if (_targetToggle != null)
							_targetToggle.OnPointerDown(eventData);
						if (_targetEventTrigger != null)
							_targetEventTrigger.OnPointerDown(eventData);
						if (_targetEventTriggerDelegate != null)
							_targetEventTriggerDelegate.OnPointerDown(eventData);
						_isPointDownInTargetRectTransform = true;
					}
				}
			}
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			if (_targetTransform != null)
			{
				_isPointUpInTargetRectTransform = false;
				RectTransform targetRectTransform = _targetTransform.GetComponent<RectTransform>();
				if (targetRectTransform != null)
				{
					if (RectTransformUtility.RectangleContainsScreenPoint(targetRectTransform, eventData.pointerCurrentRaycast.screenPosition, eventData.pressEventCamera))
					{
						if (_targetButton != null)
							_targetButton.OnPointerUp(eventData);
						if (_targetToggle != null)
							_targetToggle.OnPointerUp(eventData);
						if (_targetEventTrigger != null)
							_targetEventTrigger.OnPointerUp(eventData);
						if (_targetEventTriggerDelegate != null)
							_targetEventTriggerDelegate.OnPointerUp(eventData);
						_isPointUpInTargetRectTransform = true;
					}
				}
			}
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			if (_targetTransform != null)
			{
				RectTransform targetRectTransform = _targetTransform.GetComponent<RectTransform>();
				if (targetRectTransform != null)
				{
					if (RectTransformUtility.RectangleContainsScreenPoint(targetRectTransform, eventData.pointerCurrentRaycast.screenPosition, eventData.pressEventCamera)
					    && _isPointDownInTargetRectTransform
					    && _isPointUpInTargetRectTransform)
					{
						if (_targetButton != null)
							_targetButton.OnPointerClick(eventData);
						if (_targetToggle != null)
							_targetToggle.OnPointerClick(eventData);
						if (_targetEventTrigger != null)
							_targetEventTrigger.OnPointerClick(eventData);
						if (_targetEventTriggerDelegate != null)
							_targetEventTriggerDelegate.OnPointerClick(eventData);
						OnClickTarget();
					}
				}
			}
		}

		private Vector2 GetAnchoredVector2 (AnchorType anchorType)
		{
			switch (anchorType)
			{
				case AnchorType.TopLeft:
					return new Vector2(0, 1);
				case AnchorType.TopRight:
					return new Vector2(1, 1);
				case AnchorType.BottomRight:
					return new Vector2(1, 0);
				case AnchorType.BottomLeft:
					return Vector2.zero;
				default:
					return Vector2.zero;
			}
		}

		private Quaternion GetArrowLocalRotationByIndicateDirection (IndicateDirection indicateDirection)
		{
			switch (indicateDirection)
			{
				case IndicateDirection.Up:
					return Quaternion.Euler(new Vector3(0, 0, 90));
				case IndicateDirection.Down:
					return Quaternion.Euler(new Vector3(0, 0, 270));
				case IndicateDirection.Left:
					return Quaternion.Euler(Vector3.zero);
				case IndicateDirection.Right:
					return Quaternion.Euler(new Vector3(0, 0, 180));
				default:
					return Quaternion.Euler(Vector3.zero);
			}
		}

		void Awake ()
		{
			handRoot.gameObject.SetActive(false);
			arrowRoot.gameObject.SetActive(false);
			tutorialMask.gameObject.SetActive(false);
			dialogRootRectTransform.gameObject.SetActive(false);
		}

		void Start ()
		{
			LeanTween.delayedCall(gameObject, 0.1f, StartIndicate).setIgnoreTimeScale(true);
		}

		void OnDestroy ()
		{
			LeanTween.cancel(gameObject);
			LeanTween.cancel(handImage.gameObject);
			LeanTween.cancel(arrowImage.gameObject);
			if (Observers.Facade.Instance != null)
			{
				if (_forceCompleteMSGIDList != null)
				{
					for (int index = 0, count = _forceCompleteMSGIDList.Count; index < count; index++)
					{
						Observers.Facade.Instance.RegisterObserver(_forceCompleteMSGIDList[index].ToString(), OnReceiveForceCompleteMSG);
					}
				}
				for (int index = 0, count = _waitMSGIDList.Count; index < count; index++)
				{
					Observers.Facade.Instance.RemoveObserver(_waitMSGIDList[index].ToString(), OnReceiveWaitMSG);
				}
			}
		}

		void Update ()
		{
			if (_targetTransform != null)
			{
				Vector3 targetTransformRefLocalPos = transform.InverseTransformPoint(_targetTransform.position);
				tutorialMask.SetIndicateLocalPosition(targetTransformRefLocalPos + new Vector3(_tutorialStepData.highlightUIOffset.x, _tutorialStepData.highlightUIOffset.y, 0));
			}

			for (int index = 0, count = _maskedTransformList.Count; index < count; index++)
			{
				if (_maskedTransformList[index].gameObject != null)
				{
					Vector3 maskedTargetPosition = _maskedTargetTransformList[index].position;
					_maskedTransformList[index].position = new Vector3(maskedTargetPosition.x, maskedTargetPosition.y, 0);
				}
			}

			if (_handIndicateUITransform != null)
			{
				if (handRoot.position.x != _handIndicateUITransform.position.x
				    || handRoot.position.y != _handIndicateUITransform.position.y)
				{
					handRoot.position = new Vector3(_handIndicateUITransform.position.x, _handIndicateUITransform.position.y, handRoot.position.z);
					handRoot.localPosition += _handRootOffset;
				}
			}

			if (_arrowIndicateUITransform != null)
			{
				if (arrowRoot.position.x != _arrowIndicateUITransform.position.x
				    || arrowRoot.position.y != _arrowIndicateUITransform.position.y)
				{
					arrowRoot.position = new Vector3(_arrowIndicateUITransform.position.x, _arrowIndicateUITransform.position.y, arrowRoot.position.z);
					arrowRoot.localPosition += _arrowRootOffset;
				}
			}
		}

		public static TutorialView Open (TutorialChapterData tutorialChapterData, TutorialStepData tutorialStepData)
		{
			TutorialView.Close();
			TutorialView tutorialView = UIMgr.instance.Open<TutorialView>(PREFAB_PATH, EUISortingLayer.Tutorial, UIOpenMode.Replace);
			tutorialView.SetData(tutorialChapterData, tutorialStepData);
			return tutorialView;
		}

		public static void Close ()
		{
			if (UIMgr.instance.IsOpening(PREFAB_PATH))
			{
				UIMgr.instance.CloseImmediate(PREFAB_PATH);
			}
		}

		public void SetData (TutorialChapterData tutorialChapterData, TutorialStepData tutorialStepData)
		{
			_tutorialChapterData = tutorialChapterData;
			_tutorialStepData = tutorialStepData;
			_waitMSGIDList = new List<string>(_tutorialStepData.waitMSGIDList);
			for (int index = 0, count = _waitMSGIDList.Count; index < count; index++)
			{
				Observers.Facade.Instance.RegisterObserver(_waitMSGIDList[index].ToString(), OnReceiveWaitMSG);
			}

			if (_tutorialStepData.forceCompleteMSGIDList.Count > 0)
			{
				_forceCompleteMSGIDList = new List<string>(_tutorialStepData.forceCompleteMSGIDList);
				for (int index = 0, count = _forceCompleteMSGIDList.Count; index < count; index++)
				{
					Observers.Facade.Instance.RegisterObserver(_forceCompleteMSGIDList[index].ToString(), OnReceiveForceCompleteMSG);
				}
			}
			else
			{
				_forceCompleteMSGIDList = null;
			}

			_handRootOffset = tutorialStepData.handIndicatorOffset;
			_arrowRootOffset = tutorialStepData.arrowIndicatorOffset;

			maskImage.gameObject.SetActive(_tutorialStepData.enableMask);
			maskImage.GetComponent<UnityEngine.UI.Mask>().showMaskGraphic = _tutorialStepData.showMask;
			nextStepButton.gameObject.SetActive(_tutorialStepData.enableNextStepButton);
			if (_tutorialStepData.showDialog)
			{
				AnchorType dialogAnchorType = (AnchorType)Enum.Parse(typeof(AnchorType), _tutorialStepData.dialogAnchor);
				dialogRootRectTransform.anchorMin = GetAnchoredVector2(dialogAnchorType);
				dialogRootRectTransform.anchorMax = GetAnchoredVector2(dialogAnchorType);
				dialogRootRectTransform.pivot = GetAnchoredVector2(dialogAnchorType);
				dialogRootRectTransform.anchoredPosition = _tutorialStepData.dialogOffset;
				dialogText.text = Localization.Get(_tutorialStepData.dialogContentID);
			}
			if (_tutorialStepData.showNPC)
			{
				NPCShowData npcShowData = NPCShowData.GetNPCShowData(_tutorialStepData.npcShowID);
				string npcBodyPath = ResPath.GetNPCTexturePath(npcShowData.npc_name);
				string npcFacePath = ResPath.GetNPCTexturePath(npcShowData.faceDic[_tutorialStepData.npcFace]);
				npcBodyRawImage.texture = ResMgr.instance.Load<Texture>(npcBodyPath);
				npcBodyRawImage.SetNativeSize();
				npcBodyRawImage.transform.localScale = new Vector3(_tutorialStepData.npcFlip ? -1 : 1, 1, 1);
				npcFaceRawImage.texture = ResMgr.instance.Load<Texture>(npcFacePath);
				npcFaceRawImage.SetNativeSize();
				npcFaceRawImage.rectTransform.anchorMin = Vector2.zero;
				npcFaceRawImage.rectTransform.anchorMax = Vector2.zero;
				npcFaceRawImage.rectTransform.pivot = Vector2.zero;
				npcFaceRawImage.rectTransform.anchoredPosition = npcShowData.facePosition;

				AnchorType npcAnchorType = (AnchorType)Enum.Parse(typeof(AnchorType), _tutorialStepData.npcAnchor);
				npcBodyRawImage.rectTransform.anchorMin = GetAnchoredVector2(npcAnchorType);
				npcBodyRawImage.rectTransform.anchorMax = GetAnchoredVector2(npcAnchorType);
				npcBodyRawImage.rectTransform.pivot = GetAnchoredVector2(npcAnchorType);
				npcBodyRawImage.rectTransform.anchoredPosition = _tutorialStepData.npcOffset;
			}
			if (_tutorialStepData.showIllustrateImage)
			{
				illustrateRawImage.texture = ResMgr.instance.Load<Texture>(ResPath.GetTutorialIllustrateImagePath(_tutorialStepData.illustrateImageName));
				illustrateRawImage.SetNativeSize();
				illustrateRawImage.rectTransform.anchoredPosition = _tutorialStepData.illustrateImageAnchoredPosition;
			}
			illustrateRawImage.gameObject.SetActive(_tutorialStepData.showIllustrateImage);
			dialogRootRectTransform.gameObject.SetActive(_tutorialStepData.showDialog);
			npcBodyRawImage.gameObject.SetActive(_tutorialStepData.showNPC);

			// Add for masked ui path
			Transform maskedTarget = null;
			Transform maskedTransform = null;
			for (int index = 0, count = _tutorialStepData.maskedUIPathList.Count; index < count; index++)
			{
				maskedTarget = UIMgr.instance.GetParticularUITransform(_tutorialStepData.maskedUIPathList[index][0], _tutorialStepData.maskedUIPathList[index][1]);
				_maskedTargetTransformList.Add(maskedTarget);
				maskedTransform = Instantiate(maskedTarget);
				DestroyImmediate(maskedTransform.GetComponent<Button>());
				DestroyImmediate(maskedTransform.GetComponent<Toggle>());

				Image[] images = maskedTransform.GetComponentsInChildren<Image>();
				for (int i = 0, imagesCount = images.Length; i < imagesCount; i++)
				{
					images[i].color = new Color(0, 0, 0 , 0);
				}
				RawImage[] rawImages = maskedTransform.GetComponentsInChildren<RawImage>();
				for (int i = 0, rawImagesCount = rawImages.Length; i < rawImagesCount; i++)
				{
					rawImages[i].color = new Color(0, 0, 0, 0);
				}

				maskedTransform.SetParent(maskedGameObjectsRoot, false);
				maskedTransform.position = new Vector3(maskedTarget.position.x, maskedTarget.position.y, 0);
				maskedTransform.SetAsFirstSibling();
				_maskedTransformList.Add(maskedTransform);
			}
			// Add for masked ui path

			skipButton.gameObject.SetActive(_tutorialChapterData.isSkippable);
			SetTargetTransform();
		}

		private void SetTargetTransform ()
		{
			if (_targetTransform != null)
				return;

			if (_tutorialStepData.highlightUIPath != null)
			{
				_targetTransform = UIMgr.instance.GetParticularUITransform(_tutorialStepData.highlightUIPath[0], _tutorialStepData.highlightUIPath[1]);
				
				_targetButton = null;
				_targetToggle = null;
				_targetEventTrigger = null;
				_targetEventTriggerDelegate = null;
				
				Button[] buttons = _targetTransform.GetComponentsInChildren<Button>(true);
				Toggle[] toggles = _targetTransform.GetComponentsInChildren<Toggle>(true);
				EventTrigger[] eventTriggers = _targetTransform.GetComponentsInChildren<EventTrigger>(true);
				EventTriggerDelegate[] eventTriggerDelegates = _targetTransform.GetComponentsInChildren<EventTriggerDelegate>(true);
				if (buttons.Length > 0)
				{
					_targetButton = buttons[0];
				}
				if (toggles.Length > 0)
				{
					_targetToggle = toggles[0];
				}
				if (eventTriggers.Length > 0)
				{
					_targetEventTrigger = eventTriggers[0];
				}
				if (eventTriggerDelegates.Length > 0)
				{
					_targetEventTriggerDelegate = eventTriggerDelegates[0];
				}
				
				Vector3 targetTransformRefLocalPos = transform.InverseTransformPoint(_targetTransform.position);
				tutorialMask.SetIndicateLocalPosition(targetTransformRefLocalPos + new Vector3(_tutorialStepData.highlightUIOffset.x, _tutorialStepData.highlightUIOffset.y, 0));
				tutorialMask.gameObject.SetActive(_tutorialStepData.enableMask);
			}
		}

		private void StartIndicate ()
		{
			SetTargetTransform();
			if (_tutorialStepData.showHandIndicator)
			{
				highlightGlowParticleSystem.Stop();
				highlightGlowParticleSystem.Play();

				IndicateDirection indicateDirection = (IndicateDirection)Enum.Parse(typeof(IndicateDirection), _tutorialStepData.handIndicateDirection);
				switch (indicateDirection)
				{
					case IndicateDirection.UpLeft:
						handImage.transform.localScale = new Vector3(1, -1, 1);
						break;
					case IndicateDirection.UpRight:
						handImage.transform.localScale = new Vector3(-1, -1, 1);
						break;
					case IndicateDirection.DownLeft:
						handImage.transform.localScale = Vector3.one;
						break;
					case IndicateDirection.DownRight:
						handImage.transform.localScale = new Vector3(-1, 1, 1);
						break;
					default:
						break;
				}
				_handIndicateUITransform = UIMgr.instance.GetParticularUITransform(_tutorialStepData.handIndicateUIPath[0], _tutorialStepData.handIndicateUIPath[1]);
				handRoot.position = new Vector3(_handIndicateUITransform.position.x, _handIndicateUITransform.position.y, handRoot.position.z);
				handRoot.localPosition += _handRootOffset;
				HandIndicate(indicateDirection);
			}
			if (_tutorialStepData.showArrowIndicator)
			{
				IndicateDirection indicateDirection = (IndicateDirection)Enum.Parse(typeof(IndicateDirection), _tutorialStepData.arrowIndicateDirection);
				arrowImage.transform.localRotation = GetArrowLocalRotationByIndicateDirection(indicateDirection);
				_arrowIndicateUITransform = UIMgr.instance.GetParticularUITransform(_tutorialStepData.arrowIndicateUIPath[0], _tutorialStepData.arrowIndicateUIPath[1]);
				arrowRoot.position = new Vector3(_arrowIndicateUITransform.position.x, _arrowIndicateUITransform.position.y, arrowRoot.position.z);
				arrowRoot.localPosition += _arrowRootOffset;
				ArrowIndicate(indicateDirection);
			}
			handRoot.gameObject.SetActive(_tutorialStepData.showHandIndicator);
			arrowRoot.gameObject.SetActive(_tutorialStepData.showArrowIndicator);
		}

		private void HandIndicate (IndicateDirection indicateDirection)
		{
			LeanTween.cancel(handImage.gameObject);
			LTDescr ltDescr;
			switch (indicateDirection)
			{
				case IndicateDirection.UpLeft:
				case IndicateDirection.UpRight:
					ltDescr = LeanTween.moveLocalY(handImage.gameObject, -15, 0.5f);
					ltDescr.setIgnoreTimeScale(true);
					ltDescr.setLoopPingPong();
					break;
				case IndicateDirection.DownLeft:
				case IndicateDirection.DownRight:
					ltDescr = LeanTween.moveLocalY(handImage.gameObject, 15, 0.5f);
					ltDescr.setIgnoreTimeScale(true);
					ltDescr.setLoopPingPong();
					break;
				default:
					break;
			}
		}

		private void ArrowIndicate (IndicateDirection indicateDirection)
		{
			LeanTween.cancel(arrowImage.gameObject);
			LTDescr ltDescr;
			switch (indicateDirection)
			{
				case IndicateDirection.Up:
					ltDescr = LeanTween.moveLocalY(arrowImage.gameObject, -15, 0.5f);
					break;
				case IndicateDirection.Down:
					ltDescr = LeanTween.moveLocalY(arrowImage.gameObject, 15, 0.5f);
					break;
				case IndicateDirection.Left:
					ltDescr = LeanTween.moveLocalX(arrowImage.gameObject, 15, 0.5f);
					break;
				case IndicateDirection.Right:
					ltDescr = LeanTween.moveLocalX(arrowImage.gameObject, -15, 0.5f);
					break;
				default:
					return;
			}
			ltDescr.setIgnoreTimeScale(true);
			ltDescr.setLoopPingPong();
		}

		private void CheckCompleteStep ()
		{
			if (TutorialProxy.instance.CurrentTutorialStepData != _tutorialStepData)
				return;

			if ((_forceCompleteMSGIDList != null && _forceCompleteMSGIDList.Count <= 0)
			    || (_waitMSGIDList == null || _waitMSGIDList.Count <= 0))
			{
				TutorialView.Close();
				TutorialController.instance.ExecuteStepComplete(_tutorialStepData);
				for (int i = 0, count = _tutorialStepData.onCompleteMSGList.Count; i < count; i++)
				{
					Observers.Facade.Instance.SendNotification(_tutorialStepData.onCompleteMSGList[i]);
				}
			}
		}

		private bool OnReceiveForceCompleteMSG (Observers.Interfaces.INotification note)
		{
			_forceCompleteMSGIDList.Remove(note.Name);
			CheckCompleteStep();
			return true;
		}

		private bool OnReceiveWaitMSG (Observers.Interfaces.INotification note)
		{
			_waitMSGIDList.Remove(note.Name);
			CheckCompleteStep();
			return true;
		}

		public void OnClickTarget ()
		{
			CheckCompleteStep();
		}
		
		#region UI event handlers
		public void OnClickNextStepButtonHandler ()
		{
			CheckCompleteStep();
		}

		public void ClickSkipHandler ()
		{
			TutorialController.instance.SkipCurrentTutorialChapter();
		}
		#endregion UI event handlers
	}
}