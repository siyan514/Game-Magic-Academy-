using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManagement : MonoBehaviour
{
    public static PlayerManagement instance;
    public GameObject playerPre1;
    public GameObject playerPre2;
    // public GameObject AIPlayerPre1;
    // public GameObject AIPlayerPre2;
    public List<Vector2> accessiblePoints;


    public List<PlayerBase> players = new List<PlayerBase>();

    private void Awake()
    {
        instance = this;
    }


    public void CreatePlayers(MapController mapController)
    {
        // Create Player 1
        GameObject player1Obj = Instantiate(playerPre1);
        player1Obj.transform.position = mapController.GetPlayerPos(1);
        PlayerBase player1 = player1Obj.GetComponent<PlayerBase>();
        player1.Init(1, 2, 1.5f, 1);
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

    // PlayerManagement.cs
    // public void CreateAIPlayer(MapController mapController, List<Vector2> accessiblePoints)
    // {
    //     if (AIPlayerPre1 != null)
    //     {
    //         GameObject aiObj1 = Instantiate(AIPlayerPre1);
    //         aiObj1.transform.position = mapController.GetPlayerPos(3);
    //         PlayerBase aiPlayer1 = aiObj1.GetComponent<PlayerBase>();
    //         aiPlayer1.Init(1, 1, 1.5f, 3);

    //         AIController aiController1 = aiObj1.GetComponent<AIController>();
    //         if (aiController1 != null)
    //         {
    //             // aiController1.SetAccessiblePoints(accessiblePoints);
    //         }

    //         players.Add(aiPlayer1);
    //         print("Created AI Player 3");
    //     }
    //     else
    //     {
    //         Debug.LogError("AIPlayerPre1 is not assigned!");
    //     }

    //     if (GameController.AICount == 2)
    //     {
    //         if (AIPlayerPre2 != null)
    //         {
    //             GameObject aiObj2 = Instantiate(AIPlayerPre2);
    //             aiObj2.transform.position = mapController.GetPlayerPos(4);
    //             PlayerBase aiPlayer2 = aiObj2.GetComponent<PlayerBase>();
    //             aiPlayer2.Init(1, 1, 1.5f, 4);

    //             AIController aiController2 = aiObj2.GetComponent<AIController>();
    //             if (aiController2 != null)
    //             {
    //                 // aiController2.SetAccessiblePoints(accessiblePoints);
    //             }

    //             players.Add(aiPlayer2);
    //             print("Created AI Player 4");
    //         }
    //         else
    //         {
    //             Debug.LogError("AIPlayerPre2 is not assigned!");
    //         }
    //     }

    // }

    public void DestroyAllPlayers()
    {
        foreach (PlayerBase player in players)
        {
            if (player != null)
                Destroy(player.gameObject);
        }
        players.Clear(); // 清空列表
    }
    public PlayerBase GetPlayer(int index)
    {
        return players.Find(p => p.PlayerIndex == index);
    }
}