using UnityEngine;

public class Tile : MonoBehaviour {
    public ClickablePlayer Occupant { get; private set; }
    public Vector2Int Coords { get; private set; }

    public void SetOccupant(ClickablePlayer player) {
        Occupant = player;
    }

    public void SetCoords(int x, int y) {
        Coords = new Vector2Int(x, y);
    }
}
