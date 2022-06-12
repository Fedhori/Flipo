using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

// 에디터의 타일과 관련된 로직들을 수행
public class EditorTileManager : MonoBehaviour {
  public static EditorTileManager editorTileManager;

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
  int yLength = 10;
  int xLength = 10;
  int tileWidthOffset;
  int tileHeightOffset;
  int originalTileSize = 32;
  int tileSize = 64;

  private const int MAXActionLength = 100;

  private bool isDragging;
  private Vector2Int startCoordinate;
  private Vector2Int currentCoordinate;

  public DragAction currentDragAction = new DragAction(DragActionType.DECREASE, 1);

  private void Awake() {
    editorTileManager = this;
    GenerateStage();
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
    Debug.Log(coordinate);
    if (CheckIsTile(coordinate)) {
      isDragging = true;
      startCoordinate = coordinate;
    }
  }

  void OnHandleButtonUp() {
    if (isDragging) {
      MemoryTileCounterArray();
      UpdateTiles();
      ShowActionContainer();
      ClearSelectedTiles();
      isDragging = false;
      actionIndex++;
    }
  }

  void OnHandleButtonDrag(Vector2Int coordinate) {
    if (isDragging && CheckIsTile(coordinate)) {
      currentCoordinate = coordinate;
      SetSelectedTiles();
    }
  }

  void UndoAction() {
    actionIndex--;
    HideActionContainer();
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
        HandleActionType(i, j);
      }
    }
  }

  void ShowActionContainer() {
    dragActionObjectArray[actionIndex].GetComponent<ActionManager>().ShowActionContainer();
  }

  void HideActionContainer() {
    dragActionObjectArray[actionIndex].GetComponent<ActionManager>().HideAction();
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
    return coordinate.x < xLength && coordinate.x >= 0 && coordinate.y < yLength && coordinate.y >= 0;
  }

  Vector2Int GetCoordinate(Vector3 position) {
    Vector2 worldPosition = mainCamera.ScreenToWorldPoint(position);
    Vector2 offsetRemovedPosition = (Vector2) worldPosition + new Vector2(tileWidthOffset, tileHeightOffset);
    return new Vector2Int((int) (offsetRemovedPosition.x / tileSize + 0.5f),
      (int) (offsetRemovedPosition.y / tileSize + 0.5f));
  }

  public void SetStage() {
    tileHeightOffset = xLength * tileSize / 2 - tileSize / 2;
    tileWidthOffset = yLength * tileSize / 2 - tileSize / 2;

    tileCounterArray = new int[xLength, yLength];
    tileCounterMemoryArray = new int[MAXActionLength, xLength, yLength];
    tileObjectArray = new GameObject[xLength, yLength];
    
    dragActionArray = new DragAction[MAXActionLength];
    dragActionObjectArray = new GameObject[MAXActionLength];
  }

  void GenerateTiles() {
    for (int i = 0; i < xLength; i++) {
      for (int j = 0; j < yLength; j++) {
        GameObject tile = Instantiate(tilePrefab, gameObject.transform);
        tile.transform.localScale =
          new Vector2((float) tileSize / originalTileSize, (float) tileSize / originalTileSize);
        tile.transform.Translate(i * tileSize - tileWidthOffset, j * tileSize - tileHeightOffset, 0);
        tile.GetComponent<TileInfo>().SetIsEditorTile(true);
        tile.GetComponent<TileInfo>().SetCounter(0);
        tileObjectArray[i, j] = tile;
      }
    }
  }
  
  void AddAction(DragAction dragAction) {
    GameObject actionContainer = Instantiate(actionContainerPrefab, actionsContainer);
    actionContainer.GetComponent<ActionManager>().SetDragAction(dragAction);
    dragActionObjectArray[actionIndex] = actionContainer;
    actionIndex++;
  }

  public void GenerateStage() {
    SetStage();
    GenerateTiles();
  }
  
  void HandleActionType(int x, int y) {
    switch (currentDragAction.dragActionType) {
      case DragActionType.DECREASE:
        tileCounterArray[x, y]++;
        break;
      case DragActionType.INCREASE:
        tileCounterArray[x, y]--;
        break;
    }

    dragActionArray[actionIndex] = currentDragAction;
    tileObjectArray[x, y].GetComponent<TileInfo>().SetCounter(tileCounterArray[x, y]);
  }
}