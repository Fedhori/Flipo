using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParsingButton : MonoBehaviour {
  public void OnClick() {
    EditorStageManager.editorStageManager.SerializeStage(
      EditorTileManager.editorTileManager.tileCounterArray,
      EditorTileManager.editorTileManager.dragActionArray);
  }
}