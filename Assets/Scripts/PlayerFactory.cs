using UnityEngine;

public class PlayerFactory : MonoBehaviour {
    public GameObject playerPrefab;
    public PlayerType[] allTypes;

    public void SpawnPlayer(PlayerType type, Vector2Int coords) {
        Tile tile = BoardManager.Instance.GetTile(coords);
        Vector3 pos = tile.transform.position;
        GameObject go = Instantiate(playerPrefab, pos, Quaternion.identity, transform);
        ClickablePlayer cp = go.GetComponent<ClickablePlayer>();
        cp.Tile = tile;
        tile.SetOccupant(cp);
        cp.PlayerType = type;
    }
}