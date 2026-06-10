using UnityEngine;
using UnityEngine.InputSystem; // WYMAGANE dla PlayerInput

public class PlayerDeath : MonoBehaviour
{
    public GameObject deathCanvas;
    private bool isDead = false; 

    public void Die()
    {
        if (isDead) return; 
        isDead = true;

        if (deathCanvas != null) deathCanvas.SetActive(true);

        // Zatrzymujemy czas gry na ekranie śmierci
        Time.timeScale = 0f;

        // 1. Wyłączamy skrypt odpowiedzialny za chodzenie i skakanie
        var fpsController = GetComponent("FirstPersonController") as MonoBehaviour;
        if (fpsController != null) fpsController.enabled = false;

        // 2. Zamiast wyłączać całe Inputs, przełączamy mapę sterowania na UI, 
        // dzięki czemu myszka działa na przyciskach, ale gracz nie może się rozglądać.
        PlayerInput playerInput = GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            playerInput.SwitchCurrentActionMap("UI"); // Przełącza system na tryb UI
        }

        // Odblokowanie kursora, żeby gracz mógł kliknąć przycisk Restart
        Cursor.visible = true;                                          
        Cursor.lockState = CursorLockMode.None;         
        
        Debug.Log("Zginąłeś! Kliknij przycisk Restart lub wciśnij R, aby obudzić się w Asylum.");
    }

    void Update()
    {
        if (isDead)
        {
            // Przy Time.timeScale = 0f i New Input System, Input.GetKeyDown() może nie zawsze 
            // złapać sygnał, ale za chwilę przywrócimy też klikanie myszką.
            if (Input.GetKeyDown(KeyCode.R))
            {
                RestartGame();
            }
            else if (Input.GetKeyDown(KeyCode.O))
            {
                ExitGame();
            }
        }
    }

    public void RestartGame()
    {
        // 1. Przywracamy czas
        Time.timeScale = 1f; 

        // 2. Chowamy ekran śmierci
        if (deathCanvas != null) deathCanvas.SetActive(false);

        // 3. Przywracamy domyślną mapę sterowania dla gracza
        PlayerInput playerInput = GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            playerInput.SwitchCurrentActionMap("Player"); 
        }

        // 4. Blokujemy kursor do rozgrywki
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // 5. Szukamy AmbushTrigger i odpalamy sekwencję omdlenia
        AmbushTrigger ambushTrigger = Object.FindFirstObjectByType<AmbushTrigger>();
        if (ambushTrigger != null)
        {
            ambushTrigger.TriggerKnockoutFromDeath(gameObject);
        }
        else
        {
            Debug.LogError("Nie znaleziono skryptu AmbushTrigger na scenie! Nie można przenieść gracza.");
        }
    }

    public void ExitGame()
    {
        Time.timeScale = 1f; 
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    public void Revive()
    {
        isDead = false;
    }
}