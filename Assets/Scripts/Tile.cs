using UnityEngine;

public class Tile : MonoBehaviour {
    public ClickablePlayer Occupant { get; private set; }
    public Vector2Int Coords { get; private set; }
    [SerializeField] SpriteRenderer highlightSR;
    [SerializeField] SpriteRenderer pathHighlightSR;

    void Awake() {
        if (highlightSR == null) {
            Transform h = transform.Find("Highlight");
            if (h != null) highlightSR = h.GetComponent<SpriteRenderer>();
        }
        if (pathHighlightSR == null) {
            Transform p = transform.Find("PathHighlight");
            if (p != null) pathHighlightSR = p.GetComponent<SpriteRenderer>();
        }
        SetHighlight(false);
        SetPathHighlight(false);
    }

    public void SetOccupant(ClickablePlayer player) {
        Occupant = player;
    }

    public void SetCoords(int x, int y) {
        Coords = new Vector2Int(x, y);
    }

    public void SetHighlight(bool on) {
        if (highlightSR != null) highlightSR.enabled = on;
    }

    public void SetPathHighlight(bool on) {
        if (pathHighlightSR != null) pathHighlightSR.enabled = on;
    }
}