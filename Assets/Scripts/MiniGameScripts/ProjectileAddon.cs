using UnityEngine;

public class ProjectileAddon : MonoBehaviour
{
    private Rigidbody rb;
    private bool hit;

    void Start() => rb = GetComponent<Rigidbody>();

    private void OnCollisionEnter(Collision collision)
{
    if (hit) return;
    hit = true;

    rb.isKinematic = true; 
    transform.SetParent(collision.transform);

   
    Target target = collision.gameObject.GetComponent<Target>();
    if (target != null) 
    {
        target.Hit();
    }
}
}