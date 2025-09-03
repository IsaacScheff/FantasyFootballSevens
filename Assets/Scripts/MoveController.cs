using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoveController : MonoBehaviour {
    public static MoveController Instance { get; private set; }

    ClickablePlayer selected;
    HashSet<Tile> allowed = new HashSet<Tile>();
    List<Tile> previewPath = new List<Tile>();
    List<bool> previewStepIsDash = new List<bool>();
    Tile previewDest;
    int remainingSteps;
    int dashLeft;
    int dashUsedSoFar;
    readonly Color cRush = new Color(1f, 1f, 0f, 0.28f);
    readonly Color cDodge = new Color(1f, 0.45f, 0.45f, 0.33f);
    readonly Color cDodgeHeavy = new Color(0.85f, 0.2f, 0.2f, 0.38f);
    public bool IsAnimating { get; private set; }
    public event System.Action<bool> AnimationStateChanged;
    void SetAnimating(bool v)
    {
        if (IsAnimating == v) return;
        IsAnimating = v;
        AnimationStateChanged?.Invoke(v);
    }

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    void Update()
    {
        if (!IsAwaitingMove) return;
        if (Input.touchCount > 0)
        {
            var t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began)
            {
                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(t.fingerId)) return;
                if (!HitTileOrPlayer(t.position)) CancelMove();
                return;
            }
        }
        if (Input.GetMouseButtonDown(0) &&
            (EventSystem.current == null || !EventSystem.current.IsPointerOverGameObject()))
        {
            if (!HitTileOrPlayer(Input.mousePosition)) CancelMove();
        }
    }

    public bool IsAwaitingMove
    {
        get { return selected != null; }
    }

    public void BeginMove(ClickablePlayer player)
    {
        if (selected != null && selected != player) selected.Activated = true;
        Clear();
        selected = player;
        remainingSteps = Mathf.Max(0, selected.MoveLeft);
        dashLeft = 2;
        dashUsedSoFar = 0;
        if (selected.State == GroundState.Prone)
        {
            if (remainingSteps < 3) { CancelMove(); return; }
            selected.MoveLeft -= 3;
            remainingSteps -= 3;
            selected.SetState(GroundState.Standing);
        }
        previewPath.Clear();
        previewPath.Add(player.Tile);
        previewStepIsDash.Clear();
        OptionMenuManager.Instance.HideMenu();
        allowed = ComputeReachable(CurrentEnd(), remainingSteps + dashLeft);
        ShowAllowed();
    }

    public void OnTileClicked(Tile tile)
    {
        if (selected == null) return;

        if (previewDest != null && tile == previewDest)
        {
            StartCoroutine(MoveAlongPath(previewPath, previewStepIsDash));
            return;
        }

        int idx = previewPath.IndexOf(tile);
        if (idx >= 0) {
            for (int i = idx + 1; i < previewPath.Count; i++) previewPath[i].SetPathState(Tile.PathKind.None);

            int dashRemoved = 0;
            int normalRemoved = 0;
            for (int s = idx; s < previewStepIsDash.Count; s++) {
                if (previewStepIsDash[s]) dashRemoved++; else normalRemoved++;
            }
            previewPath.RemoveRange(idx + 1, previewPath.Count - (idx + 1));
            previewStepIsDash.RemoveRange(idx, previewStepIsDash.Count - idx);
            remainingSteps += normalRemoved;
            dashLeft += dashRemoved;
            previewDest = previewPath.Count > 1 ? previewPath[previewPath.Count - 1] : null;
            HideAllowed();
            allowed = ComputeReachable(CurrentEnd(), remainingSteps + dashLeft);
            ShowAllowed();
            return;
        }

        if (!allowed.Contains(tile)) {
            CancelMove();
            return;
        }

        var segment = ComputeShortestPath(CurrentEnd(), tile, remainingSteps + dashLeft);
        if (segment == null || segment.Count <= 1) return;

        int steps = segment.Count - 1;
        int useNormal = Mathf.Min(remainingSteps, steps);
        int useDash = steps - useNormal;
        if (useDash > dashLeft) return;

        for (int i = 1; i < segment.Count; i++) {
            bool isDash = i > useNormal;
            previewPath.Add(segment[i]);
            previewStepIsDash.Add(isDash);

            var from = segment[i - 1];
            var to = segment[i];

            bool requiresDodgeStep = CountAdjacentOpponents(from, selected) > 0;
            int destAdj = CountAdjacentOpponents(to, selected);

            Tile.PathKind pk = Tile.PathKind.Normal;
            if (requiresDodgeStep) pk = destAdj > 0 ? Tile.PathKind.DodgeHeavy : Tile.PathKind.Dodge;
            else if (isDash) pk = Tile.PathKind.Rush;

            segment[i].SetPathState(pk);
        }

        previewDest = tile;
        remainingSteps -= useNormal;
        dashLeft -= useDash;

        HideAllowed();
        allowed = ComputeReachable(CurrentEnd(), remainingSteps + dashLeft);
        ShowAllowed();
    }

    IEnumerator MoveAlongPath(List<Tile> path, List<bool> stepIsDash)
    {
        HideAllowed();
        SetAnimating(true);
        int steps = path.Count - 1;
        int normalsExecuted = 0;
        int dashesExecuted = 0;

        for (int i = 1; i <= steps; i++)
        {
            Tile from = selected.Tile;
            Tile to = path[i];

            from.SetOccupant(null);
            to.SetOccupant(selected);
            selected.Tile = to;
            selected.transform.position = to.transform.position;

            if (RequiresDodge(from, selected))
            {
                int penalty = CountAdjacentOpponents(to, selected);
                int r = Dice.D8();
                int total = r + selected.PlayerType.stats.Dex - penalty;
                GameLog.Instance?.Log($"{selected.DisplayName} dodge: d8 {r} + Dex {selected.PlayerType.stats.Dex} - EZ {penalty} = {total} {(total >= 9 ? "success" : "failure")}");
                if (total < 9)
                {
                    SetAnimating(false);
                    ResolveFall(selected);
                    HidePath();
                    CancelMove();
                    yield break;
                }
            }

            bool isDash = stepIsDash[i - 1];
            if (isDash) {
                int threshold = (dashUsedSoFar + dashesExecuted == 0) ? 2 : 3;
                int roll = Dice.D8();
                GameLog.Instance?.Log($"{selected.DisplayName} dashes on {threshold}+, rolls {roll} {(roll >= threshold ? "success" : "failure")}");
                if (roll < threshold) {
                    SetAnimating(false);
                    ResolveFall(selected);
                    HidePath();
                    CancelMove();
                    yield break;
                }
                dashesExecuted++;
            } else {
                normalsExecuted++;
            }

            yield return new WaitForSeconds(0.33f);
        }

        selected.MoveLeft = Mathf.Max(0, selected.MoveLeft - normalsExecuted);
        dashUsedSoFar += dashesExecuted;
        HidePath();
        SetAnimating(false);

        if (selected.MoveLeft > 0 || dashLeft > 0) {
            previewPath.Clear();
            previewPath.Add(selected.Tile);
            previewStepIsDash.Clear();
            previewDest = null;
            remainingSteps = selected.MoveLeft;
            allowed = ComputeReachable(CurrentEnd(), remainingSteps + dashLeft);
            ShowAllowed();
        } else {
            selected.Activated = true;
            CancelMove();
        }
    }


    public void CancelMove() {
        HidePath();
        HideAllowed();
        SetAnimating(false);
        selected = null;
        allowed.Clear();
        previewPath.Clear();
        previewStepIsDash.Clear();
        previewDest = null;
        remainingSteps = 0;
        dashLeft = 0;
        dashUsedSoFar = 0;
    }

    void ShowAllowed() {
        var start = CurrentEnd();
        var dists = ComputeDistances(start, remainingSteps + dashLeft);

        foreach (var t in allowed) {
            if (t.Occupant != null && t != start) continue;

            int steps = dists.ContainsKey(t) ? dists[t] : remainingSteps;
            bool needsDash = steps > remainingSteps;

            bool dodgeOnFirstStep = false;
            if (dists.ContainsKey(t) && dists[t] == 1) {
                dodgeOnFirstStep = CountAdjacentOpponents(start, selected) > 0;
            }

            var kind = Tile.HighlightKind.Normal;
            if (dodgeOnFirstStep) {
                int destAdj = CountAdjacentOpponents(t, selected);
                kind = destAdj > 0 ? Tile.HighlightKind.DodgeHeavy : Tile.HighlightKind.Dodge;
            } else if (needsDash) {
                kind = Tile.HighlightKind.Rush;
            }

            t.SetHighlightState(kind);
        }
    }

    void HideAllowed() {
        foreach (var t in allowed) t.SetHighlightState(Tile.HighlightKind.None);
    }

    void HidePath() {
        foreach (var t in previewPath) t.SetPathState(Tile.PathKind.None);
        previewDest = null;
    }

    void Clear()
    {
        HidePath();
        HideAllowed();
        selected = null;
        allowed.Clear();
        previewPath.Clear();
        previewStepIsDash.Clear();
        previewDest = null;
        remainingSteps = 0;
        dashLeft = 0;
        dashUsedSoFar = 0;
    }

    Tile CurrentEnd()
    {
        return previewPath.Count > 0 ? previewPath[previewPath.Count - 1] : null;
    }

    bool HitTileOrPlayer(Vector2 screenPos)
    {
        Vector3 wp = Camera.main.ScreenToWorldPoint(screenPos);
        RaycastHit2D hit = Physics2D.Raycast((Vector2)wp, Vector2.zero);
        if (hit.collider == null) return false;
        if (hit.collider.GetComponent<Tile>() != null) return true;
        if (hit.collider.GetComponent<ClickablePlayer>() != null) return true;
        return false;
    }

    HashSet<Tile> ComputeReachable(Tile start, int range)
    {
        var visited = new HashSet<Tile>();
        var q = new Queue<(Tile t, int d)>();
        visited.Add(start);
        q.Enqueue((start, 0));
        while (q.Count > 0)
        {
            var cur = q.Dequeue();
            if (cur.d == range) continue;
            foreach (var n in Neighbors(cur.t))
            {
                if (visited.Contains(n)) continue;
                if (n.Occupant != null && n != start) continue;
                visited.Add(n);
                q.Enqueue((n, cur.d + 1));
            }
        }
        visited.Remove(start);
        return visited;
    }

    List<Tile> ComputeShortestPath(Tile start, Tile goal, int maxSteps)
    {
        var parent = new Dictionary<Tile, Tile>();
        var dist = new Dictionary<Tile, int>();
        var q = new Queue<Tile>();
        q.Enqueue(start);
        dist[start] = 0;
        while (q.Count > 0)
        {
            var cur = q.Dequeue();
            if (cur == goal) break;
            foreach (var n in Neighbors(cur))
            {
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
        while (t != start)
        {
            path.Add(t);
            t = parent[t];
        }
        path.Add(start);
        path.Reverse();
        return path;
    }

    IEnumerable<Tile> Neighbors(Tile t)
    {
        var bm = BoardManager.Instance;
        var c = t.Coords;
        var dirs = new Vector2Int[] {
            new Vector2Int(1,0), new Vector2Int(-1,0),
            new Vector2Int(0,1), new Vector2Int(0,-1),
            new Vector2Int(1,1), new Vector2Int(1,-1),
            new Vector2Int(-1,1), new Vector2Int(-1,-1)
        };
        foreach (var d in dirs)
        {
            var nc = c + d;
            if (bm.InBounds(nc)) yield return bm.GetTile(nc);
        }
    }

    bool RequiresDodge(Tile from, ClickablePlayer mover)
    {
        return CountAdjacentOpponents(from, mover) > 0;
    }

    int CountAdjacentOpponents(Tile t, ClickablePlayer mover)
    {
        int c = 0;
        foreach (var n in Neighbors(t))
        {
            var occ = n.Occupant;
            if (occ != null && occ.Team != mover.Team) c++;
        }
        return c;
    }

    void ResolveFall(ClickablePlayer p)
    {
        p.SetState(GroundState.Prone);
        int a1, a2;
        int armor = Dice.Roll2D6(out a1, out a2);
        GameLog.Instance?.Log($"Armor roll for {p.DisplayName}: {a1} + {a2} = {armor} {(p.PlayerType != null && armor > p.PlayerType.stats.Con ? "— break" : "— no break")}");
        if (p.PlayerType != null && armor > p.PlayerType.stats.Con)
        {
            int i1, i2;
            int inj = Dice.Roll2D6(out i1, out i2);
            if (inj <= 7)
            {
                p.SetState(GroundState.Stunned);
                GameLog.Instance?.Log($"Injury roll for {p.DisplayName}: {i1} + {i2} = {inj} — stunned");
            }
            else
            {
                GameLog.Instance?.Log($"Injury roll for {p.DisplayName}: {i1} + {i2} = {inj} — removed");
                RemoveFromPitch(p);
            }
        }
        p.Activated = true;
    }

    void RemoveFromPitch(ClickablePlayer p)
    {
        Tile t = p.Tile;
        if (t != null && t.Occupant == p) t.SetOccupant(null);
        Destroy(p.gameObject);
    }
    
    Dictionary<Tile,int> ComputeDistances(Tile start, int range) {
        var dist = new Dictionary<Tile,int>();
        var q = new Queue<Tile>();
        dist[start] = 0;
        q.Enqueue(start);
        while (q.Count > 0) {
            var cur = q.Dequeue();
            int d = dist[cur];
            if (d == range) continue;
            foreach (var n in Neighbors(cur)) {
                if (n.Occupant != null && n != start) continue;
                if (dist.ContainsKey(n)) continue;
                dist[n] = d + 1;
                q.Enqueue(n);
            }
        }
        dist.Remove(start);
        return dist;
    }
}