using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeath : MonoBehaviour
{
    public GameObject deathCanvas;
    private bool isDead = false; 

    public void Die()
    {
        if (isDead) return; 
        isDead = true;

        if (deathCanvas != null) deathCanvas.SetActive(true);

        // Zatrzymujemy czas
        Time.timeScale = 0f;

        // Wyłączamy sterowanie
        var fpsController = GetComponent("FirstPersonController") as MonoBehaviour;
        if (fpsController != null) fpsController.enabled = false;

        var inputs = GetComponent("StarterAssetsInputs") as MonoBehaviour;
        if (inputs != null) inputs.enabled = false;
        
        Debug.Log("Zginąłeś! R - Restart, O - Menu Główne");
    }

    void Update()
    {
        if (isDead)
        {
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
        Time.timeScale = 1f; 
        // Restartujemy aktualną scenę - Ekwipunek.Start() zajmie się resztą
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitGame()
    {
        Time.timeScale = 1f; 
        Debug.Log("Powrót do Menu Głównego...");
        SceneManager.LoadScene("MainMenu");
    }
}
