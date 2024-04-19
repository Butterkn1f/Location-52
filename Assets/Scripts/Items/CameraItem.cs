using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(PhotoCapture))]
[RequireComponent(typeof(GalleryManager))]

public class CameraItem : Item
{
    [SerializeField] GameObject _cameraObject;
    [SerializeField] Vector3 zoomCameraPos = new Vector3(0, 0, 0.45f);

    [SerializeField] Slider batterySlider;
    [SerializeField] TMPro.TMP_Text batteryText;
    [SerializeField] GameObject NoBatteryObject;
    [SerializeField] Image NoBatteryImage;
    Sequence batteryYoYo;
    IEnumerator batteryDrainCoroutine;

    PhotoCapture pc;
    GalleryManager gm;
    DepthOfField dof;
    
    float origFocalLength = 1.0f;
    Vector3 origCameraPos;

    bool IsADS = false;

    float batteryPercent = 1.0f;

    protected override void Start()
    {
        base.Start();
        pc = GetComponent<PhotoCapture>();
        gm = GetComponent<GalleryManager>();
        batteryPercent = 1.0f;

        origCameraPos = _cameraObject.transform.localPosition;
        GameObject dofVolume = GameObject.FindGameObjectWithTag("DOFVolume");
        if (dofVolume != null)
        {
            dofVolume.GetComponent<Volume>().profile.TryGet(out dof);
            origFocalLength = dof.focalLength.value;
        }
        else
        {
            Debug.LogWarning("Add a DOF volume to this scene! For the camera's fading out effect when ADS.");
        }
    }

    private void ToggleADS(InputAction.CallbackContext context)
    {
        bool tempIsAds = !IsADS;

        if (tempIsAds)
            PlayZoomInAnimation();
        else
            PlayZoomOutAnimation();
    }

    private void PlayZoomInAnimation(System.Action onComplete = null)
    {
        if (!_cameraObject)
            return;

        PlayerUIManager.Instance.SetHideHUD(true, true);
        PlayerUIManager.Instance.ControlsManager.SetControlActive(ControlsType.CameraAds, true);

        float focalLength = dof.focalLength.value;
        var seq = DOTween.Sequence();

        seq.Append(DOTween.To(() => focalLength, x => focalLength = x, 300, 0.5f)
                .OnUpdate(() => dof.focalLength.value = focalLength)
                .SetEase(Ease.InQuad)
            )
            .Join(_cameraObject.transform.DOLocalMove(zoomCameraPos, 0.5f))
            .OnComplete(() => {
                onComplete?.Invoke();
                IsADS = true;
            });
    }

    private void PlayZoomOutAnimation(System.Action onComplete = null)
    {
        if (!_cameraObject)
            return;

        gm.ToggleGallery(false);
        PlayerUIManager.Instance.SetHideHUD(false, true);
        PlayerUIManager.Instance.ControlsManager.SetControlActive(ControlsType.CameraAds, false);

        float focalLength = dof.focalLength.value;
        var seq = DOTween.Sequence();

        seq.Append(DOTween.To(() => focalLength, x => focalLength = x, origFocalLength, 0.5f)
                .OnUpdate(() => dof.focalLength.value = focalLength)
                .SetEase(Ease.OutQuad)
            )
            .Join(_cameraObject.transform.DOLocalMove(origCameraPos, 0.5f))
            .OnComplete(() => {
                onComplete?.Invoke();
                IsADS = false;
            });
    }

    private IEnumerator DrainBattery()
    {
        while (batteryPercent > 0)
        {
            batteryPercent = Mathf.Clamp(batteryPercent - (pc.IsPermanentFlash ? 0.05f : 0.01f), 0, 1);
            batteryText.text = Mathf.FloorToInt(batteryPercent * 100f).ToString() + "%";
            batterySlider.value = batteryPercent;
            yield return new WaitForSeconds(1f);
        }

        batteryPercent = 0;
        SetShowNoBattery(true);
    }

    protected override void OnActive()
    {
        base.OnActive();

        if (batteryPercent > 0)
        {
            batteryDrainCoroutine = DrainBattery();
            StartCoroutine(batteryDrainCoroutine);
        }
    }

    protected override void OnInactive()
    {
        if (!IsADS)
        {
            base.OnInactive();
            StopCoroutine(batteryDrainCoroutine);
            return;
        }

        // If currently ADS and swapped out of camera, we un-ADS first before setting inactive.
        PlayZoomOutAnimation(() => {
            base.OnInactive();
            StopCoroutine(batteryDrainCoroutine);
        });
    }

    protected override void AssignControls()
    {
        base.AssignControls();
        _controls.MainGameplay.ADSItem.performed += ToggleADS;
        _controls.MainGameplay.ViewAlbum.performed += ToggleGallery;

        _controls.MainGameplay.ToggleFlash.performed += ToggleFlash;
        _controls.MainGameplay.UseItem.performed += UseItem;
    }

    protected override void UnassignControls()
    {
        _controls.MainGameplay.ADSItem.performed -= ToggleADS;
        _controls.MainGameplay.ViewAlbum.performed -= ToggleGallery;

        _controls.MainGameplay.ToggleFlash.performed -= ToggleFlash;
        _controls.MainGameplay.UseItem.performed -= UseItem;
    }

    private void UseItem(InputAction.CallbackContext context)
    {
        if (gm.bIsGalleryOpen || batteryPercent <= 0)
            return;

        if (IsADS)
            pc.TakePhoto();
    }

    private void ToggleGallery(InputAction.CallbackContext context)
    {
        if (IsADS && batteryPercent > 0)
            gm.ToggleGallery();
    }

    private void ToggleFlash(InputAction.CallbackContext context)
    {
        if (batteryPercent > 0)
            pc.ToggleFlash();
    }

    public void SetShowNoBattery(bool show)
    {
        batteryYoYo?.Kill();
        NoBatteryObject.SetActive(show);

        if (show)
        {
            pc.IsPermanentFlash = false;
            pc.ToggleFlash(false);
            NoBatteryImage.color = Color.white;
            batteryYoYo = DOTween.Sequence();
            batteryYoYo.Join(NoBatteryImage.DOFade(0.1f, 0.5f))
            .SetLoops(-1, LoopType.Yoyo);
        }
    }
}

