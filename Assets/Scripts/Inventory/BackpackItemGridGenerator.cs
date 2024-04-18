using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UniRx;

[ExecuteInEditMode]
public class BackpackItemGridGenerator : MonoBehaviour
{
    [SerializeField] List<RectTransform> Images;
    BackpackItem backpackItem;

    private void Start()
    {
        backpackItem = GetComponent<BackpackItem>();
        backpackItem.Info.ObserveEveryValueChanged(x => x.GridSize)
            .Subscribe(x => UpdateGridSize(x));
    }

    void Update()
    {
        if (Application.isEditor && !EditorApplication.isPlaying && backpackItem != null)
        {
            UpdateGridSize(backpackItem.Info.GridSize);
        }
    }

    private void UpdateGridSize(Vector2Int size)
    {
        var gridSize = new Vector2(50 * size.x, 50 * size.y);
        foreach (var image in Images)
        {
            image.sizeDelta = gridSize;
        }

        GetComponent<RectTransform>().sizeDelta = gridSize;
    }
}
