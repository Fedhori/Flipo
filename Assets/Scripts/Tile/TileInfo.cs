using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TileInfo : MonoBehaviour {

  private static readonly Color[] tileColorArray = {
    new Color(140f / 255f, 179f / 255f, 105f / 255f),
    new Color(244f / 255f, 226f / 255f, 133f / 255f),
    new Color(244f / 255f, 162f / 255f, 89f / 255f),
    new Color(188f / 255f, 75f / 255f, 81f / 255f)
  };

  private bool isEditorTile;
  bool isSelected;
  int counter;

  public TextMeshPro textMeshPro;
  public SpriteRenderer tileColor;
  public SpriteRenderer colorFilter;

  public void SetCounter(int value) {
    counter = value;
    textMeshPro.text = counter.ToString();
    tileColor.color = tileColorArray[(counter + tileColorArray.Length - 1) % tileColorArray.Length];
    if (counter == 0 && !isEditorTile) {
      gameObject.SetActive(false);
    }
    else {
      gameObject.SetActive(true);
    }
  }

  public void SetIsEditorTile(bool value) {
    isEditorTile = value;
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