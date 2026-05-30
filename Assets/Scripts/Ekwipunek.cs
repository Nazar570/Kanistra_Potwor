using UnityEngine;

public class Ekwipunek : MonoBehaviour
{
    // Stare przedmioty
    public static bool maLom = false;
    public static bool maKlucz = false;

    // NOWE przedmioty do auta
    public static bool maAkumulator = false;
    public static bool maSwiecaZaplonowa = false;
    public static bool maKanister = false;

    [Header("Twoje Obrazki z Canvasu")]
    public GameObject uiObrazekLom;
    public GameObject uiObrazekKlucz;
    public GameObject uiObrazekAkumulator;
    public GameObject uiObrazekSwieca;
    public GameObject uiObrazekKanister;

    [Header("Sterowanie Awaryjne")]
    public float predkoscObrotu = 120f;

    void Start()
    {
        maLom = false;
        maKlucz = false;
        maAkumulator = false;
        maSwiecaZaplonowa = false;
        maKanister = false;
        Debug.Log("Ekwipunek wyczyszczony.");
    }

    void Update()
    {
        if (uiObrazekLom != null) uiObrazekLom.SetActive(maLom);
        if (uiObrazekKlucz != null) uiObrazekKlucz.SetActive(maKlucz);
        if (uiObrazekAkumulator != null) uiObrazekAkumulator.SetActive(maAkumulator);
        if (uiObrazekSwieca != null) uiObrazekSwieca.SetActive(maSwiecaZaplonowa);
        if (uiObrazekKanister != null) uiObrazekKanister.SetActive(maKanister);

        if (Input.GetKey(KeyCode.Q))
            transform.Rotate(0, -predkoscObrotu * Time.deltaTime, 0);
        if (Input.GetKey(KeyCode.E))
            transform.Rotate(0, predkoscObrotu * Time.deltaTime, 0);
        if (Input.GetKeyDown(KeyCode.L))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}