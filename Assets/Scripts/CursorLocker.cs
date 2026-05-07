using UnityEngine;

public class CursorLocker : MonoBehaviour
{
    void Start()
    {
        LockIt();
    }

    void OnApplicationFocus(bool hasFocus)
    {
        // To naprawia problem, gdy klikasz poza okno gry i wracasz
        if (hasFocus) 
        {
            LockIt();
        }
    }

    private void LockIt()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
