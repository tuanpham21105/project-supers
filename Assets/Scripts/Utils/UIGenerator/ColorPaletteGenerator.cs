using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Static class tạo UI bảng màu (color palette grid) gồm 15 cột màu,
/// mỗi cột có 10 sắc độ từ đậm nhất đến nhạt nhất.
/// </summary>
public static class ColorPaletteGenerator
{
    public enum ColorColumn
    {
        Red, RedOrange, Orange, YellowOrange, Yellow,
        YellowGreen, Green, BlueGreen, Blue, BlueViolet,
        Violet, Pink, RedViolet, Brown, Black
    }

    private const int ShadeCount = 10;

    // Màu hue gốc (HSV) cho từng cột — Brown và Black xử lý riêng
    private static readonly float[] _hues = new float[]
    {
        0f,    // Red
        15f,   // RedOrange
        30f,   // Orange
        45f,   // YellowOrange
        60f,   // Yellow
        90f,   // YellowGreen
        120f,  // Green
        165f,  // BlueGreen
        220f,  // Blue
        260f,  // BlueViolet
        280f,  // Violet
        330f,  // Pink
        345f,  // RedViolet
        0f,    // Brown (xử lý riêng)
        0f     // Black (xử lý riêng)
    };

    private static readonly string[] _columnNames = new string[]
    {
        "Red", "Red-Orange", "Orange", "Yellow-Orange", "Yellow",
        "Yellow-Green", "Green", "Blue-Green", "Blue", "Blue-Violet",
        "Violet", "Pink", "Red-Violet", "Brown", "Black"
    };

    // ─────────────────────────────────────────────
    // Public API
    // ─────────────────────────────────────────────

    /// <summary>
    /// Tạo UI bảng màu vào parent transform cho sẵn.
    /// Parent nên có GridLayoutGroup hoặc sẽ tự thêm vào.
    /// </summary>
    [MenuItem("Tools/UI/Create RGBA Color Picker")]
    public static GameObject Generate()
    {
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        Transform parent = canvas.transform;

        GameObject root = new GameObject("ColorPalette");
        root.transform.SetParent(parent, false);
        float cellSize = 48f;
        float spacing = 4f;

        RectTransform rootRt = root.AddComponent<RectTransform>();

        GridLayoutGroup grid = root.AddComponent<GridLayoutGroup>();
        grid.cellSize = new Vector2(cellSize, cellSize);
        grid.spacing = new Vector2(spacing, spacing);
        grid.startAxis = GridLayoutGroup.Axis.Horizontal;
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = _columnNames.Length;

        ContentSizeFitter fitter = root.AddComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        fitter.verticalFit   = ContentSizeFitter.FitMode.PreferredSize;

        // Render theo hàng (đậm nhất → nhạt nhất), mỗi hàng đi qua hết các cột
        for (int row = 0; row < ShadeCount; row++)
        {
            float shadeT = row / (float)(ShadeCount - 1); // 0 = đậm nhất, 1 = nhạt nhất

            for (int col = 0; col < _columnNames.Length; col++)
            {
                Color color = GetColor((ColorColumn)col, shadeT);
                CreateSwatch(root.transform, color, _columnNames[col], row);
            }
        }

        return root;
    }

    /// <summary>Lấy màu tại 1 cột và 1 mức sắc độ (0 = đậm nhất, 1 = nhạt nhất)</summary>
    public static Color GetColor(ColorColumn column, float shadeT)
    {
        shadeT = Mathf.Clamp01(shadeT);

        if (column == ColorColumn.Black)
            return GetBlackShade(shadeT);

        if (column == ColorColumn.Brown)
            return GetBrownShade(shadeT);

        float hue = _hues[(int)column] / 360f;

        // Đậm nhất (shadeT=0): saturation cao, value thấp
        // Nhạt nhất (shadeT=1): saturation thấp, value cao
        float saturation = Mathf.Lerp(1.2f, 0.45f, shadeT); 
        float value       = Mathf.Lerp(0.4f, 1f, shadeT);

        return Color.HSVToRGB(hue, saturation, value);
    }

    // ─────────────────────────────────────────────
    // Private helpers
    // ─────────────────────────────────────────────

    private static Color GetBlackShade(float shadeT)
    {
        // Đậm nhất = đen tuyệt đối, nhạt nhất = xám gần trắng
        float value = Mathf.Lerp(0.0f, 0.92f, shadeT);
        return new Color(value, value, value);
    }

    private static Color GetBrownShade(float shadeT)
    {
        // Brown: hue cam đất (~25°), saturation và value thấp hơn Orange
        float hue = 25f / 360f;
        float saturation = Mathf.Lerp(0.55f, 0.12f, shadeT);
        float value       = Mathf.Lerp(0.22f, 0.85f, shadeT);
        return Color.HSVToRGB(hue, saturation, value);
    }

    private static void CreateSwatch(Transform parent, Color color, string columnName, int row)
    {
        GameObject swatch = new GameObject($"{columnName}_{row}");
        swatch.transform.SetParent(parent, false);

        Image img = swatch.AddComponent<Image>();
        img.color = color;
    }
}
