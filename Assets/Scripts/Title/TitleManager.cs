using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TitleManager : MonoBehaviour
{
    [SerializeField] private float delaySeconds = 2f;       // 遷移までの待ち時間
    [SerializeField] private Animator sceneChangeAnimator;  // フェード等のAnimator
    [SerializeField] private int nextSceneIndex = 1;        // 遷移先シーン番号

    private AsyncOperation preloadOperation;                // 事前ロード用
    private bool isButtonPressed = false;

    private void Start()
    {
        // BGM再生
        NovelSystems.NovelAudioManager.Instance.PlayBGM(NovelSystems.enAudioClip.BGM_default);

        // カーソルを表示し、固定を解除
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // 次のシーンを事前ロードしておく（遷移はまだしない）
        preloadOperation = SceneManager.LoadSceneAsync(nextSceneIndex);
        preloadOperation.allowSceneActivation = false;
    }


    // ボタンイベントで呼ばれる関数
    public void OnStartButtonPressed()
    {
        if (isButtonPressed) return; // 多重押し防止
        isButtonPressed = true;

        AudioManager.Instance.OneShotSE(enAudioClip.UI_StartSE);
        // アニメーション開始
        if (sceneChangeAnimator != null)
        {
            sceneChangeAnimator.SetTrigger("SceneChange");
        }

        // 遷移用コルーチンを開始
        StartCoroutine(ActivateSceneAfterDelay());
    }



    private IEnumerator ActivateSceneAfterDelay()
    {
        yield return new WaitForSeconds(delaySeconds);

        // 事前ロードが終わっていない場合は待機
        while (preloadOperation != null && preloadOperation.progress < 0.9f)
        {
            yield return null;
        }

        // シーン遷移実行
        if (preloadOperation != null)
        {
            preloadOperation.allowSceneActivation = true;
        }
    }
}
