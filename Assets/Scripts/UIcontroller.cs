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

    //ui elements
    Button buttoncalc, buttonset;
    Label error, liters;
    TextField time, speed, size;
    ScrollView colours;
    VisualElement renderWindow;

    //events
    public UnityEvent<int> errorstate;
    public UnityEvent<float> speedevent, timeevent, sizeevent;
    void Start()
    {
        errorstate.AddListener(errorchange);

        //debug
        speedevent.AddListener(speedchange);

        maindoc = GetComponent<UIDocument>();

        //button
        buttoncalc = maindoc.rootVisualElement.Q("calculate") as Button;
        buttoncalc.RegisterCallback<ClickEvent>(calc);

        buttonset = maindoc.rootVisualElement.Q("set") as Button;
        buttonset.RegisterCallback<ClickEvent>(set);

        //text
        liters = maindoc.rootVisualElement.Q("litersUSS") as Label;
        error = maindoc.rootVisualElement.Q("error") as Label;

        //inpufield
        speed = maindoc.rootVisualElement.Q("sebesseg") as TextField;

        size = maindoc.rootVisualElement.Q("meret") as TextField;

        time = maindoc.rootVisualElement.Q("ido") as TextField;

        //scrollview
        colours = maindoc.rootVisualElement.Q("szinek") as ScrollView;
        colours.RegisterCallback<ClickEvent>(kolor);
        Color kolort = new Color(0f, 05f, 1f);

        //renderwindow
        renderWindow = maindoc.rootVisualElement.Q("rendererUSS") as VisualElement;

    }
    private void Update()
    {
        //hunornak ha nincs adat
        buttoncalc.SetEnabled(false);
        //uncomment merge után
        //liters.text = Utils.FormatValue(fluidContainer.liters);
    }

    //error box state change based on event int
    public void errorchange(int state) 
    {
        switch (state)
        {
            case 0:
                error.style.display = new(StyleKeyword.Null);
                error.text = "jó";
                renderWindow.style.display = StyleKeyword.Initial;

                break;

                case 1:
                renderWindow.style.display = StyleKeyword.Null;
                error.style.display = new(StyleKeyword.Initial);
                error.text = "nem megfelelõ bemenet, kérlek csak számokat használj";
                break;
            case 2:
                renderWindow.style.display = new(StyleKeyword.Null);
                error.style.display = new(StyleKeyword.Initial);
                error.text = "túl nagy a szám hogy leszimuláljuk";
                break;
        }
    }
    public void taimeset(InputEvent evt) 
    {
        Debug.Log(time.value);
    }

    //calc clicked, try to invoke inputfield data
    public void calc(ClickEvent evt)
    {
        Debug.Log("calc");
        try
        {
            timeevent.Invoke(float.Parse(time.value));
            speedevent.Invoke(float.Parse(speed.value));
            sizeevent.Invoke(float.Parse(size.value));
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

    //setting on or off
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

    //kolor test
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
    //debug
    public void speedchange(float state)
    {
        Debug.Log(state);  
     
    }
}
