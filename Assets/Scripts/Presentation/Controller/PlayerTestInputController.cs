using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTestInputController : MonoBehaviour
{
    [SerializeField] private CharacterDebuffController debuffController;

    void Start()
    {
        if (debuffController == null) debuffController = GetComponent<CharacterDebuffController>();
        if (debuffController == null) debuffController = FindAnyObjectByType<CharacterDebuffController>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1)) HandleF1();
        if (Input.GetKeyDown(KeyCode.F2)) HandleF2();
        if (Input.GetKeyDown(KeyCode.F3)) HandleF3();
        if (Input.GetKeyDown(KeyCode.F4)) HandleF4();
        if (Input.GetKeyDown(KeyCode.F5)) HandleF5();
        if (Input.GetKeyDown(KeyCode.F6)) HandleF6();
        if (Input.GetKeyDown(KeyCode.F7)) HandleF7();
        if (Input.GetKeyDown(KeyCode.F8)) HandleF8();
        if (Input.GetKeyDown(KeyCode.F9)) HandleF9();
        if (Input.GetKeyDown(KeyCode.F10)) HandleF10();
        if (Input.GetKeyDown(KeyCode.F11)) HandleF11();
        if (Input.GetKeyDown(KeyCode.F12)) HandleF12();
    }

    private void HandleF1()
    {
        if (debuffController != null)
        {
            Debug.Log("F1: Triggering Hit");
            // debuffController.Hit();
        }
    }

    private void HandleF2()
    {
        if (debuffController != null)
        {
            // debuffController.Deflected();
        }
    }
    private void HandleF3()
    {
        if (debuffController != null)
        {
            // Debug.Log("F3: Triggering KnockBack");
            // Mock knockback: backwards and slightly upwards
            Vector3 knockback = -Vector3.forward * 100f + Vector3.down * 200f;
            // debuffController.KnockOut(knockback, true);
        }
    }
    private void HandleF4() { Debug.Log("F4 Placeholder"); 
    }
    private void HandleF5() { Debug.Log("F5 Placeholder"); }
    private void HandleF6() { Debug.Log("F6 Placeholder"); }
    private void HandleF7() { Debug.Log("F7 Placeholder"); }
    private void HandleF8() { Debug.Log("F8 Placeholder"); }
    private void HandleF9() { Debug.Log("F9 Placeholder"); }
    private void HandleF10() { Debug.Log("F10 Placeholder"); }
    private void HandleF11() { Debug.Log("F11 Placeholder"); }
    private void HandleF12() { Debug.Log("F12 Placeholder"); }
}
