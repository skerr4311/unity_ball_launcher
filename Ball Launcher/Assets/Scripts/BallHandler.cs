using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

public class BallHandler : MonoBehaviour
{
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Rigidbody2D pivot;
    [SerializeField] private float detachDelay;
    [SerializeField] private float respawnDelay;
    private Camera mainCamera;
    private Rigidbody2D currentBallRigidbody;
    private SpringJoint2D currentBallSpringJoint;
    private bool isDragging;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;

        SpawnNewBall();
    }

    void OnEnable(){
        EnhancedTouchSupport.Enable();
    }

    void OnDisable() {
        EnhancedTouchSupport.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentBallRigidbody == null) { return; }
        if(UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches.Count == 0){
            if(isDragging){
                LaunchBall();
            }
            isDragging = false;
            return;
        }
        isDragging = true;
        currentBallRigidbody.isKinematic = true;

        Vector2 touchPosition = new Vector2();

        foreach(UnityEngine.InputSystem.EnhancedTouch.Touch touch in UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches){
            touchPosition += touch.screenPosition;
        }

        touchPosition /= UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches.Count;

        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(touchPosition);
        
        currentBallRigidbody.position = worldPosition;
    }

    private void LaunchBall() {
        currentBallRigidbody.isKinematic = false;
        currentBallRigidbody = null;

        Invoke(nameof(DelayRelease), detachDelay);
    }

    private void SpawnNewBall(){
        GameObject ballInstance = Instantiate(ballPrefab, pivot.position, Quaternion.identity);

        currentBallRigidbody = ballInstance.GetComponent<Rigidbody2D>();
        currentBallSpringJoint = ballInstance.GetComponent<SpringJoint2D>();

        currentBallSpringJoint.connectedBody = pivot;
    }

    private void DelayRelease(){
        currentBallSpringJoint.enabled = false;
        currentBallSpringJoint = null;

        Invoke(nameof(SpawnNewBall), respawnDelay);
    }
}
