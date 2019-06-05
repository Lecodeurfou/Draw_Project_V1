using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class saveload : MonoBehaviour
{
    public GameObject textField;
    // Start is called before the first frame update
    void Awake()
    {
        textField = GameObject.FindWithTag("saveText");

    }

    public void saver()
    {
        Debug.Log(textField.GetComponent<TMP_InputField>().text);
        textField.SetActive(false);
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
