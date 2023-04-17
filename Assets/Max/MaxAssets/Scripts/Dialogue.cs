using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI textBox;
    public string[] samLines;
    public float textSpeed;
    public GameObject dialogue;

    private string start = "Your code aint workin";
    private int index;
    private int currentIndex;
    // Start is called before the first frame update
    void Start()
    {
        dialogue.SetActive(true);
        textBox.text = start;
        index = 0;
        NewLine(0);
    }
    IEnumerator TypeLine()
    {
        foreach(char c in samLines[index].ToCharArray())
        {
            textBox.text += c;
            yield return new WaitForSeconds(textSpeed);
            currentIndex = index;
            yield return new WaitForSeconds(3);
            dialogue.SetActive(false);
        }
    }
    public void NewLine(int newIndex)
    {
        dialogue.SetActive(true);
        index = newIndex;
        TypeLine();
    }
}
