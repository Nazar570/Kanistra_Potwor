using UnityEngine;

public class Ekwipunek : MonoBehaviour
{
    // Globalna pamięć gry (static sprawia, że dane zostają w pamięci po restarcie)
    public static bool maLom = false;
    public static bool maKlucz = false;

    [Header("Twoje Obrazki z Canvasu")]
    public GameObject uiObrazekLom;
    public GameObject uiObrazekKlucz;

    [Header("Sterowanie Awaryjne")]
    public float predkoscObrotu = 120f;

    void Start()
    {
        // KLUCZOWE: Resetujemy przedmioty przy starcie każdej sceny
        maLom = false;
        maKlucz = false;
        Debug.Log("Ekwipunek został wyczyszczony na starcie sceny.");
    }

    void Update()
    {
        // 1. OBSŁUGA EKWIPUNKU (pokazywanie ikon na ekranie)
        if (uiObrazekLom != null) uiObrazekLom.SetActive(maLom);
        if (uiObrazekKlucz != null) uiObrazekKlucz.SetActive(maKlucz);

        // 2. OBRACANIE KLAWIATURĄ
        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(0, -predkoscObrotu * Time.deltaTime, 0);
        }

        if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(0, predkoscObrotu * Time.deltaTime, 0);
        }
        
        // 3. BLOKADA KURSORA
        if (Input.GetKeyDown(KeyCode.L))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
