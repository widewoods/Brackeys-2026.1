using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float maxRunSpeed = 10f;
    [SerializeField] private float acceleration = 50f; // 초당 증가할 속도
    [SerializeField] private float deceleration = 40f; // 입력이 없을 때 멈추는 속도

    [SerializeField] private float dashCoolTime = 3f; // 대시 쿨타임
    [SerializeField] private float dashSpeed = 20f; // 대시 속도
    [SerializeField] private float dashDuration = 0.2f; // 대시 지속 시간
    private bool isDashing = false; // 대시 중인지 여부
    private bool canDash = true;

    [SerializeField] private float mouseSpeed = 1.5f;

    Vector3 currentVelocity;
    float xRot;
    Transform camTr;

    CharacterController cc;

    void Start()
    {
        cc = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        camTr = Camera.main.transform;
    }

    void Update()
    {
        Look();
        Move();
    }

    void Move()
    {
        if (isDashing)
        {
            cc.Move(currentVelocity * Time.deltaTime);
            return;
        }
        
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 targetDir = transform.right * h + transform.forward * v;
        Vector3 targetVelocity = targetDir.normalized * maxRunSpeed;

        if (Input.GetKeyDown(KeyCode.Space) && canDash) // dash
        {
            Debug.Log("Dash");
            StartCoroutine(DashProcess());
            return; // 대시 시작 프레임에서는 아래 가속 로직을 타지 않음
        }

        float speedChange = (targetDir.magnitude > 0) ? acceleration : deceleration;
        
        currentVelocity = Vector3.MoveTowards(currentVelocity, targetVelocity, speedChange * Time.deltaTime);

        cc.Move(currentVelocity * Time.deltaTime);
    }

    IEnumerator DashProcess()
    {
        canDash = false;
        isDashing = true;

        Vector3 dashDir = (cc.velocity.magnitude > 0.1f) ? currentVelocity.normalized : transform.forward;
        
        currentVelocity = dashDir * dashSpeed;

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;

        yield return new WaitForSeconds(dashCoolTime);
        canDash = true;
        Debug.Log("Dash Ready!");
    }

    

    void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSpeed;

        xRot -= mouseY;
        xRot = Mathf.Clamp(xRot, -90f, 90f);

        camTr.localRotation = Quaternion.Euler(xRot, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
}
