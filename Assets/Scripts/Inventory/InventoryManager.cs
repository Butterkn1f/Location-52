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
    
    private void Awake()
    {
        _controls = new Controls();
        AssignControls();
    }

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
        inventoryPanel.SetActive(false);
    }

    private void ToggleInventory()
    {
        bIsInventoryOpen = !bIsInventoryOpen;

        if (bIsInventoryOpen)
        {
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
    }

    private IEnumerator RefreshViewport()
    {
        inventoryGroup.enabled = false;
        yield return new WaitForEndOfFrame();
        inventoryGroup.enabled = true;
        inventoryPanel.SetActive(true);
    }

}
