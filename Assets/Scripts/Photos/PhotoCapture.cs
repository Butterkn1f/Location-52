using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(PhotoAnomalyChecker))]
public class PhotoCapture : MonoBehaviour
{
    [SerializeField] RenderTexture spookCameraTexture; 
    [SerializeField] Image previewImage;
    [SerializeField] Image flashImage;
    [SerializeField] Light flash;

    PhotoAnomalyChecker anomalyChecker;

    Texture2D screenCapture;
    bool isViewingPhoto = false;
    float origFlashIntensity = 100;

    bool IsPermanentFlash = false;

    void Start()
    {
        anomalyChecker = GetComponent<PhotoAnomalyChecker>();

        origFlashIntensity = flash.intensity;
        previewImage.gameObject.SetActive(false);
        screenCapture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false, true);
    }

    public void TakePhoto()
    {
        if (!isViewingPhoto)
        {
            StartCoroutine(FlashEffect());
        }
    }

    public void ToggleFlash(bool? isOn = null)
    {
        if (isOn.HasValue && IsPermanentFlash)
            return;

        if (!isOn.HasValue)
            IsPermanentFlash = !IsPermanentFlash;

        bool flashOn = isOn ?? IsPermanentFlash;
        if (flashOn)
            flash.intensity = origFlashIntensity;

        flash.gameObject.SetActive(flashOn);
    }

    private IEnumerator CapturePhoto()
    {
        yield return new WaitForEndOfFrame();
        RenderTexture.active = spookCameraTexture;
        screenCapture.ReadPixels(new Rect(0, 0, spookCameraTexture.width, spookCameraTexture.height), 0, 0);
        screenCapture.Apply();

        Sprite photoSprite = Sprite.Create(screenCapture, new Rect(0f, 0f, screenCapture.width, screenCapture.height), new Vector2(0.5f, 0.5f), 100f);
        ShowPhoto(photoSprite);

        var anomalyCaught = anomalyChecker.CheckAnomaliesInView(out var pointsDetected);
        PhotoManager.Instance.AddPhoto(screenCapture, anomalyCaught, pointsDetected);
    }

    private IEnumerator FlashEffect()
    {
        ToggleFlash(true);
        flashImage.color = Color.white;


        yield return new WaitForSeconds(0.25f);
        StartCoroutine(CapturePhoto());

        float alpha = 1;
        DOTween.To(() => alpha, x => alpha = x, 0, 0.25f)
               .OnUpdate(() =>
               {
                   flashImage.color = new Color(1, 1, 1, alpha);

                   if (!IsPermanentFlash)
                    flash.intensity = origFlashIntensity * alpha;
               })
               .SetEase(Ease.InQuad)
               .OnComplete(() =>
               {
                   ToggleFlash(false);
               });
    }

    private IEnumerator HidePhotoAfterTime()
    {
        yield return new WaitForSeconds(1.0f);
        var hidePhotoSequence = DOTween.Sequence();
        hidePhotoSequence.Append(previewImage.rectTransform.DOScale(0.25f, 0.5f).SetEase(Ease.OutQuad))
            .Append(previewImage.rectTransform.DOAnchorPosX(Screen.width * 0.25f, 0.25f))
            .OnComplete(RemovePhoto);
    }

    private void ShowPhoto(Sprite photoSprite)
    {
        isViewingPhoto = true;
        previewImage.sprite = photoSprite;

        // Reset transforms
        previewImage.rectTransform.localScale = Vector3.one;
        previewImage.rectTransform.DOAnchorPosX(0, 0);
        previewImage.gameObject.SetActive(true);

        StartCoroutine(HidePhotoAfterTime());
    }

    private void RemovePhoto()
    {
        previewImage.gameObject.SetActive(false);
        isViewingPhoto = false;
    }
}
