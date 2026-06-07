using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using TMPro; // DODANE: Wymagane, aby skrypt obsługiwał komponenty TextMeshPro

public class BearAttackEvent : MonoBehaviour
{
    [Header("Referencje do Scenki")]
    public GameObject bearObject;
    public Transform bearStartAndEscapePoint;
    public Transform carTargetPoint;
    public ParticleSystem smokeParticles;

    [Header("Ustawienia Animacji Niedźwiedzia")]
    public string runAnimationParam = "Speed"; 
    public string attackAnimationTrigger = "Attack";

    [Header("Ustawienia Czasowe")]
    public float czasTrwaniaDymu = 5f;

    private NavMeshAgent bearAgent;
    private Animator bearAnimator;
    private HintManager hintManager;
    private CarRequirements carRequirements;

    // Zmienne do zapamiętania oryginalnych ustawień gracza na czas blokady
    private float oryginalnyWalkSpeed = 4f;
    private float oryginalnySprintSpeed = 6f;
    private float oryginalnyJump = 1.2f;

    [Header("Strefa Minigry")]
    [Tooltip("Przeciągnij tutaj obiekt Minigame_Area z Hierarchy")]
    public GameObject minigameArea;
    public GameObject puszkiManager;

    [Header("Porządki po ataku (Nowe opcje)")]

    [Tooltip("Przeciągnij tu element tekstowy, który ma zmienić treść")]
    public TextMeshProUGUI tekstDoZmiany; // DODANE: Referencja do Twojego tekstu w UI
    
    [Tooltip("Wpisz tekst, jaki ma się pojawić po ataku niedźwiedzia")]
    public string nowyTekst = "Już otwarte"; // DODANE: Zawartość nowego napisu

    void Start()
    {
        hintManager = Object.FindFirstObjectByType<HintManager>();
        carRequirements = Object.FindFirstObjectByType<CarRequirements>();
        
        if (bearObject != null)
        {
            bearAgent = bearObject.GetComponent<NavMeshAgent>();
            bearAnimator = bearObject.GetComponent<Animator>();
            
            // Ukrywamy niedźwiedzia od razu po uruchomieniu gry, 
            // żeby nie stał w lesie przed rozpoczęciem misji
            bearObject.SetActive(false); 
        }

        if (smokeParticles != null)
        {
            smokeParticles.Stop();
        }
    }

    // Tę funkcję automatycznie wywołuje skrypt auta (CarRequirements) po wlaniu paliwa
    public void TriggerBearAttack(MonoBehaviour playerMovementScript)
    {
        StartCoroutine(BearCutsceneCoroutine(playerMovementScript));
    }

    private IEnumerator BearCutsceneCoroutine(MonoBehaviour playerMovement)
    {
        if (carRequirements != null) carRequirements.enabled = false;

        // --- 1. BLOKADA RUCHU (MYSZKA DZIAŁA) ---
        CharacterController charController = null;
        Rigidbody playerRb = null;

        if (playerMovement != null)
        {
            if (playerMovement.GetType().Name == "FirstPersonController")
            {
                ZmienPredkoscFPController(playerMovement, 0f, 0f, 0f);
            }
            else
            {
                charController = playerMovement.GetComponent<CharacterController>();
                if (charController != null) charController.enabled = false;

                playerRb = playerMovement.GetComponent<Rigidbody>();
                if (playerRb != null) playerRb.linearVelocity = Vector3.zero;
            }
        }

        if (hintManager != null) hintManager.ShowHint("Co to za dźwięk...?", 3f);
        yield return new WaitForSeconds(2.5f);

        // --- 2. POJAWIENIE SIĘ NIEDŹWIEDZIA I BIEG DO AUTA ---
        if (bearObject != null && bearStartAndEscapePoint != null && carTargetPoint != null)
        {
            // Teleportujemy ukrytego misia do punktu startowego w lesie...
            bearObject.transform.position = bearStartAndEscapePoint.position;
            bearObject.transform.rotation = bearStartAndEscapePoint.rotation;
            
            // ...i dopiero TERAZ robimy go widzialnym!
            bearObject.SetActive(true);
            
            if (bearAnimator != null) bearAnimator.SetFloat(runAnimationParam, 1f);
            
            if (bearAgent != null)
            {
                bearAgent.isStopped = false;
                bearAgent.SetDestination(carTargetPoint.position);

                while (bearAgent.remainingDistance > 1.5f || bearAgent.pathPending)
                {
                    yield return null;
                }
                bearAgent.isStopped = true;
            }

            // --- 3. UDERZENIE W AUTO ---
            if (bearAnimator != null)
            {
                bearAnimator.SetFloat(runAnimationParam, 0f);
                bearAnimator.SetTrigger(attackAnimationTrigger);
            }

            if (hintManager != null) hintManager.ShowHint("ROOOAAARRR!", 2f);
            yield return new WaitForSeconds(1.5f);

            if (smokeParticles != null) smokeParticles.Play();

            // --- 4. NIEDŹWIEDŹ UCIEKA DO LASU ---
            if (bearAgent != null)
            {
                bearAgent.isStopped = false;
                bearAgent.SetDestination(bearStartAndEscapePoint.position);
            }
            if (bearAnimator != null) bearAnimator.SetFloat(runAnimationParam, 1f);

            if (bearAgent != null)
            {
                while (bearAgent.remainingDistance > 1.5f || bearAgent.pathPending)
                {
                    yield return null;
                }
            }

            // Miś dobiegł z powrotem do lasu, więc znowu robimy go niewidzialnym
            bearObject.SetActive(false); 

            // --- TUTAJ: NIEDŹWIEDŹ ZNIKNĄŁ, AKTYWUJEMY STREFĘ MINIGRY ---
            if (minigameArea != null)
            {
                minigameArea.SetActive(true);
                puszkiManager.SetActive(true);
            }

           

            if (tekstDoZmiany != null)
            {
                tekstDoZmiany.text = nowyTekst; // Zamienia stary napis na "Już otwarte"
            }
        }

        // --- 5. OCZEKIWANIE NA OSTYGNIĘCIE SILNIKA ---
        if (hintManager != null) hintManager.ShowHint("No nie! Dym spod maski...\nAuto zepsute!", 4f);
        yield return new WaitForSeconds(czasTrwaniaDymu);

        if (smokeParticles != null) smokeParticles.Stop();

        // --- 6. ODBLOKOWANIE RUCHU GRACZA ---
        if (playerMovement != null)
        {
            if (playerMovement.GetType().Name == "FirstPersonController")
            {
                ZmienPredkoscFPController(playerMovement, oryginalnyWalkSpeed, oryginalnySprintSpeed, oryginalnyJump);
            }
            else
            {
                if (charController != null) charController.enabled = true;
            }
        }

        if (carRequirements != null)
        {
            carRequirements.enabled = true;
            carRequirements.blokadaWsiadania = false;
        }

    }

    private void ZmienPredkoscFPController(MonoBehaviour script, float walk, float sprint, float jump)
    {
        try
        {
            var type = script.GetType();
            
            if (walk == 0f && oryginalnyWalkSpeed == 4f)
            {
                oryginalnyWalkSpeed = (float)type.GetField("MoveSpeed").GetValue(script);
                oryginalnySprintSpeed = (float)type.GetField("SprintSpeed").GetValue(script);
                oryginalnyJump = (float)type.GetField("JumpHeight").GetValue(script);
            }

            type.GetField("MoveSpeed")?.SetValue(script, walk);
            type.GetField("SprintSpeed")?.SetValue(script, sprint);
            type.GetField("JumpHeight")?.SetValue(script, jump);
        }
        catch { }
    }
}