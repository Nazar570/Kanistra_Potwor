using UnityEngine;

public class DzwiekPuszki : MonoBehaviour
{
    [Header("Ustawienia Dźwięku")]
    public AudioClip dzwiekUderzenia;
    
    [Range(0f, 1f)] // Tworzy ładny suwak w Inspektorze od 0 do 1
    public float glosnoscDziweku = 0.3f; // Domyślnie ustawione na 30% głośności

    private AudioSource glosnikPuszki;
    private Rigidbody rb;
    private bool czyHuknal = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        glosnikPuszki = gameObject.AddComponent<AudioSource>();
        glosnikPuszki.playOnAwake = false;
        
        // ZMIANA NA 3D: Dźwięk będzie cichszy, im dalej gracz stoi od puszki
        glosnikPuszki.spatialBlend = 1.0f; 
        
        // Ustawiamy zasięg słyszalności w świecie 3D (opcjonalnie)
        glosnikPuszki.minDistance = 1f;
        glosnikPuszki.maxDistance = 15f;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > 1.5f)
        {
            if (!czyHuknal && dzwiekUderzenia != null)
            {
                // Przekazujemy naszą zmienną głośności jako drugi parametr!
                glosnikPuszki.PlayOneShot(dzwiekUderzenia, glosnoscDziweku);
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