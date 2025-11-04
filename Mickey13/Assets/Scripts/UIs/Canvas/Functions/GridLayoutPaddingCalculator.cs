using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class GridLayoutPaddingCalculator : MonoBehaviour
{
    private GridLayoutGroup grid;

    private void OnEnable()
    {
        CalculateHorizontal();
    }
    public void CalculateHorizontal()
    {
        grid = GetComponent<GridLayoutGroup>();

        Vector2 cellSize = grid.cellSize;
        Vector2 spacing = grid.spacing;
        Vector2 rectSize = (grid.transform as RectTransform).rect.size;

        float mod = rectSize.x % (cellSize.x + spacing.x);

        RectOffset padding = new RectOffset();
        padding.top = grid.padding.top;
        padding.bottom = grid.padding.bottom;

        Debug.Log("mod: " + mod.ToString());
        int horizontalPadding = Mathf.RoundToInt(mod / 2f);
        padding.left = (int)(horizontalPadding + (spacing.x / 2f));
        grid.padding = padding;
    }
}
