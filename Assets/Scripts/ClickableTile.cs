using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ClickableTile : MonoBehaviour {
    [HideInInspector] public Vector2Int coordinates;

    void Update() {
        if (Input.touchCount > 0) {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began) {
                if (!EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                    TryHandleInput(touch.position);
                return;
            }
        }

        if (Input.GetMouseButtonDown(0) &&
            !EventSystem.current.IsPointerOverGameObject())
        {
            TryHandleInput(Input.mousePosition);
        }
    }

    void TryHandleInput(Vector2 screenPos) {
        Vector3 worldPt = Camera.main.ScreenToWorldPoint(screenPos);
        RaycastHit2D hit = Physics2D.Raycast((Vector2)worldPt, Vector2.zero);
        if (hit.collider != null && hit.collider.gameObject == gameObject)
            HandleClick();
    }

    void HandleClick() {
        Debug.Log($"Clicked tile at [{coordinates.x}, {coordinates.y}]");

        var options = new List<OptionMenuManager.OptionData>() {
            new OptionMenuManager.OptionData("Move", () =>
                Debug.Log($"Move from [{coordinates.x},{coordinates.y}]")),
            new OptionMenuManager.OptionData("Blitz", () =>
                Debug.Log($"Blitz from [{coordinates.x},{coordinates.y}]")),
        };
        OptionMenuManager.Instance.ShowMenu(transform.position, options);
    }
}