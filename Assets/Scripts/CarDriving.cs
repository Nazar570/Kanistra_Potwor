using UnityEngine;

public class CarDriving : MonoBehaviour
{
    [Header("Ustawienia Jazdy")]
    public float predkoscJazdy = 15f;
    public float predkoscSkrecania = 80f;
    public float distanceToEnter = 5f;

    [Header("Pozycja Gracza (X=Lewo/Prawo, Y=Góra/Dół, Z=Przód/Tył)")]
    public Vector3 pozycjaWAuta = new Vector3(-0.3f, -0.31f, 1.23f);

    private bool isDriving = false;
    private Transform playerTransform;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;
        this.enabled = false;
    }

    void Update()
    {
        if (isDriving && Input.GetKeyDown(KeyCode.E))
        {
            WysiadajZAuta();
            return;
        }

        if (!isDriving) return;

        float gaz = 0f;
        float skret = 0f;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) gaz = 1f;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) gaz = -1f;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) skret = -1f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) skret = 1f;

        transform.Translate(Vector3.forward * gaz * predkoscJazdy * Time.deltaTime);
        if (gaz != 0)
            transform.Rotate(Vector3.up * skret * predkoscSkrecania * Time.deltaTime);
    }

    public void WymusWsiadanieBezposrednie()
    {
        if (!isDriving)
            WsiadajDoAuta();
    }

    void WsiadajDoAuta()
    {
        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) playerTransform = playerObj.transform;
            else return;
        }

        GameObject player = playerTransform.gameObject;

        CarInteraction ci = GetComponent<CarInteraction>();
        if (ci != null)
        {
            ci.zablokowanyNaTejScenie = true;
            ci.enabled = false;
        }

        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        player.transform.SetParent(this.transform);
        player.transform.localPosition = pozycjaWAuta;
        player.transform.localRotation = Quaternion.identity;

        isDriving = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void WysiadajZAuta()
    {
        if (playerTransform == null) return;
        GameObject player = playerTransform.gameObject;

        isDriving = false;
        player.transform.SetParent(null);
        player.transform.position = transform.position - (transform.right * 2f) + (Vector3.up * 0.5f);
        player.transform.rotation = Quaternion.LookRotation(transform.forward);

        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = true;

        MonoBehaviour pc = player.GetComponent("FirstPersonController") as MonoBehaviour;
        if (pc != null) pc.enabled = true;

        MonoBehaviour pi = player.GetComponent("PlayerInput") as MonoBehaviour;
        if (pi != null) pi.enabled = true;

        CarInteraction ci = GetComponent<CarInteraction>();
        if (ci != null) ci.zablokowanyNaTejScenie = false;

        CarRequirements cr = GetComponent<CarRequirements>();
        if (cr != null)
        {
            cr.GraczWysiadl();
            cr.blokadaWsiadania = true;
            cr.czasBlokady = 0.5f;
            
        }
    }
}