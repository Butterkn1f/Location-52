using MainGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


public class Backpack : MonoBehaviour, IInteractable
{
    public bool IsInteractable => true;
    [SerializeField] GridLayoutGroup inventoryGroup;
    [SerializeField] Transform inventoryItemParent;
    [SerializeField] Transform cameraPos;
    [SerializeField] GameObject closeButton;

    [SerializeField] private UnityEvent _bagTutorialEvent;
    [SerializeField] private UnityEvent _QAMTutorialEvent;

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

        if (!MainGameManager.Instance.GetHasFinishedTutorial())
        {
            _bagTutorialEvent.Invoke();
        }

        closeButton.SetActive(true);
        Characters.Player.PlayerManager.Instance.Camera.LockCameraToPosition(cameraPos.gameObject);
        InventoryBackpackManager.Instance.InstantiateGrid(inventoryGroup, inventoryItemParent, false);
        PlayerUIManager.Instance.SetHideHUD(true, true);
        PlayerUIManager.Instance.ControlsManager.SetControlActive(ControlsType.Grid, true);
    }

    public void StopHover()
    {
    }

    public void ExitBackpack()
    {
        Characters.Player.PlayerManager.Instance.Camera.ReturnCamToNormal();
        InventoryBackpackManager.Instance.ClearGrid();
        closeButton.SetActive(false);
        PlayerUIManager.Instance.SetHideHUD(false, true);
        PlayerUIManager.Instance.ControlsManager.SetControlActive(ControlsType.Grid, false);

        if (!MainGameManager.Instance.GetHasFinishedTutorial())
        {
            PlayerUIManager.Instance.SetHideHUD(false);
            _QAMTutorialEvent.Invoke();
        }
    }
}
