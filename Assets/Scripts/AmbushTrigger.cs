using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AmbushTrigger : MonoBehaviour
{
    [Header("UI i Efekty")]
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1.0f;

    [Header("Cel Teleportacji")]
    [SerializeField] private Transform punktSpawnuWAsylum;

    [Header("Dźwięk i Model Mutanta (3D)")]
    [Tooltip("Przeciągnij tutaj bezpośrednio obiekt Mutanta z głośnikiem")]
    [SerializeField] private AudioSource mutantAudioSource;

    private bool isCarRepaired = false;
    private bool isTriggered = false;

    private void Start()
    {
        // Zerujemy czarny ekran na starcie gry
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;
        }

        // UWAGA: Nie wyłączamy i nie uciszamy tu mutanta bezpośrednio. 
        // Obiekt sciezkaKrzyk jest domyślnie wyłączony przez samochód na starcie, więc mutant i tak śpi.
    }

    public void ActivateAmbushSequence()
    {
        isCarRepaired = true;
        Debug.Log("Samochód naprawiony! Kapsuła w budynku aktywowana.");

        // Jeśli przypisaliśmy głośnik mutanta
        if (mutantAudioSource != null)
        {
            // Włączamy fizycznie cały obiekt mutanta za pomocą jego komponentu AudioSource
            mutantAudioSource.gameObject.SetActive(true);
            Debug.Log($"[AMBUSH] Włączono obiekt mutanta: {mutantAudioSource.gameObject.name}");

            // Wymuszamy odtworzenie dźwięku krzyk
            if (!mutantAudioSource.isPlaying)
            {
                Debug.Log("[AMBUSH] Odpalam odtwarzanie dźwięku krzyku.");
                mutantAudioSource.Play();

                if (mutantAudioSource.clip == null)
                {
                    Debug.LogError("UWAGA: AudioSource nie ma przypisanego fizycznego klipu audio w polu Audio Generator!");
                }
            }
        }
    }

    public void TriggerKnockoutFromDeath(GameObject player)
    {
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

        // Gracz zemdlał, więc uciszamy potwora na dobre
        if (mutantAudioSource != null)
        {
            mutantAudioSource.Stop(); 
            mutantAudioSource.gameObject.SetActive(false); // Opcjonalnie: chowamy go, gdy gracz mdleje
        }

        AmbushPath path = Object.FindFirstObjectByType<AmbushPath>();
        if (path != null)
        {
            path.DeactivatePath();
        }

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
        
        isTriggered = false; 
    }
}