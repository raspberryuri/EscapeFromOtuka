using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    // GameManagerをキャッシュするための変数
    private UIManager uiManager;

    void Start()
    {
        // シーン内からUIManagerコンポーネントを持つオブジェクトを探して取得
        // パフォーマンスのため、毎回の衝突時ではなく最初に一度だけ取得します
        uiManager = FindObjectOfType<UIManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // 接触したオブジェクトのタグが "Player" かどうかを確認
        if (other.CompareTag("Player"))
        {
            // UIManagerが見つかっていれば、カウンターを増やすメソッドを呼び出す
            if (uiManager != null)
            {
                uiManager.AddScore(1);
            }

            // このオブジェクトを非表示（非アクティブ）にする
            gameObject.SetActive(false);
        }
    }
}