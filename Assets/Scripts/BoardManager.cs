using UnityEngine;

public class BoardManager : MonoBehaviour {
    [Header("Grid Size")]
    public int columns = 20;
    public int rows    = 11;

    [Header("Spacing/Margin")]
    public float tileSize   = 1f;
    public Vector2 margin   = new Vector2(1f, 1f);

    [Header("References")]
    public GameObject tilePrefab;

    void Start() {
        GenerateGrid();
    }

    void GenerateGrid() {
        // center the grid around the origin, then add margin
        Vector2 offset = new Vector2(
            -columns * tileSize * 0.5f + tileSize * 0.5f + margin.x,
            -rows    * tileSize * 0.5f + tileSize * 0.5f + margin.y
        );

        for (int x = 0; x < columns; x++)
        for (int y = 0; y < rows;    y++) {
            Vector3 pos = new Vector3(
                x * tileSize + offset.x,
                y * tileSize + offset.y,
                0f
            );
            GameObject tile = Instantiate(tilePrefab, pos, Quaternion.identity, transform);
            // attach a little helper to catch clicks
            var ct = tile.AddComponent<ClickableTile>();
            ct.coordinates = new Vector2Int(x, y);
        }
    }
}