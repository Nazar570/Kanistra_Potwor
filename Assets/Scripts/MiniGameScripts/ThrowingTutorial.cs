using UnityEngine;
using UnityEngine.InputSystem;

public class ThrowingTutorial : MonoBehaviour
{
    [Header("Referencje")]
    public Camera cam;           
    public Transform attackPoint;   
    public GameObject objectToThrow; 
    public GameObject celownikUI;

    [Header("Ustawienia")]
    public float throwForce = 35f;        
    public float throwUpwardForce = 1f;   
    public float throwCooldown = 0.5f;

    private bool readyToThrow = true;
    private ScoreManager sm;

    void Start()
    {
        ZnajdzScoreManager();

        if (celownikUI != null) 
        {
            celownikUI.SetActive(false);
        }
    }

    void Update()
    {
        if (sm == null) 
        {
            ZnajdzScoreManager();
            return;
        }

        SterujCelownikiem();

        if (Input.GetKeyDown(KeyCode.G))
        {
            if (!readyToThrow) return;

            if (sm.isInThrowZone)
            {
                Throw();
            }
            else
            {
                Debug.Log("Nie możesz rzucać! Stań na białym polu.");
            }
        }
    }

    void ZnajdzScoreManager()
    {
        sm = GetComponent<ScoreManager>();
        
        if (sm == null)
        {
            sm = FindFirstObjectByType<ScoreManager>();
        }
        
        if (sm != null)
        {
            Debug.Log("<color=cyan>[Tutorial] Pomyślnie podłączono ScoreManager do celownika.</color>");
        }
    }

    void SterujCelownikiem()
    {
        if (celownikUI == null) return;

        if (sm.isInThrowZone)
        {
            if (!celownikUI.activeSelf) celownikUI.SetActive(true);
        }
        else 
        {
            if (celownikUI.activeSelf) celownikUI.SetActive(false);
        }
    }

    void Throw()
    {
        if (objectToThrow == null || attackPoint == null)
        {
            Debug.LogError("Zapomniałeś przypisać Prefab lub AttackPoint!");
            return;
        }

        readyToThrow = false;
        
      
        if (sm != null) sm.OdtworzDzwiekRzutu();

  
        if (sm != null)
        {
           sm.ZarejestrujWykonanieRzutu(0);
        }

        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        Vector3 targetPoint = Physics.Raycast(ray, out hit) ? hit.point : ray.GetPoint(75);
        Vector3 direction = (targetPoint - attackPoint.position).normalized;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        GameObject projectile = Instantiate(objectToThrow, attackPoint.position, targetRotation);
        
        projectile.tag = "Projectile";

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb == null) {
            Debug.LogError("Prefab rzutki NIE MA komponentu Rigidbody!");
            Destroy(projectile); 
            return;
        }

        Collider projCollider = projectile.GetComponent<Collider>();
        Collider playerCollider = GetComponent<Collider>();
        if (projCollider != null && playerCollider != null)
        {
            Physics.IgnoreCollision(projCollider, playerCollider);
        }

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.AddForce(direction * throwForce, ForceMode.Impulse);
        rb.AddForce(Vector3.up * throwUpwardForce, ForceMode.Impulse);

        Invoke(nameof(ResetThrow), throwCooldown);
    }

    void ResetThrow() => readyToThrow = true;
}