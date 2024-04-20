using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DuckObject : AnomalyObject
{
    public override void OnPhotoTaken()
    {
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(2.0f)
            .Append(transform.parent.DOShakeRotation(4.0f, 10, 5, 30, true, ShakeRandomnessMode.Harmonic))
            .Join(transform.parent.DOMoveY(-6, 4.0f)
            .OnComplete(() => gameObject.SetActive(false)));
    }
}
