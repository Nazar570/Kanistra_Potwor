using UnityEngine;
using UnityEngine.SceneManagement; // To pozwala ładować sceny!

public class MenuScript : MonoBehaviour
{
    public void Graj()
    {
        // Ładuje scenę o nazwie "GameScene"
        SceneManager.LoadScene("Game Start");
    }

    public void Wyjdz()
    {
        // Wyłącza grę (działa po zbudowaniu .exe)
        Debug.Log("Wychodzę z gry!"); 
        Application.Quit();
    }
}
