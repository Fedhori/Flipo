using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// 타일과 관련된 로직들을 수행
public class TileManager : MonoBehaviour
{
    public static TileManager tileManager;

    public GameObject tilePrefab;

    [SerializeField] private Camera mainCamera;

    int[,] tileCounterArray;
    GameObject[,] tileObjectArray;
    DragAction[] dragActionList;
    int tileWidth;
    int tileHeight;
    int tileWidthOffset;
    int tileHeightOffset;
    int originalTileSize = 32;
    int tileSize = 64;

    private bool isDragging = false;
    private Vector2 dragStartCoordinate;

    private void Awake()
    {
        tileManager = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        SetStage(StageManager.stageManager.GetCurrentStageInfo(0));
        GenerateTiles();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            OnHandleButtonDown();
    }

    void OnHandleButtonDown()
    {
        Vector2 coordinate = GetCoordinate(Input.mousePosition);
        if (CheckIsTile(coordinate))
        {
            isDragging = true;
            dragStartCoordinate = coordinate;
            Debug.Log(coordinate);
        }
    }

    bool CheckIsTile(Vector2 coordinate)
    {
        return coordinate.x < tileWidth && coordinate.x >= 0 && coordinate.y < tileHeight && coordinate.y >= 0 &&
               tileCounterArray[(int) coordinate.y, (int) coordinate.x] != 0;
    }

    Vector2 GetCoordinate(Vector3 position)
    {
        Vector2 worldPosition = mainCamera.ScreenToWorldPoint(position);
        Vector2 offsetRemovedPosition = (Vector2) worldPosition + new Vector2(tileWidthOffset, tileHeightOffset);
        return new Vector2((int) (offsetRemovedPosition.x / tileSize + 0.5f), (int) (offsetRemovedPosition.y / tileSize + 0.5f));
    }

    void SetStage(StageInfo stageInfo)
    {
        tileCounterArray = stageInfo.tileCounterArray;
        dragActionList = stageInfo.dragActionList;
        tileHeight = stageInfo.tileCounterArray.GetLength(0);
        tileWidth = stageInfo.tileCounterArray.GetLength(1);
        tileHeightOffset = tileHeight * tileSize / 2 - tileSize / 2;
        tileWidthOffset = tileWidth * tileSize / 2 - tileSize / 2;
    }

    void GenerateTiles()
    {
        for (int i = 0; i < tileHeight; i++)
        {
            for (int j = 0; j < tileWidth; j++)
            {
                GameObject tile = Instantiate(tilePrefab);
                tile.transform.Translate(j * tileSize - tileWidthOffset, i * tileSize - tileHeightOffset, 0);
                tile.GetComponent<TileInfo>().SetCounter(tileCounterArray[i, j]);
                //tileObjectArray[i, j] = tile;
            }
        }
    }
}