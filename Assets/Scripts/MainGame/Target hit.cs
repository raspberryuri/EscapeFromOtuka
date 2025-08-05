using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 他のスクリプトから呼ばれると、Inspector に登録された処理を実行するクラス
/// </summary>


namespace MainGame
{
    public class TargetHit : MonoBehaviour
    {
        [Header("ヒット時に呼び出すイベント")]
        public UnityEvent onHit;  // Inspectorで呼びたい関数を登録
        [Header("インプットパネルID")]
        [Tooltip("0: 高床\n1: 竪穴迷路\n2: 竪穴ツボ")]
        public int ID;

        /// <summary>
        /// 外部からこの関数を呼べば、登録された処理が実行される
        /// </summary>
        public void TriggerHit()
        {
            Debug.Log($"{gameObject.name} : TriggerHit 実行");
            GameManager.InputPanelID = ID;
            onHit?.Invoke();
        }
    }
}
