using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour {

	public void newGameBTN(string newGameLevel)
    {
        SceneManager.LoadScene(newGameLevel);
    }
    public void exitGameBTN()
    {
        Application.Quit();
    }
    public void creditsBTN(string Credits)
    {
        SceneManager.LoadScene(Credits);
    }
    public void BackBTN(string MainMenu)
    {
        SceneManager.LoadScene(MainMenu);
    }
    
}
