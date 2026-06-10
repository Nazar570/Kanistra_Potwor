using UnityEngine;

public class AmbushPath : MonoBehaviour
{
    [Header("Ustawienia Podpowiedzi Tekstowej")]
    [TextArea(3, 6)]
    public string tekstPodpowiedzi = "To musi być w którymś z domów!";
    public float czasWyswietlania = 4f;

    [Header("Następny punkt nawigacji (Domino)")]
    [Tooltip("Przeciągnij tutaj KOLEJNĄ strefę, która ma się włączyć po tej")]
    public AmbushPath nastepnaStrefa;

    private Collider mojCollider;

    void Awake()
    {
        mojCollider = GetComponent<Collider>();
    }

    public void ActivatePath()
    {
        if (mojCollider != null)
        {
            mojCollider.enabled = true;
        }
    }

    public void DeactivatePath()
    {
        if (mojCollider != null) mojCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HintManager hintManager = FindFirstObjectByType<HintManager>();
            if (hintManager != null)
            {
                // Wymuszenie czarnego koloru tekstu wokół zmiennej konfiguracyjnej
                hintManager.ShowHint($"<color=black>{tekstPodpowiedzi}</color>", czasWyswietlania);
            }

            if (nastepnaStrefa != null)
            {
                nastepnaStrefa.ActivatePath();
            }

            DeactivatePath();
        }
    }
}