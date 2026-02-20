using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float hideSpeed = 2f;
    [SerializeField] private float normalSpeed = 5f;
    [SerializeField] private float acceleration = 50f;
    [SerializeField] private float deceleration = 40f;
    [SerializeField] private float dashMomentumDeceleration = 5f; 

    [Header("Dash Settings")]
    [SerializeField] private float dashCoolTime = 3f;
    [SerializeField] private float dashSpeed = 25f;
    [SerializeField] private float dashDuration = 0.2f;
    private bool isDashing = false;
    private bool canDash = true;
    
    private bool isHiding = false;

    [Header("Mouse Settings")]
    [SerializeField] private float mouseSpeed = 1.5f;

    private Rigidbody rb;
    private float xRot;
    private float yRot; // 추가: 몸통 좌우 회전값을 누적할 변수
    private Transform camTr;
    private Vector3 moveInput;

    [Header("Camera Effect")]
    [SerializeField] private float normalFOV = 60f;
    [SerializeField] private float dashFOV = 80f;
    [SerializeField] private float dashZOffset = -0.5f;
    [SerializeField] private float hideYOffset = -1.0f; 
    [SerializeField] private float cameraLerpSpeed = 10f;

    private Camera mainCam;
    private Vector3 originalCamLocalPos;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.drag = 0;

        Cursor.lockState = CursorLockMode.Locked;
        camTr = Camera.main.transform;
        
        mainCam = Camera.main;
        originalCamLocalPos = camTr.localPosition; 
        mainCam.fieldOfView = normalFOV;

        // 추가: 시작할 때 플레이어의 현재 Y 회전값을 가져옴 (시작하자마자 0도로 튀는 현상 방지)
        yRot = transform.eulerAngles.y; 
    }

    void Update()
    {
        Look();
        
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        moveInput = (transform.right * h + transform.forward * v).normalized;

        // 수정: 앉아있지 않을 때(!isHiding)만 대시 가능하도록 조건 추가
        if (Input.GetKeyDown(KeyCode.Space) && canDash && !isHiding)
        {
            StartCoroutine(DashProcess());
        }

        isHiding = Input.GetKey(KeyCode.LeftShift);

        HandleCameraEffects();
    }

    void FixedUpdate()
    {
        // 수정: 물리 엔진 안에서 안전하게 몸통(좌우)을 회전시킴 -> 버벅임 해결!
        rb.MoveRotation(Quaternion.Euler(0f, yRot, 0f));

        if (!isDashing)
        {
            ApplyMovement();
        }
    }

    void ApplyMovement()
    {
        float maxRunSpeed = isHiding ? hideSpeed : normalSpeed;

        Vector3 currentHorizontalVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        Vector3 targetVelocity = moveInput * maxRunSpeed;
        Vector3 velocityDiff = targetVelocity - currentHorizontalVelocity;
        
        float speedChange;

        if (currentHorizontalVelocity.magnitude > maxRunSpeed)
        {
            speedChange = dashMomentumDeceleration;
        }
        else
        {
            speedChange = (moveInput.magnitude > 0) ? acceleration : deceleration;
        }

        rb.AddForce(velocityDiff * speedChange * Time.fixedDeltaTime, ForceMode.VelocityChange);
    }

    IEnumerator DashProcess()
    {
        canDash = false;
        isDashing = true;

        Vector3 dashDir = (moveInput.magnitude > 0.1f) ? moveInput : transform.forward;
        rb.velocity = dashDir * dashSpeed;
        Debug.Log("Dashing");

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;

        yield return new WaitForSeconds(dashCoolTime);
        canDash = true;
        Debug.Log("Dash is ready");
    }

    void HandleCameraEffects()
    {
        float targetFOV = isDashing ? dashFOV : normalFOV;
        mainCam.fieldOfView = Mathf.Lerp(mainCam.fieldOfView, targetFOV, Time.deltaTime * cameraLerpSpeed);

        float targetY = isHiding ? originalCamLocalPos.y + hideYOffset : originalCamLocalPos.y;
        float targetZ = isDashing ? originalCamLocalPos.z + dashZOffset : originalCamLocalPos.z;
        
        Vector3 targetPos = new Vector3(originalCamLocalPos.x, targetY, targetZ);
        
        camTr.localPosition = Vector3.Lerp(camTr.localPosition, targetPos, Time.deltaTime * cameraLerpSpeed);
    }

    void Look()
    {
        // 팁: GetAxis 대신 GetAxisRaw를 쓰면 마우스 인풋 딜레이가 줄어들어 더 빠릿합니다.
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSpeed; 
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSpeed;

        // 카메라 위아래(X축) 회전
        xRot -= mouseY;
        xRot = Mathf.Clamp(xRot, -90f, 90f);
        camTr.localRotation = Quaternion.Euler(xRot, 0f, 0f);
        
        // 수정: transform.Rotate를 지우고, yRot 변수에 마우스 좌우 이동값만 누적
        yRot += mouseX; 
    }
}