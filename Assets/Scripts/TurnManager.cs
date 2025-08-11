using UnityEngine;
using System;

public class TurnManager : MonoBehaviour {
    public static TurnManager Instance { get; private set; }

    [SerializeField] string coachTeamName = "Home";
    [SerializeField] string aiTeamName = "Away";
    [SerializeField] int coachScore = 0;
    [SerializeField] int aiScore = 0;
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

    public string CoachTeamName => coachTeamName;
    public string AITeamName => aiTeamName;
    public int CoachScore => coachScore;
    public int AIScore => aiScore;
    public int Half => half;
    public int TurnInHalf => turnInHalf;
    public bool IsCoachTurn => coachTurn;

    public void SetTeamNames(string coach, string ai) {
        coachTeamName = coach;
        aiTeamName = ai;
        Notify();
    }

    public void SetScore(int coach, int ai) {
        coachScore = coach;
        aiScore = ai;
        Notify();
    }

    public string TurnLabel() {
        string halfLabel = half == 1 ? "1st" : "2nd";
        string who = coachTurn ? "Coach" : "Opponent";
        return $"Turn {turnInHalf}/7 — {halfLabel} Half — {who}";
    }

    public void NextTurn() {
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

    void Notify() {
        Changed?.Invoke();
    }
}