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
    private int time = 0;
    private PlayerManagement playerManager;

    private int player1Wins = 0;
    private int player2Wins = 0;
    private int currentEnemies = 0;

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
        // InitializeUI();
    }

    // private void InitializeUI()
    // {
    //     if (GameUIController.instance != null)
    //     {
    //         // 初始化玩家UI
    //         GameUIController.instance.InitializePlayerUI(1, 3); // 假设初始3心
    //         if (playerCount == 2)
    //             GameUIController.instance.InitializePlayerUI(2, 3);

    //         // 隐藏第二个玩家面板（如果是单人模式）
    //         if (playerCount == 1)
    //             GameUIController.instance.player2Panel.SetActive(false);
    //     }
    // }

    private void Update()
    {
        CountEnemies();

        GameUIController.instance.Refresh(playerManager.players[0].HP, playerManager.players[1].HP, player1Wins, player2Wins,
        levelCount, time, currentEnemies);

        if (CheckGameEnd() && !VictoryScene.instance.active)
        {
            LevelCtrl();
        }
    }

    private void LevelCtrl()
    {
        if (playerManager != null)
            playerManager.DestroyAllPlayers();

        AIEnemy[] enemies = FindObjectsOfType<AIEnemy>();
        foreach (AIEnemy enemy in enemies)
        {
            Destroy(enemy.gameObject);
        }

        PropController[] props = FindObjectsOfType<PropController>();
        foreach (PropController prop in props)
        {
            prop.ResetProp();
            ObjectPool.instance.Add(ObjectType.Prop, prop.gameObject);
        }

        int x = 7;
        int y = 6;

        mapController.initMap(x, y, x * y);
        playerManager.CreatePlayers(mapController);
        // playerManager.CreateAIPlayer(mapController, mapController.accessiblePointList);
        levelCount++;
    }

    private void CountEnemies()
    {
        // 统计所有AIEnemy实例
        AIEnemy[] enemies = FindObjectsOfType<AIEnemy>();
        EnemyWallController[] hiddenEnemies = FindObjectsOfType<EnemyWallController>();

        currentEnemies =  enemies.Length + hiddenEnemies.Length;

        // if (GameUIController.instance != null)
        // {
        //     GameUIController.instance.totalEnemies = currentEnemies;
        //     GameUIController.instance.UpdateEnemyCount(currentEnemies);
        // }
    }

    // 检查游戏结束条件
    private bool CheckGameEnd()
    {
        if (currentEnemies <= 0)
        {
            // 所有敌人被消灭
            if (playerCount == 1)
            {
                // 单人模式：玩家胜利
                player1Wins++;
                VictoryScene.instance.display(1);

                return true;
            }
            else
            {
                // 双人模式：检查存活玩家
                PlayerBase winner = DetermineWinner();
                if (winner != null)
                {
                    if (winner.PlayerIndex == 1)
                    {
                        player1Wins++;
                        VictoryScene.instance.display(1);
                    }
                    else
                    {
                        player2Wins++;
                        VictoryScene.instance.display(2);
                    }
                    return true;
                }
            }
        }

        return false;
    }

    private PlayerBase DetermineWinner()
    {
        PlayerManagement playerManager = FindObjectOfType<PlayerManagement>();
        if (playerManager == null) return null;

        PlayerBase alivePlayer = null;
        foreach (PlayerBase player in playerManager.players)
        {
            if (player.IsActive && player.HP > 0)
            {
                if (alivePlayer == null) alivePlayer = player;
                else return null;
            }
        }
        return alivePlayer;
    }



    public bool IsSuperWall(Vector2 pos)
    {
        return mapController.IsSuperWall(pos);
    }
}