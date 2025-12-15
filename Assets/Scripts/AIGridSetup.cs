using UnityEngine;

namespace HolidayGameJam
{
    public class AIGridSetup : MonoBehaviour
    {
        [Header("Grid Configuration")]
        [SerializeField] private Transform targetSurface;
        [SerializeField] private int gridResolution = 20;
        [SerializeField] private float spawnHeight = 10f;

        [Header("Calculated Grid Data (Read-Only)")]
        [SerializeField] private Vector3 gridOrigin;
        [SerializeField] private float cellSize;
        [SerializeField] private Vector2 surfaceSize;
        [SerializeField] private Bounds gridBounds;

        [Header("Debug Visualization")]
        [SerializeField] private bool showGridGizmos = true;

        public Vector3 GridOrigin => gridOrigin;
        public float CellSize => cellSize;
        public Bounds GridBounds => gridBounds;
        public float SpawnHeight => spawnHeight;
        public int GridResolution => gridResolution;

        public void RecalculateGrid()
        {
            if (targetSurface == null)
            {
                Debug.LogWarning("AIGridSetup: Target surface not assigned. Cannot calculate grid.");
                return;
            }

            Bounds bounds;

            Collider surfaceCollider = targetSurface.GetComponent<Collider>();
            if (surfaceCollider != null)
            {
                bounds = surfaceCollider.bounds;
            }
            else
            {
                Renderer surfaceRenderer = targetSurface.GetComponent<Renderer>();
                if (surfaceRenderer != null)
                {
                    bounds = surfaceRenderer.bounds;
                }
                else
                {
                    Debug.LogWarning("AIGridSetup: Target surface has no Collider or Renderer. Cannot calculate grid.");
                    return;
                }
            }

            gridOrigin = bounds.center;
            surfaceSize = new Vector2(bounds.size.x, bounds.size.z);
            cellSize = surfaceSize.x / gridResolution;
            gridBounds = bounds;

            Debug.Log($"AIGridSetup: Grid recalculated - Origin: {gridOrigin}, Cell Size: {cellSize}, Surface: {surfaceSize}");
        }

        public Vector2Int WorldToGridCell(Vector3 worldPosition)
        {
            int gridX = Mathf.RoundToInt((worldPosition.x - gridOrigin.x) / cellSize + (gridResolution / 2f));
            int gridZ = Mathf.RoundToInt((worldPosition.z - gridOrigin.z) / cellSize + (gridResolution / 2f));

            gridX = Mathf.Clamp(gridX, 0, gridResolution - 1);
            gridZ = Mathf.Clamp(gridZ, 0, gridResolution - 1);

            return new Vector2Int(gridX, gridZ);
        }

        public Vector3 GridCellToWorld(Vector2Int cellPosition)
        {
            float halfGrid = gridResolution / 2f;
            float worldX = gridOrigin.x + ((cellPosition.x - halfGrid) * cellSize);
            float worldZ = gridOrigin.z + ((cellPosition.y - halfGrid) * cellSize);

            return new Vector3(worldX, gridOrigin.y, worldZ);
        }

        public bool IsValidGridCell(Vector2Int cellPosition)
        {
            return cellPosition.x >= 0 && cellPosition.x < gridResolution &&
                   cellPosition.y >= 0 && cellPosition.y < gridResolution;
        }

        private void OnDrawGizmosSelected()
        {
            if (!showGridGizmos || gridResolution <= 0 || cellSize <= 0)
                return;

            Gizmos.color = Color.white;

            float halfGrid = gridResolution / 2f;
            float gridWorldSize = gridResolution * cellSize;

            for (int x = 0; x <= gridResolution; x++)
            {
                float worldX = gridOrigin.x + ((x - halfGrid) * cellSize);
                Vector3 startZ = new Vector3(worldX, gridOrigin.y, gridOrigin.z - (halfGrid * cellSize));
                Vector3 endZ = new Vector3(worldX, gridOrigin.y, gridOrigin.z + (halfGrid * cellSize));
                Gizmos.DrawLine(startZ, endZ);
            }

            for (int z = 0; z <= gridResolution; z++)
            {
                float worldZ = gridOrigin.z + ((z - halfGrid) * cellSize);
                Vector3 startX = new Vector3(gridOrigin.x - (halfGrid * cellSize), gridOrigin.y, worldZ);
                Vector3 endX = new Vector3(gridOrigin.x + (halfGrid * cellSize), gridOrigin.y, worldZ);
                Gizmos.DrawLine(startX, endX);
            }

            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(gridOrigin, 0.3f);

            Gizmos.color = new Color(0f, 1f, 1f, 0.3f);
            Vector3 spawnPlaneCenter = new Vector3(gridOrigin.x, gridOrigin.y + spawnHeight, gridOrigin.z);
            Gizmos.DrawWireCube(spawnPlaneCenter, new Vector3(gridWorldSize, 0.1f, gridWorldSize));
        }
    }
}
