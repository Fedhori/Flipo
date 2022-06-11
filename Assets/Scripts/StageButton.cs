using System;
using TMPro;
using UnityEngine;

public class StageButton : MonoBehaviour {
  public int stageNumber;
  public TextMeshProUGUI stageNumberText;

  public void SetStageNumber(int number) {
    stageNumber = number;
    stageNumberText.text = number.ToString();
  }

  public void OnClick() {
    StageManager.stageManager.StartStage(stageNumber);
  }
}