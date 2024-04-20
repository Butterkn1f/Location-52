using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GazeboObject : AnomalyObject
{
    public Material tvGlow;

    private void Start()
    {
        var currColor = tvGlow.GetColor("_EmissionColor");
        tvGlow.SetColor("_EmissionColor", new Color(currColor.r, currColor.g, currColor.b, 3));
    }
    public override void OnPhotoTaken()
    {
        var currColor = tvGlow.GetColor("_EmissionColor");
        tvGlow.SetColor("_EmissionColor", new Color(currColor.r, currColor.g, currColor.b, -5));
        foreach (Transform child in transform.parent)
        {
            child.gameObject.layer = LayerMask.NameToLayer("Default");
            foreach (Transform childsChild in child.transform)
            {
                childsChild.gameObject.layer = LayerMask.NameToLayer("Default");
            }
        }
    }
}
