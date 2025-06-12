using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// The whole game controller implementation class
/// </summary>
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
    /// <summary>
    /// update the game progress, check if current game is end 
    /// </summary>
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

    /// <summary>
    /// start the next level
    /// </summary>
    public void StartNextLevel()
    {
        LevelCtrl();
        interval = false;
    }
    /// <summary>
    /// level control function, initialize the next level
    /// </summary>
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
    /// <summary>
    /// get the number of enemies
    /// </summary>
    private void CountEnemies()
    {
        AIEnemy[] enemies = FindObjectsOfType<AIEnemy>();
        EnemyWallController[] hiddenEnemies = FindObjectsOfType<EnemyWallController>();

        currentEnemies =  enemies.Length + hiddenEnemies.Length;
    }
    /// <summary>
    /// if the current level failed , player can resart the current level
    /// </summary>
    public void RestartLevel()
    {
        levelCount--;
        LevelCtrl();
        interval = false;
    }
    /// <summary>
    /// check if the current game is end
    /// </summary>
    /// <returns></returns>
    private bool CheckGameEnd()
    {
        if (CheckAllPlayersDead())
        {
            // Add an empty reference check
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

    /// <summary>
    /// if any player win, than display the winner page
    /// </summary>
    /// <param name="winnerIndex"></param>
    /// <returns></returns>
    private bool HandleVictory(int winnerIndex)
    {
        // Update the victory count
        if (winnerIndex == 1)
            player1Wins++;
        else
            player2Wins++;

        currentWinner = winnerIndex;

        // Check if it is the last level
        if (levelCount >= 3) 
        {
            // Judge the ultimate winner
            int finalWinner = 0; 
            if (player1Wins > player2Wins)
                finalWinner = 1;
            else if (player2Wins > player1Wins)
                finalWinner = 2;

            VictoryScene.instance.displayFinalPage(finalWinner);
        }
        else
        {
            // Victory in the ordinary level
            VictoryScene.instance.display(winnerIndex);
        }

        return true;
    }
    /// <summary>
    /// check if all players dead
    /// </summary>
    /// <returns></returns>
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
    /// <summary>
    /// Count the number of surviving players
    /// </summary>
    /// <returns></returns>
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
    /// <summary>
    /// return the winner
    /// </summary>
    /// <returns></returns>
    private PlayerBase DetermineWinner()
    {
        if (playerManager == null) return null;

        PlayerBase alivePlayer = null;

        foreach (PlayerBase player in playerManager.players)
        {
            if (player != null && player.HP > 0)
            {
                Debug.Log("这里" + player.PlayerIndex + player.HP);
                alivePlayer = player;
            }
        }

        return alivePlayer;
    }
    /// <summary>
    /// check if the wall is superwall that cannot be destroyed
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public bool IsSuperWall(Vector2 pos)
    {
        return mapController.IsSuperWall(pos);
    }
}