using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class TutorialMask : MonoBehaviour
{
	private Image _image;
	private Texture _maskTexture;
	private float _maskScale = 1.0f;

	private Vector2 _indicateLocalPosition = Vector2.zero;

	void Awake ()
	{
		_image = GetComponent<Image>();
		_image.material = new Material(_image.material);
		_maskTexture = _image.material.GetTexture("_MaskTex");
		_image.material.SetColor("_Color", new Color(0, 0, 0, 0));
		Show();
	}

	void Start ()
	{
//		Invoke("Show", 1f);
//		LeanTween.delayedCall(1f, Show);
	}

	void OnDestroy ()
	{
//		CancelInvoke("Show");
		LeanTween.cancel(gameObject);
	}

	void Show ()
	{
		ZoomIn();
		FadeIn();		
	}

	private void ZoomIn ()
	{
		LTDescr ltDescr = LeanTween.value(gameObject, 20f, 1f, 0.6f);
		ltDescr.tweenType = LeanTweenType.easeOutQuad;
		ltDescr.setIgnoreTimeScale(true);
		ltDescr.setOnUpdate(OnMaskScaleUpdate);
		ltDescr.setOnComplete(OnMaskScaleComplete);
	}

	private void FadeIn ()
	{
		LTDescr fadeLTdescr = LeanTween.value(gameObject, 0f, 1f, 0.6f);
		fadeLTdescr.tweenType = LeanTweenType.easeOutQuad;
		fadeLTdescr.setIgnoreTimeScale(true);
		fadeLTdescr.setOnUpdate(OnMaskAlphaUpdate);
		fadeLTdescr.setOnComplete(OnFadeInComplete);
	}

	private void FadeOut ()
	{
		LTDescr fadeLTdescr = LeanTween.value(gameObject, 1f, 0f, 1.6f);
		fadeLTdescr.setIgnoreTimeScale(true);
		fadeLTdescr.setOnUpdate(OnMaskAlphaUpdate);
		fadeLTdescr.setOnComplete(OnFadeOutComplete);
	}

	private void OnMaskScaleUpdate (float scale)
	{
		_maskScale = 1.0f / scale;
		
		Vector2 imageSize = _image.rectTransform.rect.size;
		float offsetX = -((_indicateLocalPosition.x / 100) * _maskScale + (imageSize.x * _maskScale) / (100 * 2) - (float)_maskTexture.width / (100 * 2));
		float offsetY = -((_indicateLocalPosition.y / 100) * _maskScale + (imageSize.y * _maskScale) / (100 * 2) - (float)_maskTexture.height / (100 * 2));
		Vector2 offset = new Vector2(offsetX, offsetY);
		_image.material.SetTextureOffset("_MaskTex", offset);
		
		float tilingX = 6.4f * imageSize.x / imageSize.y;
		float tilingY = 6.4f;
		tilingX *= _maskScale;
		tilingY *= _maskScale;
		_image.material.SetTextureScale("_MaskTex", new Vector2(tilingX, tilingY));
	}

	private void OnMaskScaleComplete ()
	{
	}

	private void OnMaskAlphaUpdate (float alpha)
	{
		_image.material.SetColor("_Color", new Color(0, 0, 0, alpha));
	}

	private void OnFadeInComplete ()
	{
		FadeOut();
	}

	private void OnFadeOutComplete ()
	{
		Show();
	}

	public void SetIndicateLocalPosition (Vector3 indicateLocalPosition)
	{
		_indicateLocalPosition = indicateLocalPosition;
	}
}