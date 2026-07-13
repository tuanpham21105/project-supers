using System.Collections.Generic;
using com.cyborgAssets.inspectorButtonPro;
using UnityEngine;

/// <summary>
/// Sinh prefab rải khắp vùng đất theo thuật toán Poisson Disk Sampling (biến thể Bridson),
/// bắt đầu từ tâm (0,0) lan ra ngoài, khoảng cách giữa các object random 3-6 đơn vị.
/// Đảm bảo nhân vật 1x1 luôn có khe hở để di chuyển giữa các object.
/// </summary>
public class ForestGenerator : MonoBehaviour
{
    /// <summary>
    /// Sinh prefab trong vùng [areaMin, areaMax], khoảng cách random [minSpacing, maxSpacing].
    /// </summary>
    /// <param name="prefab">Prefab cần rải (cây, đá, chướng ngại vật...)</param>
    /// <param name="parent">Transform cha để gom các object sinh ra (để dọn dẹp/tổ chức Hierarchy)</param>
    /// <param name="areaMin">Góc dưới-trái vùng sinh, VD: (-150,-150)</param>
    /// <param name="areaMax">Góc trên-phải vùng sinh, VD: (150,150)</param>
    /// <param name="minSpacing">Khoảng cách tối thiểu giữa 2 object (tâm-tâm)</param>
    /// <param name="maxSpacing">Khoảng cách tối đa giữa 2 object (tâm-tâm)</param>
    /// <param name="obstacleRadius">
    /// Bán kính chiếm chỗ thực tế của prefab (VD: gốc cây to 0.5 đơn vị).
    /// Dùng để đảm bảo khe hở CÒN LẠI giữa 2 vật cản (sau khi trừ bán kính)
    /// vẫn đủ cho nhân vật 1x1 đi qua. Mặc định 0.5.
    /// </param>
    /// <param name="randomYRotation">Xoay ngẫu nhiên quanh trục Y cho tự nhiên (rừng cây)</param>
    /// <param name="groundY">Độ cao Y đặt object (mặt đất)</param>
    /// <param name="maxAttemptsPerPoint">Số lần thử sinh điểm con quanh 1 điểm trước khi bỏ qua</param>
    /// <param name="seed">Seed ngẫu nhiên — dùng để tái tạo lại đúng kết quả nếu cần</param>
    /// 
    [ProButton]
    public void GenerateForest(
        GameObject prefab,
        Transform parent,
        Vector2 areaMin,
        Vector2 areaMax,
        float minSpacing = 3f,
        float maxSpacing = 6f,
        float obstacleRadius = 0.5f,
        bool randomYRotation = true,
        float groundY = 0f,
        int maxAttemptsPerPoint = 30,
        int seed = 0)
    {
        if (seed != 0)
            Random.InitState(seed);

        List<Vector2> points = GeneratePoissonPoints(areaMin, areaMax, minSpacing, maxSpacing, maxAttemptsPerPoint);

        foreach (Vector2 p in points)
        {
            Vector3 worldPos = new Vector3(p.x, groundY, p.y);

            Quaternion rotation = randomYRotation
                ? Quaternion.Euler(0f, Random.Range(0f, 360f), 0f)
                : Quaternion.identity;

            GameObject obj = Object.Instantiate(prefab, worldPos, rotation, parent);
        }
    }

    // ─────────────────────────────────────────────
    // Poisson Disk Sampling (biến thể Bridson, bán kính riêng mỗi điểm)
    // ─────────────────────────────────────────────

    private struct SamplePoint
    {
        public Vector2 position;
        public float radius; // "bán kính cá nhân" — nửa khoảng cách tối thiểu mà điểm này yêu cầu
    }

    private static List<Vector2> GeneratePoissonPoints(
        Vector2 areaMin, Vector2 areaMax,
        float minSpacing, float maxSpacing,
        int maxAttemptsPerPoint)
    {
        // Kích thước ô lưới — dựa theo bán kính nhỏ nhất có thể (worst case) để tra cứu hàng xóm an toàn
        float cellSize = (minSpacing * 0.5f) / Mathf.Sqrt(2f);
        int gridWidth = Mathf.CeilToInt((areaMax.x - areaMin.x) / cellSize);
        int gridHeight = Mathf.CeilToInt((areaMax.y - areaMin.y) / cellSize);

        int[,] grid = new int[gridWidth, gridHeight];
        for (int x = 0; x < gridWidth; x++)
            for (int y = 0; y < gridHeight; y++)
                grid[x, y] = -1;

        List<SamplePoint> points = new List<SamplePoint>();
        List<int> activeList = new List<int>();

        // Điểm khởi đầu — tâm (0,0)
        Vector2 origin = Vector2.zero;
        float originRadius = Random.Range(minSpacing, maxSpacing) * 0.5f;
        AddPoint(origin, originRadius, points, activeList, grid, areaMin, cellSize);

        while (activeList.Count > 0)
        {
            int randomIndex = Random.Range(0, activeList.Count);
            int pointIndex = activeList[randomIndex];
            SamplePoint current = points[pointIndex];

            bool foundAny = false;

            for (int attempt = 0; attempt < maxAttemptsPerPoint; attempt++)
            {
                float newRadius = Random.Range(minSpacing, maxSpacing) * 0.5f;

                // Khoảng cách yêu cầu giữa điểm mới và điểm hiện tại
                float requiredDist = current.radius + newRadius;

                // Sinh điểm ứng viên quanh điểm hiện tại, cách [requiredDist, requiredDist * 2]
                float angle = Random.Range(0f, Mathf.PI * 2f);
                float dist = Random.Range(requiredDist, requiredDist * 2f);

                Vector2 candidate = current.position + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * dist;

                if (candidate.x < areaMin.x || candidate.x > areaMax.x ||
                    candidate.y < areaMin.y || candidate.y > areaMax.y)
                {
                    continue;
                }

                if (IsValidPoint(candidate, newRadius, points, grid, areaMin, cellSize, gridWidth, gridHeight))
                {
                    AddPoint(candidate, newRadius, points, activeList, grid, areaMin, cellSize);
                    foundAny = true;
                    break;
                }
            }

            if (!foundAny)
            {
                activeList.RemoveAt(randomIndex);
            }
        }

        List<Vector2> result = new List<Vector2>(points.Count);
        foreach (var p in points)
            result.Add(p.position);

        return result;
    }

    private static void AddPoint(
        Vector2 position, float radius,
        List<SamplePoint> points, List<int> activeList,
        int[,] grid, Vector2 areaMin, float cellSize)
    {
        SamplePoint point = new SamplePoint { position = position, radius = radius };
        int index = points.Count;
        points.Add(point);
        activeList.Add(index);

        Vector2Int cell = WorldToCell(position, areaMin, cellSize);
        grid[cell.x, cell.y] = index;
    }

    private static bool IsValidPoint(
        Vector2 candidate, float candidateRadius,
        List<SamplePoint> points, int[,] grid,
        Vector2 areaMin, float cellSize, int gridWidth, int gridHeight)
    {
        Vector2Int cell = WorldToCell(candidate, areaMin, cellSize);

        // Số ô cần kiểm tra xung quanh — đủ rộng để bắt được điểm có bán kính lớn nhất có thể
        int searchRadius = Mathf.CeilToInt((candidateRadius * 2f) / cellSize) + 1;

        int minX = Mathf.Max(0, cell.x - searchRadius);
        int maxX = Mathf.Min(gridWidth - 1, cell.x + searchRadius);
        int minY = Mathf.Max(0, cell.y - searchRadius);
        int maxY = Mathf.Min(gridHeight - 1, cell.y + searchRadius);

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                int pointIndex = grid[x, y];
                if (pointIndex == -1) continue;

                SamplePoint other = points[pointIndex];
                float requiredDist = candidateRadius + other.radius;
                float actualDist = Vector2.Distance(candidate, other.position);

                if (actualDist < requiredDist)
                    return false;
            }
        }

        return true;
    }

    private static Vector2Int WorldToCell(Vector2 position, Vector2 areaMin, float cellSize)
    {
        int x = Mathf.FloorToInt((position.x - areaMin.x) / cellSize);
        int y = Mathf.FloorToInt((position.y - areaMin.y) / cellSize);
        return new Vector2Int(x, y);
    }
}
