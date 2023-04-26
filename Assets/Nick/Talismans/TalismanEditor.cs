using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR

[CustomEditor(typeof(Talisman))]
public class TalismanEditor : Editor
{
    private void OnSceneGUI()
    {
        Talisman talisman = (Talisman)target;

        Handles.color = Color.white;

        foreach (Transform trans in talisman.spawnPointHolder.GetComponentInChildren<Transform>())
        {
            Handles.DrawWireCube(trans.position, Vector3.one);
        }
    }
}

#endif