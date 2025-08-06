using UnityEngine;

public class BoardManager : MonoBehaviour {
    public int columns = 20, rows = 11;
    public float tileSize = 1f;
    public Vector2 margin = Vector2.one;
    public GameObject tilePrefab;
    public GameObject playerPrefab;
    public Vector2Int startingPlayerCoord = new Vector2Int(0, 0);

    private Tile[,] grid;

    void Start() {
        grid = new Tile[columns, rows];
        GenerateGrid();
        SpawnPlayer(startingPlayerCoord);
    }

    void GenerateGrid() {
        Vector2 offset = new Vector2(
            -columns * tileSize * 0.5f + tileSize * 0.5f + margin.x,
            -rows * tileSize * 0.5f + tileSize * 0.5f + margin.y
        );

        for (int x = 0; x < columns; x++)
        for (int y = 0; y < rows;    y++) {
            Vector3 pos = new Vector3(
                x * tileSize + offset.x,
                y * tileSize + offset.y,
                0f
            );

            GameObject go = Instantiate(tilePrefab, pos, Quaternion.identity, transform);
            var tileComp = go.GetComponent<Tile>();
            grid[x, y] = tileComp;

            var ct = go.AddComponent<ClickableTile>();
            ct.TileComponent = tileComp;
        }
    }

    void SpawnPlayer(Vector2Int coords) {
        var tile = grid[coords.x, coords.y];
        Vector3 spawnPos = tile.transform.position;
        GameObject pl = Instantiate(playerPrefab, spawnPos, Quaternion.identity, transform);
        var cp = pl.GetComponent<ClickablePlayer>();
        cp.Tile = tile;
        tile.SetOccupant(cp);
    }
}
