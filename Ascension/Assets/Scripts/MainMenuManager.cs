using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] GameObject scene0;
    [SerializeField] GameObject scene1;
    [SerializeField] GameObject scene2;
    [SerializeField] GameObject scene3;
    [SerializeField] GameObject scene4;
    [SerializeField] GameObject sceneRules;
    [SerializeField] GameObject sceneSettings;
    [SerializeField] Button backButton;
    [SerializeField] Button settingsButton;
    [SerializeField] Button button2;
    [SerializeField] Button button4;

    private int sceneNumber = 0;

    /// <summary>
    /// Quit the game
    /// </summary>
    public void QuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    /// <summary>
    /// Return to the previous screen
    /// </summary>
    public void Back()
    {
        sceneNumber--;
        OpenScreen(sceneNumber);
    }

    /// <summary>
    /// Open the chosen scene
    /// </summary>
    /// <param name="number">Chosen scene's number</param>
    public void OpenScreen(int number)
    {
        CloseAllScreens();
        switch (number)
        {
            case 0:
                scene0.SetActive(true);
                backButton.gameObject.SetActive(false);
                break;
            case 1:
                scene1.SetActive(true);
                backButton.gameObject.SetActive(true);
                break;
            case 2:
                scene2.SetActive(true);
                backButton.gameObject.SetActive(true);
                break;
            case 3:
                scene3.SetActive(true);
                backButton.gameObject.SetActive(true);
                break;
            case 4:
                scene4.SetActive(true);
                backButton.gameObject.SetActive(true);
                break;
            default:
                OpenScreen(sceneNumber);
                settingsButton.gameObject.SetActive(true);
                break;
        }
        if (number >= 0) sceneNumber = number;
    }

    /// <summary>
    /// Close all the UI screens
    /// </summary>
    public void CloseAllScreens()
    {
        scene0.SetActive(false);
        scene1.SetActive(false);
        scene2.SetActive(false);
        scene3.SetActive(false);
        scene4.SetActive(false);
        sceneRules.SetActive(false);
        sceneSettings.SetActive(false);
    }

    /// <summary>
    /// Make Player1 a player if 1, Player2 a player if 2
    /// </summary>
    /// <param name="number">Number of the player</param>
    public void ChoosePlayer(int number)
    {
        if (number == 1) DataManager.InstanceDataManager.modeJ1 = "J";
        else DataManager.InstanceDataManager.modeJ2 = "J";
    }
    /// <summary>
    /// Make Player1 an AI if 1, Player2 an AI if 2
    /// </summary>
    /// <param name="number">Number of the player</param>
    public void ChooseAI(int number)
    {
        if (number == 1) DataManager.InstanceDataManager.modeJ1 = "IA";
        else DataManager.InstanceDataManager.modeJ2 = "IA";
    }

    /// <summary>
    /// Make the number of tile on the board equal to size
    /// Disable the possibilities of having :
    /// Size = 15 and Move per turn = 2
    /// Size = 11 and Move per turn = 4
    /// (Bad modes)
    /// </summary>
    /// <param name="size">Number of tile on the board</param>
    public void ChooseSize(int size)
    {
        DataManager.InstanceDataManager.tailleTableau = size;
        if (size == 15)
        {
            button2.interactable = false;
            button4.interactable = true;
        }
        else if (size == 11)
        {
            button2.interactable = true;
            button4.interactable = false;
        }
        else
        {
            button2.interactable = true;
            button4.interactable = true;
        }
    }

    /// <summary>
    /// Make the number of move per turn equal to cPT
    /// Start the game
    /// </summary>
    /// <param name="cPT">Number of move per turn</param>
    public void ChooseMovePerTurn(int cPT)
    {
        DataManager.InstanceDataManager.coupParTour = cPT;
        DataManager.InstanceDataManager.UpdateGameString(DataManager.InstanceDataManager.tailleTableau + "." + DataManager.InstanceDataManager.coupParTour + ";");
        SceneManager.LoadScene(1);
    }


    public void SetAIDelay(bool condition)
    {
        if (condition) DataManager.InstanceDataManager.delayAI = true;
        else DataManager.InstanceDataManager.delayAI = false;
    }
}
