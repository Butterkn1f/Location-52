using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
using DG.Tweening;
using UnityEngine.EventSystems;

public class GalleryManager : MonoBehaviour
{
    [SerializeField] GameObject galleryObject;
    [SerializeField] GameObject galleryItemPrefab;
    [SerializeField] RectTransform photoContent;
    [SerializeField] HorizontalLayoutGroup content;
    [SerializeField] GameObject noPhotosText;
    [SerializeField] GameObject viewport;
    [SerializeField] TMP_Text textCount, textMax;
    [SerializeField] Button buttonDelete;
    [SerializeField] Scrollbar scrollbar;

    public bool bIsGalleryOpen { get; private set; } = false;

    Controls _controls;

    List<GalleryItem> galleryItems = new();
    GalleryItem currItem = null;
    int currIndex = -1;
    float galleryInterval = 0;

    Sequence zoomInSequence;
    Sequence zoomOutSequence;

    private void Awake()
    {
        _controls = new Controls();
        AssignControls();
    }

    private void Start()
    {
        galleryObject.SetActive(false);
    }

    private void AssignControls()
    {
        if (!_controls.MainGameplay.enabled)
        {
            _controls.MainGameplay.Enable();
        }

        Observable.EveryUpdate()
            .Where(_ => bIsGalleryOpen && Input.GetMouseButtonDown(0) && EventSystem.current.currentSelectedGameObject != buttonDelete.gameObject)
            .Subscribe(_ => ZoomOutCurrentItem());

        Observable.EveryUpdate()
            .Where(_ => bIsGalleryOpen && Input.GetMouseButtonUp(0) && EventSystem.current.currentSelectedGameObject != buttonDelete.gameObject)
            .Subscribe(_ => SnapNearestItem());

        _controls.MainGameplay.Scroll.performed += ctx => SnapToIndex();
    }

    private void ZoomOutCurrentItem()
    {
        if (!currItem || galleryItems.Count < 2)
            return;

        buttonDelete.interactable = false;
        zoomOutSequence = DOTween.Sequence();
        zoomOutSequence.Append(currItem.transform.DOScale(1.0f, 0.5f))
            .OnComplete(() => currItem = null);
    }

    private void SnapNearestItem()
    {
        switch (galleryItems.Count)
        {
            case 0:
                return;

            case 1:
                {
                    // Immediately snap to 0.5f (the only photo)
                    float scrollVal = scrollbar.value;
                    DOTween.To(() => scrollVal, x => scrollVal = x, 0.5f, 0.25f)
                    .OnUpdate(() => scrollbar.value = scrollVal)
                    .SetEase(Ease.OutQuad);
                    return;
                }
        }

        if (zoomInSequence != null)
            zoomInSequence.Kill();
        if (currItem != null)
        {
            zoomOutSequence.Kill();
            currItem.transform.DOScale(1.0f, 0);
        }

        // Round to nearest index in scroll value
        currIndex = Mathf.Clamp(Mathf.RoundToInt(scrollbar.value / galleryInterval), 0, galleryItems.Count - 1);
        currItem = galleryItems[currIndex];
        textCount.text = (currIndex + 1).ToString();
        buttonDelete.interactable = false;
        float scrollValue = scrollbar.value;

        zoomInSequence = DOTween.Sequence();
        zoomInSequence.Append(DOTween.To(() => scrollValue, x => scrollValue = x, galleryInterval * currIndex, 0.25f)
                .OnUpdate(() => scrollbar.value = scrollValue)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => buttonDelete.interactable = true)
            )
            .Join(currItem.transform.DOScale(1.25f, 0.5f));

    }

    private void SnapToIndex(int? index = null)
    {
        if (!bIsGalleryOpen)
            return;

        int newIndex;
        if (index.HasValue)
        {
            newIndex = Mathf.Clamp(index.Value, 0, galleryItems.Count);
        }
        else
        {
            newIndex = Mathf.Clamp(
                _controls.MainGameplay.Scroll.ReadValue<Vector2>().y > 0
                ? currIndex - 1
                : currIndex + 1
            , 0, galleryItems.Count - 1);

            if (galleryItems.Count < 2 || newIndex == currIndex)
                return;
        }

        if (currItem != null)
        {
            currItem.transform.DOScale(1.0f, 0.5f);
        }

        currIndex = newIndex;
        currItem = galleryItems[currIndex];
        textCount.text = (currIndex + 1).ToString();
        buttonDelete.interactable = false;
        float scrollValue = scrollbar.value;

        zoomInSequence = DOTween.Sequence();
        zoomInSequence.Append(DOTween.To(() => scrollValue, x => scrollValue = x, galleryInterval * currIndex, 0.25f)
                .OnUpdate(() => scrollbar.value = scrollValue)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => buttonDelete.interactable = true)
            )
            .Join(currItem.transform.DOScale(1.25f, 0.5f));
    }

    private void OnToggleActive()
    {
        Characters.Player.PlayerManager.Instance.Camera.UnlockMouseCursor(true);
        var photos = PhotoManager.Instance.Photos;

        if (photos.Count == 0)
        {
            noPhotosText.SetActive(true);
            viewport.SetActive(false);
            return;
        }

        foreach (var photo in photos)
        {
            GameObject obj = Instantiate(galleryItemPrefab, photoContent);
            obj.GetComponent<GalleryItem>().Initialize(photo);

            galleryItems.Add(obj.GetComponent<GalleryItem>());
        }

        currIndex = galleryItems.Count - 1;
        galleryInterval = currIndex > 0 ? 1.0f / currIndex : 0;
        currItem = galleryItems[currIndex];
        scrollbar.value = 1;

        textCount.text = textMax.text = (galleryItems.Count).ToString();

        StartCoroutine(RefreshViewport(true));
    }

    private void OnToggleInactive()
    {
        Characters.Player.PlayerManager.Instance.Camera.UnlockMouseCursor(false);
        foreach (var item in galleryItems)
        {
            Destroy(item.gameObject);
        }

        galleryItems.Clear();
        currIndex = -1;
        currItem = null;
    }

    private IEnumerator RefreshViewport(bool isStart)
    {
        content.enabled = false;
        yield return new WaitForSeconds(0.1f);
        content.enabled = true;

        if (isStart)
        {
            yield return new WaitForSeconds(0.25f);
            InitialZoomIn();

            noPhotosText.SetActive(false);
            viewport.SetActive(true);
        }
    }

    private void InitialZoomIn()
    {
        if (!currItem)
            return;

        currItem.transform.DOScale(1.25f, 0.25f);
    }

    public void ToggleGallery(bool? toggle = null)
    {
        bIsGalleryOpen = toggle ?? !bIsGalleryOpen;
        galleryObject.SetActive(bIsGalleryOpen);
        PlayerUIManager.Instance.ControlsManager.SetControlActive(ControlsType.CameraGallery, bIsGalleryOpen);

        if (bIsGalleryOpen)
        {
            OnToggleActive();
            InventoryManager.Instance.SetInventorySwapEnabled(false);
        }
        else
        {
            OnToggleInactive();
            InventoryManager.Instance.SetInventorySwapEnabled(true);
        }
    }

    public void DeleteItem()
    {
        if (!currItem)
            return;

        PhotoManager.Instance.Photos.RemoveAt(currIndex);
        galleryItems.RemoveAt(currIndex);
        Destroy(currItem.gameObject);
        currItem = null;
        textMax.text = galleryItems.Count.ToString();

        if (galleryItems.Count == 0)
        {
            noPhotosText.SetActive(true);
            viewport.SetActive(false);
        }
        else
        {
            galleryInterval = 1.0f / (galleryItems.Count - 1);
            SnapToIndex(currIndex - 1);
            StartCoroutine(RefreshViewport(false));
        }
    }
}
