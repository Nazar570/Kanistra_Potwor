using UnityEngine;

public class Target : MonoBehaviour
{
    public bool isCenter;
    
    public void Hit()
    {
        
        ScoreManager sm = FindFirstObjectByType<ScoreManager>();
        if (sm != null)
        {
            sm.AddPoint();
        }
    }
}