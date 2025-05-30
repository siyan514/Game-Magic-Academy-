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
    private PlayerManagement playerManager;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        mapController = GetComponent<MapController>();
        playerManager = gameObject.AddComponent<PlayerManagement>();
        playerManager.playerPre1 = playerPre1;
        playerManager.playerPre2 = playerPre2;
        LevelCtrl();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            LevelCtrl();
        }
    }

    private void LevelCtrl()
    {
        int x = 7 + 1 * (levelCount / 4);
        int y = 5 + 1 * (levelCount / 4);
        if (x >= 8) x = 8;
        if (y >= 8) y = 8;

        mapController.initMap(x, y, x * y);
        playerManager.CreatePlayers(mapController);
        levelCount++;
    }

    public bool IsSuperWall(Vector2 pos)
    {
        return mapController.IsSuperWall(pos);
    }
}