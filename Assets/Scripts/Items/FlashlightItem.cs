using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FlashlightItem : Item
{
    [SerializeField] GameObject flashlight;

    protected override void Start()
    {
        base.Start();
        flashlight.SetActive(false);
    }

    protected override void AssignControls()
    {
        base.AssignControls();
        _controls.MainGameplay.UseItem.performed += UseItem;
    }

    protected override void UnassignControls()
    {
        _controls.MainGameplay.UseItem.performed -= UseItem;
    }

    protected override void OnInactive()
    {
        base.OnInactive();
        flashlight.SetActive(false);
    }

    private void UseItem(InputAction.CallbackContext context)
    {
        flashlight.SetActive(!flashlight.activeSelf);
    }
}

