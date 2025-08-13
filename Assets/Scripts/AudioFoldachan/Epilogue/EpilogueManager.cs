using UnityEngine;
using NovelSystems;

public class EpilogueManager : MonoBehaviour
{
    public enum EpilogueType
    {
        GameClear,
        GameOver
    }

    [Header("エピローグの種類を選択")]
    [SerializeField] private EpilogueType epilogueType = EpilogueType.GameClear;

    [Header("ゲームクリア時のシナリオ名")]
    [SerializeField] private string clearScenarioName = "Clear";

    [Header("ゲームオーバー時のシナリオ名")]
    [SerializeField] private string gameOverScenarioName = "GameOver";

    void Start()
    {
        var novelManager = NovelManager.Instance;
        novelManager.SetHomeAndLogButtonsActive(true);

        switch (epilogueType)
        {
            case EpilogueType.GameClear:
                novelManager?.PlayScenario(clearScenarioName);
                break;

            case EpilogueType.GameOver:
                novelManager?.PlayScenario(gameOverScenarioName);
                break;
        }
    }
}
