using System;
using System.Collections.Generic;
using com.cyborgAssets.inspectorButtonPro;
using UnityEngine;

public class MapObstaclesController : MonoBehaviour
{
    [Header("Resources")]
    [SerializeField] private MapObstaclesSO mapObstaclesSO;
    [SerializeField] private Mesh mesh;
    [SerializeField] private Material material;

    [Header("Configs")]
    private const int MAX_INSTANCE_PER_BATCH = 1023;
    [SerializeField] private Vector3Int minPos;
    [SerializeField] private Vector3Int maxPos;
    [SerializeField] private float deceleration = 5f;
    [SerializeField] private float gravity = 9.8f;

    [Serializable]
    public class ObstacleData
    {
        public Vector3 position;
        public Vector3 rotation;
        public float scale = 1;
        public float moveSpeed = 0;
        public Vector3 moveDirection = Vector3.zero;
        public float gravityMoveSpeed = 0;

        public string String()
        {   
            return position.ToString();
        }
    }


    [Header("Runtime")]
    private Dictionary<Vector3Int, ObstacleData> obstacles = new Dictionary<Vector3Int, ObstacleData>();
    [SerializeField] private List<ObstacleData> movingObjects = new List<ObstacleData>();
    private List<Matrix4x4> matrices = new List<Matrix4x4>();

    // Chỉ rebuild matrices khi thực sự cần — tránh tốn CPU mỗi frame lúc scene đứng yên hoàn toàn
    private bool matricesDirty = true;

    private void Awake()
    {
        BuildObstaclesFromSO();
        RebuildMatrices();
    }

    private void FixedUpdate()
    {
        MoveMovingObstacles();
    }

    private void Update()
    {
        DrawObstacles();
    }

    // 6 hướng lân cận trực tiếp (trục X/Y/Z) dùng để kiểm tra 1 ô có bị bao kín hoàn toàn hay không
    private static readonly Vector3Int[] NeighborOffsets = new Vector3Int[]
    {
        new Vector3Int( 1, 0, 0), new Vector3Int(-1, 0, 0),
        new Vector3Int( 0, 1, 0), new Vector3Int( 0,-1, 0),
        new Vector3Int( 0, 0, 1), new Vector3Int( 0, 0,-1),
    };

    /// <summary>
    /// Xếp dữ liệu từ MapObstaclesSO (danh sách các vùng start/size) vào dictionary obstacles.
    /// Chỉ giữ lại các ô nằm ở BỀ MẶT (có ít nhất 1 mặt tiếp giáp ô trống), loại bỏ toàn bộ
    /// ô nằm hoàn toàn bên trong khối (bị bao kín cả 6 hướng) vì chúng không bao giờ hiển thị được.
    /// -> Kết quả: khối obstacle trở nên "rỗng ruột", giảm đáng kể số lượng instance cần vẽ.
    /// </summary>
    private void BuildObstaclesFromSO()
    {
        obstacles.Clear();

        if (mapObstaclesSO == null || mapObstaclesSO.obstacleSpaces == null)
            return;

        // Bước 1: đánh dấu toàn bộ ô "đặc" (occupied) từ tất cả các vùng, dùng HashSet để tra cứu O(1)
        HashSet<Vector3Int> occupied = new HashSet<Vector3Int>();

        foreach (MapObstacleSpace space in mapObstaclesSO.obstacleSpaces)
        {
            Vector3Int start = space.start;
            Vector3Int size = space.size;

            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    for (int z = 0; z < size.z; z++)
                    {
                        occupied.Add(start + new Vector3Int(x, y, z));
                    }
                }
            }
        }

        // Bước 2: với mỗi ô đặc, chỉ giữ lại nếu ít nhất 1 trong 6 ô lân cận KHÔNG đặc
        // (nghĩa là ô đó có mặt lộ ra ngoài không khí, cần được vẽ).
        // Ô bị bao kín hoàn toàn cả 6 hướng (nằm sâu bên trong) sẽ bị bỏ qua, không thêm vào obstacles.
        foreach (Vector3Int cellPos in occupied)
        {
            bool isSurfaceCell = false;

            foreach (Vector3Int offset in NeighborOffsets)
            {
                if (!occupied.Contains(cellPos + offset))
                {
                    isSurfaceCell = true;
                    break;
                }
            }

            if (!isSurfaceCell)
                continue; // ô nằm hoàn toàn bên trong -> rỗng ruột, không cần vẽ

            ObstacleData data = new ObstacleData
            {
                position = cellPos,
                rotation = Vector3.zero,
                scale = 1f,
                moveSpeed = 0f
            };

            obstacles.Add(cellPos, data);
        }
    }

    [ProButton]
    public void ApplyForce(Vector3Int pos, float force, Vector3 direction)
    {
        if (!obstacles.TryGetValue(pos, out ObstacleData data))
            return;

        obstacles.Remove(pos);

        data.rotation = Quaternion.LookRotation(direction).eulerAngles;
        data.moveSpeed = force;
        data.moveDirection = direction.normalized;

        if (!movingObjects.Contains(data))
            movingObjects.Add(data);

        matricesDirty = true;
    }

    private void StopMovingObstacle(int index)
    {
        ObstacleData data = movingObjects[index];
        data.moveSpeed = 0f;
        data.moveDirection = Vector3.zero;
        data.gravityMoveSpeed = 0f;
        movingObjects.RemoveAt(index);

        // Dùng RoundToInt thay vì FloorToInt — vị trí lúc này có thể lệch rất nhỏ
        // (VD: 1.999997 thay vì 2.0) do sai số cộng dồn từ nhiều bước di chuyển float.
        // Floor sẽ làm tròn XUỐNG sai thành ô (1,1,1) thay vì đúng ra là (1,2,1).
        // Round xử lý đúng vì 1.999997 vẫn được coi là gần 2.0 nhất.
        Vector3Int finalCell = new Vector3Int(
            Mathf.RoundToInt(data.position.x),
            Mathf.RoundToInt(data.position.y),
            Mathf.RoundToInt(data.position.z)
        );

        // Đồng bộ luôn data.position về đúng lưới — tránh vật thể hiển thị
        // lệch vài phần nghìn đơn vị so với ô nó thực sự đang chiếm dụng
        data.position = finalCell;

        // Dùng indexer thay vì Add() để tránh exception nếu chẳng may đã có object khác
        // chiếm đúng ô này (edge case 2 vật thể cùng dừng lại 1 vị trí)
        obstacles[finalCell] = data;

        matricesDirty = true;
    }

    private bool IsOutOfBounds(Vector3 pos)
    {
        return pos.x < minPos.x || pos.x > maxPos.x
            || pos.y < minPos.y || pos.y > maxPos.y
            || pos.z < minPos.z || pos.z > maxPos.z;
    }

    // ─────────────────────────────────────────────
    // Di chuyển vật thể
    // ─────────────────────────────────────────────

    void MoveMovingObstacles()
    {
        if (movingObjects.Count == 0) return;

        float dt = Time.fixedDeltaTime;

        for (int i = movingObjects.Count - 1; i >= 0; i--)
        {
            ObstacleData data = movingObjects[i];

            // 1. Giảm dần moveSpeed theo thời gian (deceleration)
            if (data.moveSpeed > 0f)
            {
                data.moveSpeed = Mathf.Max(0f, data.moveSpeed - deceleration * dt);
            }

            // 2. Di chuyển ngang theo moveDirection — có kiểm tra va chạm từng ô trên đường đi
            if (data.moveSpeed > 0f && data.moveDirection != Vector3.zero)
            {
                TryMoveAlongPath(data, data.moveDirection, data.moveSpeed * dt, isGravity: false);
            }

            // 3. Luôn áp dụng trọng lực — kể cả khi moveSpeed = 0, miễn là chưa có vật chống đỡ bên dưới
            ApplyGravity(data, dt);

            // 4. Vật thể chỉ dừng hoàn toàn khi CẢ moveSpeed và gravityMoveSpeed đều = 0
            if (data.moveSpeed <= 0f && data.gravityMoveSpeed <= 0f)
            {
                StopMovingObstacle(i);
            }
        }

        matricesDirty = true; // vị trí vừa thay đổi -> cần vẽ lại đúng vị trí mới
    }

    /// <summary>
    /// Áp dụng trọng lực lên 1 object đang di chuyển.
    /// Nếu ô ngay bên dưới KHÔNG có vật chống đỡ (obstacle tĩnh) -> tiếp tục rơi, tăng tốc theo gravity.
    /// Nếu có vật chống đỡ -> dừng rơi hoàn toàn (gravityMoveSpeed = 0).
    /// </summary>
    private void ApplyGravity(ObstacleData data, float dt)
    {
        Vector3Int belowCell = Vector3Int.FloorToInt(data.position) + new Vector3Int(0, -1, 0);
        bool hasSupport = obstacles.ContainsKey(belowCell);

        if (hasSupport)
        {
            data.gravityMoveSpeed = 0f;
            return;
        }

        // Không có support bên dưới -> rơi tiếp, vận tốc rơi tăng dần theo thời gian
        data.gravityMoveSpeed += gravity * dt;

        TryMoveAlongPath(data, Vector3.down, data.gravityMoveSpeed * dt, isGravity: true);
    }

    /// <summary>
    /// Di chuyển 1 object theo hướng "direction" quãng đường "distance", kiểm tra TỪNG Ô Vector3Int
    /// trên đường đi (bước 1 đơn vị mỗi lần) xem có bị chiếm dụng hoặc ngoài giới hạn không.
    /// Nếu ô tiếp theo bị chặn -> dừng lại đúng tại ranh giới ô hiện tại, không đi vào ô đó.
    /// </summary>
    /// <param name="isGravity">
    /// true = đây là bước di chuyển do trọng lực (chạm đất -> reset gravityMoveSpeed)
    /// false = đây là bước di chuyển ngang do lực đẩy (chạm vật cản -> reset moveSpeed/moveDirection)
    /// </param>
    private void TryMoveAlongPath(ObstacleData data, Vector3 direction, float distance, bool isGravity)
    {
        if (distance <= 0f) return;

        Vector3 dir = direction.normalized;
        Vector3 currentPos = data.position;
        float traveled = 0f;

        while (traveled < distance)
        {
            float stepLen = Mathf.Min(1f, distance - traveled);
            Vector3 nextPos = currentPos + dir * stepLen;

            Vector3Int currentCell = Vector3Int.RoundToInt(currentPos);
            Vector3Int nextCell = Vector3Int.RoundToInt(nextPos);

            // Chỉ cần kiểm tra khi thực sự bước SANG một ô Vector3Int khác
            if (nextCell != currentCell)
            {
                if (IsOutOfBounds(nextPos) || obstacles.ContainsKey(nextCell))
                {
                    // Bị chặn — dừng đúng tại vị trí hiện tại, không tiến vào ô bị chiếm
                    data.position = currentPos;

                    if (isGravity)
                        data.gravityMoveSpeed = 0f; // chạm đất/vật cản bên dưới -> dừng rơi
                    else
                    {
                        data.moveSpeed = 0f;
                        data.moveDirection = Vector3.zero; // chạm vật cản ngang -> dừng lực đẩy
                    }

                    return;
                }
            }

            currentPos = nextPos;
            traveled += stepLen;
        }

        data.position = currentPos;
    }

    /// <summary>
    /// Dựng lại danh sách ma trận (vị trí/xoay/scale) từ CẢ obstacle tĩnh lẫn đang di chuyển.
    /// Gọi lại hàm này mỗi khi obstacles/movingObjects thay đổi (thêm/xóa/di chuyển).
    /// </summary>
    private void RebuildMatrices()
    {
        matrices.Clear();

        foreach (ObstacleData data in obstacles.Values)
        {
            matrices.Add(BuildMatrix(data));
        }

        foreach (ObstacleData data in movingObjects)
        {
            matrices.Add(BuildMatrix(data));
        }

        matricesDirty = false;
    }

    private Matrix4x4 BuildMatrix(ObstacleData data)
    {
        return Matrix4x4.TRS(
            data.position,
            Quaternion.Euler(data.rotation),
            Vector3.one * data.scale
        );
    }

    /// <summary>
    /// Vẽ toàn bộ obstacle bằng Graphics.DrawMeshInstanced, chia batch theo giới hạn 1023 instance.
    /// </summary>
    private void DrawObstacles()
    {
        if (mesh == null || material == null || matrices.Count == 0)
            return;

        // Chỉ rebuild khi có thay đổi thực sự (có object đang di chuyển hoặc vừa ApplyForce/dừng lại)
        if (matricesDirty)
        {
            RebuildMatrices();
        }

        int total = matrices.Count;
        int index = 0;

        Matrix4x4[] batchArray = new Matrix4x4[MAX_INSTANCE_PER_BATCH];

        while (index < total)
        {
            int batchCount = Mathf.Min(MAX_INSTANCE_PER_BATCH, total - index);

            matrices.CopyTo(index, batchArray, 0, batchCount);

            Graphics.DrawMeshInstanced(
                mesh,
                0,
                material,
                batchArray,
                batchCount
            );

            index += batchCount;
        }
    }

    [ProButton]
    public void GetObstacle(Vector3Int pos)
    {
        Debug.Log(obstacles[pos].String());
    }

    [ProButton]
    public void TimeChange(float time)
    {
        Time.timeScale = time;
    }
}
