using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableRenderer : MonoBehaviour
{
    public Renderer m_renderer;
    // Start is called before the first frame update
    void Start()
    {
        m_renderer=GetComponent<Renderer>();
        m_renderer.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
