using UnityEngine;
using TMPro;

public class ScoreboardUI : MonoBehaviour {
    [SerializeField] TMP_Text homeNameText;
    [SerializeField] TMP_Text homeScoreText;
    [SerializeField] TMP_Text awayNameText;
    [SerializeField] TMP_Text awayScoreText;
    [SerializeField] TMP_Text turnText;

    void OnEnable() {
        if (TurnManager.Instance != null) TurnManager.Instance.Changed += Refresh;
        Refresh();
    }

    void OnDisable() {
        if (TurnManager.Instance != null) TurnManager.Instance.Changed -= Refresh;
    }

    public void Refresh() {
        var tm = TurnManager.Instance;
        if (tm == null) return;
        if (homeNameText) homeNameText.text = tm.CoachTeamName;
        if (awayNameText) awayNameText.text = tm.AITeamName;
        if (homeScoreText) homeScoreText.text = tm.CoachScore.ToString();
        if (awayScoreText) awayScoreText.text = tm.AIScore.ToString();
        if (turnText) turnText.text = tm.TurnLabel();
    }
}