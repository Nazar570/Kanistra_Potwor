using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [Header("UI")]
    public bool isInThrowZone;

    [Header("Limity rzutów")]
    public int maxRzutowNaTarcze = 5;

    private int rzutyTarcza1 = 0;
    private int rzutyTarcza2 = 0;
    private int rzutyTarcza3 = 0;

    private bool graSkonczona = false;
    private bool oddanoPierwszyRzut = false;
    private bool instrukcjaJuzPokazana = false;
    private bool blokadaTekstuTrafienia = false;

    private bool srodekTarczy1 = false;
    private bool srodekTarczy2 = false;
    private bool srodekTarczy3 = false;

    [Header("Nagroda")]
    public GameObject prefabCzesciSamochodu;
    public Transform miejsceSpawnuNagrody;

    [Header("Dźwięki")]
    public AudioSource glosnik;
    public AudioClip dzwiekRzutu;
    public AudioClip dzwiekWbicia;
    public AudioClip dzwiekWygranej;
    public AudioClip dzwiekPrzegranej;

    private HintManager hintManager;
    private string obecnyKomunikat = "";

    void Start()
    {
        isInThrowZone = false;
        hintManager = Object.FindFirstObjectByType<HintManager>();
    }

    void Update()
    {
        if (graSkonczona)
        {
            // Pozwól wyświetlać tekst o wygranej/przegranej zdefiniowany w metodach końcowych
            if (hintManager != null && !string.IsNullOrEmpty(obecnyKomunikat))
            {
                hintManager.ShowHint(obecnyKomunikat, 0.5f);
            }
            return;
        }

        if (!blokadaTekstuTrafienia)
        {
            if (isInThrowZone && !oddanoPierwszyRzut)
            {
                obecnyKomunikat = "Rzuć nożem w środek każdej tarczy aby otrzymać nagrodę!\nNaciśnij G aby rzucić.";
                instrukcjaJuzPokazana = true;
            }
            else if (!isInThrowZone && instrukcjaJuzPokazana && !oddanoPierwszyRzut)
            {
                obecnyKomunikat = "";
                instrukcjaJuzPokazana = false;
                UkryjPanelHinta();
            }
        }

        // Jeśli trwa wyświetlanie komunikatu o trafieniu (blokada), wymuszamy jego odświeżanie niezależnie od strefy
        if (blokadaTekstuTrafienia && hintManager != null && !string.IsNullOrEmpty(obecnyKomunikat))
        {
            hintManager.ShowHint(obecnyKomunikat, 0.5f);
        }
        // W przeciwnym wypadku wyświetlamy instrukcję startową tylko w strefie rzutu
        else if (isInThrowZone && hintManager != null && !string.IsNullOrEmpty(obecnyKomunikat))
        {
            hintManager.ShowHint(obecnyKomunikat, 0.5f);
        }
    }

    public void OdtworzDzwiekRzutu() 
    { 
        if (glosnik != null && dzwiekRzutu != null) 
            glosnik.PlayOneShot(dzwiekRzutu); 
    }
    public void OdtworzDzwiekWbicia() { if (glosnik != null && dzwiekWbicia != null) glosnik.PlayOneShot(dzwiekWbicia); }
    public void OdtworzDzwiekWygranej() { if (glosnik != null && dzwiekWygranej != null) glosnik.PlayOneShot(dzwiekWygranej); } 
    public void OdtworzDzwiekPrzegranej() { if (glosnik != null && dzwiekPrzegranej != null) glosnik.PlayOneShot(dzwiekPrzegranej); } 

    public bool CzyMoznaRzucicWTarcze(int numerTarczy)
    {
        if (graSkonczona) return false;

        if (numerTarczy == 1) return rzutyTarcza1 < maxRzutowNaTarcze;
        if (numerTarczy == 2) return rzutyTarcza2 < maxRzutowNaTarcze;
        if (numerTarczy == 3) return rzutyTarcza3 < maxRzutowNaTarcze;

        return false;
    }

    public void ZarejestrujWykonanieRzutu(int numerTarczy)
    {
        if (graSkonczona) return;

        if (!oddanoPierwszyRzut)
        {
            oddanoPierwszyRzut = true;
            obecnyKomunikat = "";
            UkryjPanelHinta();
        }

        if (numerTarczy == 1) rzutyTarcza1++;
        if (numerTarczy == 2) rzutyTarcza2++;
        if (numerTarczy == 3) rzutyTarcza3++;

        SprawdzWarunkiGry();
    }

    public void ZarejestrujTrafienieSrodka(int numerTarczy)
    {
        if (graSkonczona) return;

        if (numerTarczy == 1 && srodekTarczy1) return;
        if (numerTarczy == 2 && srodekTarczy2) return;
        if (numerTarczy == 3 && srodekTarczy3) return;

        if (numerTarczy == 1) srodekTarczy1 = true;
        if (numerTarczy == 2) srodekTarczy2 = true;
        if (numerTarczy == 3) srodekTarczy3 = true;

        if (srodekTarczy1 && srodekTarczy2 && srodekTarczy3)
        {
            Wygrana();
            return;
        }

        int trafione = System.Convert.ToInt32(srodekTarczy1) + System.Convert.ToInt32(srodekTarczy2) + System.Convert.ToInt32(srodekTarczy3);

        blokadaTekstuTrafienia = true;
        obecnyKomunikat = $"Trafiono środek tarczy {numerTarczy}!\nZostało jeszcze {3 - trafione} tarcz!";

        // NATYCHMIASTOWE WYMUSZENIE POKAZANIA: Zapobiega jednoklatkowemu opóźnieniu z Update i ignoruje stan isInThrowZone
        if (hintManager != null)
        {
            hintManager.ShowHint(obecnyKomunikat, 2.0f);
        }

        CancelInvoke(nameof(UkryjKomunikat));
        Invoke(nameof(UkryjKomunikat), 2f);
    }

    void UkryjKomunikat()
    {
        blokadaTekstuTrafienia = false;
        obecnyKomunikat = "";
        UkryjPanelHinta();
    }

    private void SprawdzWarunkiGry()
    {
        if (rzutyTarcza1 >= maxRzutowNaTarcze && rzutyTarcza2 >= maxRzutowNaTarcze && rzutyTarcza3 >= maxRzutowNaTarcze)
        {
            if (!srodekTarczy1 || !srodekTarczy2 || !srodekTarczy3)
                Przegrana();
        }
    }

    void Wygrana()
    {
        graSkonczona = true;
        blokadaTekstuTrafienia = false;
        OdtworzDzwiekWygranej(); 

        CancelInvoke(nameof(UkryjKomunikat));
        obecnyKomunikat = "<color=green>Brawo! Trafiłeś wszystkie środki!\nPodejdź do nagrody i naciśnij E!</color>";

        if (hintManager != null)
        {
            hintManager.ShowHint(obecnyKomunikat, 5.0f);
        }

        if (prefabCzesciSamochodu != null && miejsceSpawnuNagrody != null)
        {
            GameObject nagroda = Instantiate(prefabCzesciSamochodu, miejsceSpawnuNagrody.position, miejsceSpawnuNagrody.rotation);
            OdbierzNagrode skryptOdbioru = Object.FindFirstObjectByType<OdbierzNagrode>();

            if (skryptOdbioru != null)
                skryptOdbioru.UstawNagrodeNaZiemi(nagroda);
            else
                Debug.LogError("[ScoreManager] Nie znaleziono OdbierzNagrode!");
        }
    }

    public void CzyscTekstPoOdebraniuNagrody()
    {
        obecnyKomunikat = "";
        UkryjPanelHinta();
    }

    void Przegrana()
    {
        graSkonczona = true;
        blokadaTekstuTrafienia = false;
        OdtworzDzwiekPrzegranej(); 

        CancelInvoke(nameof(UkryjKomunikat));
        obecnyKomunikat = "<color=red>Nie trafiłeś wszystkich środków!\nSpróbuj ponownie...</color>";

        if (hintManager != null)
        {
            hintManager.ShowHint(obecnyKomunikat, 3.0f);
        }

        Invoke(nameof(ResetujGre), 3f);
    }

    private void UkryjPanelHinta()
    {
        if (hintManager != null && hintManager.hintPanel != null)
            hintManager.hintPanel.SetActive(false);
    }

    public void ResetujGre()
    {
        GameObject[] noze = GameObject.FindGameObjectsWithTag("Projectile");
        foreach (GameObject noz in noze) Destroy(noz);

        rzutyTarcza1 = 0; rzutyTarcza2 = 0; rzutyTarcza3 = 0;
        srodekTarczy1 = false; srodekTarczy2 = false; srodekTarczy3 = false;

        graSkonczona = false;
        oddanoPierwszyRzut = false;
        instrukcjaJuzPokazana = false;
        blokadaTekstuTrafienia = false;
        obecnyKomunikat = "";

        UkryjPanelHinta();
    }
}