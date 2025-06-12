using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
/// <summary>
/// The victory page display implementation class
/// </summary>
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
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        gameObject.SetActive(false);  
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    public bool IsActive => gameObject.activeSelf;

    /// <summary>
    /// display the winner page
    /// </summary>
    /// <param name="winnerIndex"></param>
    public void display(int winnerIndex)
    {
        // Obtain the index of winning players
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
    /// <summary>
    /// display the finnal page
    /// </summary>
    /// <param name="winnerIndex"></param>
    public void displayFinalPage(int winnerIndex)
    {
        // Obtain the index of winning players
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

    /// <summary>
    /// display the failed page
    /// </summary>
    public void displayFailed()
    {
        gameObject.SetActive(true);
        victoryImage.sprite = failed;

        returnMainMenuButton.gameObject.SetActive(false);
        nextLevelButton.gameObject.SetActive(false);
        restartLevelButton.gameObject.SetActive(true);
        mainMenuButton.gameObject.SetActive(true);

        restartLevelButton.onClick.RemoveAllListeners();
        restartLevelButton.onClick.AddListener(OnRestartLevel); 

        mainMenuButton.onClick.RemoveAllListeners();
        mainMenuButton.onClick.AddListener(OnMainMenu);
    }
    /// <summary>
    /// player can restart the failed level
    /// </summary>
    private void OnRestartLevel()
    {
        gameObject.SetActive(false);
        GameController.instance.RestartLevel();
    }
    /// <summary>
    /// Move on to the next level
    /// </summary>
    private void OnNextLevel()
    {
        gameObject.SetActive(false);
        GameController.instance.StartNextLevel();
    }
    /// <summary>
    /// Return to the main menu
    /// </summary>
    private void OnMainMenu()
    {
        SceneManager.LoadScene("Start"); 
    }
}