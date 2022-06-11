using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[Serializable]
public class StageInfo {
  public StageInfo(int[,] tileCounterArray, DragAction[] dragActionList) {
    this.tileCounterArray = tileCounterArray;
    this.dragActionList = dragActionList;
  }
  
  public int[,] tileCounterArray;
  public DragAction[] dragActionList;
}