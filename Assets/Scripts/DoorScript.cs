using UnityEngine;

public class DoorScript : MonoBehaviour
{
    [Header("Teksty Interakcji")]
    public string tekstOtworz = "Naciśnij [E] aby otworzyć";
    public string tekstZamknij = "Naciśnij [E] aby zamknąć";
    public string tekstZablokowane = "Potrzebujesz klucza!";

    [Header("Ustawienia Ruchu")]
    public float openAngle = 90f; 
    public float smoothSpeed = 5f; 
    public Vector3 przesuniecieZawiasu = new Vector3(0.5f, 0f, 0f); 

    [Header("SYSTEM KLUCZA")]
    [Tooltip("Zaznacz (1), jeśli gracz ma klucz. Odznacz (0), jeśli nie ma.")]
    public bool czyMoznaOtworzyc = false; 

    [Header("UKRYTY PRZEDMIOT (Zabezpieczenie)")]
    [Tooltip("Przeciągnij tutaj BoxCollider klucza, żeby gracz nie mógł go podnieść przez zamknięte drzwi")]
    public Collider ukrytyCollider;

    public bool isOpen = false;
    private bool isPlayerNear = false;
    private Transform autoZawias;
    private Quaternion defaultRotation;
    private Quaternion openRotation;
    private HintManager hintManager;

    void Awake()
    {
        // Budowanie zawiasu w locie
        autoZawias = new GameObject("AutoZawias_" + gameObject.name).transform;
        autoZawias.position = transform.position + transform.TransformDirection(przesuniecieZawiasu);
        autoZawias.rotation = transform.rotation;
        transform.SetParent(autoZawias);

        defaultRotation = autoZawias.localRotation;
        openRotation = Quaternion.Euler(defaultRotation.eulerAngles.x, defaultRotation.eulerAngles.y + openAngle, defaultRotation.eulerAngles.z);

        hintManager = Object.FindFirstObjectByType<HintManager>();
        
        if (ukrytyCollider != null) ukrytyCollider.enabled = false;
    }

    void Update()
    {
        // Płynny ruch drzwi
        if (isOpen)
            autoZawias.localRotation = Quaternion.Slerp(autoZawias.localRotation, openRotation, Time.deltaTime * smoothSpeed);
        else
            autoZawias.localRotation = Quaternion.Slerp(autoZawias.localRotation, defaultRotation, Time.deltaTime * smoothSpeed);

        // Zarządzanie wyświetlaniem podpowiedzi w polu widzenia
        if (isPlayerNear && hintManager != null)
        {
            string wiadomosc = "";
            if (!czyMoznaOtworzyc)
            {
                wiadomosc = tekstZablokowane;
            }
            else
            {
                wiadomosc = isOpen ? tekstZamknij : tekstOtworz;
            }

            // Wymuszenie czarnego koloru tekstu dla jasnego panelu UI
            hintManager.ShowHint($"<color=black>{wiadomosc}</color>", 0.5f);

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (czyMoznaOtworzyc)
                {
                    isOpen = !isOpen;
                    
                    if (ukrytyCollider != null)
                    {
                        ukrytyCollider.enabled = isOpen;
                    }
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!this.enabled) return; 

        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!this.enabled) return; 

        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            
            // Czyszczenie podpowiedzi po odejściu od drzwi
            if (hintManager != null && hintManager.hintPanel != null)
            {
                hintManager.hintPanel.SetActive(false);
            }
        }
    }
}