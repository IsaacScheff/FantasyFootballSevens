using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameLog : MonoBehaviour {
    public static GameLog Instance { get; private set; }

    [SerializeField] TMP_Text logText;
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] RectTransform content;   // ScrollRect.content
    [SerializeField] RectTransform viewport;  // ScrollRect.viewport
    [SerializeField] int maxLines = 200;
    [SerializeField] float paddingBottom = 16f;

    readonly List<string> lines = new List<string>();
    bool stickToBottom = true;

    void Awake() {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (scrollRect == null) scrollRect = GetComponentInChildren<ScrollRect>(true);
        if (logText == null) logText = GetComponentInChildren<TMP_Text>(true);
        if (content == null && scrollRect != null) content = scrollRect.content;
        if (viewport == null && scrollRect != null) viewport = scrollRect.viewport;

        if (scrollRect != null) {
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scrollRect.inertia = false;
            scrollRect.onValueChanged.AddListener(v => stickToBottom = v.y <= 0.001f);
        }
    }

    public void Log(string msg) {
        if (string.IsNullOrEmpty(msg)) return;
        lines.Add(msg);
        if (lines.Count > maxLines) lines.RemoveAt(0);
        if (logText != null) logText.text = string.Join("\n", lines);
        StartCoroutine(ResizeAndSnap());
    }

    public void Clear() {
        lines.Clear();
        if (logText != null) logText.text = "";
        StartCoroutine(ResizeAndSnap());
    }

    IEnumerator ResizeAndSnap() {
        yield return null; // wait for TMP to update preferredHeight
        if (content != null && logText != null && viewport != null) {
            float needed = Mathf.Max(viewport.rect.height, logText.preferredHeight + paddingBottom);
            content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, needed);
        }
        Canvas.ForceUpdateCanvases();
        if (scrollRect != null && stickToBottom) scrollRect.verticalNormalizedPosition = 0f;
    }
}