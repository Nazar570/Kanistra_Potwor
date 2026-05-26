using UnityEngine;

public class Canned : MonoBehaviour
{
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }


    void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.name.Contains("Sphere") || collision.relativeVelocity.magnitude > 2f)
        {
      
            rb.constraints = RigidbodyConstraints.None;
            Debug.Log("Puszka trafiona! Odblokowuję fizykę.");
        }
    }
}