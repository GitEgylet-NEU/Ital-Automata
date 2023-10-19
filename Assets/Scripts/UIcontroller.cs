using JetBrains.Annotations;
using System.ComponentModel;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
public class UIcontroller : MonoBehaviour
{
    public GameObject settingsmenu;
    UIDocument maindoc;
    Button buttoncalc, buttonset;
    Label error;
    TextField time, speed, size;
    ScrollView colours;
    public UnityEvent<int> errorstate;
    void Start()
    {
        errorstate.AddListener(errorchange);
        maindoc = GetComponent<UIDocument>();

        //button
        buttoncalc = maindoc.rootVisualElement.Q("calculate") as Button;
        buttoncalc.RegisterCallback<ClickEvent>(calc);

        buttonset = maindoc.rootVisualElement.Q("set") as Button;
        buttonset.RegisterCallback<ClickEvent>(set);

        //text
        error = maindoc.rootVisualElement.Q("error") as Label;

        //inpufield
        speed = maindoc.rootVisualElement.Q("sebesseg") as TextField;

        size = maindoc.rootVisualElement.Q("meret") as TextField;

        time = maindoc.rootVisualElement.Q("ido") as TextField;

        //scrollview
        colours = maindoc.rootVisualElement.Q("szinek") as ScrollView;
        colours.RegisterCallback<ClickEvent>(kolor);
        Color kolort = new Color(0f, 05f, 1f);

    }
    public void errorchange(int state) 
    {
        switch (state)
        {
            case 0:
                error.text = "jó";
                error.visible = false;
                break;

                case 1:
                error.visible = true;
                error.text = "nem megfelelõ bemenet, kérlek csak számokat használj";
                break;
            case 2:
                error.visible = true;
                error.text = "túl nagy a szám hogy leszimuláljuk";
                break;
        }
    }
    public void taimeset(InputEvent evt) 
    {
        Debug.Log(time.value);
    }
    public void calc(ClickEvent evt)
    {
        try
        {
           int timeint = int.Parse(time.value);
            int speedint = int.Parse(speed.value);
            int sizeint = int.Parse(size.value);
        }
        catch (System.Exception)
        {
            errorstate.Invoke(1);
            throw;
        }
        errorstate.Invoke(0);
        Debug.Log("Calculate");
        //call simulation func
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
