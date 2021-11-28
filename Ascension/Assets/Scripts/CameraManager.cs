using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Camera cameraJ1;
    [SerializeField] private Camera cameraJ2;
    [SerializeField] private Camera cameraPlateau;

    public void ActiveCameraJ1()
    {
        cameraJ1.gameObject.SetActive(true);
        cameraJ2.gameObject.SetActive(false);
        cameraPlateau.gameObject.SetActive(false);
    }
    public void ActiveCameraJ2()
    {
        cameraJ1.gameObject.SetActive(false);
        cameraJ2.gameObject.SetActive(true);
        cameraPlateau.gameObject.SetActive(false);
    }
    public void ActiveCameraPlateau()
    {
        cameraJ1.gameObject.SetActive(false);
        cameraJ2.gameObject.SetActive(false);
        cameraPlateau.gameObject.SetActive(true);
    }
}
