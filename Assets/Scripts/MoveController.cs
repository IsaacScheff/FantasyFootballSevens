using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoveController : MonoBehaviour {
    public static MoveController Instance { get; private set; }

    ClickablePlayer selected;
    HashSet<Tile> allowed = new HashSet<Tile>();
    List<Tile> previewPath = new List<Tile>();
    Tile previewDest;
    int remainingSteps;

    void Awake() {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    void Update() {
        if (!IsAwaitingMove) return;
        if (Input.touchCount > 0) {
            var t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began) {
                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(t.fingerId)) return;
                if (!HitTileOrPlayer(t.position)) CancelMove();
                return;
            }
        }
        if (Input.GetMouseButtonDown(0) &&
            (EventSystem.current == null || !EventSystem.current.IsPointerOverGameObject())) {
            if (!HitTileOrPlayer(Input.mousePosition)) CancelMove();
        }
    }

    public bool IsAwaitingMove {
        get { return selected != null; }
    }

    public void BeginMove(ClickablePlayer player) {
        Clear();
        selected = player;
        remainingSteps = player.PlayerType.stats.Spd;
        previewPath.Clear();
        previewPath.Add(player.Tile);
        OptionMenuManager.Instance.HideMenu();
        allowed = ComputeReachable(CurrentEnd(), remainingSteps);
        ShowAllowed();
    }

    public void OnTileClicked(Tile tile) {
        if (selected == null) return;

        if (previewDest != null && tile == previewDest) {
            StartCoroutine(MoveAlongPath(previewPath));
            return;
        }

        int idx = previewPath.IndexOf(tile);
        if (idx >= 0) {
            for (int i = idx + 1; i < previewPath.Count; i++) previewPath[i].SetPathHighlight(false);
            int used = previewPath.Count - 1;
            int newUsed = idx;
            remainingSteps += used - newUsed;
            previewPath.RemoveRange(idx + 1, previewPath.Count - (idx + 1));
            previewDest = previewPath.Count > 1 ? previewPath[previewPath.Count - 1] : null;
            HideAllowed();
            allowed = ComputeReachable(CurrentEnd(), remainingSteps);
            ShowAllowed();
            return;
        }

        if (!allowed.Contains(tile)) {
            CancelMove();
            return;
        }

        var segment = ComputeShortestPath(CurrentEnd(), tile, remainingSteps);
        if (segment == null || segment.Count <= 1) return;

        for (int i = 1; i < segment.Count; i++) {
            previewPath.Add(segment[i]);
            segment[i].SetPathHighlight(true);
        }
        previewDest = tile;
        remainingSteps -= (segment.Count - 1);
        HideAllowed();
        allowed = ComputeReachable(CurrentEnd(), remainingSteps);
        ShowAllowed();
    }

    public void CancelMove() {
        Clear();
    }

    IEnumerator MoveAlongPath(List<Tile> path) {
        HideAllowed();
        for (int i = 1; i < path.Count; i++) {
            Tile from = selected.Tile;
            Tile to = path[i];
            from.SetOccupant(null);
            to.SetOccupant(selected);
            selected.Tile = to;
            selected.transform.position = to.transform.position;
            yield return new WaitForSeconds(0.33f);
        }
        selected.Activated = true;
        HidePath();
        CancelMove();
    }

    void ShowAllowed() {
        foreach (var t in allowed) t.SetHighlight(true);
    }

    void HideAllowed() {
        foreach (var t in allowed) t.SetHighlight(false);
    }

    void HidePath() {
        foreach (var t in previewPath) t.SetPathHighlight(false);
        previewDest = null;
    }

    void Clear() {
        HidePath();
        HideAllowed();
        selected = null;
        allowed.Clear();
        previewPath.Clear();
        previewDest = null;
        remainingSteps = 0;
    }

    Tile CurrentEnd() {
        return previewPath.Count > 0 ? previewPath[previewPath.Count - 1] : null;
    }

    bool HitTileOrPlayer(Vector2 screenPos) {
        Vector3 wp = Camera.main.ScreenToWorldPoint(screenPos);
        RaycastHit2D hit = Physics2D.Raycast((Vector2)wp, Vector2.zero);
        if (hit.collider == null) return false;
        if (hit.collider.GetComponent<Tile>() != null) return true;
        if (hit.collider.GetComponent<ClickablePlayer>() != null) return true;
        return false;
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

    List<Tile> ComputeShortestPath(Tile start, Tile goal, int maxSteps) {
        var parent = new Dictionary<Tile, Tile>();
        var dist = new Dictionary<Tile, int>();
        var q = new Queue<Tile>();
        q.Enqueue(start);
        dist[start] = 0;
        while (q.Count > 0) {
            var cur = q.Dequeue();
            if (cur == goal) break;
            foreach (var n in Neighbors(cur)) {
                if (n.Occupant != null && n != goal) continue;
                int nd = dist[cur] + 1;
                if (nd > maxSteps) continue;
                if (dist.ContainsKey(n)) continue;
                dist[n] = nd;
                parent[n] = cur;
                q.Enqueue(n);
            }
        }
        if (!dist.ContainsKey(goal)) return null;
        var path = new List<Tile>();
        var t = goal;
        while (t != start) {
            path.Add(t);
            t = parent[t];
        }
        path.Add(start);
        path.Reverse();
        return path;
    }

    IEnumerable<Tile> Neighbors(Tile t) {
        var bm = BoardManager.Instance;
        var c = t.Coords;
        var dirs = new Vector2Int[] {
            new Vector2Int(1,0), new Vector2Int(-1,0),
            new Vector2Int(0,1), new Vector2Int(0,-1),
            new Vector2Int(1,1), new Vector2Int(1,-1),
            new Vector2Int(-1,1), new Vector2Int(-1,-1)
        };
        foreach (var d in dirs) {
            var nc = c + d;
            if (bm.InBounds(nc)) yield return bm.GetTile(nc);
        }
    }
}