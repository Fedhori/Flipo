using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEditor.SceneManagement;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static StageManager stageManager;
    public TextAsset[] stageJsonList;

    private void Awake()
    {
        stageManager = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SerializeStage(string downloadPath, string stageName)
    {
        StageInfo stageInfo = new StageInfo(
            new int[10, 10]
            {
                {1, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 2, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 3, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            },
            new DragAction[]
            {
                new DragAction(DragActionType.DECREASE, 1),
                new DragAction(DragActionType.DECREASE, 1),
                new DragAction(DragActionType.DECREASE, 1),
            });
        File.WriteAllText(Path.Combine(Application.dataPath + downloadPath + "/" + stageName + ".json"),
            JsonConvert.SerializeObject(stageInfo));
    }

    public StageInfo DeserializeStage(TextAsset stageJson)
    {
       return JsonConvert.DeserializeObject<StageInfo>(stageJson.text);
    }

    public StageInfo GetCurrentStageInfo(int stageNumber)
    {
        return DeserializeStage(stageJsonList[stageNumber]);
    }
}