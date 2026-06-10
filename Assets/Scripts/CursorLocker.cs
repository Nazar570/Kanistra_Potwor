using UnityEngine;

public class CursorLocker : MonoBehaviour
{
    void Start()
    {
        LockIt();
    }

    void OnApplicationFocus(bool hasFocus)
    {
        // Jeśli czas gry stoi (śmierć / pauza), ignorujemy powrót do okna gry i NIE blokujemy myszki
        if (Time.timeScale == 0f) return;

        // To naprawia problem, gdy klikasz poza okno gry i wracasz podczas rozgrywki
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