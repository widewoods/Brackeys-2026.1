using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class GrabFruit : MonoBehaviour
{
    [SerializeField] private Transform rightHand;
    // 오른손 원래 위치 및 각도
    private Vector3 originalHandLocalPos;
    private Quaternion originalHandLocalRot;

    private Transform cameraTransform;

    [SerializeField] private float range = 5f;
    
    // 과일을 잡기 위해 누르고 있던 시간을 측정할 변수
    private float currentHoldTime = 0f; 
    // 현재 바라보고 있는 과일을 기억할 변수 (시선을 돌리면 초기화하기 위함)
    private GameObject currentTarget = null;

    void Start()
    {
        cameraTransform = Camera.main.transform;

        originalHandLocalPos = rightHand.localPosition;
        originalHandLocalRot = rightHand.localRotation;
    }

    void Update()
    {
        RaycastHit hit;
        int layerMask = LayerMask.GetMask("Fruit");

        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, 10f, layerMask))
        {
            if (hit.distance <= range)
            {
                Debug.DrawRay(cameraTransform.position, cameraTransform.forward * hit.distance, Color.red);

                // 손 움직이기
                rightHand.position = Vector3.Lerp(rightHand.position, hit.point, Time.deltaTime * 10f);

                // 카메라 보는 방향으로 손 회전하기
                rightHand.rotation = Quaternion.Lerp(rightHand.rotation, Quaternion.LookRotation(hit.normal), Time.deltaTime * 10f);
                
                if (currentTarget != hit.collider.gameObject)
                {
                    currentTarget = hit.collider.gameObject;
                    currentHoldTime = 0f;
                }

                float grabtime = hit.collider.gameObject.GetComponent<FruitController>().Stats.grabTime;

                // 마우스 왼쪽 버튼을 '누르고 있는 동안(Hold)' 계속 실행
                if (Input.GetMouseButton(0))
                {
                    currentHoldTime += Time.deltaTime;
                    
                    Debug.Log($"잡는 중... {currentHoldTime:F1} / {grabtime:F1}");

                    // 누른 시간이 요구 시간(grabtime)을 채웠을 때
                    if (currentHoldTime >= grabtime)
                    {
                        Debug.Log("Grab!");
                        
                        currentHoldTime = 0f; 
                        
                        // TODO: 여기에 실제로 과일을 획득하거나 파괴하는 로직을 추가하세요.
                        hit.collider.gameObject.SetActive(false); // 예시로 과일을 비활성화
                    }
                }
                else
                {
                    currentHoldTime = 0f;
                }
            }
            else
            {
                ResetGrabState();
            }
        }
        else
        {
            Debug.DrawRay(cameraTransform.position, cameraTransform.forward * 10f, Color.green);
            // 허공을 보거나 다른 레이어를 볼 때 타이머 초기화
            ResetGrabState();
        }
    }

    // 초기화 코드가 중복되어 따로 함수로 분리했습니다.
    private void ResetGrabState()
    {
        currentHoldTime = 0f;
        currentTarget = null;
        // 손을 원래 위치로 되돌리기
        rightHand.localPosition = originalHandLocalPos;
        rightHand.localRotation = originalHandLocalRot;
    }
}