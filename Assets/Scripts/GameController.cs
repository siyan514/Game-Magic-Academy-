using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    public static int playerCount = 2;
    public GameObject playerPre1;
    public GameObject playerPre2;
    private MapController mapController;
    private int levelCount = 0;

    private GameObject player1;
    private GameObject player2;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        mapController = GetComponent<MapController>();
        LevelCtrl();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            LevelCtrl();
        }
    }

    private void CreatePlayers()
    {

    }

    private void LevelCtrl()
    {
        int x = 7 + 1 * (levelCount / 4);
        int y = 5 + 1 * (levelCount / 4);
        if(x >= 8) x = 8;
        if(y >= 8) y = 8;

        mapController.initMap(x, y, x*y);

        if(player1 == null)
        {
            player1 = Instantiate(playerPre1);
        }
        player1.transform.position = mapController.GetPlayerPos(1);
        player1.GetComponent<PlayerController>().Init(1, 1, 1.5f,1);

        if(playerCount == 2)
        {
            if (player2 == null)
            {
                player2 = Instantiate(playerPre2);
            }
            player2.transform.position = mapController.GetPlayerPos(2);
            player2.GetComponent<PlayerController>().Init(1, 1, 1.5f,2);
        }
        levelCount++;
    }

    public bool IsSuperWall(Vector2 pos)
    {
        return mapController.IsSuperWall(pos);
    }
}
