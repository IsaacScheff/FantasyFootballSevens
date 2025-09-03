using UnityEngine;

[CreateAssetMenu(menuName = "Game/PlayerType")]
public class PlayerType : ScriptableObject {
    public string typeName;

    [System.Serializable]
    public struct Statline {
        public int Spd;
        public int Str;
        public int Dex;
        public int Kac;
        public int Con;
    }

    public Statline stats;
    public Sprite redSprite;
    public Sprite blueSprite;
}