using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public enum DragActionType
{
    INCREASE,
    DECREASE,
}

[Serializable]
public class DragAction
{
    public DragAction(DragActionType dragActionType, int dragActionValue)
    {
        this.dragActionType = dragActionType;
        this.dragActionValue = dragActionValue;
    }
    
    public DragActionType dragActionType;
    public int dragActionValue;
}

[Serializable]
public class StageInfo
{
    public StageInfo(int[,] tileCounterArray, DragAction[] dragActionList)
    {
        this.tileCounterArray = tileCounterArray;
        this.dragActionList = dragActionList;
    }

    public int[,] tileCounterArray;
    public DragAction[] dragActionList;
}