using UnityEngine;

public class BoardManager : MonoBehaviour {
    public static BoardManager Instance { get; private set; }
    public int columns = 20, rows = 11;
    public float tileSize = 1f;
    public Vector2 margin = Vector2.zero;
    public GameObject tilePrefab;
    public GameObject playerPrefab;
    public Vector2Int startingPlayerCoord = new Vector2Int(0, 0);
    public Vector2Int startingPlayerCoordTwo = new Vector2Int(5, 5);

    private Tile[,] grid;

    void Awake() {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
        grid = new Tile[columns, rows];
    }
    void Start() {
        grid = new Tile[columns, rows];
        GenerateGrid();

        var factory = FindObjectOfType<PlayerFactory>();
        var type = factory.allTypes[0];

        factory.SpawnPlayer(type, new Vector2Int(3, 5), TeamSide.Home);
        factory.SpawnPlayer(type, new Vector2Int(5, 5), TeamSide.Home);

        factory.SpawnPlayer(type, new Vector2Int(14, 5), TeamSide.Away);
        factory.SpawnPlayer(type, new Vector2Int(16, 5), TeamSide.Away);
    }

    void GenerateGrid() {
        Vector2 offset = new Vector2(
            -columns * tileSize * 0.5f + tileSize * 0.5f + margin.x,
            -rows * tileSize * 0.5f + tileSize * 0.5f + margin.y
        );
        for (int x = 0; x < columns; x++)
        for (int y = 0; y < rows;    y++) {
            Vector3 pos = new Vector3(x * tileSize + offset.x, y * tileSize + offset.y, 0f);
            GameObject go = Instantiate(tilePrefab, pos, Quaternion.identity, transform);
            Tile tileComp = go.GetComponent<Tile>();
            tileComp.SetCoords(x, y);
            grid[x, y] = tileComp;
            ClickableTile ct = go.AddComponent<ClickableTile>();
            ct.TileComponent = tileComp;
        }
    }

    void SpawnPlayer(Vector2Int coords) {
        Tile tile = grid[coords.x, coords.y];
        Vector3 spawnPos = tile.transform.position;
        GameObject pl = Instantiate(playerPrefab, spawnPos, Quaternion.identity, transform);
        ClickablePlayer cp = pl.GetComponent<ClickablePlayer>();
        cp.Tile = tile;
        tile.SetOccupant(cp);
    }

    public Vector3 GetWorldPosition(Vector2Int coords) {
        if (coords.x < 0 || coords.x >= columns || coords.y < 0 || coords.y >= rows) return Vector3.zero;
        return grid[coords.x, coords.y].transform.position;
    }

    public Tile GetTile(Vector2Int coords) {
        return grid[coords.x, coords.y];
    }

    public bool InBounds(Vector2Int c) {
        return c.x >= 0 && c.x < columns && c.y >= 0 && c.y < rows;
    }
}