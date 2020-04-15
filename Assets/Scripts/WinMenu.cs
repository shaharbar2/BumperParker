using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinMenu : MonoBehaviour
{
    public void ReloadGame()
    {
        SceneManager.LoadScene(1);
    }
}
