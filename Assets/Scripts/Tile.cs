using UnityEngine;

public class Tile : MonoBehaviour
{
    public ClickablePlayer Occupant { get; private set; }
    public Vector2Int Coords { get; private set; }

    [SerializeField] SpriteRenderer highlightSR;
    [SerializeField] SpriteRenderer rushHighlightSR;
    [SerializeField] SpriteRenderer dodgeHighlightSR;
    [SerializeField] SpriteRenderer dodgeHeavyHighlightSR;

    [SerializeField] SpriteRenderer pathHighlightSR;
    [SerializeField] SpriteRenderer pathRushSR;
    [SerializeField] SpriteRenderer pathDodgeSR;
    [SerializeField] SpriteRenderer pathDodgeHeavySR;

    public enum HighlightKind { None, Normal, Rush, Dodge, DodgeHeavy }
    public enum PathKind { None, Normal, Rush, Dodge, DodgeHeavy }

    void Awake()
    {
        if (highlightSR == null) { var t = transform.Find("Highlight"); if (t != null) highlightSR = t.GetComponent<SpriteRenderer>(); }
        if (rushHighlightSR == null) { var t = transform.Find("Highlight_Rush"); if (t != null) rushHighlightSR = t.GetComponent<SpriteRenderer>(); }
        if (dodgeHighlightSR == null) { var t = transform.Find("Highlight_Dodge"); if (t != null) dodgeHighlightSR = t.GetComponent<SpriteRenderer>(); }
        if (dodgeHeavyHighlightSR == null) { var t = transform.Find("Highlight_DodgeHeavy"); if (t != null) dodgeHeavyHighlightSR = t.GetComponent<SpriteRenderer>(); }

        if (pathHighlightSR == null) { var t = transform.Find("PathHighlight"); if (t != null) pathHighlightSR = t.GetComponent<SpriteRenderer>(); }
        if (pathRushSR == null) { var t = transform.Find("PathHighlight_Rush"); if (t != null) pathRushSR = t.GetComponent<SpriteRenderer>(); }
        if (pathDodgeSR == null) { var t = transform.Find("PathHighlight_Dodge"); if (t != null) pathDodgeSR = t.GetComponent<SpriteRenderer>(); }
        if (pathDodgeHeavySR == null) { var t = transform.Find("PathHighlight_DodgeHeavy"); if (t != null) pathDodgeHeavySR = t.GetComponent<SpriteRenderer>(); }

        SetHighlightState(HighlightKind.None);
        SetPathState(PathKind.None);
    }

    public void SetOccupant(ClickablePlayer player)
    {
        Occupant = player;
    }

    public void SetCoords(int x, int y)
    {
        Coords = new Vector2Int(x, y);
    }

    public void SetHighlight(bool on)
    {
        SetHighlightState(on ? HighlightKind.Normal : HighlightKind.None);
    }

    public void SetPathHighlight(bool on)
    {
        SetPathState(on ? PathKind.Normal : PathKind.None);
    }

    public void SetHighlightState(HighlightKind kind)
    {
        if (highlightSR != null) highlightSR.enabled = false;
        if (rushHighlightSR != null) rushHighlightSR.enabled = false;
        if (dodgeHighlightSR != null) dodgeHighlightSR.enabled = false;
        if (dodgeHeavyHighlightSR != null) dodgeHeavyHighlightSR.enabled = false;

        switch (kind)
        {
            case HighlightKind.Normal: if (highlightSR != null) highlightSR.enabled = true; break;
            case HighlightKind.Rush: if (rushHighlightSR != null) rushHighlightSR.enabled = true; break;
            case HighlightKind.Dodge: if (dodgeHighlightSR != null) dodgeHighlightSR.enabled = true; break;
            case HighlightKind.DodgeHeavy: if (dodgeHeavyHighlightSR != null) dodgeHeavyHighlightSR.enabled = true; break;
            case HighlightKind.None: default: break;
        }
    }

    public void SetPathState(PathKind kind) {
        if (pathHighlightSR != null) pathHighlightSR.enabled = false;
        if (pathRushSR != null) pathRushSR.enabled = false;
        if (pathDodgeSR != null) pathDodgeSR.enabled = false;
        if (pathDodgeHeavySR != null) pathDodgeHeavySR.enabled = false;

        switch (kind) {
            case PathKind.Normal: if (pathHighlightSR != null) pathHighlightSR.enabled = true; break;
            case PathKind.Rush: if (pathRushSR != null) pathRushSR.enabled = true; break;
            case PathKind.Dodge: if (pathDodgeSR != null) pathDodgeSR.enabled = true; break;
            case PathKind.DodgeHeavy: if (pathDodgeHeavySR != null) pathDodgeHeavySR.enabled = true; break;
            case PathKind.None: default: break;
        }
    }
    
}