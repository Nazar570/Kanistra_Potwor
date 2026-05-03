using UnityEngine;

public class Dragandthrow : MonoBehaviour
{
    private Rigidbody rb;
    private bool isShoot = false;
    private Vector3 mousePressdownpos;
    private Vector3 mouseReleasepos;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

       
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        Debug.Log("Skrypt gotowy! Kliknij na kulę, żeby rzucić.");
    }

    void Update()
    {
        
        if (Input.anyKeyDown)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    void OnMouseDown()
    {
        Debug.Log("Złapałaś kulę!");
        mousePressdownpos = Input.mousePosition;
    }

    void OnMouseUp()
    {
        if (isShoot) return;

        mouseReleasepos = Input.mousePosition;
        
        if (Camera.main != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(mouseReleasepos);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("Celujesz w: " + hit.collider.name);
                Shoot(hit.point);
            }
            else
            {
                Debug.LogWarning("Nie trafiłaś w nic! Celuj w podłogę lub puszki.");
            }
        }
    }

    void Shoot(Vector3 target)
    {
        isShoot = true;
        Vector3 direction = (target - transform.position).normalized;
        
       
       direction += Vector3.up * 0.03f;

        rb.AddForce(direction * 100f, ForceMode.Impulse);
        Destroy(gameObject, 5f);
        Debug.Log("Kula wystrzelona!");
    }
}