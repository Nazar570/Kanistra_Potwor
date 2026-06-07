using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class CarInteraction : MonoBehaviour
{
    [Header("Ustawienia Sceny")]
    public string nextSceneName = "City";

    [Header("Ustawienia Jazdy")]
    public float driveSpeed = 15f;
    public float driveDuration = 3f;
    public float distanceToEnter = 5f;

    [Header("Referencje UI")]
    public Image faderImage;

    [Header("NOWOŚĆ: Ustawienia Podpowiedzi (Hint)")]
    [SerializeField] private string hintMessage = "Chyba ten samochód to moja jedyna opcja, żeby gdzieś się dostać...";
    [SerializeField] private float hintDuration = 3f;

    // NOWOŚĆ: Blokada, która uratuje nas przed restartem sceny
    [HideInInspector]
    public bool zablokowanyNaTejScenie = false;

    private bool isDriving = false;
    private Transform playerTransform;
    private HintManager hintManager; // Referencja do Twojego HintManagera

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;

        if (faderImage == null)
        {
            Debug.LogError("Zapomniałeś przypisać Fader Image na aucie!");
        }

        // Automatycznie szukamy HintManagera na scenie, żebyś nie musiał go przeciągać ręcznie
        hintManager = Object.FindFirstObjectByType<HintManager>();
        if (hintManager == null)
        {
            Debug.LogWarning("Nie znaleziono HintManagera na scenie! Napisy nie będą się wyświetlać.");
        }
    }

    void Update()
    {
        // Jeśli skrypt jest zablokowany logicznie, nic nie rób
        if (zablokowanyNaTejScenie) return;

        if (isDriving)
        {
            transform.Translate(Vector3.forward * driveSpeed * Time.deltaTime);
            return;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            HandleInteraction();
        }
    }

    void HandleInteraction()
    {
        // Jeśli skrypt jest zablokowany logicznie, ignoruj wywołanie (również przez SendMessage)
        if (zablokowanyNaTejScenie) return;

        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerTransform = player.transform;
            else return;
        }

        float dystans = Vector3.Distance(playerTransform.position, transform.position);

        if (dystans <= distanceToEnter)
        {
            StartCoroutine(StartCarSequence());
        }
    }

    IEnumerator StartCarSequence()
    {
        if (playerTransform == null) yield break;
        GameObject player = playerTransform.gameObject;

        CarDriving cd = GetComponent<CarDriving>();
        if (cd != null) cd.enabled = false;

        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        player.transform.SetParent(this.transform);
        player.transform.localPosition = new Vector3(-0.3f, -0.31f, 1.23f);
        player.transform.localRotation = Quaternion.identity;

        isDriving = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        yield return new WaitForSeconds(driveDuration);

        if (faderImage != null)
        {
            float timer = 0;
            float fadeDuration = 1f;
            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                Color c = faderImage.color;
                c.a = Mathf.Clamp01(timer / fadeDuration);
                faderImage.color = c;
                yield return null;
            }
        }

        SceneManager.LoadScene(nextSceneName);
    }

    // ==========================================
    // NOWOŚĆ: WYKRYWANIE ZETKNIĘCIA Z POJAZDEM
    // ==========================================

    // Wersja 1: Jeśli gracz fizycznie wpada na auto (zwykła kolizja)
   private void OnCollisionEnter(Collision collision)
    {
        // Ta linijka wypisze w konsoli nazwę wszystkiego, co dotknie auta:
        Debug.Log("Auto dotknięte przez (Collision): " + collision.gameObject.name);

        if (collision.gameObject.CompareTag("Player") && !isDriving && !zablokowanyNaTejScenie)
        {
            TriggerCarHint();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Ta linijka wypisze w konsoli nazwę wszystkiego, w co auto wejdzie (Trigger):
        Debug.Log("Auto przeniknięte przez (Trigger): " + other.gameObject.name);

        if (other.CompareTag("Player") && !isDriving && !zablokowanyNaTejScenie)
        {
            TriggerCarHint();
        }
    }

    private void TriggerCarHint()
    {
        if (hintManager != null)
        {
            hintManager.ShowHint(hintMessage, hintDuration);
        }
    }
}