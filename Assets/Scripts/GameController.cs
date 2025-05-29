using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    public GameObject playerPre;
    private MapController mapController;
    private int levelCount = 0;

    private GameObject player;

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

    private void LevelCtrl()
    {
        int x = 7 + 1 * (levelCount / 4);
        int y = 5 + 1 * (levelCount / 4);
        if(x >= 8) x = 8;
        if(y >= 8) y = 8;

        mapController.initMap(x, y, x*y);
        if(player == null)
        {
            player = Instantiate(playerPre);
        }
        player.transform.position = mapController.GetPlayerPos();
        player.GetComponent<PlayerController>().Init(1, 1, 1.5f);

        levelCount++;
    }

    public bool IsSuperWall(Vector2 pos)
    {
        return mapController.IsSuperWall(pos);
    }
}
