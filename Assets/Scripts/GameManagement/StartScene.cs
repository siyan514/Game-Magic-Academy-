using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartScene : MonoBehaviour
{
    public Button StartButton, ExitButton;

    private void Awake()
    {
        StartButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("Game");
        });
        // InstructionButton.onClick.AddListener(() =>
        // {

        // });
        ExitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }
}
