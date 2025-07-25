using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshProを使用するために必要

public class UIManager : MonoBehaviour
{
    // InspectorからUIテキストをアタッチするための変数
    [SerializeField]
    private TextMeshProUGUI scoreText;
    [SerializeField]
    private TextMeshProUGUI TimerText;
    [SerializeField]
    private Image TimerImage; // タイマー用のUIイメージ

    [SerializeField]
    private float maxTimeSec = 60; // 最大時間（秒）
    private float timer; // タイマーの変数s

    // 現在のスコアを保持する変数
    private int score = 0;
    

    void Start()
    {
        // ゲーム開始時にスコア表示を初期化
        UpdateScoreText();
    }

    void Update()
    {
        // タイマーの更新（必要に応じて実装）
        // ここでは単純に時間をカウントダウンする例を示す
        if(timer < maxTimeSec)
        {
            timer += Time.deltaTime;
            TimerImage.fillAmount = 1 - (timer / maxTimeSec); // タイマーのUIイメージを更新
            TimerText.enabled = false; // タイマーのテキストを非表示にする
        }
        else
        {
            // タイムアップ時の処理をここに追加
            TimerImage.fillAmount = 0; // タイマーのUIイメージを0に設定
            Debug.Log("Time's up!");
            TimerText.enabled = true; // タイマーのテキストを非表示にする

        }
    }

    // スコアを加算するメソッド (他のスクリプトから呼び出せるようにpublicにする)
    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreText();
    }

    // UIテキストの表示を更新するメソッド
    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }
}