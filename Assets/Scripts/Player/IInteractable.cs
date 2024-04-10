using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for all interactable items
/// </summary>
public interface IInteractable
{
    public bool IsInteractable { get; }

    /// <summary>
    /// For when you look at an item which is interactable
    /// </summary>
    public void Hover(Vector3 CameraPosition, Vector3 hitPoint);
    /// <summary>
    /// Stop looking
    /// </summary>
    public void StopHover();

    /// <summary>
    /// When you click interact
    /// </summary>
    public void Interact(Vector3 CameraPosition, Vector3 hitPoint);


}