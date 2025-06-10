using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VictoryScene : MonoBehaviour
{
    public static VictoryScene instance;
    public Image victoryImage; // 用于显示胜利图片
    public Sprite player1WinSprite; // 玩家1胜利图片
    public Sprite player2WinSprite; // 玩家2胜利图片

    public Button nextLevelButton;
    public Button mainMenuButton;
    public int winPlayer;

    private void Awake()
    {
        instance = this;
        gameObject.SetActive(false);
    }

    public void display(int winnerIndex)
    {
        // 获取胜利玩家索引
        gameObject.SetActive(true);

        if (winnerIndex == 1)
        {
            victoryImage.sprite = player1WinSprite;
        }
        else
        {
            victoryImage.sprite = player2WinSprite;
        }


        nextLevelButton.onClick.RemoveAllListeners();
        nextLevelButton.onClick.AddListener(OnNextLevel);

        mainMenuButton.onClick.RemoveAllListeners();
        mainMenuButton.onClick.AddListener(OnMainMenu);
    }

    // 继续下一关
    private void OnNextLevel()
    {
        gameObject.SetActive(false);
    }

    // 返回主菜单
    private void OnMainMenu()
    {
        SceneManager.LoadScene("Start"); // 你的主菜单场景名称
    }
}