using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameUIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI victoryText;
    [SerializeField] private GameObject restartScreen;
    [SerializeField] private GameObject gameScreen;
    public GameObject textJ1;
    public GameObject textJ2;
    public TextMeshProUGUI textCAJ;
    public TextMeshProUGUI textCN;

    // Start is called before the first frame update
    void Start()
    {
        UpdateCAJ(DataManager.InstanceDataManager.coupParTour);
    }

    public void UpCamera()
    {
        if (CameraManager.InstanceCamera.cameraActive != 3)
        {
            if (restartScreen.activeSelf) restartScreen.SetActive(false);
            if (GameManager.InstanceGameManager.tour == 1) CameraManager.InstanceCamera.ActiveCameraPlateau1();
            else CameraManager.InstanceCamera.ActiveCameraPlateau2();
        }
        else
        {
            if (!gameScreen.activeSelf) restartScreen.SetActive(true);
            if (GameManager.InstanceGameManager.tour == 1) CameraManager.InstanceCamera.ActiveCameraJ1();
            else CameraManager.InstanceCamera.ActiveCameraJ2();
        }
    }

    public void BackToMenu()
    {
        DataManager.InstanceDataManager.ClearGameString();
        SceneManager.LoadScene(0);
    }

    public void BackToGame()
    {
        if (GameManager.InstanceGameManager.gameOver)
            restartScreen.SetActive(true);
        else
            gameScreen.SetActive(true);
    }

    public void UpdateCAJ(int caj)
    {
        textCAJ.text = "Moves\nleft : " + caj;
    }
    public void UpdateCN(int cn)
    {
        textCN.gameObject.SetActive(true);
        if (cn == 1) textCN.text = "Wait for\nyour turn";
        else textCN.text = "Moves\nnecessary :\n" + cn;
    }
    public void VictoryMenu(int joueur)
    {
        if (joueur == 0)
        {
            victoryText.text = "NO WINNER !";
        }
        else
        {
            victoryText.text = "VICTORY\nPLAYER " + joueur;
        }
        gameScreen.SetActive(false);
        restartScreen.SetActive(true);
    }
    public void Restart()
    {
        DataManager.InstanceDataManager.UpdateGameString(DataManager.InstanceDataManager.tailleTableau + "." + DataManager.InstanceDataManager.coupParTour + ";");
        SceneManager.LoadScene(1);
    }
}
