using UnityEngine;
using TMPro;

public class DoorScript : MonoBehaviour
{
    [Header("UI i Teksty")]
    public TextMeshProUGUI interactionText; 
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

    private bool isOpen = false;
    private bool isPlayerNear = false;
    private Transform autoZawias;
    private Quaternion defaultRotation;
    private Quaternion openRotation;

    // ZMIANA TUTAJ: Zamieniamy Start() na Awake()
    void Awake()
    {
        // Budowanie zawiasu w locie
        autoZawias = new GameObject("AutoZawias_" + gameObject.name).transform;
        autoZawias.position = transform.position + transform.TransformDirection(przesuniecieZawiasu);
        autoZawias.rotation = transform.rotation;
        transform.SetParent(autoZawias);

        defaultRotation = autoZawias.localRotation;
        openRotation = Quaternion.Euler(defaultRotation.eulerAngles.x, defaultRotation.eulerAngles.y + openAngle, defaultRotation.eulerAngles.z);

        if (interactionText != null) interactionText.gameObject.SetActive(false);
        
        // ZABEZPIECZENIE: Wyłącza czujnik klucza ZANIM SystemZamka uśpi ten skrypt
        if (ukrytyCollider != null) ukrytyCollider.enabled = false;
    }

    void Update()
    {
        // Płynny ruch
        if (isOpen)
            autoZawias.localRotation = Quaternion.Slerp(autoZawias.localRotation, openRotation, Time.deltaTime * smoothSpeed);
        else
            autoZawias.localRotation = Quaternion.Slerp(autoZawias.localRotation, defaultRotation, Time.deltaTime * smoothSpeed);

        if (isPlayerNear)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (czyMoznaOtworzyc)
                {
                    isOpen = !isOpen;
                    UpdateText(); 
                    
                    // MAGIA: Aktywuje czujnik klucza TYLKO gdy drzwi są otwarte
                    if (ukrytyCollider != null)
                    {
                        ukrytyCollider.enabled = isOpen;
                    }
                }
            }
        }
    }

    private void UpdateText()
    {
        if (interactionText == null) return;

        if (!czyMoznaOtworzyc)
        {
            interactionText.text = tekstZablokowane;
        }
        else
        {
            interactionText.text = isOpen ? tekstZamknij : tekstOtworz;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!this.enabled) return; 

        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
            UpdateText(); 
            interactionText.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!this.enabled) return; 

        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            interactionText.gameObject.SetActive(false);
        }
    }
}
