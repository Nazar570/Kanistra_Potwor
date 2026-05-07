//using UnityEngine;
//
//public class Ekwipunek : MonoBehaviour
//{
//    // Globalna pamięć gry (czy masz przedmioty)
//    public static bool maLom = false;
//    public static bool maKlucz = false;
//
//    [Header("Twoje Obrazki z Canvasu")]
//    public GameObject uiObrazekLom;
//    public GameObject uiObrazekKlucz;
//
//    void Update()
//    {
//        // Automatycznie pokazuje lub chowa obrazki w rogu ekranu
//        if (uiObrazekLom != null) uiObrazekLom.SetActive(maLom);
//        if (uiObrazekKlucz != null) uiObrazekKlucz.SetActive(maKlucz);
//    }
//}


using UnityEngine;

public class Ekwipunek : MonoBehaviour
{
    // Globalna pamięć gry (czy masz przedmioty)
    public static bool maLom = false;
    public static bool maKlucz = false;

    [Header("Twoje Obrazki z Canvasu")]
    public GameObject uiObrazekLom;
    public GameObject uiObrazekKlucz;

    [Header("Sterowanie Awaryjne")]
    public float predkoscObrotu = 120f; // Możesz to zmienić w Inspektorze

    void Update()
    {
        // 1. OBSŁUGA EKWIPUNKU
        if (uiObrazekLom != null) uiObrazekLom.SetActive(maLom);
        if (uiObrazekKlucz != null) uiObrazekKlucz.SetActive(maKlucz);

        // 2. OBRACANIE KLAWIATURĄ (Gdy myszka nie działa)
        // Obrót w LEWO (Klawisz Q)
        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(0, -predkoscObrotu * Time.deltaTime, 0);
        }

        // Obrót w PRAWO (Klawisz E)
        if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(0, predkoscObrotu * Time.deltaTime, 0);
        }
        
        // 3. BLOKADA KURSORA (Wciśnij L, żeby "złapać" myszkę)
        if (Input.GetKeyDown(KeyCode.L))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Debug.Log("Kursor zablokowany!");
        }
    }
}
