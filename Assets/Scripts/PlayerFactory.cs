using System.Collections.Generic;
using UnityEngine;

public class PlayerFactory : MonoBehaviour {
    public GameObject playerPrefab;
    public List<PlayerType> allTypes = new List<PlayerType>();

    public ClickablePlayer SpawnPlayer(PlayerType type, Vector2Int coords, TeamSide team) {
        var bm = BoardManager.Instance;
        var tile = bm.GetTile(coords);
        var go = Instantiate(playerPrefab, tile.transform.position, Quaternion.identity, bm.transform);

        var cp = go.GetComponent<ClickablePlayer>();
        cp.PlayerType = type;
        cp.Team = team;
        cp.Tile = tile;
        cp.MoveLeft = type.stats.Spd;

        var sr = go.GetComponent<SpriteRenderer>() ?? go.GetComponentInChildren<SpriteRenderer>();
        if (sr != null) sr.sprite = team == TeamSide.Home ? type.redSprite : type.blueSprite;

        tile.SetOccupant(cp);
        return cp;
    }
}