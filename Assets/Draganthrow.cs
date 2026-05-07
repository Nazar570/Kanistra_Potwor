using UnityEngine;

public class Dragandthrow : MonoBehaviour
{
    private Rigidbody rb;
    private bool isShoot = false;
    private Transform playerTransform;

    [Header("Ustawienia dystansu")]
    public float interactionDistance = 5f; // Jak blisko musisz być, by kursor się pojawił

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Szukamy gracza w scenie po Tagu
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("Nie znaleziono obiektu z tagiem 'Player'! Ustaw tag na swoim graczu.");
        }
    }

    void Update()
    {
        // Jeśli już rzuciliśmy kulą, nie chcemy, żeby kursor nadal wariował przez tę kulę
        if (playerTransform == null || isShoot) return;

        // Obliczamy dystans między TYM obiektem (kulą) a graczem
        float distance = Vector3.Distance(transform.position, playerTransform.position);

        // Zarządzanie kursorem zależnie od dystansu
        if (distance <= interactionDistance)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            // Możesz to zakomentować, jeśli masz wiele kul i nie chcesz, 
            // żeby jedna kula wyłączała kursor, gdy inna jest blisko.
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    // Wykrywa kliknięcie myszką na obiekt (wymaga Collidera na kuli!)
    void OnMouseDown()
    {
        if (isShoot) return;
        Debug.Log("Złapałaś kulę: " + gameObject.name);
    }

    void OnMouseUp()
    {
        if (isShoot) return;

        if (Camera.main != null)
        {
            // Tworzymy promień z miejsca, gdzie puściliśmy myszkę
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Shoot(hit.point);
            }
        }
    }

    void Shoot(Vector3 target)
    {
        isShoot = true;
        
        // Po strzale kursor powinien zniknąć, bo już nie sterujemy tą kulą
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        Vector3 direction = (target - transform.position).normalized;
        direction += Vector3.up * 0.1f; // Lekka parabola do góry

        rb.AddForce(direction * 100f, ForceMode.Impulse);
        
        // Usuwamy kulę po 5 sekundach, żeby nie zaśmiecać sceny
        Destroy(gameObject, 5f);
    }
}