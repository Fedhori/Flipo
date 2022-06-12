using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActionManager: MonoBehaviour {
  public TextMeshProUGUI actionValueText;
  public Image actionTypeImage;

  public void SetDragAction(DragAction dragAction) {
    HandleActionType(dragAction.dragActionType);
    HandleActionValue(dragAction.dragActionValue);
  }

  void HandleActionType(DragActionType dragActionType) {
    switch (dragActionType) {
      case DragActionType.INCREASE: {
        actionTypeImage.sprite = Resources.Load<Sprite>("/Sprites/ActionType/Plus");
        break;
      }
      case DragActionType.DECREASE: {
        actionTypeImage.sprite = null;
        break;
      }
    }
  }

  void HandleActionValue(int dragActionValue) {
    actionValueText.text = dragActionValue.ToString();
  }

  public void HideAction() {
    gameObject.SetActive(false);
  }

  public void ShowActionContainer() {
    gameObject.SetActive(true);
  }
}