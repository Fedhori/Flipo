using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public static TileManager tileManager;

    public GameObject tilePrefab;

    int[,] tileCounterArray;
    GameObject[,] tileObjectArray;
    DragAction[] dragActionList;
    int tileWidth;
    int tileHeight;
    int tileWidthOffset;
    int tileHeightOffset;
    int originalTileSize = 32;
    int tileSize = 64;

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
                tile.transform.Translate(j * tileSize - tileWidthOffset, -i * tileSize + tileHeightOffset, 0);
                tile.GetComponent<TileInfo>().SetCounter(tileCounterArray[i, j]);
                //tileObjectArray[i, j] = tile;
            }
        }
    }
}