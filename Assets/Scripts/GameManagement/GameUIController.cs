using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameUIController : MonoBehaviour
{
    public static GameUIController instance;
    public Text txt_Level, txt_Enemy;
    public Text txt_HP_1, txt_HP_2;
    public Text txt_win_1, txt_win_2;
    public Button mainMenuButton;


    private void Awake()
    {
        instance = this;
    }

    public void Refresh(int hp1, int hp2, int win1, int win2, int level, int enemy)
    {
        txt_HP_1.text = hp1.ToString();
        txt_HP_2.text = hp2.ToString();
        txt_win_1.text = win1.ToString();
        txt_win_2.text = win2.ToString();


        txt_Level.text = "Level: " + level.ToString();
        txt_Enemy.text = enemy.ToString();

        mainMenuButton.onClick.RemoveAllListeners();
        mainMenuButton.onClick.AddListener(OnMainMenu);
    }
    private void OnMainMenu()
    {
        SceneManager.LoadScene("Start"); // 你的主菜单场景名称
    }
}