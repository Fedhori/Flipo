using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DragActionTypeManager : MonoBehaviour {
  private DragActionType dragActionType;
  public Image colorFilter;
  public Image actionTypeImage;

  public void SetDragActionType(DragActionType value) {
    dragActionType = value;
    HandleActionType();
  }

  public DragActionType GetDragActionType() {
    return dragActionType;
  }

  void HandleActionType() {
    switch (dragActionType) {
      case DragActionType.INCREASE: {
        actionTypeImage.sprite = Resources.Load<Sprite>("Sprites/ActionType/Plus");
        break;
      }
      case DragActionType.DECREASE: {
        actionTypeImage.sprite = null;
        break;
      }
    }
  }

  public void HighlightContainer() {
    Color tempColor = colorFilter.color;
    tempColor.a = 0.1f;
    colorFilter.color = tempColor;
  }

  public void UnHighlightContainer() {
    Color tempColor = colorFilter.color;
    tempColor.a = 0;
    colorFilter.color = tempColor;
  }

  public void OnClick() {
    EditorTileManager.editorTileManager.SetCurrentDragAction(new DragAction(dragActionType,
      EditorTileManager.editorTileManager.currentDragAction.dragActionValue));
  }
}