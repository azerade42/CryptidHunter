using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NickSceneManager : MonoBehaviour
{
    public static void Restart()
    {
        SceneManager.LoadScene(0);
    }
}
