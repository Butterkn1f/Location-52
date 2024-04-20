using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class KisaragiObject : AnomalyObject
{
    public override void OnPhotoTaken()
    {
        transform.parent.gameObject.SetActive(false);
    }
}
