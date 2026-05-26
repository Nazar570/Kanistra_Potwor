using UnityEngine;

public class NozWbijanie : MonoBehaviour
{
    private Rigidbody rb;
    private bool zostalWbity = false;
    private bool trafilWSrodek = false; 

    [Header("Korekcja rotacji")]
    public Vector3 korekcjaRotacji = new Vector3(0, 0, 0);

    [Header("Rozmiar środka tarczy")]
    [Tooltip("Dostosuj do rozmiaru środka twojej tarczy")]
    public float promienSrodka = 0.3f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        gameObject.tag = "Projectile";
    }

    void Update()
    {
        if (zostalWbity) return;
        if (rb.linearVelocity.sqrMagnitude > 0.1f)
        {
            transform.rotation = Quaternion.LookRotation(rb.linearVelocity.normalized)
                               * Quaternion.Euler(korekcjaRotacji);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (zostalWbity) return;

        Target tarcza = collision.gameObject.GetComponentInParent<Target>();

        if (tarcza != null || !collision.gameObject.CompareTag("Player"))
        {
            if (tarcza != null)
            {
                Vector3 punktUderzenia = collision.contacts[0].point;
                float odleglosc = Vector3.Distance(punktUderzenia, tarcza.transform.position);

                Debug.Log($"[ODLEGLOSC] {odleglosc} od środka tarczy {tarcza.numerTarczy}");

                if (odleglosc <= promienSrodka)
                {
                    trafilWSrodek = true;
                    tarcza.isCenter = true;
                    Debug.Log($"[SRODEK] Trafiono środek tarczy {tarcza.numerTarczy}!");
                }
                else
                {
                    tarcza.isCenter = false;
                }

                WbijNoz(collision.transform);
                tarcza.Hit();
            }
            else
            {
               
                WbijNoz(collision.transform);
            }
        }
    }

    void WbijNoz(Transform parent)
    {
        zostalWbity = true;

        Vector3 dir = rb.linearVelocity;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;

        if (dir.sqrMagnitude > 0.01f)
        {
            transform.rotation = Quaternion.LookRotation(dir.normalized)
                               * Quaternion.Euler(korekcjaRotacji);
        }

        Collider knifeCollider = GetComponent<Collider>();
        if (knifeCollider != null) knifeCollider.enabled = false;

        transform.SetParent(parent);
    }
}