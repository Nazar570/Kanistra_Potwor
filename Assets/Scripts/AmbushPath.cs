using UnityEngine;

public class AmbushPath : MonoBehaviour
{
    [Header("Ustawienia Podpowiedzi Tekstowej")]
    [TextArea(3, 6)]
    public string tekstPodpowiedzi = "S³yszê krzyk z wnêtrza budynku... Muszê wejœæ do œrodka!";
    public float czasWyswietlania = 4f;

    [Header("Nastêpny punkt nawigacji (Domino)")]
    [Tooltip("Przeci¹gnij tutaj KOLEJN¥ strefê, która ma siê w³¹czyæ po tej")]
    public AmbushPath nastepnaStrefa;

    private Collider mojCollider;

    void Awake()
    {
        mojCollider = GetComponent<Collider>();
    }

    // Tê funkcjê wywo³uje auto (na pierwszej strefie) LUB poprzednia strefa na drodze
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
            // 1. Pokazujemy napis przypisany do tej strefy
            HintManager hintManager = FindFirstObjectByType<HintManager>();
            if (hintManager != null)
            {
                hintManager.ShowHint(tekstPodpowiedzi, czasWyswietlania);
            }

            // 2. Aktywujemy kolejn¹ strefê na drodze (jeœli zosta³a przypisana)
            if (nastepnaStrefa != null)
            {
                nastepnaStrefa.ActivatePath();
            }

            // 3. Wy³¹czamy bie¿¹c¹ strefê, ¿eby napis nie wyskakiwa³ ponownie
            if (mojCollider != null) mojCollider.enabled = false;
        }
    }
}