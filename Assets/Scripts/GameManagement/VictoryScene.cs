using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VictoryScene : MonoBehaviour
{
    public static VictoryScene instance;
    public Image victoryImage;
    public Sprite player1WinSprite;
    public Sprite player2WinSprite; 
    public Sprite player1FinalWin;
    public Sprite player2FinalWin;
    public Sprite equalWin;
    public Sprite failed;
    public Button restartLevelButton;
    public Button nextLevelButton;
    public Button mainMenuButton;
    public Button returnMainMenuButton;
    public int winPlayer;

    private void Awake()
    {
        instance = this;
        gameObject.SetActive(false);
    }

    public bool IsActive => gameObject.activeSelf;

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

        nextLevelButton.gameObject.SetActive(true);
        mainMenuButton.gameObject.SetActive(true);
        restartLevelButton.gameObject.SetActive(false);
        returnMainMenuButton.gameObject.SetActive(false);

        nextLevelButton.onClick.RemoveAllListeners();
        nextLevelButton.onClick.AddListener(OnNextLevel);

        mainMenuButton.onClick.RemoveAllListeners();
        mainMenuButton.onClick.AddListener(OnMainMenu);
    }

    public void displayFinalPage(int winnerIndex)
    {
        // 获取胜利玩家索引
        gameObject.SetActive(true);

        if (winnerIndex == 1)
        {
            victoryImage.sprite = player1FinalWin;
        }
        else if (winnerIndex == 2)
        {
            victoryImage.sprite = player2FinalWin;
        }
        else
        {
            victoryImage.sprite = equalWin;
        }

        nextLevelButton.gameObject.SetActive(false);
        mainMenuButton.gameObject.SetActive(false);
        restartLevelButton.gameObject.SetActive(false);
        returnMainMenuButton.gameObject.SetActive(true);


        returnMainMenuButton.onClick.RemoveAllListeners();
        returnMainMenuButton.onClick.AddListener(OnMainMenu);
    }
    public void displayFailed()
    {
        gameObject.SetActive(true);
        victoryImage.sprite = failed;

        returnMainMenuButton.gameObject.SetActive(false);
        nextLevelButton.gameObject.SetActive(false);
        restartLevelButton.gameObject.SetActive(true);
        mainMenuButton.gameObject.SetActive(true);

        restartLevelButton.onClick.RemoveAllListeners();
        restartLevelButton.onClick.AddListener(OnRestartLevel); // 修改为重新开始当前关卡

        mainMenuButton.onClick.RemoveAllListeners();
        mainMenuButton.onClick.AddListener(OnMainMenu);
    }

    private void OnRestartLevel()
    {
        Debug.Log("重新开始当前关卡");
        gameObject.SetActive(false);
        GameController.instance.RestartLevel();
    }

    // 继续下一关
    private void OnNextLevel()
    {
        Debug.Log("按了");
        gameObject.SetActive(false);
        GameController.instance.StartNextLevel();
    }

    // 返回主菜单
    private void OnMainMenu()
    {
        SceneManager.LoadScene("Start"); // 你的主菜单场景名称
    }
}