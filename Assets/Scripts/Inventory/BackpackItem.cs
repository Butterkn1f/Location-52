using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using DG.Tweening;
using UniRx;

[RequireComponent(typeof(BackpackItemGridGenerator))]
public class BackpackItem : MonoBehaviour
{
    [Header("Config")]
    public BackpackItemInfo Info;
    [SerializeField] Color validPosColor, invalidPosColor, returnPosColor;
    [Header("Objects")]
    [SerializeField] Image itemBackground;
    [SerializeField] Transform model;
    public Image selectedItemBackground;

    [HideInInspector] public bool bIsPlacedInGrid = false;

    Vector3 originalPos;
    Vector3 originalRot;

    Sequence hoverYoYo;
    Sequence moveSequence;
    Sequence rotateSequence;
    System.IDisposable moveDisposable;
    bool bIsGrabbed = false;
    bool bIsAnimating = false;

    Controls _controls = null;

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
    }

    private void Start()
    {
        itemBackground.gameObject.SetActive(false);
        selectedItemBackground.gameObject.SetActive(true);
        originalPos = transform.position;
        originalRot = transform.rotation.eulerAngles;

        InventoryBackpackManager.Instance.ObserveEveryValueChanged(x => x.bShouldDisableItemButtons)
            .Subscribe(x => selectedItemBackground.gameObject.SetActive(!x));
    }

    private void RotateItem(InputAction.CallbackContext context)
    {
        if (!bIsGrabbed)
            return;

        rotateSequence.Kill();
        rotateSequence = DOTween.Sequence();
        var rotateAngle = 0;
        switch (Info.Direction)
        {
            case Direction.Up:
                {
                    rotateAngle = 90;
                    Info.Direction = Direction.Right;
                }
                break;

            case Direction.Right:
                {
                    rotateAngle = 180;
                    Info.Direction = Direction.Down;
                }
                break;

            case Direction.Down:
                {
                    rotateAngle = 270;
                    Info.Direction = Direction.Left;
                }
                break;

            case Direction.Left:
                {
                    rotateAngle = 0;
                    Info.Direction = Direction.Up;
                }
                break;
        }
        rotateSequence.Append(model.DOLocalRotate(new Vector3(0, 0, rotateAngle), 0.25f).SetEase(Ease.OutCubic));
        // Swap grid x and y upon rotation
        Info.GridSize = new Vector2Int(Info.GridSize.y, Info.GridSize.x);
    }

    private void MoveItem()
    {
        if (bIsAnimating)
            return;

        var worldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.z - transform.position.z));
        var currGrid = InventoryBackpackManager.Instance.CurrGrid;

        if (currGrid == null)
        {
            itemBackground.color = returnPosColor;
            moveSequence = DOTween.Sequence();
            moveSequence.Append(transform.DOMove(new Vector3(worldPos.x, worldPos.y, transform.position.z), 0.25f))
                .Join(transform.DORotate(originalRot, 0.25f));

            if (Input.GetMouseButtonUp(0))
                ReleaseItem();
        }
        else
        {
            // Snap to current hovered grid
            transform.DOMove(currGrid.gameObject.transform.position, 0.25f);
            transform.DORotate(currGrid.gameObject.transform.rotation.eulerAngles, 0.25f);

            var validPos = InventoryBackpackManager.Instance.GetIsValidSpot(Info);
            itemBackground.color = validPos ? validPosColor : invalidPosColor;

            if (Input.GetMouseButtonUp(0))
            {
                if (validPos)
                    ReleaseItem();
                else
                    ShakeItem();
            }
        }
    }

    private void ReleaseItem()
    {
        moveDisposable.Dispose();
        moveSequence.Kill();
        bIsGrabbed = false;
        selectedItemBackground.gameObject.SetActive(true);
        itemBackground.gameObject.SetActive(false);

        InventoryBackpackManager.Instance.bShouldDisableItemButtons = false;
        _controls.MainGameplay.RotateInventoryItem.performed -= RotateItem;

        if (InventoryBackpackManager.Instance.CurrGrid == null)
        {
            bIsPlacedInGrid = false;
            moveSequence = DOTween.Sequence();
            moveSequence.Append(transform.DOMove(originalPos, 0.25f))
                .Join(transform.DORotate(originalRot, 0.25f))
                .Join(model.DOLocalRotate(Vector3.zero, 0.25f));

            if (Info.Direction == Direction.Right || Info.Direction == Direction.Left)
                Info.GridSize = new Vector2Int(Info.GridSize.y, Info.GridSize.x);
            Info.Direction = Direction.Up;
        }
        else
        {
            // Placed successfully in grid, update grid!
            Info.GridPosition = InventoryBackpackManager.Instance.CurrGrid.Position;
            InventoryBackpackManager.Instance.OnPlacedItem(Info);
            bIsPlacedInGrid = true;
        }
    }

    private void ShakeItem()
    {
        bIsAnimating = true;
        transform.DOShakePosition(0.5f, 5, 10, 90, false, true, ShakeRandomnessMode.Harmonic)
            .OnComplete(() => bIsAnimating = false);
    }

    public void OnPointerEnterSelectedImage()
    {
        if (bIsGrabbed)
            return;

        hoverYoYo.Kill();
        selectedItemBackground.color = Color.white;
        hoverYoYo = DOTween.Sequence();
        hoverYoYo.Append(selectedItemBackground.DOFade(0.3f, 0.8f))
            .SetLoops(-1, LoopType.Yoyo);
    }

    public void OnPointerExitSelectedImage()
    {
        if (bIsGrabbed || hoverYoYo == null)
            return;

        hoverYoYo.Kill();
        selectedItemBackground.DOFade(1.0f, 0f);
    }

    public void OnClick()
    {
        if (bIsGrabbed)
            return;
 
        bIsGrabbed = true;
        selectedItemBackground.gameObject.SetActive(false);
        itemBackground.gameObject.SetActive(true);
        InventoryBackpackManager.Instance.bShouldDisableItemButtons = true;
        _controls.MainGameplay.RotateInventoryItem.performed += RotateItem;

        if (bIsPlacedInGrid)
        {
            InventoryBackpackManager.Instance.OnRemoveItem(Info.GridPosition);
            Info.GridPosition = Vector2Int.zero;
            bIsPlacedInGrid = false;
        }

        moveDisposable = Observable.EveryUpdate()
            .Where(_ => bIsGrabbed == true)
            .Subscribe(_ => MoveItem());

    }
}

[System.Serializable]
public class BackpackItemInfo
{
    public InventoryItemType Type;
    public Direction Direction;
    public Vector2Int GridSize;
    public Vector2Int GridPosition;
}

public enum InventoryItemType
{
    Camera,
    Anomalyser,
    MovementDetector,
    Map,
    Trap,
    Noisemaker,
    Flare
}

public enum Direction
{
    Up,
    Right,
    Down,
    Left,
}
