using UnityEngine;

public class EpilogueManager : MonoBehaviour
{
    [Header("再生するシナリオタイプ")]
    [SerializeField] private NovelGame.ScenarioType scenarioType = NovelGame.ScenarioType.Clear;

    private GameObject novelCanvas;

    void Start()
    {
        FindNovelCanvas();

        // 入力マップをノベル用に切替
        InputManager.Instance.SwitchActionMap(GameMode.NovelGame);

        var novelmanager = NovelGame.NovelManager.Instance;
        // Inspector で設定したシナリオを読み込む
        novelmanager.userScriptManager.LoadScenario(scenarioType);
        novelCanvas?.SetActive(true);
        novelmanager.mainTextController.StartTextNovel();
    }

    private void FindNovelCanvas()
    {
        var novelManagerObj = GameObject.Find("NovelManager");
        if (novelManagerObj == null)
        {
            Debug.LogWarning("NovelManager がシーンに見つかりません。");
            return;
        }

        var canvas = novelManagerObj.GetComponentInChildren<Canvas>(true);
        novelCanvas = canvas?.gameObject;
        if (novelCanvas == null)
        {
            Debug.LogWarning("NovelCanvas が見つかりませんでした。");
        }
    }
}
