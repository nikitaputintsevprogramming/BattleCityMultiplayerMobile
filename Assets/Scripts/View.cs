using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class View : MonoBehaviour
{
    public Vector2 min;
    public Vector2 max;

void Start()
{
    // границы екрана
    min = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));
    max = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));
}

    void Update()
    {
        
    }
}
