using UnityEngine;

public class Tile : MonoBehaviour {
    public ClickablePlayer Occupant { get; private set; }

    public void SetOccupant(ClickablePlayer player) {
        Occupant = player;
    }
}