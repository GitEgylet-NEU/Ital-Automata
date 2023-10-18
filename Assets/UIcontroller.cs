using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UIElements;

public class UIcontroller : MonoBehaviour
{
    public GameObject settingsmenu;
    UIDocument maindoc;
    Button buttoncalc, buttonset;
    Label error;
    void Start()
    {
        maindoc = GetComponent<UIDocument>();

        buttoncalc = maindoc.rootVisualElement.Q("calculate") as Button;
        buttoncalc.RegisterCallback<ClickEvent>(calc);

        buttonset = maindoc.rootVisualElement.Q("set") as Button;
        buttonset.RegisterCallback<ClickEvent>(set);

        error = maindoc.rootVisualElement.Q("error") as Label;
        error.text = "Test";
      
    }

    // Update is called once per frame
    void Update()
    {
        
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
            maindoc.rootVisualElement.Blur();
            settingsmenu.SetActive(true);
            Debug.Log("be");
        }
        else { settingsmenu.SetActive(false); Debug.Log("ki"); }
    }
}
