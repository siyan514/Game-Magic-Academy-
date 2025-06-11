using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartScene : MonoBehaviour
{
    public Button startButton;
    public Button exitButton;
    public Button instructionButton;
    public Button mainMenuButton;

    public GameObject mainMenuCanvas; // Reference to the main menu UI canvas
    public GameObject instructionCanvas; // Reference to the instruction UI canvas

    private void Awake()
    {
        // Ensure instruction screen is hidden at start
        instructionCanvas.SetActive(false);
        mainMenuCanvas.SetActive(true);

        // Set up button listeners
        startButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("Game");
        });

        instructionButton.onClick.AddListener(() =>
        {
            // Switch to instruction screen
            mainMenuCanvas.SetActive(false);
            instructionCanvas.SetActive(true);
        });

        mainMenuButton.onClick.AddListener(() =>
        {
            // Return to main menu
            instructionCanvas.SetActive(false);
            mainMenuCanvas.SetActive(true);
        });

        exitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }
}