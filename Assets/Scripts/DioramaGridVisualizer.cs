using UnityEngine;

public class DioramaGridVisualizer : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private int gridWidth = 12;
    [SerializeField] private int gridHeight = 12;
    [SerializeField] private float cellSize = 0.25f;
    [SerializeField] private float gridYOffset = 0.01f;
    
    [Header("Visual Settings")]
    [SerializeField] private Color gridColor = new Color(1f, 1f, 1f, 0.3f);
    [SerializeField] private Material lineMaterial;

    private GameObject gridContainer;

    private void Start()
    {
        CreateRuntimeGrid();
    }

    private void CreateRuntimeGrid()
    {
        if (gridContainer != null)
        {
            Destroy(gridContainer);
        }

        gridContainer = new GameObject("Grid Lines");
        gridContainer.transform.SetParent(transform);
        gridContainer.transform.localPosition = Vector3.zero;

        float totalWidth = gridWidth * cellSize;
        float totalHeight = gridHeight * cellSize;
        Vector3 gridOrigin = new Vector3(-totalWidth / 2f, gridYOffset, -totalHeight / 2f);

        for (int x = 0; x <= gridWidth; x++)
        {
            CreateLine($"VerticalLine_{x}", 
                gridOrigin + new Vector3(x * cellSize, 0, 0),
                gridOrigin + new Vector3(x * cellSize, 0, totalHeight));
        }

        for (int z = 0; z <= gridHeight; z++)
        {
            CreateLine($"HorizontalLine_{z}",
                gridOrigin + new Vector3(0, 0, z * cellSize),
                gridOrigin + new Vector3(totalWidth, 0, z * cellSize));
        }
    }

    private void CreateLine(string name, Vector3 start, Vector3 end)
    {
        GameObject lineObj = new GameObject(name);
        lineObj.transform.SetParent(gridContainer.transform);
        lineObj.transform.localPosition = Vector3.zero;

        LineRenderer lineRenderer = lineObj.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.useWorldSpace = true;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        lineRenderer.startWidth = 0.02f;
        lineRenderer.endWidth = 0.02f;
        
        if (lineMaterial != null)
        {
            lineRenderer.material = lineMaterial;
        }
        else
        {
            lineRenderer.material = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            lineRenderer.material.color = gridColor;
        }
        
        lineRenderer.startColor = gridColor;
        lineRenderer.endColor = gridColor;
        lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lineRenderer.receiveShadows = false;
        lineRenderer.allowOcclusionWhenDynamic = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = gridColor;
        
        float totalWidth = gridWidth * cellSize;
        float totalHeight = gridHeight * cellSize;
        
        Vector3 gridOrigin = transform.position + new Vector3(-totalWidth / 2f, gridYOffset, -totalHeight / 2f);
        
        for (int x = 0; x <= gridWidth; x++)
        {
            Vector3 start = gridOrigin + new Vector3(x * cellSize, 0, 0);
            Vector3 end = start + new Vector3(0, 0, totalHeight);
            Gizmos.DrawLine(start, end);
        }
        
        for (int z = 0; z <= gridHeight; z++)
        {
            Vector3 start = gridOrigin + new Vector3(0, 0, z * cellSize);
            Vector3 end = start + new Vector3(totalWidth, 0, 0);
            Gizmos.DrawLine(start, end);
        }
    }

    private void OnDestroy()
    {
        if (gridContainer != null)
        {
            Destroy(gridContainer);
        }
    }
}
