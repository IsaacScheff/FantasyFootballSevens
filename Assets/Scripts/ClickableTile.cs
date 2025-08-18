using UnityEngine;
using UnityEngine.EventSystems;

public class ClickableTile : MonoBehaviour {
    [HideInInspector] public Tile TileComponent;

    void Update() {
        if (Input.touchCount > 0) {
            var t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began) {
                if (!EventSystem.current.IsPointerOverGameObject(t.fingerId)) TryHandleInput(t.position);
                return;
            }
        }
        if (Input.GetMouseButtonDown(0) &&
            (EventSystem.current == null || !EventSystem.current.IsPointerOverGameObject())) {
            TryHandleInput(Input.mousePosition);
        }
    }

    void TryHandleInput(Vector2 screenPos) {
        Vector3 wp = Camera.main.ScreenToWorldPoint(screenPos);
        RaycastHit2D hit = Physics2D.Raycast((Vector2)wp, Vector2.zero);
        if (hit.collider != null && hit.collider.gameObject == gameObject) HandleClick();
    }

    void HandleClick() {
        if (MoveController.Instance != null && MoveController.Instance.IsAwaitingMove) {
            MoveController.Instance.OnTileClicked(TileComponent);
            return;
        }
        if (TileComponent.Occupant != null) {
            TileComponent.Occupant.HandleClick();
        }
    }
}