using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float hideSpeed = 2f;
    [SerializeField] private float normalSpeed = 5f;
    [SerializeField] private float acceleration = 50f;
    [SerializeField] private float globalDeceleration = 2f; 
    [SerializeField] private float hideRotationSpeed = 60f;
    [SerializeField] private float rotationSpeed = 100f;

    [Header("Dash Settings")]
    [SerializeField] private float dashCoolTime = 3f;
    [SerializeField] private float dashSpeed = 25f;
    [SerializeField] private float dashDuration = 0.2f;
    private bool isDashing = false;
    private bool canDash = true;
    private bool isHiding = false;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource rollingSound; 
    [SerializeField] private AudioSource dashSound;    

    [Header("Mouse Settings")]
    [SerializeField] private float mouseSpeed = 1.5f;
    [SerializeField] private float mouseXLimit = 60f; 

    private Rigidbody rb;
    private float xRot; 
    private float mouseXRot; 
    private float yRot; 
    
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

    [SerializeField] private Renderer cartRenderer;

    public bool IsCatched = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        
        // 관성 로직을 직접 제어하기 위해 물리 드래그는 0으로 고정
        rb.drag = 0;

        Cursor.lockState = CursorLockMode.Locked;
        camTr = Camera.main.transform;
        
        mainCam = Camera.main;
        originalCamLocalPos = camTr.localPosition; 
        mainCam.fieldOfView = normalFOV;

        yRot = transform.eulerAngles.y; 
    }

    void Update()
    {
        Look();

        // A, D 키로 몸체 회전
        float h = IsCatched ? 0 : Input.GetAxisRaw("Horizontal") ;
        float rotation = isHiding ? hideRotationSpeed : rotationSpeed;
        yRot += h * rotation * Time.deltaTime;

        // W, S 키 이동 입력
        float v = IsCatched ? 0 : Input.GetAxisRaw("Vertical");
        moveInput = (transform.forward * v).normalized;

        UpdateSound();

        // 마우스 오른쪽 클릭(1)으로 대시
        if (Input.GetMouseButtonDown(1) && canDash && !isHiding && !IsCatched)
        {
            StartCoroutine(DashProcess());
        }

        isHiding = Input.GetKey(KeyCode.LeftShift) && !isDashing && !IsCatched;

        HandleCameraEffects();
        HandleShoppingCartAlpha();
    }

    void Look()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSpeed;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSpeed;

        xRot -= mouseY;
        xRot = Mathf.Clamp(xRot, -90f, 90f);

        mouseXRot += mouseX;
        mouseXRot = Mathf.Clamp(mouseXRot, -mouseXLimit, mouseXLimit);

        camTr.localRotation = Quaternion.Euler(xRot, mouseXRot, 0f);
    }

    void FixedUpdate()
    {
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

    // --- 추가된 관성 방향 수정 로직 ---
    if (currentHorizontalVelocity.magnitude > 0.1f)
    {
        // 현재 속도의 크기를 유지하면서, 방향만 현재 캐릭터의 앞방향(transform.forward) 쪽으로 서서히 보간
        // 이 수치(10f)가 높을수록 회전 시 즉각적으로 방향이 꺾이고, 낮을수록 크게 미끄러집니다.
        float rotationAlignmentSpeed = 5f; 
        Vector3 alignedVelocity = transform.forward * currentHorizontalVelocity.magnitude;
        currentHorizontalVelocity = Vector3.Lerp(currentHorizontalVelocity, alignedVelocity, Time.fixedDeltaTime * rotationAlignmentSpeed);
        
        // 변경된 속도를 실제 Rigidbody에 반영 (Y축 속도는 유지)
        rb.velocity = new Vector3(currentHorizontalVelocity.x, rb.velocity.y, currentHorizontalVelocity.z);
    }
    // ------------------------------

    // 1. 이동 입력이 있을 때 (가속)
    if (moveInput.magnitude > 0)
    {
        Vector3 targetVelocity = moveInput * maxRunSpeed;
        Vector3 velocityDiff = targetVelocity - currentHorizontalVelocity;
        
        rb.AddForce(velocityDiff * acceleration * Time.fixedDeltaTime, ForceMode.VelocityChange);
    }
    // 2. 입력이 없거나, 대시 후 감속이 필요할 때 (관성 저항 적용)
    else
    {
        Vector3 resistance = -currentHorizontalVelocity * globalDeceleration * Time.fixedDeltaTime;
        rb.AddForce(resistance, ForceMode.VelocityChange);
    }
}

    void UpdateSound()
    {
        float currentSpeed = new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude;
        float maxRunSpeed = isHiding ? hideSpeed : normalSpeed;

        if (currentSpeed > 0.1f && !isDashing)
        {
            if (!rollingSound.isPlaying) rollingSound.Play();
            rollingSound.volume = Mathf.Clamp01(currentSpeed / maxRunSpeed);
            rollingSound.pitch = 0.5f + (currentSpeed / maxRunSpeed) * 0.5f;
        }
        else
        {
            if (rollingSound.isPlaying) rollingSound.Pause();
        }
    }

    IEnumerator DashProcess()
    {
        canDash = false;
        isDashing = true;
        dashSound.Play(); 

        Vector3 dashDir = (moveInput.magnitude > 0.1f) ? moveInput : transform.forward;
        rb.velocity = dashDir * dashSpeed;

        yield return new WaitForSeconds(dashDuration);
        isDashing = false;

        yield return new WaitForSeconds(dashCoolTime);
        canDash = true;
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

    void HandleShoppingCartAlpha()
    {
        if (cartRenderer == null) return;
        float targetAlpha = isHiding ? 0.3f : 1f;
        Material[] materials = cartRenderer.materials;
        for (int i = 0; i < materials.Length; i++)
        {
            Color color = materials[i].color;
            color.a = targetAlpha;
            materials[i].color = color;
        }
    }
}