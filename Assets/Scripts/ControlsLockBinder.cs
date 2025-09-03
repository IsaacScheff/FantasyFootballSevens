using System.Collections;
using UnityEngine;

public class ControlsLockBinder : MonoBehaviour {
    public CanvasGroup targetGroup;
    bool subscribed;

    void Awake() {
        if (targetGroup == null) targetGroup = GetComponent<CanvasGroup>();
        if (targetGroup == null) targetGroup = gameObject.AddComponent<CanvasGroup>();
    }

    void OnEnable() {
        StartCoroutine(BindWhenReady());
        Sync();
    }

    void OnDisable() {
        var mc = MoveController.Instance;
        if (mc != null && subscribed) {
            mc.AnimationStateChanged -= OnAnim;
            subscribed = false;
        }
    }

    IEnumerator BindWhenReady() {
        while (MoveController.Instance == null) yield return null;

        MoveController.Instance.AnimationStateChanged += OnAnim;
        subscribed = true;
       
        Sync();  // Align initial state to current animation state
    }

    void OnAnim(bool animating) {
        SetLocked(animating);
    }

    void Sync() {
        bool anim = MoveController.Instance != null && MoveController.Instance.IsAnimating;
        SetLocked(anim);
    }

    void SetLocked(bool locked) {
        targetGroup.interactable = !locked;
        targetGroup.blocksRaycasts = !locked;
    }
}