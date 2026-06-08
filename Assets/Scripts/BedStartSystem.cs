using UnityEngine;
using StarterAssets;

public class BedStartSystem : MonoBehaviour
{
    [Header("Skrypt First Person Controller z PlayerCapsule")]
    public FirstPersonController skryptKontrolera;

    [Header("Ile sekund trwa wstawanie?")]
    public float czasWstawania = 2f;

    private Transform cameraRoot;
    private bool czyTrwaWstawanie = false;
    private float timer = 0f;

    void OnEnable()
    {
        // Szukamy obiektu odpowiedzialnego za obrót kamery w dzieciach gracza
        cameraRoot = transform.Find("PlayerCameraRoot");

        if (skryptKontrolera != null) 
        {
            // 1. Blokujemy sterowanie kontrolera na starcie pobudki
            skryptKontrolera.enabled = false;
            
            // 2. Jeœli znaleŸliœmy kamerê, od razu wymuszamy obrót pionowo w sufit (-90)
            if (cameraRoot != null)
            {
                cameraRoot.localRotation = Quaternion.Euler(-90f, 0f, 0f);
            }
            
            timer = 0f;
            czyTrwaWstawanie = true;
            Debug.Log("[BedStartSystem] Rozpoczêto wstawanie. Wzrok skierowany w sufit (-90).");
        }
    }

    void Update()
    {
        if (czyTrwaWstawanie)
        {
            // Zabezpieczenie: ca³y czas trzymamy kontroler wy³¹czony, dopóki trwa animacja
            if (skryptKontrolera != null) skryptKontrolera.enabled = false;

            timer += Time.deltaTime;
            float progres = timer / czasWstawania;

            // P³ynny ruch rotacji: od sufitu (-90f) do poziomu przed siebie (0f)
            float aktualnyKat = Mathf.Lerp(-90f, 0f, progres);

            if (cameraRoot != null)
            {
                cameraRoot.localRotation = Quaternion.Euler(aktualnyKat, 0f, 0f);
            }

            if (timer >= czasWstawania)
            {
                czyTrwaWstawanie = false;
                
                // Animacja skoñczona -> zerujemy ostatecznie obrót kamery i w³¹czamy sterowanie
                if (cameraRoot != null) cameraRoot.localRotation = Quaternion.Euler(0f, 0f, 0f);
                if (skryptKontrolera != null) skryptKontrolera.enabled = true; 
                
                Debug.Log("[BedStartSystem] Gracz wsta³, sterowanie odblokowane!");
                this.enabled = false; // Skrypt sam siê wy³¹cza
            }
        }
    }
}