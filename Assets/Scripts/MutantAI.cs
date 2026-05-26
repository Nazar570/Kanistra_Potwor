using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class MutantAI : MonoBehaviour
{
    [Header("Ustawienia Ruchu")]
    public List<Transform> punktyPatrolu;
    public float predkoscPatrolu = 2f;
    public float predkoscPogoni = 6f; 

    [Header("Widzenie (Line of Sight)")]
    public Transform gracz;
    public float dystansWidzenia = 30f; 
    public float katWidzenia = 120f; 
    public LayerMask przeszkodyMask; 

    [Header("Walka")]
    public float cooldownAtaku = 1.5f;
    public float zasiegAtaku = 1.5f; 
    private float czasOstatniegoAtaku = 0;

    [Header("Warunek Bramy")]
    public DoorScript glownaBrama; 

    private NavMeshAgent agent;
    private Animator anim;
    private int aktualnyPunkt = 0;
    private bool czyGoni = false;

    private float czasUtkniecia = 0f;
    private bool przeciskaSie = false;
    private bool isStunned = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        
        if (gracz == null) {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if(playerObj != null) gracz = playerObj.transform;
        }

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true; 

        CapsuleCollider col = GetComponent<CapsuleCollider>();
        if (col != null) col.isTrigger = true; 

        IdzDoNastepnegoPunktu();
    }

    void Update()
    {
        if (isStunned) return;

        if (przeciskaSie) 
        {
            AktualizujAnimacje();
            return; 
        }

        if (!czyGoni)
        {
            if (CzyWidziGracza() || (glownaBrama != null && glownaBrama.isOpen)) 
            {
                czyGoni = true;
                agent.stoppingDistance = 0f; 
                agent.autoBraking = false;   
                agent.acceleration = 20f;    
            }
        }

        if (czyGoni)
        {
            GonGracza();
        }
        else
        {
            Patroluj();
        }

        AktualizujAnimacje();
    }

    // ---------------------------------------------
    // NOWE: Funkcja budząca potwora po ataku z tyłu
    // ---------------------------------------------
    public void ZauwazTrafienieWPlecy()
    {
        if (isStunned) return; 
        
        if (!czyGoni)
        {
            Debug.Log("Mutant dostał gazem w plecy! Zauważył Cię!");
            czyGoni = true;
            agent.stoppingDistance = 0f; 
            agent.autoBraking = false;   
            agent.acceleration = 20f; 
        }
    }

    public void OgluszMutanta()
    {
        if (isStunned) return; 
        StartCoroutine(RutynaOgluszenia());
    }

    IEnumerator RutynaOgluszenia()
    {
        isStunned = true;
        czyGoni = false; 
        agent.enabled = false; 

        anim.SetFloat("Speed", 0f);
        anim.SetTrigger("Death");

        float czasLezenia = Random.Range(10f, 15f);
        yield return new WaitForSeconds(czasLezenia);
        
        anim.Rebind(); 
        anim.Update(0f);
        
        agent.enabled = true;
        isStunned = false;

        agent.stoppingDistance = 2f; 
        agent.autoBraking = true;
        IdzDoNastepnegoPunktu();
    }

    bool CzyWidziGracza()
    {
        if(gracz == null) return false;
        float dystans = Vector3.Distance(transform.position, gracz.position);
        
        if (dystans < dystansWidzenia)
        {
            Vector3 kierunekDoGracza = (gracz.position - transform.position).normalized;
            float kat = Vector3.Angle(transform.forward, kierunekDoGracza);
            
            if (kat < katWidzenia / 2f)
            {
                RaycastHit hit;
                Vector3 startPromienia = transform.position + Vector3.up * 1.8f;
                if (Physics.Raycast(startPromienia, kierunekDoGracza, out hit, dystansWidzenia, przeszkodyMask))
                {
                    if (hit.transform == gracz) return true;
                }
            }
        }
        return false;
    }

    void Patroluj()
    {
        if (!agent.enabled) return;
        agent.speed = predkoscPatrolu;
        agent.stoppingDistance = 2f; 
        agent.autoBraking = true;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.2f)
        {
            IdzDoNastepnegoPunktu();
        }
    }

    void IdzDoNastepnegoPunktu()
    {
        if (punktyPatrolu.Count == 0 || !agent.enabled) return;
        agent.destination = punktyPatrolu[aktualnyPunkt].position;
        aktualnyPunkt = (aktualnyPunkt + 1) % punktyPatrolu.Count;
    }

    void GonGracza()
    {
        if (!agent.enabled || gracz == null) return;
        
        agent.speed = predkoscPogoni;

        NavMeshPath sciezka = new NavMeshPath();
        agent.CalculatePath(gracz.position, sciezka);

        if (sciezka.status == NavMeshPathStatus.PathPartial && sciezka.corners.Length > 0)
        {
            Vector3 krawedzPrzepasci = sciezka.corners[sciezka.corners.Length - 1];
            agent.SetDestination(krawedzPrzepasci);

            float dystansDoKrawedzi = Vector3.Distance(transform.position, krawedzPrzepasci);
            
            if (dystansDoKrawedzi <= 0.6f)
            {
                czasUtkniecia += Time.deltaTime;
                if (czasUtkniecia > 0.5f)
                {
                    StartCoroutine(PrzecisnijPrzezProg());
                }
            }
            else
            {
                czasUtkniecia = 0f;
            }
        }
        else
        {
            czasUtkniecia = 0f;
            agent.SetDestination(gracz.position);
        }

        Vector3 kierunekPatrzenia = new Vector3(gracz.position.x, transform.position.y, gracz.position.z);
        transform.LookAt(kierunekPatrzenia);

        float dystansDoGracza = Vector3.Distance(transform.position, gracz.position);

        if (dystansDoGracza <= zasiegAtaku) 
        {
            if (Time.time > czasOstatniegoAtaku + cooldownAtaku)
            {
                anim.SetTrigger("Attack");
                czasOstatniegoAtaku = Time.time;
                StartCoroutine(ZabijPoAnimacji());
            }
        }
    }

    IEnumerator PrzecisnijPrzezProg()
    {
        przeciskaSie = true;
        agent.enabled = false; 

        DoorScript znalezioneDrzwi = null;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 3f);
        
        foreach (var hitCollider in hitColliders)
        {
            DoorScript ds = hitCollider.GetComponentInParent<DoorScript>();
            if (ds != null)
            {
                znalezioneDrzwi = ds;
                break; 
            }
        }

        Vector3 startPos = transform.position;
        Vector3 celPos;

        if (znalezioneDrzwi != null)
        {
            if (!znalezioneDrzwi.isOpen)
            {
                znalezioneDrzwi.isOpen = true; 
                yield return new WaitForSeconds(0.2f); 
            }

            Vector3 kierunekPrzezDrzwi = (znalezioneDrzwi.transform.position - transform.position).normalized;
            kierunekPrzezDrzwi.y = 0; 
            celPos = znalezioneDrzwi.transform.position + kierunekPrzezDrzwi * 1.5f;
        }
        else
        {
            Vector3 kierunekDoGracza = (gracz.position - transform.position).normalized;
            kierunekDoGracza.y = 0;
            celPos = transform.position + kierunekDoGracza * 2.5f;
        }

        float czasSlizgu = 0.4f; 
        float t = 0;
        while (t < czasSlizgu)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, celPos, t / czasSlizgu);
            yield return null;
        }

        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 2.0f, NavMesh.AllAreas))
        {
            transform.position = hit.position;
        }

        agent.enabled = true;
        czasUtkniecia = 0f;
        przeciskaSie = false;
    }

    IEnumerator ZabijPoAnimacji()
    {
        yield return new WaitForSeconds(0.8f);
        PlayerDeath pd = gracz.GetComponent<PlayerDeath>();
        if (pd != null) pd.Die();
    }

    void AktualizujAnimacje()
    {
        if (przeciskaSie)
        {
            anim.SetFloat("Speed", 2f); 
            return;
        }

        if(!agent.enabled) return;
        
        if (czyGoni)
        {
            anim.SetFloat("Speed", 2f);
        }
        else
        {
            if (agent.velocity.magnitude > 0.2f)
                anim.SetFloat("Speed", 1f);
            else
                anim.SetFloat("Speed", 0f);
        }
    }
}
