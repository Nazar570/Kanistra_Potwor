using UnityEngine;

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

        // Wyłączamy sterowanie
        var fpsController = GetComponent("FirstPersonController") as MonoBehaviour;
        if (fpsController != null) fpsController.enabled = false;

        var inputs = GetComponent("StarterAssetsInputs") as MonoBehaviour;
        if (inputs != null) inputs.enabled = false;

        // Odblokowanie kursora, żeby gracz mógł kliknąć przycisk Restart
        Cursor.visible = true;                          
        Cursor.lockState = CursorLockMode.None;         
        
        Debug.Log("Zginąłeś! Kliknij przycisk Restart lub wciśnij R, aby obudzić się w Asylum.");
    }

    void Update()
    {
        if (isDead)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                RestartGame();
            }
            // Opcjonalnie możesz zostawić lub usunąć powrót do menu:
            else if (Input.GetKeyDown(KeyCode.O))
            {
                ExitGame();
            }
        }
    }

    public void RestartGame()
    {
        // --- KLUCZOWA ZMIANA ---
        // 1. Przywracamy czas, żeby korutyna omdlenia mogła wystartować
        Time.timeScale = 1f; 

        // 2. Chowamy ekran śmierci
        if (deathCanvas != null) deathCanvas.SetActive(false);

        // 3. Blokujemy kursor (bo teraz zaczyna się nieliniowa sekwencja omdlenia)
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // 4. Szukamy AmbushTrigger i odpalamy w nim teleportację oraz fading
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

    // Funkcja wywoływana przez AmbushTrigger na samym końcu, gdy gracz już wstanie z łóżka
    public void Revive()
    {
        isDead = false;
    }
}