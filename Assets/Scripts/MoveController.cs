using System.Collections.Generic;
using UnityEngine;

public class MoveController : MonoBehaviour {
    public static MoveController Instance { get; private set; }

    private ClickablePlayer selected;
    private HashSet<Tile> allowed = new HashSet<Tile>();

    void Awake() {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    public bool IsAwaitingMove {
        get { return selected != null && allowed.Count > 0; }
    }

    public void BeginMove(ClickablePlayer player) {
        selected = player;
        allowed = ComputeReachable(player.Tile, player.PlayerType.stats.Spd);
        OptionMenuManager.Instance.HideMenu();
    }

    public void TrySelectTile(Tile tile) {
        if (selected == null) return;
        if (!allowed.Contains(tile)) return;
        Tile from = selected.Tile;
        from.SetOccupant(null);
        tile.SetOccupant(selected);
        selected.Tile = tile;
        selected.transform.position = tile.transform.position;
        selected.Activated = true;
        Clear();
    }

    void Clear() {
        selected = null;
        allowed.Clear();
    }

    HashSet<Tile> ComputeReachable(Tile start, int range) {
        var visited = new HashSet<Tile>();
        var q = new Queue<(Tile t, int d)>();
        visited.Add(start);
        q.Enqueue((start, 0));
        while (q.Count > 0) {
            var cur = q.Dequeue();
            if (cur.d == range) continue;
            foreach (var n in Neighbors(cur.t)) {
                if (visited.Contains(n)) continue;
                if (n.Occupant != null && n != start) continue;
                visited.Add(n);
                q.Enqueue((n, cur.d + 1));
            }
        }
        visited.Remove(start);
        return visited;
    }

    IEnumerable<Tile> Neighbors(Tile t) {
        var bm = BoardManager.Instance;
        var c = t.Coords;
        var dirs = new Vector2Int[] { new Vector2Int(1,0), new Vector2Int(-1,0), new Vector2Int(0,1), new Vector2Int(0,-1) };
        foreach (var d in dirs) {
            var nc = c + d;
            if (bm.InBounds(nc)) yield return bm.GetTile(nc);
        }
    }
}