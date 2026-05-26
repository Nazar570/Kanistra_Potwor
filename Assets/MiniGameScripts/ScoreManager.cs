using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [Header("UI")]
    public bool isInThrowZone;
    public TextMeshProUGUI tekstKomunikatowUI;

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

    void Start()
    {
        isInThrowZone = false;

        if (tekstKomunikatowUI != null)
            tekstKomunikatowUI.gameObject.SetActive(false);
    }

    void Update()
    {
        if (graSkonczona || blokadaTekstuTrafienia) return;

        if (tekstKomunikatowUI != null)
        {
            if (isInThrowZone && !instrukcjaJuzPokazana && !oddanoPierwszyRzut)
            {
                tekstKomunikatowUI.gameObject.SetActive(true);
                tekstKomunikatowUI.text =
                    "Rzuć nożem w środek każdej tarczy aby otrzymać nagrodę!\nNaciśnij G aby rzucić.";

                instrukcjaJuzPokazana = true;
            }
            else if (!isInThrowZone && instrukcjaJuzPokazana && !oddanoPierwszyRzut)
            {
                tekstKomunikatowUI.gameObject.SetActive(false);
                instrukcjaJuzPokazana = false;
            }
        }
    }

   
  public void OdtworzDzwiekRzutu() 
{ 
    Debug.Log("PRÓBA ODTWORZENIA DŹWIĘKU RZUTU");
    if (glosnik == null) Debug.LogError("BRAK AUDIO SOURCE!");
    if (dzwiekRzutu == null) Debug.LogError("BRAK PLIKU DŹWIĘKOWEGO!");
    
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

            if (tekstKomunikatowUI != null)
                tekstKomunikatowUI.gameObject.SetActive(false);
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

        int trafione =
            System.Convert.ToInt32(srodekTarczy1) +
            System.Convert.ToInt32(srodekTarczy2) +
            System.Convert.ToInt32(srodekTarczy3);

        if (tekstKomunikatowUI != null)
        {
            blokadaTekstuTrafienia = true;
            tekstKomunikatowUI.gameObject.SetActive(true);
            tekstKomunikatowUI.text =
                $"Trafiono środek tarczy {numerTarczy}!\nZostało jeszcze {3 - trafione} tarcz!";

            CancelInvoke(nameof(UkryjKomunikat));
            Invoke(nameof(UkryjKomunikat), 2f);
        }
    }

    void UkryjKomunikat()
    {
        blokadaTekstuTrafienia = false;

        if (tekstKomunikatowUI != null && !graSkonczona)
            tekstKomunikatowUI.gameObject.SetActive(false);
    }

    private void SprawdzWarunkiGry()
    {
        if (rzutyTarcza1 >= maxRzutowNaTarcze &&
            rzutyTarcza2 >= maxRzutowNaTarcze &&
            rzutyTarcza3 >= maxRzutowNaTarcze)
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

        if (tekstKomunikatowUI != null)
        {
            tekstKomunikatowUI.gameObject.SetActive(true);
            tekstKomunikatowUI.text =
                "<color=green>Brawo! Trafiłeś wszystkie środki!\nPodejdź do nagrody i naciśnij E!</color>";
        }

        if (prefabCzesciSamochodu != null && miejsceSpawnuNagrody != null)
        {
            GameObject nagroda = Instantiate(
                prefabCzesciSamochodu,
                miejsceSpawnuNagrody.position,
                miejsceSpawnuNagrody.rotation
            );

            OdbierzNagrode skryptOdbioru = FindFirstObjectByType<OdbierzNagrode>();

            if (skryptOdbioru != null)
                skryptOdbioru.UstawNagrodeNaZiemi(nagroda);
            else
                Debug.LogError("[ScoreManager] Nie znaleziono OdbierzNagrode!");
        }
    }

    public void CzyscTekstPoOdebraniuNagrody()
    {
        if (tekstKomunikatowUI != null)
        {
            tekstKomunikatowUI.text = "";
            tekstKomunikatowUI.gameObject.SetActive(false);
        }
    }

    void Przegrana()
    {
        graSkonczona = true;
        blokadaTekstuTrafienia = false;
        OdtworzDzwiekPrzegranej(); 

        CancelInvoke(nameof(UkryjKomunikat));

        if (tekstKomunikatowUI != null)
        {
            tekstKomunikatowUI.gameObject.SetActive(true);
            tekstKomunikatowUI.text =
                "<color=red>Nie trafiłeś wszystkich środków!\nSpróbuj ponownie...</color>";
        }

        Invoke(nameof(ResetujGre), 3f);
    }

    public void ResetujGre()
    {
        GameObject[] noze = GameObject.FindGameObjectsWithTag("Projectile");

        foreach (GameObject noz in noze)
            Destroy(noz);

        rzutyTarcza1 = 0;
        rzutyTarcza2 = 0;
        rzutyTarcza3 = 0;

        srodekTarczy1 = false;
        srodekTarczy2 = false;
        srodekTarczy3 = false;

        graSkonczona = false;
        oddanoPierwszyRzut = false;
        instrukcjaJuzPokazana = false;
        blokadaTekstuTrafienia = false;

        if (tekstKomunikatowUI != null)
            tekstKomunikatowUI.gameObject.SetActive(false);
    }
}