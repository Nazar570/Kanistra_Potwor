using UnityEngine;

public class Dragandthrow : MonoBehaviour
{
    private Rigidbody rb;
    private bool isShoot = false;
    private Transform playerTransform;

    [Header("Ustawienia dystansu")]
    public float interactionDistance = 5f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

  
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
  
        if (playerTransform == null || isShoot) return;

        if (Time.timeScale == 0f) return;
        float distance = Vector3.Distance(transform.position, playerTransform.position);

      
        if (distance <= interactionDistance)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

   
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
        
 
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        Vector3 direction = (target - transform.position).normalized;
        direction += Vector3.up * 0.1f; // Lekka parabola do góry

        rb.AddForce(direction * 100f, ForceMode.Impulse);
        

        Destroy(gameObject, 5f);
    }
}