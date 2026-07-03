using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(GridLayoutGroup))]
public class UIGridScaler : MonoBehaviour
{
    [Header("Grid Configuration")]
    [Tooltip("Number of rows in the grid layout.")]
    public int rows = 2;

    [Tooltip("Number of columns in the grid layout.")]
    public int columns = 4;

    [Header("Margin Adjustment")]
    [Range(0.5f, 1f)]
    [Tooltip("Percentage of cell size to ensure padding from edges. 1 = full fill, 0.97 = 3% margin.")]
    public float marginPercent = 0.97f;

    private GridLayoutGroup grid;
    private RectTransform rectTransform;

    void Awake()
    {
        grid = GetComponent<GridLayoutGroup>();
        rectTransform = GetComponent<RectTransform>();
        // Enforce fixed column count
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = columns;
    }

    void Start()
    {
        ApplyScaling();
    }

    void OnValidate()
    {
        if (grid == null) grid = GetComponent<GridLayoutGroup>();
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = columns;
        ApplyScaling();
    }

    private void ApplyScaling()
    {
        if (grid == null || rectTransform == null) return;

        // Get grid container size (UI space)
        float parentWidth = rectTransform.rect.width;
        float parentHeight = rectTransform.rect.height;

        // Calculate total spacing and padding
        float totalSpacingX = grid.spacing.x * (columns - 1) + grid.padding.left + grid.padding.right;
        float totalSpacingY = grid.spacing.y * (rows - 1) + grid.padding.top + grid.padding.bottom;

        // Available space for cells
        float availableWidth = parentWidth - totalSpacingX;
        float availableHeight = parentHeight - totalSpacingY;

        // Base cell size without margin
        float cellWidth = availableWidth / columns;
        float cellHeight = availableHeight / rows;

        // Apply margin to prevent touching screen edges
        cellWidth *= marginPercent;
        cellHeight *= marginPercent;

        grid.cellSize = new Vector2(cellWidth, cellHeight);
    }
}
