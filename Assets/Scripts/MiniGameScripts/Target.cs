using UnityEngine;

public class Target : MonoBehaviour
{
    [Header("Ustawienia Tarczy")]
    public int numerTarczy; 
    public bool isCenter;   
    
    public void Hit()
    {
        ScoreManager sm = FindFirstObjectByType<ScoreManager>();
        if (sm == null) return;

        if (isCenter)
        {
            
            sm.ZarejestrujTrafienieSrodka(numerTarczy);
            Debug.Log($"<color=green>[TARCZA] Trafiono ŚRODEK tarczy {numerTarczy}!</color>");
        }
        else
        {
            Debug.Log($"[TARCZA] Trafiono obwódkę tarczy {numerTarczy}.");
        }
    }
}

