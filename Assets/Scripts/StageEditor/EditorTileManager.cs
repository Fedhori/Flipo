using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// 에디터의 타일과 관련된 로직들을 수행
public class EditorTileManager : MonoBehaviour {
  public static EditorTileManager editorTileManager;

  [SerializeField] private GameObject tilePrefab;
  [SerializeField] private GameObject actionContainerPrefab;
  [SerializeField] private GameObject actionTypeContainerPrefab;
  [SerializeField] private TMP_InputField actionValueInput;
  [SerializeField] private RectTransform actionsContainer;
  [SerializeField] private RectTransform actionTypesContainer;
  [SerializeField] private Camera mainCamera;

  public int[,] tileCounterArray;
  private int[,,] tileCounterMemoryArray;
  GameObject[,] tileObjectArray;

  public DragAction[] dragActionArray;
  private GameObject[] dragActionObjectArray;

  private DragActionTypeManager[] actionTypeManagerArray;

  private int actionIndex;
  public int xLength = 10;
  public int yLength = 10;
  int tileWidthOffset;
  int tileHeightOffset;
  int originalTileSize = 32;
  int tileSize = 64;

  public const int MAXActionLength = 100;

  private bool isDragging;
  private Vector2Int startCoordinate;
  private Vector2Int currentCoordinate;

  public DragAction currentDragAction;

  private void Awake() {
    editorTileManager = this;
    GenerateStage();
    GenerateActionTypes();
    SetCurrentDragAction(new DragAction(DragActionType.DECREASE, 1));
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
    if (CheckIsTile(coordinate)) {
      isDragging = true;
      startCoordinate = coordinate;
    }
  }

  void OnHandleButtonUp() {
    if (isDragging) {
      MemoryTileCounterArray();
      UpdateTiles();
      AddAction();
      ClearSelectedTiles();

      isDragging = false;
    }
  }

  void OnHandleButtonDrag(Vector2Int coordinate) {
    if (isDragging && CheckIsTile(coordinate)) {
      currentCoordinate = coordinate;
      SetSelectedTiles();
    }
  }

  void UndoAction() {
    DestroyActionContainer();
    for (int i = 0; i < yLength; i++) {
      for (int j = 0; j < xLength; j++) {
        tileCounterArray[i, j] = tileCounterMemoryArray[actionIndex, i, j];
        tileObjectArray[i, j].GetComponent<TileInfo>().SetCounter(tileCounterArray[i, j]);
      }
    }
  }

  void MemoryTileCounterArray() {
    for (int i = 0; i < yLength; i++) {
      for (int j = 0; j < xLength; j++) {
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

  public void UpdateCurrentDragActionValue() {
    currentDragAction = new DragAction(currentDragAction.dragActionType, Int32.Parse(actionValueInput.text));
  }

  void DestroyActionContainer() {
    actionIndex--;
    Destroy(dragActionObjectArray[actionIndex]);
  }

  void SetSelectedTiles() {
    int startX = Math.Min(startCoordinate.x, currentCoordinate.x);
    int endX = Math.Max(startCoordinate.x, currentCoordinate.x);
    int startY = Math.Min(startCoordinate.y, currentCoordinate.y);
    int endY = Math.Max(startCoordinate.y, currentCoordinate.y);

    for (int i = 0; i < yLength; i++) {
      for (int j = 0; j < xLength; j++) {
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
    for (int i = 0; i < yLength; i++) {
      for (int j = 0; j < xLength; j++) {
        tileObjectArray[i, j].GetComponent<TileInfo>().SetSelected(false);
      }
    }
  }

  bool CheckIsTile(Vector2Int coordinate) {
    return coordinate.x < yLength && coordinate.x >= 0 && coordinate.y < xLength && coordinate.y >= 0;
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

    tileCounterArray = new int[yLength, xLength];
    tileCounterMemoryArray = new int[MAXActionLength, yLength, xLength];
    tileObjectArray = new GameObject[yLength, xLength];

    dragActionArray = new DragAction[MAXActionLength];
    dragActionObjectArray = new GameObject[MAXActionLength];
  }

  void GenerateTiles() {
    for (int i = 0; i < yLength; i++) {
      for (int j = 0; j < xLength; j++) {
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

  void AddAction() {
    GameObject actionContainer = Instantiate(actionContainerPrefab, actionsContainer);
    actionContainer.GetComponent<ActionManager>().SetDragAction(currentDragAction);
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
        tileCounterArray[x, y] += currentDragAction.dragActionValue;
        break;
      case DragActionType.INCREASE:
        tileCounterArray[x, y] -= currentDragAction.dragActionValue;
        break;
    }

    dragActionArray[actionIndex] = currentDragAction;
    tileObjectArray[x, y].GetComponent<TileInfo>().SetCounter(tileCounterArray[x, y]);
  }

  void GenerateActionTypes() {
    Array actionTypeArray = Enum.GetValues(typeof(DragActionType));
    actionTypeManagerArray = new DragActionTypeManager[actionTypeArray.Length];

    int index = 0;
    foreach (DragActionType actionType in actionTypeArray) {
      DragActionTypeManager actionTypeManager = Instantiate(actionTypeContainerPrefab, actionTypesContainer)
        .GetComponent<DragActionTypeManager>();
      actionTypeManager.SetDragActionType(actionType);
      actionTypeManagerArray[index] = actionTypeManager;
      index++;
    }
  }

  public void SetCurrentDragAction(DragAction dragAction) {
    currentDragAction = dragAction;
    actionValueInput.text = dragAction.dragActionValue.ToString();
    UpdateActionTypeContainers();

    void UpdateActionTypeContainers() {
      foreach (DragActionTypeManager actionTypeManager in actionTypeManagerArray) {
        if (actionTypeManager.GetDragActionType() == currentDragAction.dragActionType) {
          actionTypeManager.HighlightContainer();
        }
        else {
          actionTypeManager.UnHighlightContainer();
        }
      }
    }
  }
}