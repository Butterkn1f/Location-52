using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InventoryGridItem : MonoBehaviour
{
    public Vector2Int Position = Vector2Int.zero;
    public GridState State = GridState.Empty;
    public BackpackItemInfo ItemInfo = null;
    public List<InventoryGridItem> LinkedGrids = new();

    public delegate void HoverEvent();
    [HideInInspector] public UnityEvent OnHover = new UnityEvent();
    [HideInInspector] public UnityEvent OnUnhover = new UnityEvent();

    public void ResetGridInfo()
    {
        State = GridState.Empty;
        ItemInfo = null;
        LinkedGrids.Clear();
    }

    public void Hover()
    {
        OnHover.Invoke();
    }

    public void Unhover()
    {
        OnUnhover.Invoke();
    }
}

public enum GridState
{
    Empty,
    Occupied,
    LinkedOccupied
}
