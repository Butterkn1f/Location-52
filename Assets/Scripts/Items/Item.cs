using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Child class that all items should derive from
public abstract class Item : MonoBehaviour
{
    protected Controls _controls = null;
    public bool IsActive { get; protected set; } = false;

    public ControlsType controlType = new();


    void Awake()
    {
        _controls = new Controls();
    }

    protected virtual void Start()
    {
        gameObject.SetActive(false);
        IsActive = false;
    }

    protected virtual void AssignControls()
    {
        if (!_controls.MainGameplay.enabled)
        {
            _controls.MainGameplay.Enable();
        }
    }

    protected abstract void UnassignControls();

    /// <summary>
    /// Called when user toggles this item.
    /// Sets isActive in this function too in case of animations
    /// Can be used for animation or to start a coroutine (eg a placing ghost for lures)
    /// </summary>
    protected virtual void OnActive()
    {
        IsActive = true;
        gameObject.SetActive(true);
        AssignControls();

        PlayerUIManager.Instance.ControlsManager.SetControlActive(controlType, true);
    }
    /// <summary>
    /// Called when user untoggles this item
    /// Sets isActive in this function too in case of animations
    /// </summary>
    protected virtual void OnInactive()
    {
        IsActive = false;
        gameObject.SetActive(false);
        UnassignControls();
        PlayerUIManager.Instance.ControlsManager.SetControlActive(controlType, false);
    }

    // Might be needed for future checks ? But for now this is already handled by animations
    /*protected virtual bool CheckCanChangeActiveState()
    {
        return true;
    }*/

    /// <summary>
    /// Toggles whether the current item is active
    /// </summary>
    /// <param name="isActive">Optional param to specifically set whether active, if left empty, will just toggle</param>
    public void ToggleIsActive(bool? isActive = null)
    {
        bool tempActive = isActive ?? !IsActive;
        if (IsActive == tempActive)
            return;

        if (tempActive)
            OnActive();
        else
            OnInactive();
    }
}
