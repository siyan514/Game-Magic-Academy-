using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    public static int playerCount = 2;
    public static int AICount = 2;

    public GameObject playerPre1;
    public GameObject playerPre2;
    public GameObject AIPlayerPre1;
    public GameObject AIPlayerPre2;
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
        playerManager = gameObject.GetComponent<PlayerManagement>();
        playerManager.playerPre1 = playerPre1;
        playerManager.playerPre2 = playerPre2;
        // playerManager.AIPlayerPre1 = AIPlayerPre1;
        // playerManager.AIPlayerPre2 = AIPlayerPre2;
        playerManager.accessiblePoints = mapController.accessiblePointList;
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
        if (playerManager != null)
            playerManager.DestroyAllPlayers();

        int x = 7;
        int y = 6;

        mapController.initMap(x, y, x * y);
        playerManager.CreatePlayers(mapController);
        // playerManager.CreateAIPlayer(mapController, mapController.accessiblePointList);
        levelCount++;
    }



    public bool IsSuperWall(Vector2 pos)
    {
        return mapController.IsSuperWall(pos);
    }
}