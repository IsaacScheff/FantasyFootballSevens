using UnityEngine;
using System;

public class TurnManager : MonoBehaviour {
    public static TurnManager Instance { get; private set; }

    [SerializeField] string homeTeamName = "Home";
    [SerializeField] string awayTeamName = "Away";
    [SerializeField] int homeScore = 0;
    [SerializeField] int awayScore = 0;
    [SerializeField] int half = 1;
    [SerializeField] int turnInHalf = 1;
    [SerializeField] bool coachTurn = true;

    public event Action Changed;

    void Awake() {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start() {
        Notify();
    }

    public string HomeTeamName => homeTeamName;
    public string AwayTeamName => awayTeamName;
    public int HomeScore => homeScore;
    public int AwayScore => awayScore;
    public int Half => half;
    public int TurnInHalf => turnInHalf;
    public bool IsCoachTurn => coachTurn;

    public void SetTeamNames(string home, string away) {
        homeTeamName = home;
        awayTeamName = away;
        Notify();
    }

    public void SetScore(int home, int away) {
        homeScore = home;
        awayScore = away;
        Notify();
    }

    public void AddHomeScore(int delta = 1) {
        homeScore += delta;
        Notify();
    }

    public void AddAwayScore(int delta = 1) {
        awayScore += delta;
        Notify();
    }

    public string TurnLabel() {
        string halfLabel = half == 1 ? "1st" : "2nd";
        string who = coachTurn ? "Coach" : "Opponent";
        return $"Turn {turnInHalf}/7 — {halfLabel} Half — {who}";
    }

    public void NextTurn() {
        ResetActivations();
        coachTurn = !coachTurn;
        if (coachTurn) {
            turnInHalf++;
            if (turnInHalf > 7) {
                turnInHalf = 1;
                half = Mathf.Clamp(half + 1, 1, 2);
            }
        }
        Notify();
    }

    void ResetActivations() {
        var players = FindObjectsOfType<ClickablePlayer>();
        foreach (var p in players) p.Activated = false;
    }

    void Notify() {
        Changed?.Invoke();
    }
}