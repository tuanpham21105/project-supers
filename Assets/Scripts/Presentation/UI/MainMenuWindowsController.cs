using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WindowType
{
    Main = 0,
    Login = 1,
    SignUp = 2,
    Account = 3,
    EditAccount = 4,
    GuestAccount = 5,
    AddGuestAccount = 6
}

public class MainMenuWindowsController : MonoBehaviour
{
    [SerializeField] private List<GameObject> windows;
    [SerializeField] private GameObject openedWindow;

    public void OpenWindow(WindowType type)
    {
        openedWindow.SetActive(false);

        openedWindow = windows[(int)type];

        openedWindow.SetActive(true);
    }
}
