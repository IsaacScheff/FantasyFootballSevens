using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ClickablePlayer : MonoBehaviour {
    [HideInInspector] public Tile Tile;
    [HideInInspector] public PlayerType PlayerType;

    void Update() {
        if (Input.touchCount > 0) {
            var touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began) {
                if (!EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                    TryHandleInput(touch.position);
                return;
            }
        }
        if (Input.GetMouseButtonDown(0) &&
            (EventSystem.current == null || !EventSystem.current.IsPointerOverGameObject()))
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
    public void HandleClick() {
        Debug.Log($"Player clicked at world position {transform.position}");

        var options = new List<OptionMenuManager.OptionData>() {
            new OptionMenuManager.OptionData("Move", () =>
                Debug.Log("Move selected")),
            new OptionMenuManager.OptionData("Blitz", () =>
                Debug.Log("Blitz selected")),
        };
        
        OptionMenuManager.Instance.ShowMenu(transform.position, options);
    }
}