using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Child class that all items should derive from
public abstract class Item : MonoBehaviour
{
    protected Controls _controls = null;
    public bool IsActive { get; private set; } = true; // TODO: Set this to false default and implement inventory sys


    void Awake()
    {
        _controls = new Controls();
        AssignControls();
    }

    protected virtual void AssignControls()
    {
        if (!_controls.MainGameplay.enabled)
        {
            _controls.MainGameplay.Enable();
        }

        _controls.MainGameplay.UseItem.performed += ctx => {
            if (!IsActive)
                return;

            UseItem();
        };
    }

    /// <summary>
    /// Called when user left clicks while item is active
    /// </summary>
    protected abstract void UseItem();

    /// <summary>
    /// Called when user toggles this item.
    /// Sets isActive in this function too in case of animations
    /// Can be used for animation or to start a coroutine (eg a placing ghost for lures)
    /// </summary>
    protected virtual void OnActive()
    {
        IsActive = true;
    }
    /// <summary>
    /// Called when user untoggles this item
    /// Sets isActive in this function too in case of animations
    /// </summary>
    protected virtual void OnInactive()
    {
        IsActive = false;
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
