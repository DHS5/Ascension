using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Camera cameraJ1;
    // Tableau 11 : pos(4.1, 16, 18) rot(30, 180, 0)
    // Tableau 13 : pos(4.1, 18, 18.8) rot(35, 180, 0)
    // Tableau 15 : pos(4.1, 20, 19.3) rot(40, 180, 0)
    [SerializeField] private Camera cameraJ2;
    // Tableau 11 : pos(4.1, 16, 1.75) rot(30, 0, 0)
    // Tableau 13 : pos(4.1, 18, 1) rot(35, 0, 0)
    // Tableau 15 : pos(4.1, 20, 0.5) rot(40, 0, 0)
    [SerializeField] private Camera cameraPlateau1;
    [SerializeField] private Camera cameraPlateau2;

    public static CameraManager InstanceCamera { get; private set; }

    [Tooltip("")]
    public int cameraActive = 1;

    private int taille = DataManager.InstanceDataManager.tailleTableau;

    /// <summary>
    /// Initialize the camera
    /// </summary>
    private void Start()
    {
        InstanceCamera = this;
        AngleCamera();
    }

    /// <summary>
    /// Activate the camera with a player view of the board for Player1
    /// </summary>
    public void ActiveCameraJ1()
    {
        cameraJ1.gameObject.SetActive(true);
        cameraJ2.gameObject.SetActive(false);
        cameraPlateau1.gameObject.SetActive(false);
        cameraPlateau2.gameObject.SetActive(false);
        cameraActive = 1;
    }

    /// <summary>
    /// Activate the camera with a player view of the board for Player2
    /// </summary>
    public void ActiveCameraJ2()
    {
        cameraJ1.gameObject.SetActive(false);
        cameraJ2.gameObject.SetActive(true);
        cameraPlateau1.gameObject.SetActive(false);
        cameraPlateau2.gameObject.SetActive(false);
        cameraActive = 2;
    }

    /// <summary>
    /// Activate the camera with a top view of the board for Player1
    /// </summary>
    public void ActiveCameraPlateau1()
    {
        cameraJ1.gameObject.SetActive(false);
        cameraJ2.gameObject.SetActive(false);
        cameraPlateau1.gameObject.SetActive(true);
        cameraPlateau2.gameObject.SetActive(false);
        cameraActive = 3;
    }

    /// <summary>
    /// Activate the camera with a top view of the board for Player2
    /// </summary>
    public void ActiveCameraPlateau2()
    {
        cameraJ1.gameObject.SetActive(false);
        cameraJ2.gameObject.SetActive(false);
        cameraPlateau1.gameObject.SetActive(false);
        cameraPlateau2.gameObject.SetActive(true);
        cameraActive = 3;
    }

    /// <summary>
    /// Place the camera at the right position and angle given the size of the board
    /// </summary>
    private void AngleCamera()
    {
        if (taille == 11)
        {
            cameraJ1.transform.localPosition = new Vector3(0f, 7.25f, 8f);
            cameraJ1.transform.eulerAngles = new Vector3(30, 180, 0);
            cameraJ2.transform.localPosition = new Vector3(0f, 7.25f, -8f);
            cameraJ2.transform.eulerAngles = new Vector3(30, 0, 0);
            cameraPlateau1.transform.localPosition = new Vector3(0f, 7.25f, 0f);
            cameraPlateau2.transform.localPosition = new Vector3(0f, 7.25f, 0f);
        }
        else if (taille == 13)
        {
            cameraJ1.transform.localPosition = new Vector3(0f, 7.75f, 8.5f);
            cameraJ1.transform.eulerAngles = new Vector3(35, 180, 0);
            cameraJ2.transform.localPosition = new Vector3(0f, 7.75f, -8.5f);
            cameraJ2.transform.eulerAngles = new Vector3(35, 0, 0);
            cameraPlateau1.transform.localPosition = new Vector3(0f, 8.25f, 0f);
            cameraPlateau2.transform.localPosition = new Vector3(0f, 8.25f, 0f);
        }
        else if (taille == 15)
        {
            cameraJ1.transform.localPosition = new Vector3(0f, 8.25f, 9f);
            cameraJ1.transform.eulerAngles = new Vector3(40, 180, 0);
            cameraJ2.transform.localPosition = new Vector3(0f, 8.25f, -9f);
            cameraJ2.transform.eulerAngles = new Vector3(40, 0, 0);
            cameraPlateau1.transform.localPosition = new Vector3(0f, 9.25f, 0f);
            cameraPlateau2.transform.localPosition = new Vector3(0f, 9.25f, 0f);
        }
    }
}
