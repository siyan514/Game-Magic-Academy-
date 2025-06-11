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
    private MapController mapController;
    private int levelCount = 0;
    private int time = 0;
    private PlayerManagement playerManager;

    private int player1Wins = 0;
    private int player2Wins = 0;
    private int currentEnemies = 0;

    private int currentWinner = 0;
    public bool interval = false;
    

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
        playerManager.accessiblePoints = mapController.accessiblePointList;
        LevelCtrl();
    }

    private void Update()
    {
        if (!interval)
        {
            CountEnemies();

            GameUIController.instance.Refresh(playerManager.players[0].HP, playerManager.players[1].HP, player1Wins, player2Wins,
            levelCount, currentEnemies);

            bool gameEnded = CheckGameEnd();
            if (gameEnded)
            {
                interval = true;
            }
        }
        
    }
    public void StartNextLevel()
    {
        LevelCtrl();
        interval = false;
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

        levelCount++;
        mapController.initMap(x, y, x * y, levelCount);
        playerManager.CreatePlayers(mapController);
    }

    private void CountEnemies()
    {
        AIEnemy[] enemies = FindObjectsOfType<AIEnemy>();
        EnemyWallController[] hiddenEnemies = FindObjectsOfType<EnemyWallController>();

        currentEnemies =  enemies.Length + hiddenEnemies.Length;
    }
    public void RestartLevel()
    {
        levelCount--;
        LevelCtrl();
        interval = false;
    }

    private bool CheckGameEnd()
    {
        if (CheckAllPlayersDead())
        {
            // 添加空引用检查
            if (VictoryScene.instance != null)
            {
                VictoryScene.instance.displayFailed();
            }
            else
            {
                Debug.LogError("VictoryScene instance is null! Cannot display failure screen.");
            }
            return true;
        }

        if (currentEnemies <= 0)
        {
            int alivePlayers = CountAlivePlayers();
            if (playerCount == 1)
            {
                return HandleVictory(1);

            }
            else 
            {
                if (alivePlayers == 1)
                {
                    PlayerBase winner = DetermineWinner();

                    if (winner != null)
                    {
                        return HandleVictory(winner.PlayerIndex);
                    }
                }
            }
        }
        return false;
    }

    private bool HandleVictory(int winnerIndex)
    {
        // 更新胜利计数
        if (winnerIndex == 1)
            player1Wins++;
        else
            player2Wins++;

        currentWinner = winnerIndex;

        // 检查是否为最后一关
        if (levelCount >= 3) // 注意：levelCount从0开始计数
        {
            // 判断最终胜利者
            int finalWinner = 0; // 0表示平局
            if (player1Wins > player2Wins)
                finalWinner = 1;
            else if (player2Wins > player1Wins)
                finalWinner = 2;

            VictoryScene.instance.displayFinalPage(finalWinner);
        }
        else
        {
            // 普通关卡胜利
            VictoryScene.instance.display(winnerIndex);
        }

        return true;
    }

    private bool CheckAllPlayersDead()
    {
        if (playerManager == null) return false;

        foreach (PlayerBase player in playerManager.players)
        {
            if (player != null && player.HP > 0)
                return false;
        }

        return true;
    }

    // 新增辅助方法：统计存活玩家数
    private int CountAlivePlayers()
    {
        if (playerManager == null) return 0;

        int aliveCount = 0;
        foreach (PlayerBase player in playerManager.players)
        {
            if (player != null && player.HP > 0)
                aliveCount++;
        }
        return aliveCount;
    }

    private PlayerBase DetermineWinner()
    {
        if (playerManager == null) return null;

        PlayerBase alivePlayer = null;

        foreach (PlayerBase player in playerManager.players)
        {
            if (player != null && player.HP > 0)
            {
                Debug.Log("这里"+player.PlayerIndex + player.HP);
                alivePlayer = player;
            }
        }

        // 只有当有且只有一个玩家存活时才返回胜利者
        return alivePlayer;
    }



    public bool IsSuperWall(Vector2 pos)
    {
        return mapController.IsSuperWall(pos);
    }
}