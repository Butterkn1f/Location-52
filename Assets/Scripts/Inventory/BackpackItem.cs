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
    [SerializeField] public Transform Model;
    public Image selectedItemBackground;

    [HideInInspector] public bool bIsPlacedInGrid = false;

    Sequence hoverYoYo;
    Sequence moveSequence;
    Sequence rotateSequence;
    System.IDisposable moveDisposable;
    System.IDisposable disableDisposable;
    bool bIsGrabbed = false;
    bool bIsAnimating = false;

    Vector2Int? oldPosition = null;
    Direction? oldDirection = null;

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

        if (!Info.OrigPosition.HasValue)
        {
            Info.OrigPosition = transform.position;
            Info.OrigRotation = transform.rotation.eulerAngles;
        }

        disableDisposable = InventoryBackpackManager.Instance.ObserveEveryValueChanged(x => x.bShouldDisableItemButtons)
            .Subscribe(x => selectedItemBackground.gameObject.SetActive(!x));
    }

    private void OnDestroy()
    {
        disableDisposable.Dispose();
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
        rotateSequence.Append(Model.DOLocalRotate(new Vector3(0, 0, rotateAngle), 0.25f).SetEase(Ease.OutCubic));
        // Swap grid x and y upon rotation
        Info.GridSize = new Vector2Int(Info.GridSize.y, Info.GridSize.x);
    }

    private void MoveItem()
    {
        if (bIsAnimating)
            return;

        var currGrid = InventoryBackpackManager.Instance.CurrGrid;

        if (currGrid == null)
        {
            itemBackground.color = returnPosColor;
            moveSequence = DOTween.Sequence();

            if (Info.bIsHUDItem)
            {
                MoveItemHUD();
            }
            else
            {
                MoveItemNoGrid();
            }
        }
        else
        {
            MoveItemGrid(currGrid);
        }
    }

    private void MoveItemHUD()
    {
        // z is canvas near plane dist
        var worldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.7f));
        moveSequence.Append(transform.DOMove(new Vector3(worldPos.x, worldPos.y, transform.position.z), 0.05f))
            .Join(transform.DOLocalMoveZ(0.1f, 0.05f))
            .Join(transform.DOLocalRotate(Vector3.zero, 0.1f));

        var currSlot = InventoryManager.Instance.HoveredCircleSlot;
        if (currSlot != null)
        {
            InventoryManager.Instance.PreviewCurrentSlot(Info.Type);
            Model.gameObject.SetActive(false);
            itemBackground.gameObject.SetActive(false);
        }
        else
        {
            Model.gameObject.SetActive(true);
            itemBackground.gameObject.SetActive(true);
        }

        if (Input.GetMouseButtonUp(0))
            ReleaseItem();
    }

    private void MoveItemNoGrid()
    {
        var worldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.z - transform.position.z));
        moveSequence.Append(transform.DOMove(new Vector3(worldPos.x, worldPos.y, transform.position.z), 0.25f))
            .Join(transform.DORotate(Info.OrigRotation.Value, 0.25f));

        if (Input.GetMouseButtonUp(0))
            ReleaseItem();
    }

    private void MoveItemGrid(InventoryGridItem currGrid)
    {
        if (Info.bIsHUDItem && !Model.gameObject.activeSelf)
        {
            Model.gameObject.SetActive(true);
            itemBackground.gameObject.SetActive(true);
        }

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
            if (Info.bIsHUDItem && oldPosition.HasValue)
            {
                RevertToOldPosition();
            }
            else
            {
                PutBackOnShelf();
            }
        }
        else
        {
            PlaceItemAtMousePosition();
        }
    }

    private void RevertToOldPosition()
    {
        Model.gameObject.SetActive(true);
        bIsPlacedInGrid = true;
        Info.GridPosition = oldPosition.Value;
        RotateItemToSpecificDirection(this, oldDirection.Value, true);

        var grid = InventoryBackpackManager.Instance.GetGridFromPosition(oldPosition.Value);
        transform.DOMove(grid.gameObject.transform.position, 0.5f);
        transform.DORotate(grid.gameObject.transform.rotation.eulerAngles, 0.5f);

        InventoryBackpackManager.Instance.OnPlacedItem(Info, gameObject);
        InventoryManager.Instance.UpdateCurrSlot(Info.Type);
    }

    private void PutBackOnShelf()
    {
        bIsPlacedInGrid = false;
        moveSequence = DOTween.Sequence();
        moveSequence.Append(transform.DOMove(Info.OrigPosition.Value, 0.25f))
            .Join(transform.DORotate(Info.OrigRotation.Value, 0.25f))
            .Join(Model.DOLocalRotate(Vector3.zero, 0.25f));

        // Flip back grid size
        if (Info.Direction == Direction.Right || Info.Direction == Direction.Left)
            Info.GridSize = new Vector2Int(Info.GridSize.y, Info.GridSize.x);
        Info.Direction = Direction.Up;

        // Unequip if this item is in player's slots!
        InventoryManager.Instance.CheckShouldRemoveSlot(Info.Type);
    }

    private void PlaceItemAtMousePosition()
    {
        // Placed successfully in grid, update grid!
        Info.GridPosition = InventoryBackpackManager.Instance.CurrGrid.Position;
        InventoryBackpackManager.Instance.OnPlacedItem(Info, gameObject);
        bIsPlacedInGrid = true;
    }

    private void ShakeItem()
    {
        bIsAnimating = true;
        transform.DOShakePosition(0.5f, 5, 10, 90, false, true, ShakeRandomnessMode.Harmonic)
            .OnComplete(() => bIsAnimating = false);
    }

    private static void RotateItemToSpecificDirection(BackpackItem item, Direction direction, bool checkShouldSwapGrid)
    {
        if (checkShouldSwapGrid)
        {
            switch (item.Info.Direction)
            {
                case Direction.Up:
                case Direction.Down:
                    if (direction == Direction.Left || direction == Direction.Right)
                        item.Info.GridSize = new Vector2Int(item.Info.GridSize.y, item.Info.GridSize.x);
                    break;

                case Direction.Left:
                case Direction.Right:
                    if (direction == Direction.Up || direction == Direction.Down)
                        item.Info.GridSize = new Vector2Int(item.Info.GridSize.y, item.Info.GridSize.x);
                    break;

                default:
                    break;
            }
        }

        float rotateAngle = direction switch
        {
            Direction.Up => 0,
            Direction.Right => 90,
            Direction.Down => 180,
            Direction.Left => 270,
            _ => 0
        };

        item.Model.localEulerAngles = new Vector3(0, 0, rotateAngle);
        item.Info.Direction = direction;
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
            if (Info.bIsHUDItem)
            {
                oldPosition = Info.GridPosition;
                oldDirection = Info.Direction;
            }

            InventoryBackpackManager.Instance.OnRemoveItem(Info.GridPosition, gameObject);
            Info.GridPosition = Vector2Int.zero;
            bIsPlacedInGrid = false;
        }

        moveDisposable = Observable.EveryUpdate()
            .Where(_ => bIsGrabbed == true)
            .Subscribe(_ => MoveItem());

    }

    public static BackpackItem Create(GameObject prefab, Vector3 position, Quaternion rotation, BackpackItemInfo info, Transform parent, bool isHUD)
    {
        GameObject obj = Instantiate(prefab, parent);
        var item = obj.GetComponent<BackpackItem>();
        item.Info = info;
        item.Info.bIsHUDItem = isHUD;

        RotateItemToSpecificDirection(item, item.Info.Direction, false);

        item.bIsPlacedInGrid = true;
        obj.transform.rotation = rotation;
        obj.transform.position = position;

        return item;
    }
}

[System.Serializable]
public class BackpackItemInfo
{
    public InventoryItemType Type;
    public Direction Direction;
    public Vector2Int GridSize;
    public Vector2Int GridPosition;
    [HideInInspector] public Vector3? OrigPosition = null;
    [HideInInspector] public Vector3? OrigRotation = null;
    [HideInInspector] public bool bIsHUDItem = false;
}

public enum InventoryItemType
{
    Camera,
    Anomalyser,
    Flashlight,
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
