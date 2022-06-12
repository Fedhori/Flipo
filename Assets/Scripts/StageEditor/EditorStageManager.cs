using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class EditorStageManager : MonoBehaviour {
  public static EditorStageManager editorStageManager;

  private void Awake() {
    editorStageManager = this;
  }

  public void SerializeStage(int[,] tileCounterArray, DragAction[] dragActionArray) {
    Vector2Int startCoordinate = FindStartCoordinate(tileCounterArray);
    Vector2Int endCoordinate = FindEndCoordinate(tileCounterArray);
    int[,] refinedTileCounterArray =
      new int[endCoordinate.x - startCoordinate.x + 1, endCoordinate.y - startCoordinate.y + 1];
    for (int i = startCoordinate.x; i <= endCoordinate.x; i++) {
      for (int j = startCoordinate.y; j <= endCoordinate.y; j++) {
        refinedTileCounterArray[i - startCoordinate.x, j - startCoordinate.y] = tileCounterArray[i, j];
      }
    }

    DragAction[] refinedDragActionArray = dragActionArray.Where(dragAction => dragAction != null).ToArray();

    StageInfo stageInfo = new StageInfo(refinedTileCounterArray, refinedDragActionArray);
    File.WriteAllText(Path.Combine(Application.dataPath + "/StageData/StageJson.json"),
      JsonConvert.SerializeObject(stageInfo));
  }

  Vector2Int FindStartCoordinate(int[,] tileCounterArray) {
    int minX = EditorTileManager.editorTileManager.xLength;
    int minY = EditorTileManager.editorTileManager.yLength;
    for (int i = 0; i < EditorTileManager.editorTileManager.xLength; i++) {
      for (int j = 0; j < EditorTileManager.editorTileManager.yLength; j++) {
        if (tileCounterArray[i, j] != 0) {
          if (minX > i) {
            minX = i;
          }

          if (minY > j) {
            minY = j;
          }
        }
      }
    }

    return new Vector2Int(minX, minY);
  }

  Vector2Int FindEndCoordinate(int[,] tileCounterArray) {
    int maxX = 0, maxY = 0;
    for (int i = EditorTileManager.editorTileManager.xLength - 1; i >= 0; i--) {
      for (int j = EditorTileManager.editorTileManager.yLength - 1; j >= 0; j--) {
        if (tileCounterArray[i, j] != 0) {
          if (maxX < i) {
            maxX = i;
          }

          if (maxY < j) {
            maxY = j;
          }
        }
      }
    }

    return new Vector2Int(maxX, maxY);
  }
}