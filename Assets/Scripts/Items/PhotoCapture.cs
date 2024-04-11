using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PhotoCapture : MonoBehaviour
{
    [SerializeField] RenderTexture spookCameraTexture; 
    [SerializeField] Image previewImage;
    [SerializeField] Image flashImage;
    [SerializeField] Light flash;

    Texture2D screenCapture;
    bool isViewingPhoto = false;
    float origFlashIntensity = 100;

    IEnumerator hidePreviewCoroutine;
    Sequence hidePhotoSequence;

    void Start()
    {
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
        else
        {
            StopCoroutine(hidePreviewCoroutine);
            RemovePhoto();
        }
    }

    private IEnumerator CapturePhoto()
    {
        yield return new WaitForEndOfFrame();
        RenderTexture.active = spookCameraTexture;
        screenCapture.ReadPixels(new Rect(0, 0, spookCameraTexture.width, spookCameraTexture.height), 0, 0);
        screenCapture.Apply();
        ShowPhoto();
    }

    private IEnumerator FlashEffect()
    {
        flash.intensity = origFlashIntensity;
        flash.gameObject.SetActive(true);
        flashImage.color = Color.white;

        yield return new WaitForSeconds(0.25f);
        StartCoroutine(CapturePhoto());

        float alpha = 1;
        DOTween.To(() => alpha, x => alpha = x, 0, 0.25f)
               .OnUpdate(() =>
               {
                   flash.intensity = origFlashIntensity * alpha;
                   flashImage.color = new Color(1, 1, 1, alpha);
               })
               .SetEase(Ease.InQuad)
               .OnComplete(() =>
               {
                   flash.gameObject.SetActive(false);
               });
    }

    private IEnumerator HidePhotoAfterTime()
    {
        yield return new WaitForSeconds(1.0f);
        hidePhotoSequence = DOTween.Sequence();
        hidePhotoSequence.Append(previewImage.rectTransform.DOScale(0.25f, 0.5f).SetEase(Ease.OutQuad))
            .Append(previewImage.rectTransform.DOAnchorPosX(0.25f, 0.25f))
            .OnComplete(RemovePhoto);
    }

    private void ShowPhoto()
    {
        isViewingPhoto = true;
        Sprite photoSprite = Sprite.Create(screenCapture, new Rect(0f, 0f, screenCapture.width, screenCapture.height), new Vector2(0.5f, 0.5f), 100f);
        previewImage.sprite = photoSprite;

        // Reset transforms
        previewImage.rectTransform.localScale = Vector3.one;
        previewImage.rectTransform.DOAnchorPosX(0, 0);
        previewImage.gameObject.SetActive(true);

        hidePreviewCoroutine = HidePhotoAfterTime();
        StartCoroutine(hidePreviewCoroutine);

        // TODO save to temp folder here
        /*string saveName = Application.persistentDataPath + "TestImage.png";
        byte[] bytes = screenCapture.EncodeToPNG();
        System.IO.File.WriteAllBytes(saveName, bytes);
        while (!System.IO.File.Exists(saveName)) yield return null;
        Debug.Log("File written to " + saveName);*/
    }

    private void RemovePhoto()
    {
        previewImage.gameObject.SetActive(false);
        isViewingPhoto = false;
    }
}
