using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TileInfo: MonoBehaviour
{ 
    public TileInfo(int counter)
    {
        this.counter = counter;
        textMeshPro.text = counter.ToString();
    }
    
    public int counter;

    public TextMeshPro textMeshPro;

    public void SetCounter(int value)
    {
        counter = value;
        textMeshPro.text = counter.ToString();
    }
}
