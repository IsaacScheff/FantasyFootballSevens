using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public enum GroundState { Standing, Prone, Stunned }

public class ClickablePlayer : MonoBehaviour {
    [HideInInspector] public Tile Tile;
    [HideInInspector] public PlayerType PlayerType;

    [SerializeField] bool activated;
    public bool Activated {
        get { return activated; }
        set { activated = value; ApplyActivationVisual(); }
    }

    public GroundState State = GroundState.Standing;
    public int MoveLeft { get; set; }

    SpriteRenderer sr;
    [SerializeField] Transform stunOverlay;

    void Awake() {
        sr = GetComponent<SpriteRenderer>();
        if (sr == null) sr = GetComponentInChildren<SpriteRenderer>();
        if (stunOverlay == null) {
            var t = transform.Find("StunOverlay");
            if (t != null) stunOverlay = t;
        }
        ApplyActivationVisual();
        ApplyStateVisual();
    }

    void Update() {
        if (Input.touchCount > 0) {
            var touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began) {
                if (!EventSystem.current.IsPointerOverGameObject(touch.fingerId)) TryHandleInput(touch.position);
                return;
            }
        }
        if (Input.GetMouseButtonDown(0) &&
            (EventSystem.current == null || !EventSystem.current.IsPointerOverGameObject())) {
            TryHandleInput(Input.mousePosition);
        }
    }

    void TryHandleInput(Vector2 screenPos) {
        Vector3 worldPt = Camera.main.ScreenToWorldPoint(screenPos);
        RaycastHit2D hit = Physics2D.Raycast((Vector2)worldPt, Vector2.zero);
        if (hit.collider != null && hit.collider.gameObject == gameObject) HandleClick();
    }

    public void HandleClick() {
        if (!TurnManager.Instance.IsCoachTurn) return;
        if (State == GroundState.Stunned) return;
        var options = new List<OptionMenuManager.OptionData>();
        if (!Activated && PlayerType != null) {
            options.Add(new OptionMenuManager.OptionData("Move", () => MoveController.Instance.BeginMove(this)));
        }
        if (options.Count > 0) {
            OptionMenuManager.Instance.ShowMenu(transform.position, options);
        }
    }

    public void SetState(GroundState s) {
        State = s;
        ApplyStateVisual();
    }

    void ApplyActivationVisual() {
        if (sr == null) return;
        var c = sr.color;
        c.a = activated ? 0.5f : 1f;
        sr.color = c;
    }

    void ApplyStateVisual() {
        if (sr == null) return;
        bool onGround = State == GroundState.Prone || State == GroundState.Stunned;
        transform.localEulerAngles = onGround ? new Vector3(0f, 0f, 90f) : Vector3.zero;
        if (stunOverlay != null) stunOverlay.gameObject.SetActive(State == GroundState.Stunned);
    }
}