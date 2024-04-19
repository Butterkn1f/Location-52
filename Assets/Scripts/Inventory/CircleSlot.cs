using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;

public class CircleSlot : MonoBehaviour
{
    [SerializeField] Image ItemImage;
    [SerializeField] GameObject EmptyText;
    [SerializeField] Button ButtonRemove;
    [SerializeField] List<Image> FilledSprites;
    [HideInInspector] public UnityEvent OnHover = new UnityEvent();
    [HideInInspector] public UnityEvent OnUnhover = new UnityEvent();

    Button button;
    Color origNormalColor;

    bool isPreviewing = false;

    public BackpackSlot Slot;
    public InventoryItemType? ItemTypeInSlot { get; private set; } = null;
    Sequence hoverYoYo;

    private void Start()
    {
        button = GetComponent<Button>();
        origNormalColor = button.colors.normalColor;
        button.onClick.AddListener(() => InventoryManager.Instance.ToggleCircleSlot(this));

        ButtonRemove.onClick.AddListener(() =>
        {
            EmptyItemInSlot();
            InventoryManager.Instance.RemoveCurrentToggle();
        });
    }

    public void PutItemInSlot(InventoryItemType item)
    {
        hoverYoYo?.Kill();
        ItemTypeInSlot = item;
        isPreviewing = false;

        ItemImage.sprite = InventoryManager.Instance.GetSpriteFromItemType(item);
        ItemImage.DOFade(1, 0);

        EmptyText.SetActive(false);
        ItemImage.gameObject.SetActive(true);
        foreach (var sprite in FilledSprites)
        {
            sprite.DOFade(1, 0);
            sprite.gameObject.SetActive(true);
        }
    }

    public void EmptyItemInSlot()
    {
        hoverYoYo?.Kill();
        EmptyText.GetComponent<TMPro.TMP_Text>().DOFade(1, 0);
        isPreviewing = false;

        ItemTypeInSlot = null;
        ItemImage.sprite = null;
        ItemImage.DOFade(0, 0);

        EmptyText.SetActive(true);
        ItemImage.gameObject.SetActive(false);
        foreach (var sprite in FilledSprites)
        {
            sprite.DOFade(1, 0);
            sprite.gameObject.SetActive(false);
        }
    }

    public void PreviewItemInSlot(InventoryItemType item)
    {
        if (isPreviewing)
            return;

        isPreviewing = true;
        EmptyText.SetActive(false);
        ItemImage.sprite = InventoryManager.Instance.GetSpriteFromItemType(item);
        ItemImage.gameObject.SetActive(true);
        ItemImage.DOFade(1.0f, 0);

        hoverYoYo.Kill();
        hoverYoYo = DOTween.Sequence();
        hoverYoYo.Join(ItemImage.DOFade(0.3f, 0.8f));
        foreach (var sprite in FilledSprites)
        {
            sprite.gameObject.SetActive(true);
            sprite.color = Color.white;
            hoverYoYo.Join(sprite.DOFade(0.3f, 0.8f));
        }
        hoverYoYo.SetLoops(-1, LoopType.Yoyo);
    }

    public void PreviewEmptyItem()
    {
        if (isPreviewing)
            return;

        isPreviewing = true;
        ItemImage.DOFade(0, 0);
        EmptyText.SetActive(true);

        foreach (var sprite in FilledSprites)
        {
            sprite.DOFade(0, 0);
        }

        hoverYoYo.Kill();
        hoverYoYo = DOTween.Sequence();
        hoverYoYo.Join(EmptyText.GetComponent<TMPro.TMP_Text>().DOFade(0.3f, 0.8f));
        hoverYoYo.SetLoops(-1, LoopType.Yoyo);
    }

    public void StopPreviewItemInSlot()
    {
        isPreviewing = false;
        hoverYoYo.Kill();
        EmptyText.GetComponent<TMPro.TMP_Text>().DOFade(1, 0);

        foreach (var sprite in FilledSprites)
        {
            sprite.color = Color.white;
        }

        if (ItemTypeInSlot.HasValue)
        {
            PutItemInSlot(ItemTypeInSlot.Value);
        }
        else
        {
            EmptyItemInSlot();
        }
    }

    public void SetSlotToggled(bool toggled)
    {
        var block = button.colors;
        block.normalColor = toggled ? button.colors.highlightedColor : origNormalColor;
        button.colors = block;

        ButtonRemove.gameObject.SetActive(toggled && ItemTypeInSlot.HasValue);
    }

    public void Hover()
    {
        OnHover.Invoke();
    }

    public void Unhover()
    {
        OnUnhover.Invoke();

        if (ItemImage.color.a != 0 && ItemImage.color.a != 1)
            InventoryManager.Instance.StopPreviewingSlot(this);
    }
}

public enum BackpackSlot
{
    Top,
    Front,
    Side,
}
