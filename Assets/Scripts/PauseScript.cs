using UnityEngine;
using UnityEngine.InputSystem;

public class PauseScript : MonoBehaviour
{
    public GameObject pausePanel; 
    public GameObject player; // ZOBACZ TU: Dodane pole na Twojego gracza!
    
    private bool isPaused = false;
    private PlayerInput playerInput;

    void Start()
    {
        // Szukamy systemu sterowania na graczu zaraz po starcie gry
        if (player != null)
        {
            playerInput = player.GetComponent<PlayerInput>();
        }
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    void PauseGame()
    {
        pausePanel.SetActive(true);      
        Time.timeScale = 0f;             
        isPaused = true;
        
        Cursor.lockState = CursorLockMode.None; 
        Cursor.visible = true;

        // TOTALNY FREEZE: Wyłączamy odbieranie sygnałów (ruchu i kamery)
        if (playerInput != null) playerInput.enabled = false;
    }

    void ResumeGame()
    {
        pausePanel.SetActive(false);     
        Time.timeScale = 1f;             
        isPaused = false;
        
        Cursor.lockState = CursorLockMode.Locked; 
        Cursor.visible = false;

        // Wznawiamy odbieranie sygnałów
        if (playerInput != null) playerInput.enabled = true;
    }
}
