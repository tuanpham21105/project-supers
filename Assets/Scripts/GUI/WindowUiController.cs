using UnityEngine;

public class WindowUiController : MonoBehaviour
{
    [SerializeField] protected GameObject window;

    void Start()
    {
        if (window == null)
            window = gameObject;

        Initialize();
    }

    public virtual void Initialize()
    {
        
    }

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
