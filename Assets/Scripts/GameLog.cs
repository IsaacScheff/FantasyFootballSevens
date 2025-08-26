// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
// using TMPro;

// public class GameLog : MonoBehaviour {
//     public static GameLog Instance { get; private set; }
//     [SerializeField] TMP_Text logText;
//     [SerializeField] ScrollRect scrollRect;
//     [SerializeField] int maxLines = 100;
//     readonly List<string> lines = new List<string>();

//     void Awake() {
//         if (Instance != null && Instance != this) Destroy(gameObject);
//         else Instance = this;
//     }

//     public void Log(string msg) {
//         if (string.IsNullOrEmpty(msg)) return;
//         lines.Add(msg);
//         if (lines.Count > maxLines) lines.RemoveAt(0);
//         if (logText != null) logText.text = string.Join("\n", lines);
//         Canvas.ForceUpdateCanvases();
//         if (scrollRect != null) scrollRect.verticalNormalizedPosition = 0f;
//     }
// }

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameLog : MonoBehaviour {
    public static GameLog Instance { get; private set; }

    [SerializeField] TMP_Text logText;
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] int maxLines = 100;

    readonly List<string> lines = new List<string>();

    void Awake() {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (logText == null) logText = GetComponentInChildren<TMP_Text>(true);
        if (scrollRect == null) scrollRect = GetComponentInChildren<ScrollRect>(true);
    }

    public void Log(string msg) {
        if (string.IsNullOrEmpty(msg)) return;
        lines.Add(msg);
        if (lines.Count > maxLines) lines.RemoveAt(0);
        if (logText != null) logText.text = string.Join("\n", lines);
        Canvas.ForceUpdateCanvases();
        if (scrollRect != null) scrollRect.verticalNormalizedPosition = 0f;
    }

    public void Clear() {
        lines.Clear();
        if (logText != null) logText.text = "";
    }
}