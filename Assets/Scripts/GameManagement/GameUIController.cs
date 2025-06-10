using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameUIController : MonoBehaviour
{
    public static GameUIController instance;
    public Text txt_Level, txt_Enemy, txt_Time;
    public Text txt_HP_1, txt_HP_2;
    public Text txt_win_1, txt_win_2;


    private void Awake()
    {
        instance = this;
    }

    public void Refresh(int hp1, int hp2, int win1, int win2, int level, int time, int enemy)
    {
        txt_HP_1.text = hp1.ToString();
        txt_HP_2.text = hp2.ToString();
        txt_win_1.text = win1.ToString();
        txt_win_2.text = win2.ToString();


        txt_Level.text = "Level: " + level.ToString();
        txt_Enemy.text = enemy.ToString();
        txt_Time.text = time.ToString();
    }


    // [Header("Player UI")]
    // public GameObject player1Panel;
    // public GameObject player2Panel;
    // public Image player1Avatar;
    // public Image player2Avatar;
    // public Transform player1HeartsContainer;
    // public Transform player2HeartsContainer;
    // public Text player1WinText;
    // public Text player2WinText;
    // public GameObject player1DeadText;
    // public GameObject player2DeadText;
    // public GameObject heartPrefab; // 心形预制体

    // [Header("Enemy UI")]
    // public Text enemyCountText;
    // public int totalEnemies; // 敌人总数

    // [Header("Timer UI")]
    // public Text timerText;
    // private float gameTime;

    // // 单例模式
    // public static GameUIController instance;

    // private void Awake()
    // {
    //     if (instance == null) instance = this;
    //     else Destroy(gameObject);
    // }

    // private void Start()
    // {
    //     gameTime = 0f;
    //     player1DeadText.SetActive(false);
    //     player2DeadText.SetActive(false);
    // }

    // private void Update()
    // {
    //     // 更新游戏时间
    //     gameTime += Time.deltaTime;
    //     UpdateTimerUI();
    // }

    // // 初始化玩家UI
    // public void InitializePlayerUI(int playerIndex, int maxHealth)
    // {
    //     Transform container = playerIndex == 1 ? player1HeartsContainer : player2HeartsContainer;

    //     // 清除现有心形
    //     foreach (Transform child in container)
    //         Destroy(child.gameObject);

    //     // 创建新的心形
    //     for (int i = 0; i < maxHealth; i++)
    //     {
    //         Instantiate(heartPrefab, container);
    //     }
    // }

    // // 更新玩家生命值
    // public void UpdatePlayerHealth(int playerIndex, int currentHealth)
    // {
    //     Transform container = playerIndex == 1 ? player1HeartsContainer : player2HeartsContainer;

    //     // 更新心形显示
    //     for (int i = 0; i < container.childCount; i++)
    //     {
    //         Image heart = container.GetChild(i).GetComponent<Image>();
    //         heart.color = i < currentHealth ? Color.white : new Color(0.5f, 0.5f, 0.5f, 0.5f);
    //     }

    //     // 检查死亡状态
    //     if (currentHealth <= 0)
    //     {
    //         if (playerIndex == 1) player1DeadText.SetActive(true);
    //         else player2DeadText.SetActive(true);
    //     }
    // }

    // // 更新敌人计数
    // public void UpdateEnemyCount(int remainingEnemies)
    // {
    //     enemyCountText.text = $"Enemies: {remainingEnemies}/{totalEnemies}";
    // }

    // // 更新计时器
    // private void UpdateTimerUI()
    // {
    //     int minutes = Mathf.FloorToInt(gameTime / 60);
    //     int seconds = Mathf.FloorToInt(gameTime % 60);
    //     timerText.text = $"{minutes:00}:{seconds:00}";
    // }

    // // 更新胜利局数
    // public void UpdateWinCount(int playerIndex, int wins)
    // {
    //     if (playerIndex == 1) player1WinText.text = $"Wins: {wins}";
    //     else player2WinText.text = $"Wins: {wins}";
    // }
}