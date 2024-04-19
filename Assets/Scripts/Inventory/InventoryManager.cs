using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.InputSystem;
using UniRx;
using TMPro;

[System.Serializable]
public struct InventoryItemInfo
{
    public InventoryItemType ItemType;
    public Sprite Sprite;
    public string Name;
    public string Description;
    public Item Item;
}

[System.Serializable]
public struct InventoryHUDBagPart
{
    public BackpackSlot Slot;
    public GameObject Part;
}

public class InventoryManager : Common.DesignPatterns.Singleton<InventoryManager>
{
    [SerializeField] GameObject inventoryPanel;
    [SerializeField] GridLayoutGroup inventoryGroup;
    [SerializeField] List<CircleSlot> circleSlots;
    [SerializeField] List<InventoryItemInfo> inventoryItemInfo = new();

    [Header("Info Panel")]
    [SerializeField] CanvasGroup infoPanel;
    [SerializeField] Image infoSprite;
    [SerializeField] TMP_Text infoTitle;
    [SerializeField] TMP_Text infoDesc;

    [Header("HUD")]
    [SerializeField] Image hudSlotSprite;
    [SerializeField] GameObject hudEmptyText;
    [SerializeField] List<InventoryHUDBagPart> hudBagParts = new();
    [SerializeField] TMP_Text hudTitle;

    Dictionary<BackpackSlot, InventoryItemType?> EquippedSlots = new();
    Controls _controls = null;
    bool bIsInventoryOpen = false;

    public ReactiveProp<BackpackSlot> CurrentSlot { get; private set; } = new ReactiveProp<BackpackSlot>();
    public CircleSlot HoveredCircleSlot { get; private set; } = null;
    private CircleSlot EmptyPreviewSlot = null;
    private CircleSlot ToggledCircleSlot = null;

    void Start()
    {
        _controls = new Controls();
        AssignControls();
        inventoryPanel.SetActive(false);

        foreach (BackpackSlot slot in System.Enum.GetValues(typeof(BackpackSlot)))
        {
            EquippedSlots.Add(slot, null);
        }

        foreach (var circleSlot in circleSlots)
        {
            circleSlot.OnHover.AddListener(() => OnPointerEnterSlot(circleSlot));
            circleSlot.OnUnhover.AddListener(() => OnPointerExitSlot(circleSlot));
        }

        CurrentSlot.GetObservable()
            .Subscribe(_ => UpdateHUDSlot());
        EquippedSlots.ObserveEveryValueChanged(x => x[CurrentSlot.GetValue()])
            .Subscribe(_ => UpdateHUDSlot());


        CurrentSlot.SetValue(BackpackSlot.Top);
    }

    private void AssignControls()
    {
        if (!_controls.MainGameplay.enabled)
        {
            _controls.MainGameplay.Enable();
        }

        _controls.MainGameplay.OpenBag.performed += ctx => ToggleInventory();
        SetInventorySwapEnabled(true);
    }

    private void ScrollSlots(InputAction.CallbackContext context)
    {
        bool bIncrease = _controls.MainGameplay.Scroll.ReadValue<Vector2>().y > 0;

        var prevPart = hudBagParts.FirstOrDefault(x => x.Slot == CurrentSlot.GetValue()).Part;
        if (prevPart)
            prevPart.SetActive(false);

        var prevItemType = EquippedSlots[CurrentSlot.GetValue()];
        if (prevItemType != null)
        {
            var prevItem = inventoryItemInfo.FirstOrDefault(x => x.ItemType == prevItemType.Value);
            if (prevItem.Item)
                prevItem.Item.ToggleIsActive(false);
        }

        var newSlot = CurrentSlot.GetValue() switch
        {
            BackpackSlot.Top => bIncrease ? BackpackSlot.Front : BackpackSlot.Side,
            BackpackSlot.Front => bIncrease ? BackpackSlot.Side : BackpackSlot.Top,
            BackpackSlot.Side => bIncrease ? BackpackSlot.Top : BackpackSlot.Front,
            _ => BackpackSlot.Top
        };

        CurrentSlot.SetValue(newSlot);
    }

    private void UpdateHUDSlot()
    {
        var currentItem = EquippedSlots[CurrentSlot.GetValue()];
        if (currentItem == null || !currentItem.HasValue)
        {
            hudEmptyText.SetActive(true);
            hudSlotSprite.gameObject.SetActive(false);
            hudTitle.text = "Nothing";
        }
        else
        {
            var currInfo = inventoryItemInfo.FirstOrDefault(x => x.ItemType == currentItem.Value);
            hudSlotSprite.sprite = currInfo.Sprite;
            hudTitle.text = currInfo.Name;
            hudSlotSprite.gameObject.SetActive(true);
            hudEmptyText.SetActive(false);
            currInfo.Item.ToggleIsActive(true);
        }

        var newPart = hudBagParts.FirstOrDefault(x => x.Slot == CurrentSlot.GetValue()).Part;
        if (newPart)
            newPart.SetActive(true);
    }

    public void SetInventorySwapEnabled(bool enabled)
    {
        if (enabled)
        {
            _controls.MainGameplay.Scroll.performed += ScrollSlots;
        }
        else
        {
            _controls.MainGameplay.Scroll.performed -= ScrollSlots;
        }
    }

    private void ToggleInventory()
    {
        bool tempIsOpen = !bIsInventoryOpen;

        if (tempIsOpen)
        {
            // An instance of inventory is already open! Don't override it.
            // OR another UI instance is currently open (such as camera), so HUD is already hdiden. In this case, don't open inventory on top of it!
            if (InventoryBackpackManager.Instance.bIsInventoryOpen || PlayerUIManager.Instance.bIsHUDHidden)
                return;

            PlayerUIManager.Instance.SetHideHUD(true, true);
            PlayerUIManager.Instance.ControlsManager.SetControlActive(ControlsType.Grid, true);
            PlayerUIManager.Instance.ControlsManager.SetControlActive(ControlsType.GridBag, true);

            infoPanel.alpha = 0;
            if (ToggledCircleSlot)
                ToggledCircleSlot.SetSlotToggled(false);
            HoveredCircleSlot = EmptyPreviewSlot = ToggledCircleSlot = null;

            Characters.Player.PlayerManager.Instance.Camera.UnlockMouseCursor(true);
            InventoryBackpackManager.Instance.InstantiateGrid(inventoryGroup, inventoryGroup.gameObject.transform);
            InstantiateSlots();
            StartCoroutine(RefreshViewport());
        }
        else
        {
            PlayerUIManager.Instance.SetHideHUD(false, true);
            PlayerUIManager.Instance.ControlsManager.SetControlActive(ControlsType.Grid, false);
            PlayerUIManager.Instance.ControlsManager.SetControlActive(ControlsType.GridBag, false);
            Characters.Player.PlayerManager.Instance.Camera.UnlockMouseCursor(false);
            InventoryBackpackManager.Instance.ClearGrid();
            inventoryPanel.SetActive(false);
        }
        bIsInventoryOpen = tempIsOpen;
    }

    private void InstantiateSlots()
    {
        foreach (var slot in EquippedSlots)
        {
            var circleSlot = circleSlots.FirstOrDefault(x => x.Slot == slot.Key);
            if (circleSlot == null)
                break;

            if (slot.Value.HasValue)
            {
                circleSlot.PutItemInSlot(slot.Value.Value);
            }
            else
            {
                circleSlot.EmptyItemInSlot();
            }
        }
    }

    private IEnumerator RefreshViewport()
    {
        inventoryGroup.enabled = false;
        yield return new WaitForEndOfFrame();
        inventoryGroup.enabled = true;
        inventoryPanel.SetActive(true);
    }

    public void UpdateCurrSlot(InventoryItemType? type)
    {
        if (!HoveredCircleSlot)
            return;

        EquippedSlots[HoveredCircleSlot.Slot] = type;

        if (type.HasValue)
        {
            foreach (var slot in circleSlots)
            {
                if (slot.ItemTypeInSlot == type.Value)
                {
                    slot.EmptyItemInSlot();
                    EquippedSlots[slot.Slot] = null;
                    break;
                }
            }
            HoveredCircleSlot.PutItemInSlot(type.Value);

        }
        else
        {
            HoveredCircleSlot.EmptyItemInSlot();
        }
    }

    public void PreviewCurrentSlot(InventoryItemType type)
    {
        if (!HoveredCircleSlot)
            return;

        HoveredCircleSlot.PreviewItemInSlot(type);
        EmptyPreviewSlot = null;
        foreach (var slot in circleSlots)
        {
            if (slot.ItemTypeInSlot == type)
            {
                EmptyPreviewSlot = slot;
                slot.PreviewEmptyItem();
                break;
            }
        }
    }

    public void StopPreviewingSlot(CircleSlot slot)
    {
        if (EmptyPreviewSlot)
        {
            EmptyPreviewSlot.StopPreviewItemInSlot();
            EmptyPreviewSlot = null;
        }

        slot.StopPreviewItemInSlot();
    }

    public void ToggleCircleSlot(CircleSlot slot)
    {
        if (slot.ItemTypeInSlot == null)
            return;

        if (ToggledCircleSlot != null && ToggledCircleSlot != slot)
        {
            ToggledCircleSlot.SetSlotToggled(false);
        }

        ToggledCircleSlot = slot;
        ToggledCircleSlot.SetSlotToggled(true);

        infoPanel.alpha = 1;
        var info = inventoryItemInfo.FirstOrDefault(x => x.ItemType == slot.ItemTypeInSlot);
        infoSprite.sprite = info.Sprite;
        infoTitle.text = info.Name;
        infoDesc.text = info.Description;
    }

    public void RemoveCurrentToggle()
    {
        ToggledCircleSlot.SetSlotToggled(false);
        ToggledCircleSlot = null;
        infoPanel.alpha = 0;
    }

    public void CheckShouldRemoveSlot(InventoryItemType type)
    {
        foreach (var slot in circleSlots)
        {
            if (slot.ItemTypeInSlot == type)
            {
                slot.EmptyItemInSlot();
                EquippedSlots[slot.Slot] = null;
                break;
            }
        }
    }

    public bool CheckInventoryHasCamera()
    {
        foreach (var slot in EquippedSlots)
        {
            if (slot.Value == InventoryItemType.Camera)
                return true;
        }

        return false;
    }

    public Sprite GetSpriteFromItemType(InventoryItemType type)
    {
        return inventoryItemInfo.FirstOrDefault(x => x.ItemType == type).Sprite;
    }

    public void OnPointerEnterSlot(CircleSlot slot)
    {
        HoveredCircleSlot = slot;
    }

    public void OnPointerExitSlot(CircleSlot slot)
    {
        if (HoveredCircleSlot == slot)
            HoveredCircleSlot = null;
    }
}
