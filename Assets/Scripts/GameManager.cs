using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 게임의 전반적인 상태를 관리
public class GameManager : MonoBehaviour {
  public static GameManager gameManager;
  public int currentStage;

  private void Awake() {
    gameManager = this;
  }

  // Start is called before the first frame update
  void Start() { }

  // Update is called once per frame
  void Update() { }
}