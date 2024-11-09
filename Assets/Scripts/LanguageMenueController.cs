using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageMenueController : MonoBehaviour
{
    public Dropdown dropdown;
    // Start is called before the first frame update
    void Start()
    {
        dropdown=GetComponent<Dropdown>();
    }

    // Update is called once per frame
    void Update()
    {
        //print(dropdown.value);
    }
}
