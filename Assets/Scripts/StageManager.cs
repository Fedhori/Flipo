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
    DontDestroyOnLoad(gameObject);
  }

  // Start is called before the first frame update
  void Start() {
    SerializeStage("/StageData", "Test Stage 1");
    PlayerPrefs.GetInt("lastStage", 0);
    GenerateStageButtons();
  }

  public void SerializeStage(string downloadPath, string stageName) {
    StageInfo stageInfo = new StageInfo(
      new int[5, 6] {
        {1, 1, 2, 1, 1, 1},
        {1, 1, 2, 1, 1, 1},
        {2, 2, 3, 1, 1, 1},
        {1, 1, 1, 0, 0, 0},
        {1, 1, 1, 0, 0, 0},
      },
      new DragAction[] {
        new DragAction(DragActionType.DECREASE, 1),
        new DragAction(DragActionType.DECREASE, 1),
        new DragAction(DragActionType.DECREASE, 1),
      });
    File.WriteAllText(Path.Combine(Application.dataPath + downloadPath + "/" + stageName + ".json"),
      JsonConvert.SerializeObject(stageInfo));
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