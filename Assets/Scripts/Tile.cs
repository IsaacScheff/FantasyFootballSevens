using UnityEngine;

public class Tile : MonoBehaviour {
    public ClickablePlayer Occupant { get; private set; }
    public Vector2Int Coords { get; private set; }
    [SerializeField] SpriteRenderer highlightSR;

    void Awake() {
        if (highlightSR == null) {
            Transform h = transform.Find("Highlight");
            if (h != null) highlightSR = h.GetComponent<SpriteRenderer>();
        }
        SetHighlight(false);
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
}