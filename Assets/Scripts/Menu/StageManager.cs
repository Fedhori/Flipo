using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;

// 스테이지 정보들을 저장하고 이를 파싱하는 역할을 수행
public class StageManager : MonoBehaviour {
  public static StageManager stageManager;
  public GameObject StageButtonPrefab;
  public RectTransform stageButtonsContainer;
  public List<TextAsset> stageJsonList;

  public StageInfo currentStageInfo;
  
  private void Awake() {
    stageManager = this;
  }

  private void Start() {
    GenerateStageButtons();
    PlayerPrefs.GetInt("lastStage", 0);
  }

  public StageInfo DeserializeStage(TextAsset stageJson) {
    return JsonConvert.DeserializeObject<StageInfo>(stageJson.text);
  }

  public void StartStage(int stageNumber) {
    currentStageInfo = DeserializeStage(stageJsonList[stageNumber]);
    SceneManager.LoadScene("GameScene");
  }

  public void GenerateStageButtons() {
    for (int i = 0; i < stageJsonList.Count; i++) {
      GameObject stageButton = Instantiate(StageButtonPrefab, stageButtonsContainer);
      stageButton.GetComponent<StageButton>().SetStageNumber(i);
    }
  }
}