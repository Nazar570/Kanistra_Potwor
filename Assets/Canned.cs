using UnityEngine;

public class Canned : MonoBehaviour
{
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Ta funkcja uruchamia się automatycznie, gdy coś uderzy w puszkę
    void OnCollisionEnter(Collision collision)
    {
        // Sprawdzamy, czy to, co nas uderzyło, to kula (Sphere)
        if (collision.gameObject.name.Contains("Sphere") || collision.relativeVelocity.magnitude > 2f)
        {
            // ODLOKOWUJEMY PUSZKĘ
            rb.constraints = RigidbodyConstraints.None;
            Debug.Log("Puszka trafiona! Odblokowuję fizykę.");
        }
    }
}