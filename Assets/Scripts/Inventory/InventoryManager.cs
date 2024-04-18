using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] GameObject inventoryPanel;
    [SerializeField] GridLayoutGroup inventoryGroup;

    Controls _controls = null;
    bool bIsInventoryOpen = false;

    private void AssignControls()
    {
        if (!_controls.MainGameplay.enabled)
        {
            _controls.MainGameplay.Enable();
        }

        _controls.MainGameplay.OpenBag.performed += ctx => ToggleInventory();
    }

    void Start()
    {
        _controls = new Controls();
        AssignControls();
        inventoryPanel.SetActive(false);
    }

    private void ToggleInventory()
    {
        bool tempIsOpen = !bIsInventoryOpen;

        if (tempIsOpen)
        {
            // An instance of inventory is already open! Don't override it.
            if (InventoryBackpackManager.Instance.bIsInventoryOpen)
                return;

            Characters.Player.PlayerManager.Instance.Camera.UnlockMouseCursor(true);
            InventoryBackpackManager.Instance.InstantiateGrid(inventoryGroup, inventoryGroup.gameObject.transform);
            StartCoroutine(RefreshViewport());
        }
        else
        {
            Characters.Player.PlayerManager.Instance.Camera.UnlockMouseCursor(false);
            InventoryBackpackManager.Instance.ClearGrid();
            inventoryPanel.SetActive(false);
        }
        bIsInventoryOpen = tempIsOpen;
    }

    private IEnumerator RefreshViewport()
    {
        inventoryGroup.enabled = false;
        yield return new WaitForEndOfFrame();
        inventoryGroup.enabled = true;
        inventoryPanel.SetActive(true);
    }

}
