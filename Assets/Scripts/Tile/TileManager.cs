using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// 타일과 관련된 로직들을 수행
public class TileManager : MonoBehaviour {
  public static TileManager tileManager;

  public GameObject tilePrefab;

  [SerializeField] private Camera mainCamera;

  int[,] tileCounterArray;
  GameObject[,] tileObjectArray;
  DragAction[] dragActionList;
  private int actionIndex;
  int yLength;
  int xLength;
  int tileWidthOffset;
  int tileHeightOffset;
  int originalTileSize = 32;
  int tileSize = 64;

  private bool isDragging = false;
  private Vector2Int startCoordinate;
  private Vector2Int currentCoordinate;

  private void Awake() {
    tileManager = this;
  }

  // Start is called before the first frame update
  void Start() {
    SetStage(StageManager.stageManager.GetCurrentStageInfo(0));
    GenerateTiles();
  }

  // Update is called once per frame
  void Update() {
    Vector2Int coordinate = GetCoordinate(Input.mousePosition);
    if (Input.GetMouseButtonDown(0))
      OnHandleButtonDown(coordinate);
    if (Input.GetMouseButtonUp(0))
      OnHandleButtonUp();
    if (Input.GetMouseButton(0)) {
      OnHandleButtonDrag(coordinate);
    }
  }

  void OnHandleButtonDown(Vector2Int coordinate) {
    if (CheckIsTile(coordinate) && actionIndex < dragActionList.Length) {
      isDragging = true;
      startCoordinate = coordinate;
    }
  }

  void OnHandleButtonUp() {
    if (isDragging) {
      UpdateTiles(dragActionList[actionIndex]);
      ClearSelectedTiles();
      isDragging = false;
      actionIndex++;
    }
  }

  void OnHandleButtonDrag(Vector2Int coordinate) {
    if (isDragging && CheckIsTile(coordinate) && CheckIsValidDrag(coordinate)) {
      currentCoordinate = coordinate;
      SetSelectedTiles();
    }
  }

  void UpdateTiles(DragAction currentDragAction) {
    int startX = Math.Min(startCoordinate.x, currentCoordinate.x);
    int endX = Math.Max(startCoordinate.x, currentCoordinate.x);
    int startY = Math.Min(startCoordinate.y, currentCoordinate.y);
    int endY = Math.Max(startCoordinate.y, currentCoordinate.y);

    for (int i = startX; i <= endX; i++) {
      for (int j = startY; j <= endY; j++) {
        if (currentDragAction.dragActionType == DragActionType.DECREASE) {
          tileCounterArray[i, j]--;
          tileObjectArray[i, j].GetComponent<TileInfo>().SetCounter(tileCounterArray[i, j]);
        }
      }
    }
  }

  void HandleAction(DragAction dragAction) { }

  void SetSelectedTiles() {
    int startX = Math.Min(startCoordinate.x, currentCoordinate.x);
    int endX = Math.Max(startCoordinate.x, currentCoordinate.x);
    int startY = Math.Min(startCoordinate.y, currentCoordinate.y);
    int endY = Math.Max(startCoordinate.y, currentCoordinate.y);

    for (int i = 0; i < xLength; i++) {
      for (int j = 0; j < yLength; j++) {
        if (i >= startX && i <= endX && j >= startY && j <= endY) {
          tileObjectArray[i, j].GetComponent<TileInfo>().SetSelected(true);
        }
        else {
          tileObjectArray[i, j].GetComponent<TileInfo>().SetSelected(false);
        }
      }
    }
  }
  
  void ClearSelectedTiles() {
    for (int i = 0; i < xLength; i++) {
      for (int j = 0; j < yLength; j++) {
        tileObjectArray[i, j].GetComponent<TileInfo>().SetSelected(false);
      }
    }
  }

  bool CheckIsTile(Vector2Int coordinate) {
    return coordinate.x < xLength && coordinate.x >= 0 && coordinate.y < yLength && coordinate.y >= 0 &&
           tileCounterArray[coordinate.x, coordinate.y] != 0;
  }

  bool CheckIsValidDrag(Vector2Int targetCoordinate) {
    int startX = Math.Min(startCoordinate.x, targetCoordinate.x);
    int endX = Math.Max(startCoordinate.x, targetCoordinate.x);
    int startY = Math.Min(startCoordinate.y, targetCoordinate.y);
    int endY = Math.Max(startCoordinate.y, targetCoordinate.y);

    for (int i = startX; i <= endX; i++) {
      for (int j = startY; j <= endY; j++) {
        if (tileCounterArray[i, j] == 0)
          return false;
      }
    }

    return true;
  }

  Vector2Int GetCoordinate(Vector3 position) {
    Vector2 worldPosition = mainCamera.ScreenToWorldPoint(position);
    Vector2 offsetRemovedPosition = (Vector2) worldPosition + new Vector2(tileWidthOffset, tileHeightOffset);
    return new Vector2Int((int) (offsetRemovedPosition.x / tileSize + 0.5f),
      (int) (offsetRemovedPosition.y / tileSize + 0.5f));
  }

  void SetStage(StageInfo stageInfo) {
    tileCounterArray = stageInfo.tileCounterArray;
    dragActionList = stageInfo.dragActionList;
    actionIndex = 0;
    xLength = stageInfo.tileCounterArray.GetLength(0);
    yLength = stageInfo.tileCounterArray.GetLength(1);
    tileHeightOffset = xLength * tileSize / 2 - tileSize / 2;
    tileWidthOffset = yLength * tileSize / 2 - tileSize / 2;
    tileObjectArray = new GameObject[xLength, yLength];
  }

  void GenerateTiles() {
    for (int i = 0; i < xLength; i++) {
      for (int j = 0; j < yLength; j++) {
        GameObject tile = Instantiate(tilePrefab);
        tile.transform.Translate(i * tileSize - tileWidthOffset, j * tileSize - tileHeightOffset, 0);
        tile.GetComponent<TileInfo>().SetCounter(tileCounterArray[i, j]);
        tileObjectArray[i, j] = tile;
      }
    }
  }
}