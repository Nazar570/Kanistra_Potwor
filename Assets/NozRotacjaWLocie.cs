using UnityEngine;

public class NozRotacjaWLocie : MonoBehaviour
{
    private Rigidbody rb;
    private bool wbity = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (wbity) return;

        if (rb.linearVelocity.sqrMagnitude > 0.1f)
        {
            Vector3 dir = rb.linearVelocity.normalized;
            transform.rotation = Quaternion.LookRotation(dir) * Quaternion.Euler(0, -180, 0);
        }
    }

    public void Wbij()
    {
        wbity = true;
    }
}