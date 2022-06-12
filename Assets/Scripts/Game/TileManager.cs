using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

// 타일과 관련된 로직들을 수행
public class TileManager : MonoBehaviour {
  public static TileManager tileManager;

  [SerializeField] private GameObject tilePrefab;
  [SerializeField] private GameObject actionContainerPrefab;

  [SerializeField] private RectTransform actionsContainer;

  [SerializeField] private Camera mainCamera;

  int[,] tileCounterArray;
  private int[,,] tileCounterMemoryArray;
  GameObject[,] tileObjectArray;

  DragAction[] dragActionArray;
  private GameObject[] dragActionObjectArray;

  private int actionIndex;
  int yLength;
  int xLength;
  int tileWidthOffset;
  int tileHeightOffset;
  int originalTileSize = 32;
  int tileSize = 64;

  private bool isDragging;
  private Vector2Int startCoordinate;
  private Vector2Int currentCoordinate;

  private void Awake() {
    tileManager = this;
    GenerateStage(StageManager.stageManager.currentStageInfo);
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

    if (Input.GetKeyDown(KeyCode.Escape)) {
      if (actionIndex > 0) {
        UndoAction();
      }
      else {
        SceneManager.LoadScene("MenuScene");
      }
    }
  }

  void OnHandleButtonDown(Vector2Int coordinate) {
    if (CheckIsTile(coordinate) && actionIndex < dragActionArray.Length) {
      isDragging = true;
      startCoordinate = coordinate;
    }
  }

  void OnHandleButtonUp() {
    if (isDragging) {
      MemoryTileCounterArray();
      UpdateTiles();
      HideActionContainer();
      ClearSelectedTiles();
      
      isDragging = false;
    }
  }

  void OnHandleButtonDrag(Vector2Int coordinate) {
    if (isDragging && CheckIsTile(coordinate) && CheckIsValidDrag(coordinate)) {
      currentCoordinate = coordinate;
      SetSelectedTiles();
    }
  }

  void UndoAction() {
    ShowActionContainer();
    for (int i = 0; i < xLength; i++) {
      for (int j = 0; j < yLength; j++) {
        tileCounterArray[i, j] = tileCounterMemoryArray[actionIndex, i, j];
        tileObjectArray[i, j].GetComponent<TileInfo>().SetCounter(tileCounterArray[i, j]);
      }
    }
  }

  void MemoryTileCounterArray() {
    for (int i = 0; i < xLength; i++) {
      for (int j = 0; j < yLength; j++) {
        tileCounterMemoryArray[actionIndex, i, j] = tileCounterArray[i, j];
      }
    }
  }

  void UpdateTiles() {
    int startX = Math.Min(startCoordinate.x, currentCoordinate.x);
    int endX = Math.Max(startCoordinate.x, currentCoordinate.x);
    int startY = Math.Min(startCoordinate.y, currentCoordinate.y);
    int endY = Math.Max(startCoordinate.y, currentCoordinate.y);

    for (int i = startX; i <= endX; i++) {
      for (int j = startY; j <= endY; j++) {
        HandleActionType(dragActionArray[actionIndex].dragActionType, i, j);
      }
    }
  }

  void ShowActionContainer() {
    actionIndex--;
    dragActionObjectArray[actionIndex].GetComponent<ActionManager>().ShowActionContainer();
  }

  void HideActionContainer() {
    dragActionObjectArray[actionIndex].GetComponent<ActionManager>().HideActionContainer();
    actionIndex++;
  }

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

  public void SetStage(StageInfo stageInfo) {
    xLength = stageInfo.tileCounterArray.GetLength(0);
    yLength = stageInfo.tileCounterArray.GetLength(1);

    tileHeightOffset = yLength * tileSize / 2 - tileSize / 2;
    tileWidthOffset = xLength * tileSize / 2 - tileSize / 2;

    tileCounterArray = stageInfo.tileCounterArray;
    tileCounterMemoryArray = new int[stageInfo.dragActionList.Length, xLength, yLength];
    tileObjectArray = new GameObject[xLength, yLength];

    dragActionArray = stageInfo.dragActionList;
    dragActionObjectArray = new GameObject[stageInfo.dragActionList.Length];
  }

  void GenerateTiles() {
    for (int i = 0; i < xLength; i++) {
      for (int j = 0; j < yLength; j++) {
        GameObject tile = Instantiate(tilePrefab, gameObject.transform);
        tile.transform.localScale =
          new Vector2((float) tileSize / originalTileSize, (float) tileSize / originalTileSize);
        tile.transform.Translate(i * tileSize - tileWidthOffset, j * tileSize - tileHeightOffset, 0);
        tile.GetComponent<TileInfo>().SetCounter(tileCounterArray[i, j]);
        tileObjectArray[i, j] = tile;
      }
    }
  }

  void GenerateActionContainers() {
    foreach (var item in dragActionArray.Select((value, index) => (value, index))) {
      GameObject actionContainer = Instantiate(actionContainerPrefab, actionsContainer);
      actionContainer.GetComponent<ActionManager>().SetDragAction(item.value);
      dragActionObjectArray[item.index] = actionContainer;
    }
  }

  void GenerateStage(StageInfo stageInfo) {
    SetStage(stageInfo);
    GenerateTiles();
    GenerateActionContainers();
  }

  void HandleActionType(DragActionType dragActionType, int x, int y) {
    switch (dragActionType) {
      case DragActionType.DECREASE:
        tileCounterArray[x, y]--;
        break;
      case DragActionType.INCREASE:
        tileCounterArray[x, y]++;
        break;
    }

    tileObjectArray[x, y].GetComponent<TileInfo>().SetCounter(tileCounterArray[x, y]);
  }
}