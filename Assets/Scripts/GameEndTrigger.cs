using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro; // To pozwala na używanie TextMeshPro

public class GameEndTrigger : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField] private GameObject gameEndPanel;
    // Zmienione z 'Text' na 'TextMeshProUGUI'
    [SerializeField] private TextMeshProUGUI gameEndText;

    [Header("Game End Settings")]
    [SerializeField] private float delayBeforeLoad = 3f;
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    [Header("Car Requirements")]
    [SerializeField] private CarRequirements carRequirements;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("GameEndTrigger: Coś weszło w trigger samochodu: " + other.name);

        // Sprawdź czy obiekt ma tag "Player"
        if (other.CompareTag("Player"))
        {
            Debug.Log("GameEndTrigger: To gracz! Sprawdzam czy samochód jest gotowy.");

            // Sprawdź czy samochód jest gotowy (wszystkie części zamontowane)
            if (carRequirements != null)
            {
                Debug.Log("GameEndTrigger: CarRequirements przypisany. SamochodGotowy = " + carRequirements.SamochodGotowy);

                if (carRequirements.SamochodGotowy)
                {
                    Debug.Log("GameEndTrigger: Samochód jest gotowy! Kończę grę.");
                    // Wywołaj funkcję kończącą grę
                    EndGame();
                }
                else
                {
                    Debug.Log("GameEndTrigger: Samochód nie jest jeszcze gotowy do ucieczki! Brakuje części.");
                }
            }
            else
            {
                Debug.LogError("GameEndTrigger: CarRequirements nie jest przypisany! Przeciągnij Car02 do pola Car Requirements.");
            }
        }
    }

    private void EndGame()
    {
        // Wyświetl komunikat o ucieczce
        ShowGameEndUI();

        // Rozpocznij coroutine do załadowania menu głównego po opóźnieniu
        StartCoroutine(LoadMainMenuAfterDelay());
    }

    private void ShowGameEndUI()
    {
        // Jeśli przypisano panel UI, aktywuj go
        if (gameEndPanel != null)
        {
            gameEndPanel.SetActive(true);
        }

        // Jeśli przypisano tekst, ustaw komunikat
        if (gameEndText != null)
        {
            gameEndText.text = "<size=120%><color=#4CAF50>UDAŁO CI SIĘ UCIEC!</color></size>\n\n<size=80%>Asylum już za tobą...\nWolność jest Twoja.</size>";
        }
        else
        {
            // Fallback: wyświetl komunikat w konsoli
            Debug.Log("Udało Ci się uciec!");
        }
    }

    private IEnumerator LoadMainMenuAfterDelay()
    {
        // Czekaj określoną liczbę sekund
        yield return new WaitForSeconds(delayBeforeLoad);

        // Spróbuj załadować scenę menu głównego
        if (Application.isEditor || !Application.isFocused)
        {
            // W edytorze lub jeśli gra nie jest aktywna, załaduj scenę
            SceneManager.LoadScene(mainMenuSceneName);
        }
        else
        {
            // W buildzie, sprawdź czy scena istnieje
            if (SceneUtility.GetBuildIndexByScenePath(mainMenuSceneName) != -1)
            {
                SceneManager.LoadScene(mainMenuSceneName);
            }
            else
            {
                // Jeśli scena nie istnieje w buildzie, wyjdź z gry
                Application.Quit();

                // W edytorze Application.Quit() nie działa, więc wyświetl komunikat
#if UNITY_EDITOR
                Debug.Log("Gra zakończona. W buildzie aplikacja zostanie zamknięta.");
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            }
        }
    }
}