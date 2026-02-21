using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float hideSpeed = 2f;
    [SerializeField] private float normalSpeed = 5f;
    [SerializeField] private float acceleration = 50f;
    [SerializeField] private float deceleration = 15f; // 관성을 위해 감속도를 낮춤
    [SerializeField] private float dashMomentumDeceleration = 5f; 

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

    private Rigidbody rb;
    private float xRot;
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

        yRot = transform.eulerAngles.y; 
    }

    void Update()
    {
        Look();
        
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        moveInput = (transform.right * h + transform.forward * v).normalized;

        // 사운드 로직: 실제 움직이는 속도에 비례해서 볼륨 조절
        float currentSpeed = new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude;
        float maxRunSpeed = isHiding ? hideSpeed : normalSpeed;

        if (currentSpeed > 0.1f && !isDashing)
        {
            if (!rollingSound.isPlaying) rollingSound.Play();
            
            // 속도에 따라 볼륨이 0에서 1 사이로 부드럽게 변함
            rollingSound.volume = Mathf.Clamp01(currentSpeed / maxRunSpeed);
            
            rollingSound.pitch = 0.5f + currentSpeed / maxRunSpeed * 0.5f;
        }
        else
        {
            if (rollingSound.isPlaying) rollingSound.Pause();
        }

        if (Input.GetKeyDown(KeyCode.F) && canDash && !isHiding)
        {
            StartCoroutine(DashProcess());
        }

        isHiding = Input.GetKey(KeyCode.LeftShift);

        HandleCameraEffects();
        HandleShoppingCartAlpha();
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
        // shift 키를 누르고 있을 때 쇼핑카트의 material의 알파값을 0.5로, 그렇지 않을 때 1로 변경
        if (cartRenderer == null) return;

        // 목표 알파값 설정
        float targetAlpha = isHiding ? 0.5f : 1f;

        // 인스펙터에 있는 여러 개의 머티리얼을 모두 가져와서 변경
        Material[] materials = cartRenderer.materials;
        for (int i = 0; i < materials.Length; i++)
        {
            Color color = materials[i].color;
            color.a = targetAlpha;
            materials[i].color = color; // 변경된 색상을 다시 적용 (핵심)
        }
    }

    void Look()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSpeed; 
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSpeed;

        xRot -= mouseY;
        xRot = Mathf.Clamp(xRot, -90f, 90f);
        camTr.localRotation = Quaternion.Euler(xRot, 0f, 0f);
        
        yRot += mouseX; 
    }
}