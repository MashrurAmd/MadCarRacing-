using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwapButton : MonoBehaviour
{
    [SerializeField] private CarCarousel carCarousel; // Drag CarCarousel object here in inspector

    public void LoadScene(string Game)
    {
       
        PlayerPrefs.SetInt("SelectedCarIndex", carCarousel.GetCurrentIndex());
        PlayerPrefs.Save();

        SceneManager.LoadScene(Game);
    }
}
