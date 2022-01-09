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
    [SerializeField] Button backButton;
    [SerializeField] Button rulesButton;
    [SerializeField] Button button2;
    [SerializeField] Button button4;

    private int sceneNumber = 0 ;

    public void StartGame()
    {
        scene0.SetActive(false);
        scene1.SetActive(true);
        backButton.gameObject.SetActive(true);
        sceneNumber++;
    }
    public void QuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
    public void Back()
    {
        if (sceneRules.activeSelf)
        {
            rulesButton.gameObject.SetActive(true);
            if (sceneNumber == 0)
            {
                scene0.SetActive(true);
                sceneRules.SetActive(false);
                backButton.gameObject.SetActive(false);
            }
            else if (sceneNumber == 1)
            {
                scene1.SetActive(true);
                sceneRules.SetActive(false);
            }
            else if (sceneNumber == 2)
            {
                scene2.SetActive(true);
                sceneRules.SetActive(false);
            }
            else if (sceneNumber == 3)
            {
                scene3.SetActive(true);
                sceneRules.SetActive(false);
            }
            else if (sceneNumber == 4)
            {
                scene4.SetActive(true);
                sceneRules.SetActive(false);
            }
        }
        else
        {
            if (sceneNumber == 1)
            {
                scene1.SetActive(false);
                scene0.SetActive(true);
                backButton.gameObject.SetActive(false);
            }
            else if (sceneNumber == 2)
            {
                scene2.SetActive(false);
                scene1.SetActive(true);
            }
            else if (sceneNumber == 3)
            {
                scene3.SetActive(false);
                scene2.SetActive(true);
            }
            else if (sceneNumber == 4)
            {
                scene4.SetActive(false);
                scene3.SetActive(true);
            }
            sceneNumber--;
        }
    }
    public void Rules()
    {
        backButton.gameObject.SetActive(true);
        scene0.SetActive(false);
        scene1.SetActive(false);
        scene2.SetActive(false);
        scene3.SetActive(false);
        scene4.SetActive(false);
        sceneRules.SetActive(true);
        rulesButton.gameObject.SetActive(false);
    }
    public void ChoosePlayer(int number)
    {
        sceneNumber++;
        if (number == 1)
        {
            scene1.SetActive(false);
            scene2.SetActive(true);
            DataManager.InstanceDataManager.modeJ1 = "J";
        }
        else if (number == 2)
        {
            scene2.SetActive(false);
            scene3.SetActive(true);
            DataManager.InstanceDataManager.modeJ2 = "J";
        }
    }
    public void ChooseAI(int number)
    {
        sceneNumber++;
        if (number == 1)
        {
            scene1.SetActive(false);
            scene2.SetActive(true);
            DataManager.InstanceDataManager.modeJ1 = "IA";
        }
        else if (number == 2)
        {
            scene2.SetActive(false);
            scene3.SetActive(true);
            DataManager.InstanceDataManager.modeJ2 = "IA";
        }
    }
    public void ChooseSize(int size)
    {
        DataManager.InstanceDataManager.tailleTableau = size;
        scene3.SetActive(false);
        scene4.SetActive(true);
        sceneNumber++;
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
    public void ChooseMovePerTurn(int cPT)
    {
        DataManager.InstanceDataManager.coupParTour = cPT;
        DataManager.InstanceDataManager.UpdateGameString(DataManager.InstanceDataManager.tailleTableau + "." + DataManager.InstanceDataManager.coupParTour + ";");
        SceneManager.LoadScene(1);
    }
}
