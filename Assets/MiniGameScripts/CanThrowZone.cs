using UnityEngine;

public class CanThrowZone : MonoBehaviour
{
    [Header("Ręczne przypisanie (Przeciągnij obiekt PuszkiManager)")]
    public PuszkiManager puszkiManager;

    void Start()
    {
        if (puszkiManager == null)
        {
            puszkiManager = FindFirstObjectByType<PuszkiManager>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        bool czyToGracz = other.CompareTag("Player") || (other.transform.parent != null && other.transform.parent.CompareTag("Player"));

        if (czyToGracz)
        {
            if (puszkiManager != null)
            {
                puszkiManager.UstawThrowZone(true);
                Debug.Log("<color=green>[STREFA] Gracz pomyślnie wykryty! Wyświetlam komunikat.</color>");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        bool czyToGracz = other.CompareTag("Player") || (other.transform.parent != null && other.transform.parent.CompareTag("Player"));

        if (czyToGracz && puszkiManager != null)
        {
            puszkiManager.UstawThrowZone(false);
            Debug.Log("[STREFA] Gracz wyszedł ze strefy.");
        }
    }
}