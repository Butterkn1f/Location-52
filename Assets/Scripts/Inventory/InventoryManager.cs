using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

[System.Serializable]
public struct InventoryItemInfo
{
    public InventoryItemType ItemType;
    public Sprite Sprite;
    public string Name;
    public string Description;
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
    [SerializeField] TMPro.TMP_Text infoTitle;
    [SerializeField] TMPro.TMP_Text infoDesc;

    Dictionary<BackpackSlot, InventoryItemType?> EquippedSlots = new();
    Controls _controls = null;
    bool bIsInventoryOpen = false;

    public CircleSlot HoveredCircleSlot { get; private set; } = null;
    private CircleSlot EmptyPreviewSlot = null;
    private CircleSlot ToggledCircleSlot = null;

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

        foreach (BackpackSlot slot in System.Enum.GetValues(typeof(BackpackSlot)))
        {
            EquippedSlots.Add(slot, null);
        }

        foreach (var circleSlot in circleSlots)
        {
            circleSlot.OnHover.AddListener(() => OnPointerEnterSlot(circleSlot));
            circleSlot.OnUnhover.AddListener(() => OnPointerExitSlot(circleSlot));
        }
    }

    private void ToggleInventory()
    {
        bool tempIsOpen = !bIsInventoryOpen;

        if (tempIsOpen)
        {
            // An instance of inventory is already open! Don't override it.
            if (InventoryBackpackManager.Instance.bIsInventoryOpen)
                return;

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
