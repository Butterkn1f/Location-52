using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WalmartObject : AnomalyObject
{
    public override void OnPhotoTaken()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.parent.DOShakePosition(2.5f, new Vector3(10, 0, 5), 10, 90, false, true, ShakeRandomnessMode.Harmonic))
            .AppendInterval(0.5f)
            .Append(transform.parent.DOMoveY(40, 1.0f))
            .Append(transform.parent.DOScale(Vector3.zero, 1.0f).SetEase(Ease.InCubic))
            .Join(transform.parent.DOMoveY(42, 1.0f))
            .AppendCallback(() => transform.parent.gameObject.SetActive(false));
    }
}
