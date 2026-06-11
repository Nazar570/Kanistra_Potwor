using UnityEngine;

public class ThrowZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
    
        ScoreManager sm = other.GetComponent<ScoreManager>();
        if (sm == null) sm = other.GetComponentInParent<ScoreManager>();
        if (sm == null) sm = other.GetComponentInChildren<ScoreManager>();


        if (sm == null && other.CompareTag("Player"))
        {
            sm = FindFirstObjectByType<ScoreManager>();
        }

        if (sm != null)
        {
            sm.isInThrowZone = true; 
            Debug.Log("<color=green>[Strefa] Sukces! Znaleziono ScoreManager na PlayerCapsule. Włączam UI.</color>");
        }
        else
        {
           
            Debug.Log($"[Strefa] Coś weszło w strefę ({other.name}), ale nie znaleziono na tym ScoreManagera.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        ScoreManager sm = other.GetComponent<ScoreManager>();
        if (sm == null) sm = other.GetComponentInParent<ScoreManager>();
        if (sm == null) sm = other.GetComponentInChildren<ScoreManager>();

        if (sm == null && other.CompareTag("Player"))
        {
            sm = FindFirstObjectByType<ScoreManager>();
        }

        if (sm != null)
        {
            sm.isInThrowZone = false; 
            Debug.Log("<color=red>[Strefa] Gracz opuścił strefę rzutu. Ukrywam UI.</color>");
        }
    }
}