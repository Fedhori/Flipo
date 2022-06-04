using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TileInfo : MonoBehaviour {
  public TileInfo(int counter) {
    this.counter = counter;
    textMeshPro.text = counter.ToString();
  }

  private static readonly Color[] tileColorArray = {
    new Color(243f / 255f, 129f / 255f, 129f / 255f),
    new Color(252f / 255f, 227f / 255f, 138f / 255f),
    new Color(234f / 255f, 255f / 255f, 208f / 255f),
    new Color(149f / 255f, 225f / 255f, 211f / 255f)
  };

  bool isSelected;
  int counter;

  public TextMeshPro textMeshPro;
  public SpriteRenderer tileColor;
  public SpriteRenderer colorFilter;

  public void SetCounter(int value) {
    counter = value;
    textMeshPro.text = counter.ToString();
    tileColor.color = tileColorArray[(counter + tileColorArray.Length - 1) % tileColorArray.Length];
    if (counter == 0) {
      gameObject.SetActive(false);
    }
  }

  public void SetSelected(bool value) {
    isSelected = value;
    if (isSelected) {
      Color tempColor = colorFilter.color;
      tempColor.a = 0.1f;
      colorFilter.color = tempColor;
    }
    else {
      Color tempColor = colorFilter.color;
      tempColor.a = 0;
      colorFilter.color = tempColor;
    }
  }
}