using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class CarInteraction : MonoBehaviour
{
    [Header("Ustawienia Sceny")]
    public string nextSceneName = "City";

    [Header("Ustawienia Jazdy")]
    public float driveSpeed = 15f;
    public float driveDuration = 3f;
    public float distanceToEnter = 5f; // Jak blisko musisz być auta, żeby wsiąść

    [Header("Referencje UI")]
    public Image faderImage;

    private bool isDriving = false;
    private Camera mainCam;

    void Start()
    {
        // Szukamy kamery i przypisujemy ją na stałe
        mainCam = Camera.main;
        
        if (mainCam == null)
        {
            Debug.LogError("BŁĄD: Twoja kamera nie ma Tagu 'MainCamera'! Ustaw go w Inspektorze kamery.");
        }

        if (faderImage == null)
        {
            Debug.LogError("Zapomniałeś przypisać Fader Image na aucie!");
        }
    }

    void Update()
    {
        if (isDriving)
        {
            transform.Translate(Vector3.forward * driveSpeed * Time.deltaTime);
            return;
        }

        // Jeśli klikniesz Prawy Przycisk Myszy
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            HandleClick();
        }
    }

    void HandleClick()
    {
        if (mainCam == null) mainCam = Camera.main;
        if (mainCam == null) return;

        // Strzał w środek ekranu (tam gdzie patrzysz)
        Ray ray = mainCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, distanceToEnter))
        {
            // Sprawdzenie czy trafiliśmy w to auto lub jego części
            if (hit.transform == this.transform || hit.transform.IsChildOf(this.transform))
            {
                StartCoroutine(StartCarSequence());
            }
        }
    }

    IEnumerator StartCarSequence()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) yield break;

        // 1. Teleportacja
        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        player.transform.SetParent(this.transform);
        player.transform.localPosition = new Vector3(-0.3f, -0.31f, 1.23f); 
        player.transform.localRotation = Quaternion.identity;

        // 2. Jazda i ukrycie kursora (na wypadek gdyby był widoczny)
        isDriving = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        yield return new WaitForSeconds(driveDuration);

        // 3. Ściemnianie
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
}