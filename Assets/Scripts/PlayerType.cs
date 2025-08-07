using UnityEngine;

[CreateAssetMenu(menuName = "Game/Player Type")]
public class PlayerType : ScriptableObject {
    public string typeName;
    public PlayerStats stats;
    public Sprite icon;
}
