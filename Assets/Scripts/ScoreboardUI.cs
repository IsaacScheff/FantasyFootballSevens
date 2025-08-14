using UnityEngine;
using TMPro;

public class ScoreboardUI : MonoBehaviour {
    [SerializeField] TMP_Text homeTeamText;
    [SerializeField] TMP_Text homeScoreText;
    [SerializeField] TMP_Text awayTeamText;
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
        if (homeTeamText) homeTeamText.text = tm.HomeTeamName;
        if (awayTeamText) awayTeamText.text = tm.AwayTeamName;
        if (homeScoreText) homeScoreText.text = tm.HomeScore.ToString();
        if (awayScoreText) awayScoreText.text = tm.AwayScore.ToString();
        if (turnText) turnText.text = tm.TurnLabel();
    }
}