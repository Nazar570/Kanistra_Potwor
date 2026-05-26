using UnityEngine;

public class DzwiekPuszki : MonoBehaviour
{
    [Header("Ustawienia Dźwięku")]
    public AudioClip dzwiekUderzenia;
    
    private AudioSource glosnikPuszki;
    private Rigidbody rb;
    private bool czyHuknal = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
       
        glosnikPuszki = gameObject.AddComponent<AudioSource>();
        glosnikPuszki.playOnAwake = false;
        glosnikPuszki.spatialBlend = 0.0f; 
    }

    void OnCollisionEnter(Collision collision)
    {
        
        if (collision.relativeVelocity.magnitude > 1.5f)
        {
           
            if (!czyHuknal && dzwiekUderzenia != null)
            {
                glosnikPuszki.PlayOneShot(dzwiekUderzenia);
                czyHuknal = true;
                
               
                Invoke("ResetujDzwiek", 0.5f);
            }
        }
    }

    void ResetujDzwiek()
    {
        czyHuknal = false;
    }
}