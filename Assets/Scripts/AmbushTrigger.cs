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
        Debug.Log("Samochód naprawiony! Kapsu³a w budynku aktywowana.");
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
        Debug.Log("Gracz zaatakowany w budynku! Nastêpuje uderzenie.");

        // NOWOŒÆ: Wy³¹czamy œcie¿kê naprowadzaj¹c¹, poniewa¿ gracz ju¿ dotar³ do budynku
        AmbushPath path = Object.FindFirstObjectByType<AmbushPath>();
        if (path != null)
        {
            path.DeactivatePath();
        }

        MonoBehaviour fpsController = player.GetComponent("FirstPersonController") as MonoBehaviour;
        if (fpsController != null) fpsController.enabled = false;

        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

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

        if (punktSpawnuWAsylum != null)
        {
            player.transform.position = punktSpawnuWAsylum.position;
            player.transform.rotation = punktSpawnuWAsylum.rotation;
            Debug.Log("Gracz przeteleportowany do wnêtrza Asylum!");
        }
        else
        {
            Debug.LogError("B£¥D: Nie przypisa³eœ punktu narodzin w Asylum do skryptu strefy zasadzki!");
        }

        yield return new WaitForSeconds(0.5f);

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
        if (fpsController != null) fpsController.enabled = true;
    }
}