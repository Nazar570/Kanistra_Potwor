using UnityEngine;
using TMPro;

public class KanisterPickup : MonoBehaviour
{
    public TextMeshProUGUI interactionText;
    public string nazwaPrzedmiotu = "Kanister";

    private bool isPlayerNear = false;

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.F))
        {
            Ekwipunek.maKanister = true;
            if (interactionText != null)
                interactionText.gameObject.SetActive(false);
            Destroy(gameObject);
            Debug.Log("Kanister podniesiony!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
            if (interactionText != null)
            {
                interactionText.text = "Podnieœ " + nazwaPrzedmiotu + " [F]";
                interactionText.gameObject.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            if (interactionText != null)
                interactionText.gameObject.SetActive(false);
        }
    }
}