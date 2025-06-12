using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Manege all players class
/// </summary>
public class PlayerManagement : MonoBehaviour
{
    public static PlayerManagement instance;
    public GameObject playerPre1;
    public GameObject playerPre2;
    public List<Vector2> accessiblePoints;
    public List<PlayerBase> players = new List<PlayerBase>();

    private void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// create the players
    /// </summary>
    /// <param name="mapController"></param>
    public void CreatePlayers(MapController mapController)
    {
        // Create Player 1
        GameObject player1Obj = Instantiate(playerPre1);
        player1Obj.transform.position = mapController.GetPlayerPos(1);
        PlayerBase player1 = player1Obj.GetComponent<PlayerBase>();
        player1.Init(1, 1, 1.5f, 1);
        players.Add(player1);

        // Create Player 2 (if in two-player mode)
        if (GameController.playerCount == 2)
        {
            GameObject player2Obj = Instantiate(playerPre2);
            player2Obj.transform.position = mapController.GetPlayerPos(2);
            PlayerBase player2 = player2Obj.GetComponent<PlayerBase>();
            player2.Init(1, 1, 1.5f, 2);
            players.Add(player2);
        }
    }
    /// <summary>
    /// destroy all players 
    /// </summary>
    public void DestroyAllPlayers()
    {
        foreach (PlayerBase player in players)
        {
            if (player != null)
                Destroy(player.gameObject);
        }
        players.Clear(); 
    }

    public PlayerBase GetPlayer(int index)
    {
        return players.Find(p => p.PlayerIndex == index);
    }
}