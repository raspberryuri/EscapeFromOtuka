using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(Rigidbody))]
public class Inputoperation : MonoBehaviour
{
    [SerializeField]
    [Tooltip("�v���C���[�̑��x����")]
    private float speed = 0;
    [SerializeField]
    [Tooltip("�J����")]
    private Transform cameraTS;

    private Rigidbody rb;
    private float movementX;
    private float movementY;
    // Start is called before the first frame update
    void Start()
    {
        // �v���C���[�ɃA�^�b�`����Ă���Rigidbody���擾
        rb = this.gameObject.GetComponent<Rigidbody>();
    }
    /// <summary>
    /// �ړ�����i�㉺���E�L�[�Ȃǁj���擾
    /// </summary>
    /// <param name="context"></param>
    public void OnMove(InputAction.CallbackContext context)
    {
        // ���͂����s���܂��͊J�n���ꂽ�Ƃ��̂ݏ���
        if (context.performed || context.started)
        {
            Vector2 movementVector = context.ReadValue<Vector2>();
            Debug.LogError($"movementVector: {movementVector}");
            movementX = movementVector.x;
            movementY = movementVector.y;
        }
        // ���͂��L�����Z�����ꂽ�Ƃ��i�L�[�𗣂����Ƃ��j�͒�~
        else if (context.canceled)
        {
            movementX = 0f;
            movementY = 0f;
        }
    }
    private void FixedUpdate()
    {
        // ���͒l������3���x�N�g�����쐬
        Vector3 movement = new Vector3(movementX, 0.0f, movementY);
        // rigidbody��AddForce���g�p���ăv���C���[�𓮂����B
        rb.AddForce( cameraTS.forward * movementY * speed);
        rb.AddForce( cameraTS.right * movementX * speed);
    }
}
