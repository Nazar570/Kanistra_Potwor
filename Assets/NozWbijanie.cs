using UnityEngine;

public class NozWbijanie : MonoBehaviour
{
    private Rigidbody rb;
    private bool zostalWbity = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (zostalWbity) return;

        Target tarcza = collision.gameObject.GetComponentInParent<Target>();

        if (tarcza != null || !collision.gameObject.CompareTag("Player"))
        {
            WbijNoz(collision.transform);

            if (tarcza != null)
                tarcza.Hit();
        }
    }

    void WbijNoz(Transform parent)
    {
        zostalWbity = true;

        Vector3 dir = rb.linearVelocity;

        if (dir.sqrMagnitude > 0.01f)
        {
            transform.rotation = Quaternion.LookRotation(dir.normalized) 
                               * Quaternion.Euler(0, -180, 0);
        }

        rb.isKinematic = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        transform.SetParent(parent);
    }
}