using UnityEngine;

public class WindowUiController : MonoBehaviour
{
    [SerializeField] protected GameObject window;

    public void OpenWindow()
    {
        OnOpenWindow();
        window.SetActive(true);
    }

    public void CloseWindow()
    {
        OnCloseWindow();
        window.SetActive(false);
    }

    public virtual void OnOpenWindow()
    {
        
    } 

    public virtual void OnCloseWindow()
    {
        
    }
}
