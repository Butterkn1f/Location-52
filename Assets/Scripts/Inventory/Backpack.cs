using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Backpack : MonoBehaviour, IInteractable
{
    public bool IsInteractable => true;
    [SerializeField] GridLayoutGroup inventoryGroup;
    [SerializeField] Transform inventoryItemParent;
    [SerializeField] Transform cameraPos;
    [SerializeField] GameObject closeButton;

    private void Start()
    {
        closeButton.SetActive(false);
    }

    public void Hover(Vector3 CameraPosition, Vector3 hitPoint)
    {
        Debug.Log("Hover on backpack");
    }

    public void Interact(Vector3 CameraPosition, Vector3 hitPoint)
    {
        Debug.Log("Interacted with backpack");
        // An instance of inventory is already open! Don't override it.
        if (InventoryBackpackManager.Instance.bIsInventoryOpen)
            return;

        closeButton.SetActive(true);
        Characters.Player.PlayerManager.Instance.Camera.LockCameraToPosition(cameraPos.gameObject);
        InventoryBackpackManager.Instance.InstantiateGrid(inventoryGroup, inventoryItemParent, false);
    }

    public void StopHover()
    {
    }

    public void ExitBackpack()
    {
        Characters.Player.PlayerManager.Instance.Camera.ReturnCamToNormal();
        InventoryBackpackManager.Instance.ClearGrid();
        closeButton.SetActive(false);
    }
}
