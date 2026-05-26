using UnityEngine;
using TMPro;
using System.Collections;

public class HintManager : MonoBehaviour
{
    public GameObject hintPanel; // Przeciągnij tu HintPanel
    public TextMeshProUGUI textElement; // Przeciągnij tu HintText
    private Coroutine currentCoroutine;

    public void ShowHint(string message, float duration)
    {
        // Jeśli już coś się wyświetla, zatrzymaj to
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        
        currentCoroutine = StartCoroutine(DisplayHintRoutine(message, duration));
    }

    private IEnumerator DisplayHintRoutine(string message, float duration)
    {
        textElement.text = message;
        hintPanel.SetActive(true);
        
        yield return new WaitForSeconds(duration);
        
        hintPanel.SetActive(false);
    }
}