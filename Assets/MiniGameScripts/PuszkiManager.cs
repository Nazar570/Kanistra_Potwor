using UnityEngine;
using System.Collections.Generic;

public class PuszkiManager : MonoBehaviour
{
    [Header("Nagroda")]
    public GameObject prefabNagrody;
    public Transform miejsceSpawnuNagrody;
    public OdbierzNagrode skryptOdbioru;

    [Header("Ustawienia Gry")]
    public Transform canHolder;
    public float dystansSpadku = 0.8f;

    [Header("Dźwięki")]
    public AudioSource glosnik;         
    public AudioClip dzwiekWygranej;    
    public AudioClip dzwiekResetu;      
    public AudioClip dzwiekPrzegranej;  
    public AudioClip dzwiekUderzeniaPuszki;

    private bool isInThrowZone = false;
    private bool graSkonczona = false;
    private bool czyMoznaResetowac = false;
    private bool fizykaOdrzucona = false;

    private HintManager hintManager;
    private string obecnyKomunikat = "";

    private struct SzablonObiektu
    {
        public GameObject prefabZapasowy;
        public Vector3 pozycjaStartowa;
        public Quaternion rotacjaStartowa;
    }

    private List<SzablonObiektu> zapasowePuszki = new List<SzablonObiektu>();
    private List<SzablonObiektu> zapasoweKule = new List<SzablonObiektu>();

    private List<GameObject> aktywnePuszki = new List<GameObject>();
    private List<GameObject> aktywneKule = new List<GameObject>();

    void OnEnable()
    {
        hintManager = Object.FindFirstObjectByType<HintManager>();

        if (glosnik == null) glosnik = GetComponent<AudioSource>();

        Canned[] znalezionePuszki = FindObjectsByType<Canned>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (Canned puszka in znalezionePuszki)
        {
            GameObject kopia = Instantiate(puszka.gameObject);
            foreach (Transform child in kopia.transform) { Destroy(child.gameObject); }

            kopia.SetActive(false);
            kopia.name = "Zapas_" + puszka.name;

            zapasowePuszki.Add(new SzablonObiektu {
                prefabZapasowy = kopia,
                pozycjaStartowa = puszka.transform.position,
                rotacjaStartowa = puszka.transform.rotation
            });

            aktywnePuszki.Add(puszka.gameObject);
            
            Rigidbody rb = puszka.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = true;
        }

        Rigidbody[] wszystkieRb = FindObjectsByType<Rigidbody>(FindObjectsSortMode.None);
        foreach (Rigidbody rb in wszystkieRb)
        {
            if (rb.GetComponent<Canned>() == null && rb.gameObject != canHolder?.gameObject && !rb.isKinematic)
            {
                GameObject kopia = Instantiate(rb.gameObject);
                foreach (Transform child in kopia.transform) { Destroy(child.gameObject); }
                MonoBehaviour throwScriptKopia = kopia.GetComponent<Dragandthrow>();
                if (throwScriptKopia != null) Destroy(throwScriptKopia);

                kopia.SetActive(false);
                kopia.name = "Zapas_" + rb.name;

                zapasoweKule.Add(new SzablonObiektu {
                    prefabZapasowy = kopia,
                    pozycjaStartowa = rb.transform.position,
                    rotacjaStartowa = rb.transform.rotation
                });

                aktywneKule.Add(rb.gameObject);
            }
        }

        fizykaOdrzucona = false;
    }

    void Update()
    {
        if (!fizykaOdrzucona && Input.GetMouseButtonDown(0) && isInThrowZone)
        {
            OdmrozFizykePuszek();
        }

        if (czyMoznaResetowac && Input.GetKeyDown(KeyCode.R))
        {
            ResetujGrePuszek();
            return;
        }

        if (isInThrowZone && fizykaOdrzucona && !czyMoznaResetowac && !graSkonczona)
        {
            OdswiezLicznikPuszek();
        }
        
        // USUNIĘTO STĄD WYWOŁYWANIE hintManager.ShowHint() CO KLATKĘ!
    }

    void OdmrozFizykePuszek()
    {
        fizykaOdrzucona = true;
        foreach (GameObject puszka in aktywnePuszki)
        {
            if (puszka != null)
            {
                Rigidbody rb = puszka.GetComponent<Rigidbody>();
                if (rb != null) rb.isKinematic = false;
            }
        }
    }

    public void UstawThrowZone(bool wStrefie)
    {
        isInThrowZone = wStrefie;
        
        if (wStrefie)
        {
            if (!fizykaOdrzucona && !graSkonczona)
            {
                WyswietlTekstStartowy();
            }
            else if (!graSkonczona)
            {
                OdswiezLicznikPuszek();
            }
        }
        else
        {
            UkryjPanelHinta();
        }
    }

    void WyswietlTekstStartowy()
    {
        obecnyKomunikat = $"Musisz strącić wszystkie puszki!\n<color=yellow>Puszki na stole: {zapasowePuszki.Count} | Pozostałe kule: {zapasoweKule.Count}</color>";
        
        // NOWOŚĆ: Wywołujemy hinta raz, z długim czasem (np. 999 sekund), bo zniknie sam jak gracz wyjdzie ze strefy
        AplikujKomunikatUI();
    }

    void OdswiezLicznikPuszek()
    {
        if (graSkonczona || czyMoznaResetowac) return;

        int puszekNaStoliku = 0;
        foreach (GameObject puszka in aktywnePuszki)
        {
            if (puszka != null && canHolder != null)
            {
                Vector3 pozStolika = new Vector3(canHolder.position.x, 0, canHolder.position.z);
                Vector3 pozPuszki = new Vector3(puszka.transform.position.x, 0, puszka.transform.position.z);
                
                float odleglosc = Vector3.Distance(pozStolika, pozPuszki);
                if (odleglosc < dystansSpadku && puszka.transform.position.y > canHolder.position.y - 0.2f)
                {
                    puszekNaStoliku++;
                }
            }
        }

        int pozostaloKul = 0;
        for (int i = 0; i < aktywneKule.Count; i++)
        {
            GameObject kula = aktywneKule[i];
            if (kula != null && i < zapasoweKule.Count)
            {
                if (Vector3.Distance(kula.transform.position, zapasoweKule[i].pozycjaStartowa) < 0.5f)
                {
                    pozostaloKul++;
                }
            }
        }

        if (puszekNaStoliku == 0 && zapasowePuszki.Count > 0)
        {
            graSkonczona = true;
            WszystkieZbite();
            return;
        }

        string nowyKomunikat = $"Musisz strącić wszystkie puszki!\n<color=yellow>Puszki na stole: {puszekNaStoliku} | Pozostałe kule: {pozostaloKul}</color>";
        
        // NOWOŚĆ: Aktualizujemy UI tylko, jeśli dane liczbowe faktycznie się zmieniły!
        if (nowyKomunikat != obecnyKomunikat)
        {
            obecnyKomunikat = nowyKomunikat;
            AplikujKomunikatUI();
        }

        if (pozostaloKul == 0 && puszekNaStoliku > 0)
        {
            Invoke("SprawdzKoniecRundki", 3.0f);
        }
    }

    void SprawdzKoniecRundki()
    {
        if (graSkonczona || czyMoznaResetowac) return;

        int puszekNaStoliku = 0;
        foreach (GameObject puszka in aktywnePuszki)
        {
            if (puszka != null && canHolder != null)
            {
                Vector3 pozStolika = new Vector3(canHolder.position.x, 0, canHolder.position.z);
                Vector3 pozPuszki = new Vector3(puszka.transform.position.x, 0, puszka.transform.position.z);
                if (Vector3.Distance(pozStolika, pozPuszki) < dystansSpadku && puszka.transform.position.y > canHolder.position.y - 0.2f)
                    puszekNaStoliku++;
            }
        }

        if (puszekNaStoliku > 0)
        {
            czyMoznaResetowac = true;
            obecnyKomunikat = "<color=red>Brak kul! Nie udało się strącić puszek.\nNaciśnij <color=white>[ R ]</color>, aby spróbować ponownie.</color>";
            AplikujKomunikatUI();
            OtworzDzwiek(dzwiekPrzegranej);
        }
    }

    void WszystkieZbite()
    {
        czyMoznaResetowac = false;
        CancelInvoke("SprawdzKoniecRundki");

        obecnyKomunikat = "<color=green>Brawo! Strąciłeś wszystkie puszki!\nPodejdź do nagrody i naciśnij <color=white>[ E ]</color>, aby ją odebrać.</color>";
        AplikujKomunikatUI();
        OtworzDzwiek(dzwiekWygranej);

        if (prefabNagrody != null && miejsceSpawnuNagrody != null)
        {
            GameObject nagroda = Instantiate(prefabNagrody, miejsceSpawnuNagrody.position, miejsceSpawnuNagrody.rotation);
            if (skryptOdbioru != null) skryptOdbioru.UstawSwiece(nagroda); 
        }
    }

    public void ResetujGrePuszek()
    {
        CancelInvoke("SprawdzKoniecRundki"); 

        graSkonczona = false;
        fizykaOdrzucona = false;
        czyMoznaResetowac = false; 

        OtworzDzwiek(dzwiekResetu);

        foreach (GameObject puszka in aktywnePuszki) { if (puszka != null) Destroy(puszka); }
        foreach (GameObject kula in aktywneKule) { if (kula != null) Destroy(kula); }

        aktywnePuszki.Clear();
        aktywneKule.Clear();

        foreach (var zapas in zapasowePuszki)
        {
            if (zapas.prefabZapasowy != null)
            {
                GameObject nowaPuszka = Instantiate(zapas.prefabZapasowy, zapas.pozycjaStartowa, zapas.rotacjaStartowa);
                nowaPuszka.SetActive(true);
                
                Rigidbody rb = nowaPuszka.GetComponent<Rigidbody>();
                if (rb != null) rb.isKinematic = true;

                aktywnePuszki.Add(nowaPuszka);
            }
        }

        foreach (var zapas in zapasoweKule)
        {
            if (zapas.prefabZapasowy != null)
            {
                GameObject nowaKula = Instantiate(zapas.prefabZapasowy, zapas.pozycjaStartowa, zapas.rotacjaStartowa);
                nowaKula.SetActive(true);
                nowaKula.AddComponent<Dragandthrow>();
                aktywneKule.Add(nowaKula);
            }
        }

        if (isInThrowZone)
        {
            WyswietlTekstStartowy();
        }
        else
        {
            UkryjPanelHinta();
        }

        Debug.Log("[MINIGRA] Reset udany. Odliczanie anulowane, tekst startowy włączony.");
    }

    void OtworzDzwiek(AudioClip clip)
    {
        if (glosnik != null && clip != null)
        {
            glosnik.PlayOneShot(clip);
        }
    }

    // NOWA FUNKCJA: Bezpiecznie wysyła aktualny tekst do interfejsu
    private void AplikujKomunikatUI()
    {
        if (hintManager != null && isInThrowZone && !string.IsNullOrEmpty(obecnyKomunikat))
        {
            // Ponieważ gracz stoi w strefie, dajemy duży czas trwania (np. 10 minut), 
            // a gdy wyjdzie ze strefy, panel i tak zamknie funkcja UkryjPanelHinta().
            hintManager.ShowHint(obecnyKomunikat, 600f); 
        }
    }

    private void UkryjPanelHinta()
    {
        if (hintManager != null && hintManager.hintPanel != null)
            hintManager.hintPanel.SetActive(false);
    }

    public void CzyscTekst()
    {
        CancelInvoke("SprawdzKoniecRundki");
        obecnyKomunikat = "";
        UkryjPanelHinta();
    }
}