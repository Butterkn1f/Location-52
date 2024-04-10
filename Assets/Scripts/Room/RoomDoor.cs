using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomDoor : MonoBehaviour,IInteractable
{
    public bool IsInteractable => true;

    public void Hover(Vector3 CameraPosition, Vector3 hitPoint)
    {
        Debug.Log("Hover on the door");
    }

    public void Interact(Vector3 CameraPosition, Vector3 hitPoint)
    {
        Debug.Log("Interacted with the door");
    }

    public void StopHover()
    {
    }
}
