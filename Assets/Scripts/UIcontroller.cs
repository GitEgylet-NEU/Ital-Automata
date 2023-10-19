using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEditor.ShaderGraph;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UIElements;
public class UIcontroller : MonoBehaviour
{
    public GameObject settingsmenu;
    UIDocument maindoc;
    Button buttoncalc, buttonset;
    Label error;
    TextField time, speed, size;
    ScrollView colours;
    void Start()
    {
        maindoc = GetComponent<UIDocument>();

        //button
        buttoncalc = maindoc.rootVisualElement.Q("calculate") as Button;
        buttoncalc.RegisterCallback<ClickEvent>(calc);

        buttonset = maindoc.rootVisualElement.Q("set") as Button;
        buttonset.RegisterCallback<ClickEvent>(set);

        //text
        error = maindoc.rootVisualElement.Q("error") as Label;
        error.text = "Test";

        //inpufield
        speed = maindoc.rootVisualElement.Q("sebesseg") as TextField;
        Debug.Log(speed.value);

        size = maindoc.rootVisualElement.Q("meret") as TextField;
        Debug.Log(size.value);

        time = maindoc.rootVisualElement.Q("ido") as TextField;
        Debug.Log(time.value);
        time.RegisterCallback<InputEvent>(taimeset);

        //scrollview
        colours = maindoc.rootVisualElement.Q("szinek") as ScrollView;
        colours.RegisterCallback<ClickEvent>(kolor);
        Color kolort = new Color(0f, 05f, 1f);

    }

    public void taimeset(InputEvent evt) 
    {
        Debug.Log(time.value);
    }
    public void calc(ClickEvent evt)
    {
        Debug.Log("Calculate");
        buttoncalc.style.backgroundColor = Color.red;
    }

    public void set(ClickEvent evt)
    {
        Debug.Log("set");
        if (settingsmenu.activeSelf == false)
        {
            settingsmenu.SetActive(true);
            Debug.Log("be");
        }
        else { settingsmenu.SetActive(false); Debug.Log("ki"); }
    }
    public void kolor(ClickEvent evt) 
    {
        //change scrollview children background color 
        foreach (var child in colours.Children())
        {
            float a = Random.Range(0f, 1f);
            float b = Random.Range(0f, 1f);
            float c = Random.Range(0f, 1f);
            float d = Random.Range(0f, 1f);
            child.style.backgroundColor = new Color(a,b,c,d);
            var children = child as Label;
            children.text = a.ToString() + " " + b.ToString() + " " + c.ToString() + " " + d.ToString() + " ";
        }
    }
}
