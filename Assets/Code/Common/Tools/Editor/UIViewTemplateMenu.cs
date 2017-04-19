using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Logic.Enums;
using Common.Canvases;

namespace Common.Tools.Editor
{
	public class UIViewTemplateMenu
	{
		[ExecuteInEditMode]
		[MenuItem("Tools/CreateUIViewTemplate")]
		public static void CreateUIViewTemplate ()
		{
			GameObject uiRoot = GameObject.Find("ui_root");

			GameObject uiViewTemplateGameObject = new GameObject("template_view");
			RectTransform uiViewTemplateRectTransform = uiViewTemplateGameObject.AddComponent<RectTransform>();
			uiViewTemplateRectTransform.SetParent(uiRoot.transform, false);
			uiViewTemplateRectTransform.localScale = Vector3.one;
			uiViewTemplateRectTransform.localPosition = Vector3.zero;
			uiViewTemplateRectTransform.localRotation = Quaternion.identity;

			BoxCollider boxCollider = uiViewTemplateGameObject.AddComponent<BoxCollider>();
			boxCollider.size = new Vector3(1136, 640, 1);

			Canvas canvas = uiViewTemplateGameObject.AddComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceCamera;
			canvas.overrideSorting = true;

			uiViewTemplateGameObject.AddComponent<GraphicRaycaster>();

			CanvasScaler canvasScaler = uiViewTemplateGameObject.AddComponent<CanvasScaler>();
			canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
			canvasScaler.referenceResolution = new Vector2(960, 640);

			CanvasAdapter canvasAdapter = uiViewTemplateGameObject.AddComponent<CanvasAdapter>();

			GameObject bgCoreGameObject = new GameObject("bg_core");
			RectTransform bgCoreRectTransform = bgCoreGameObject.AddComponent<RectTransform>();
			bgCoreRectTransform.SetParent(uiViewTemplateGameObject.transform, false);
			bgCoreRectTransform.localScale = Vector3.one;
			bgCoreRectTransform.localPosition = Vector3.zero;
			bgCoreRectTransform.localRotation = Quaternion.identity;
			bgCoreRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);
			bgCoreRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 640);
			bgCoreRectTransform.anchorMin = new Vector2(0, 0.5f);
			bgCoreRectTransform.anchorMax = new Vector2(1, 0.5f);

			GameObject coreGameObject = new GameObject("core");
			RectTransform coreRectTransform = coreGameObject.AddComponent<RectTransform>();
			coreRectTransform.SetParent(uiViewTemplateGameObject.transform, false);
			coreRectTransform.localScale = Vector3.one;
			coreRectTransform.localPosition = Vector3.zero;
			coreRectTransform.localRotation = Quaternion.identity;
			coreRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);
			coreRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 640);
			coreRectTransform.anchorMin = new Vector2(0, 0.5f);
			coreRectTransform.anchorMax = new Vector2(1, 0.5f);

            Common.Util.TransformUtil.SwitchLayer(uiViewTemplateRectTransform, (int)LayerType.UI);
		}
	}
}
