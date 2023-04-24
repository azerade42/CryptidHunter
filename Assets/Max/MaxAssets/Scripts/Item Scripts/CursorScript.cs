using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorScript : MonoBehaviour
{   public void CursorHide()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
}
