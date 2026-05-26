using UnityEngine;

public class EkwipunekBaterii : MonoBehaviour
{
    [Header("UI Ekwipunku Baterii")]
    [Tooltip("Przeciągnij tutaj obiekt BateriaEkranUI z Hierarchy")]
    public GameObject ikonaBateriiUI;

    void Start()
    {
        if (ikonaBateriiUI != null)
            ikonaBateriiUI.SetActive(false);
    }

    public void DodajBaterieDoEkwipunku()
    {
        if (ikonaBateriiUI != null)
        {
            ikonaBateriiUI.SetActive(true);
            Debug.Log("<color=yellow>[Ekwipunek Baterii] Zdjęcie pojawiło się na ekranie!</color>");
        }
    }
}