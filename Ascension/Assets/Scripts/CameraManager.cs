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

    public int cameraActive = 1;

    private int angle = DataManager.InstanceDataManager.tailleTableau;

    private void Start()
    {
        InstanceCamera = this;
        AngleCamera();
    }

    public void ActiveCameraJ1()
    {
        cameraJ1.gameObject.SetActive(true);
        cameraJ2.gameObject.SetActive(false);
        cameraPlateau1.gameObject.SetActive(false);
        cameraPlateau2.gameObject.SetActive(false);
        cameraActive = 1;
    }
    public void ActiveCameraJ2()
    {
        cameraJ1.gameObject.SetActive(false);
        cameraJ2.gameObject.SetActive(true);
        cameraPlateau1.gameObject.SetActive(false);
        cameraPlateau2.gameObject.SetActive(false);
        cameraActive = 2;
    }
    public void ActiveCameraPlateau1()
    {
        cameraJ1.gameObject.SetActive(false);
        cameraJ2.gameObject.SetActive(false);
        cameraPlateau1.gameObject.SetActive(true);
        cameraPlateau2.gameObject.SetActive(false);
        cameraActive = 3;
    }
    public void ActiveCameraPlateau2()
    {
        cameraJ1.gameObject.SetActive(false);
        cameraJ2.gameObject.SetActive(false);
        cameraPlateau1.gameObject.SetActive(false);
        cameraPlateau2.gameObject.SetActive(true);
        cameraActive = 3;
    }
    private void AngleCamera()
    {
        if (angle == 11)
        {
            cameraJ1.transform.localPosition = new Vector3(0f, 7.25f, 8f);
            cameraJ1.transform.eulerAngles = new Vector3(30, 180, 0);
            cameraJ2.transform.localPosition = new Vector3(0f, 7.25f, -8f);
            cameraJ2.transform.eulerAngles = new Vector3(30, 0, 0);
            cameraPlateau1.transform.localPosition = new Vector3(0f, 7.25f, 0f);
            cameraPlateau2.transform.localPosition = new Vector3(0f, 7.25f, 0f);
        }
        else if (angle == 13)
        {
            cameraJ1.transform.localPosition = new Vector3(0f, 7.75f, 8.5f);
            cameraJ1.transform.eulerAngles = new Vector3(35, 180, 0);
            cameraJ2.transform.localPosition = new Vector3(0f, 7.75f, -8.5f);
            cameraJ2.transform.eulerAngles = new Vector3(35, 0, 0);
            cameraPlateau1.transform.localPosition = new Vector3(0f, 8.25f, 0f);
            cameraPlateau2.transform.localPosition = new Vector3(0f, 8.25f, 0f);
        }
        else if (angle == 15)
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
