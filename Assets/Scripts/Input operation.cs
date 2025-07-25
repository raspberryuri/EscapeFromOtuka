using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(Rigidbody))]
public class Inputoperation : MonoBehaviour
{
    [SerializeField]
    [Tooltip("プレイヤーの速度調整")]
    private float speed = 0;
    [SerializeField]
    [Tooltip("カメラ")]
    private Transform cameraTS;

    private Rigidbody rb;
    private float movementX;
    private float movementY;
    // Start is called before the first frame update
    void Start()
    {
        // プレイヤーにアタッチされているRigidbodyを取得
        rb = this.gameObject.GetComponent<Rigidbody>();
    }
    /// <summary>
    /// 移動操作（上下左右キーなど）を取得
    /// </summary>
    /// <param name="context"></param>
    public void OnMove(InputAction.CallbackContext context)
    {
        // 入力が実行中または開始されたときのみ処理
        if (context.performed || context.started)
        {
            Vector2 movementVector = context.ReadValue<Vector2>();
            Debug.LogError($"movementVector: {movementVector}");
            movementX = movementVector.x;
            movementY = movementVector.y;
        }
        // 入力がキャンセルされたとき（キーを離したとき）は停止
        else if (context.canceled)
        {
            movementX = 0f;
            movementY = 0f;
        }
    }
    private void FixedUpdate()
    {
        // 入力値を元に3軸ベクトルを作成
        Vector3 movement = new Vector3(movementX, 0.0f, movementY);
        // rigidbodyのAddForceを使用してプレイヤーを動かす。
        rb.AddForce( cameraTS.forward * movementY * speed);
        rb.AddForce( cameraTS.right * movementX * speed);
    }
}
