using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public void Resume()
    {
        print("resume");
        if (EventManager.Instance.paused != null)
            EventManager.Instance.paused.Invoke();
    }

    public void Quit()
    {
        Cursor.lockState = CursorLockMode.Confined;
        SceneManager.LoadScene(0);
    }
}
