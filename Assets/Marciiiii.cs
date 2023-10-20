using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Marciiiii : MonoBehaviour
{
    string kaki;
    public TMP_InputField input;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("asd"))
        {
            kaki = PlayerPrefs.GetString("asd");
        }
        input.text = kaki;
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetString("asd", kaki);
        PlayerPrefs.Save();
        for (int i = 0; i < 5; i++)
        {
            PlayerPrefs.SetInt($"x{i}x", 3);
            PlayerPrefs.SetInt($"x{i}y", 1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        kaki = input.text;
    }
}

