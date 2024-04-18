using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryBackpackManager : Common.DesignPatterns.Singleton<InventoryBackpackManager>
{
    [Header("Config")]
    [SerializeField] List<BackpackItem> spawnedBackpackItems; // for shelf
    // then, list of prefabs here associated with each type

    [Header("Objects")]
    [SerializeField] GameObject gridItemPrefab;
    [SerializeField] GridLayoutGroup inventoryGroup;
    [SerializeField] Transform cameraPos;

    [SerializeField] Vector2Int gridSize = new();
    Dictionary<Vector2Int, InventoryGridItem> grids = new();

    [HideInInspector] public InventoryGridItem CurrGrid = null;

    #region TEMP
    // NOTE: ALL THINGS RELATING TO CONTROLS IS TEMP UNTIL I MOVE THIS TO MAIN MENU
    Controls _controls = null;

    protected override void Awake()
    {
        base.Awake();
        _controls = new Controls();
        AssignControls();
    }

    private void AssignControls()
    {
        if (!_controls.MainGameplay.enabled)
        {
            _controls.MainGameplay.Enable();
        }

        _controls.MainGameplay.TEMPOpenInventory.performed += ctx => OpenInventory();
    }
    #endregion

    private void Start()
    {
        inventoryGroup.constraintCount = gridSize.x;
        for (int y = 0; y < gridSize.y; ++y)
        {
            for (int x = 0; x < gridSize.x; ++x)
            {
                GameObject item = Instantiate(gridItemPrefab, inventoryGroup.transform);
                var gridItem = item.GetComponent<InventoryGridItem>();
                gridItem.OnHover.AddListener(() => OnPointerEnterGrid(gridItem));
                gridItem.OnUnhover.AddListener(() => OnPointerExitGrid(gridItem));
                var position = new Vector2Int(x, y);
                gridItem.Position = position;
                grids.Add(position, gridItem);
            }
        } 
    }

    private void OpenInventory()
    {
        Characters.Player.PlayerManager.Instance.Camera.LockCameraToPosition(cameraPos.gameObject);
    }

    public bool GetIsValidSpot(BackpackItemInfo info)
    {
        if (CurrGrid == null)
            return false;

        if (CurrGrid.Position.x + info.GridSize.x > gridSize.x ||
            CurrGrid.Position.y + info.GridSize.y > gridSize.y)
            return false;

        for (int y = CurrGrid.Position.y; y < CurrGrid.Position.y + info.GridSize.y; ++y)
        {
            for (int x = CurrGrid.Position.x; x < CurrGrid.Position.x + info.GridSize.x; ++x)
            {
                if (grids[new Vector2Int(x, y)].State != GridState.Empty)
                    return false;
            }
        }

        return true;
    }

    public void OnPlacedItem(BackpackItemInfo info)
    {
        List<InventoryGridItem> linkedGrids = new();
        var topLeftGrid = grids[info.GridPosition];

        for (int y = info.GridPosition.y; y < info.GridPosition.y + info.GridSize.y; ++y)
        {
            for (int x = info.GridPosition.x; x < info.GridPosition.x + info.GridSize.x; ++x)
            {
                var grid = grids[new Vector2Int(x, y)];
                if (grid != topLeftGrid)
                {
                    grid.State = GridState.LinkedOccupied;
                    grid.ItemInfo = info;
                    grid.LinkedGrids.Add(grids[info.GridPosition]);

                    linkedGrids.Add(grid);
                }
            }
        }

        topLeftGrid.State = GridState.Occupied;
        topLeftGrid.ItemInfo = info;
        topLeftGrid.LinkedGrids = linkedGrids;
    }

    public void OnRemoveItem(Vector2Int position)
    {
        var topleftGrid = grids[position];
        // just make this grid + linked grids empty
        foreach (var grid in topleftGrid.LinkedGrids)
        {
            grid.ResetGridInfo();
        }
        topleftGrid.ResetGridInfo();
    }

    public void SetItemsInteractivity(bool interactable)
    {
        foreach (var item in spawnedBackpackItems)
        {
            item.selectedItemBackground.gameObject.SetActive(interactable);
        }
    }

    public void OnPointerEnterGrid(InventoryGridItem grid)
    {
        CurrGrid = grid;
    }

    public void OnPointerExitGrid(InventoryGridItem grid)
    {
        if (CurrGrid == grid)
            CurrGrid = null;
    }
}
