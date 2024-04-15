using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(PhotoCapture))]
[RequireComponent(typeof(GalleryManager))]

public class CameraItem : Item
{
    [SerializeField] GameObject _cameraObject;
    [SerializeField] Vector3 zoomCameraPos = new Vector3(0, 0, 0.45f);
    [SerializeField] Volume globalVolume;

    PhotoCapture pc;
    GalleryManager gm;
    DepthOfField dof;
    float origFocalLength;
    Vector3 origCameraPos;

    bool IsADS = false;

    private void Start()
    {
        pc = GetComponent<PhotoCapture>();
        gm = GetComponent<GalleryManager>();

        origCameraPos = _cameraObject.transform.position;
        globalVolume.profile.TryGet(out dof);
        origFocalLength = dof.focalLength.value;
    }

    private void ToggleADS()
    {
        bool tempIsAds = !IsADS;

        if (tempIsAds)
            PlayZoomInAnimation();
        else
            PlayZoomOutAnimation();
    }

    private void PlayZoomInAnimation(System.Action onComplete = null)
    {
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
        gm.ToggleGallery(false);

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

    protected override void OnInactive()
    {
        if (!IsADS)
        {
            base.OnInactive();
            return;
        }

        // If currently ADS and swapped out of camera, we un-ADS first before setting inactive.
        PlayZoomOutAnimation(base.OnInactive);
    }

    protected override void AssignControls()
    {
        base.AssignControls();
        _controls.MainGameplay.ADSItem.performed += ctx => ToggleADS();
        _controls.MainGameplay.ViewAlbum.performed += ctx => {
            if (IsADS)
                gm.ToggleGallery();
        };
    }

    protected override void UseItem()
    {
        if (gm.bIsGalleryOpen)
            return;

        if (IsADS)
            pc.TakePhoto();
        else
            pc.ToggleFlash();
    }
}

