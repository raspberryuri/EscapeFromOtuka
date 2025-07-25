using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



namespace Upft
{
    /// <summary>
    /// 3Dビューから2Dビューへ遷移し、UIを表示するクラス
    /// </summary>
    public class TargetHit : MonoBehaviour
    {
        [Header("UI設定")]
        public GameObject UipanelObject; // Inspectorから設定する
        
        private Camera mainCamera;
        
        void Start()
        {
            // メインカメラを取得
            mainCamera = Camera.main;
            

            // クリック対象のオブジェクトの色を設定
            var renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = Color.green;
                
            }
        }

        // オブジェクトがクリックされた時に呼ばれる
        
        /// <summary>
        /// 2Dビューに切り替える処理
        /// </summary>
        public void SwitchTo2DView(GameObject playerObject)
        {
            // 1. カメラを2D（Orthographic）に切り替える
            if (mainCamera != null)
            {
                mainCamera.orthographic = true;
            }

            // 2. 設定されたInputFieldをアクティブ（表示）にする
            if (UipanelObject != null)
            {
                UipanelObject.SetActive(true);
            }
            UpftPlayerController playerController = playerObject.GetComponent<UpftPlayerController>();
            if (playerController != null)
            {
                //  GameObject全体ではなく、スクリプトだけを無効にする
                playerController.enabled = false;
            }

            //  UI操作のためにマウスカーソルのロックを解除する
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}

