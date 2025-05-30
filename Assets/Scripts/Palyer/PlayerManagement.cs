using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManagement : MonoBehaviour
{
    public static PlayerManagement instance;
    public GameObject playerPre1;
    public GameObject playerPre2;
    private List<PlayerBase> players = new List<PlayerBase>();

    private void Awake()
    {
        instance = this;
    }

    public void CreatePlayers(MapController mapController)
    {
        // 创建玩家1
        GameObject player1Obj = Instantiate(playerPre1);
        player1Obj.transform.position = mapController.GetPlayerPos(1);
        PlayerBase player1 = player1Obj.GetComponent<PlayerBase>();
        player1.Init(1, 1, 1.5f, 1);
        players.Add(player1);

        // 创建玩家2（如果是双人模式）
        if (GameController.playerCount == 2)
        {
            GameObject player2Obj = Instantiate(playerPre2);
            player2Obj.transform.position = mapController.GetPlayerPos(2);
            PlayerBase player2 = player2Obj.GetComponent<PlayerBase>();
            player2.Init(1, 1, 1.5f, 2);
            players.Add(player2);
        }
    }

    public PlayerBase GetPlayer(int index)
    {
        return players.Find(p => p.PlayerIndex == index);
    }
}