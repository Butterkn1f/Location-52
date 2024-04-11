using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(PhotoCapture))]

public class CameraItem : Item
{
    [SerializeField] GameObject _cameraObject;
    [SerializeField] Vector3 zoomCameraPos = new Vector3(0, 0, 0.45f);
    [SerializeField] Volume globalVolume;

    PhotoCapture pc;
    DepthOfField dof;
    float origFocalLength;
    Vector3 origCameraPos;

    private void Start()
    {
        pc = GetComponent<PhotoCapture>();

        origCameraPos = _cameraObject.transform.position;
        globalVolume.profile.TryGet(out dof);
        origFocalLength = dof.focalLength.value;
    }

    protected override void SetToggleAction()
    {
        toggleInput = _controls.MainGameplay.ToggleCamera;
    }

    protected override void OnActive()
    {
        float focalLength = dof.focalLength.value;
        var seq = DOTween.Sequence();

        seq.Append(DOTween.To(() => focalLength, x => focalLength = x, 300, 0.5f)
                .OnUpdate(() => dof.focalLength.value = focalLength)
                .SetEase(Ease.InQuad)
            )
            .Join(_cameraObject.transform.DOLocalMove(zoomCameraPos, 0.5f))
            .OnComplete(base.OnActive);
    }

    protected override void OnInactive()
    {
        float focalLength = dof.focalLength.value;
        var seq = DOTween.Sequence();

        seq.Append(DOTween.To(() => focalLength, x => focalLength = x, origFocalLength, 0.5f)
                .OnUpdate(() => dof.focalLength.value = focalLength)
                .SetEase(Ease.OutQuad)
            )
            .Join(_cameraObject.transform.DOLocalMove(origCameraPos, 0.5f))
            .OnComplete(base.OnInactive);
    }

    // TODO: Move to separate PhotoCapture class
    protected override void UseItem()
    {
        pc.TakePhoto();
    }
}

