using UnityEngine;

public class ClickableTile : MonoBehaviour {
    [HideInInspector] public Vector2Int coordinates;

    void OnMouseDown() {
        Debug.Log($"Clicked tile at [{coordinates.x}, {coordinates.y}]");
    }
}
