using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DragActionType {
    INCREASE,
    DECREASE,
}
public class DragAction {
    public DragAction(DragActionType dragActionType, int dragActionValue) {
        this.dragActionType = dragActionType;
        this.dragActionValue = dragActionValue;
    }

    public DragActionType dragActionType;
    public int dragActionValue;
}