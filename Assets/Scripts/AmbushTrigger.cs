using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AmbushTrigger : MonoBehaviour
{
    [Header("Konfiguracja NPC")]
    [SerializeField] private GameObject npcObject;

    [Header("UI i Efekty")]
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1.0f;

    [Header("Cel Teleportacji")]
    [SerializeField] private Transform punktSpawnuWAsylum;

    private bool isCarRepaired = false;
    private bool isTriggered = false;

    private void Start()
    {
        if (npcObject != null) npcObject.SetActive(false);

        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;
        }
    }

    public void ActivateAmbushSequence()
    {
        isCarRepaired = true;
        if (npcObject != null) npcObject.SetActive(true);
        Debug.Log("Samochód naprawiony! Kapsuła w budynku aktywowana.");
    }

    // NOWOŚĆ: Funkcja wywoływana przez przycisk RESTART w PlayerDeath
    public void TriggerKnockoutFromDeath(GameObject player)
    {
        // Wymuszamy uruchomienie sekwencji bez względu na to, czy auto było naprawione
        isTriggered = true; 
        StartCoroutine(KnockoutSequenceRoutine(player));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isCarRepaired && !isTriggered && other.CompareTag("Player"))
        {
            isTriggered = true;
            StartCoroutine(KnockoutSequenceRoutine(other.gameObject));
        }
    }

    private IEnumerator KnockoutSequenceRoutine(GameObject player)
    {
        Debug.Log("Uruchamianie sekwencji przeniesienia gracza...");

        AmbushPath path = Object.FindFirstObjectByType<AmbushPath>();
        if (path != null)
        {
            path.DeactivatePath();
        }

        // Upewniamy się, że sterowanie jest wyłączone
        MonoBehaviour fpsController = player.GetComponent("FirstPersonController") as MonoBehaviour;
        if (fpsController != null) fpsController.enabled = false;

        MonoBehaviour inputs = player.GetComponent("StarterAssetsInputs") as MonoBehaviour;
        if (inputs != null) inputs.enabled = false;

        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        // 1. Ekran robi się czarny
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            if (fadeImage != null)
            {
                Color c = fadeImage.color;
                c.a = Mathf.Min(timer / fadeDuration, 1f);
                fadeImage.color = c;
            }
            yield return null;
        }

        yield return new WaitForSeconds(2.0f);

        // 2. Teleportacja na łóżko w Asylum
        if (punktSpawnuWAsylum != null)
        {
            player.transform.position = punktSpawnuWAsylum.position;
            player.transform.rotation = punktSpawnuWAsylum.rotation;
            Debug.Log("Gracz przeteleportowany do wnętrza Asylum!");

            // Włączamy skrypt wstawania
            BedStartSystem bedSystem = player.GetComponent<BedStartSystem>();
            if (bedSystem != null)
            {
                bedSystem.enabled = true;
            }
        }
        else
        {
            Debug.LogError("BŁĄD: Nie przypisałeś punktu narodzin w Asylum do skryptu strefy zasadzki!");
        }

        yield return new WaitForSeconds(0.5f);

        // NOWOŚĆ: Po udanej teleportacji "uzdrawiamy" wewnętrzny stan skryptu śmierci gracza
        PlayerDeath pd = player.GetComponent<PlayerDeath>();
        if (pd != null)
        {
            pd.Revive();
        }

        // 3. Ekran zaczyna się rozjaśniać
        timer = fadeDuration;
        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            if (fadeImage != null)
            {
                Color c = fadeImage.color;
                c.a = Mathf.Max(timer / fadeDuration, 0f);
                fadeImage.color = c;
            }
            yield return null;
        }

        if (cc != null) cc.enabled = true;
        
        // Resetujemy flagę, aby system mógł zadziałać ponownie, jeśli zajdzie potrzeba
        isTriggered = false; 
    }
}