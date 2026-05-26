using UnityEngine;

public class FlyingKnife : MonoBehaviour
{
    private Rigidbody rb;
    private bool zostalWbity = false;
    private bool trafilWSrodek = false;

    [Header("Korekcja rotacji - testuj wartości")]
    public Vector3 korekcjaRotacji = new Vector3(0, 0, 0);

    [Header("Rozmiar środka tarczy")]
    [Tooltip("Dostosuj do rozmiaru środka twojej tarczy")]
    public float promienSrodka = 0.15f; 

    private Vector3 ostatniKierunekLotu = Vector3.forward;
    private ScoreManager scoreManager;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        gameObject.tag = "Projectile";

      
        scoreManager = FindFirstObjectByType<ScoreManager>();
    }

    void Update()
    {
        if (zostalWbity) return;

        if (rb.linearVelocity.sqrMagnitude > 0.1f)
        {
            ostatniKierunekLotu = rb.linearVelocity.normalized;
            transform.rotation = Quaternion.LookRotation(ostatniKierunekLotu)
                               * Quaternion.Euler(korekcjaRotacji);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (zostalWbity) return;
        if (collision.gameObject.CompareTag("Player")) return;

        
        Target tarcza = collision.gameObject.GetComponent<Target>();
        if (tarcza == null) tarcza = collision.gameObject.GetComponentInParent<Target>();

        if (tarcza != null)
        {
            
            Vector3 punktUderzenia = collision.contacts[0].point;
            float odleglosc = Vector3.Distance(punktUderzenia, tarcza.transform.position);

            Debug.Log($"[TRAFIENIE] Tarcza {tarcza.numerTarczy}, odległość od centrum: {odleglosc}");

            
            if (scoreManager != null)
            {
                scoreManager.ZarejestrujWykonanieRzutu(tarcza.numerTarczy);
            }

            if (odleglosc <= promienSrodka)
            {
                trafilWSrodek = true;
                tarcza.isCenter = true;
                Debug.Log($"<color=green>[SUKCES] Trafiono środek tarczy {tarcza.numerTarczy}!</color>");

                
                if (scoreManager != null)
                {
                    scoreManager.ZarejestrujTrafienieSrodka(tarcza.numerTarczy);
                }
            }
            else
            {
                tarcza.isCenter = false;
                Debug.Log("[PUDŁO] Trafiono tarczę, ale poza środkiem.");
            }

            tarcza.Hit();
            WbijNoz(collision.transform);
        }
        else
        {
       
            WbijNoz(collision.transform);
        }
    }

    void WbijNoz(Transform parent)
    {
        zostalWbity = true;

        Collider knifeCollider = GetComponent<Collider>();
        if (knifeCollider != null) knifeCollider.enabled = false;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;

        transform.rotation = Quaternion.LookRotation(ostatniKierunekLotu)
                           * Quaternion.Euler(korekcjaRotacji);

        transform.position += transform.forward * 0.05f;

        transform.SetParent(parent);
    }
}