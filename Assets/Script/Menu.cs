using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject menuPanel;   
    public GameObject selectPanel; 

    // Called by Play Button
    public void OnPlayButton()
    {
        menuPanel.SetActive(false);    
        selectPanel.SetActive(true);   
    }

    // Called by Exit Button
    public void OnExitButton()
    {
        Application.Quit(); 
        Debug.Log("Game Quit"); 
    }
}
