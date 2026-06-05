using UnityEngine;

public class WindowUiController : MonoBehaviour
{
    [SerializeField] protected GameObject window;

    public virtual void OpenWindow()
    {
        window.SetActive(true);
    }

    public virtual void CloseWindow()
    {
        window.SetActive(false);
    }
}
