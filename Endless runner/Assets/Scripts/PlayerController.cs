using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Action<Vector3> TurnEvent;
    public Action<int> GameOverEvent;
    public Action<int> ScoreUpdateEvent;

    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask turnLayer;
    [SerializeField] LayerMask obstacleLayer;

    [SerializeField] CharacterController characterController;
    [SerializeField] Transform playerMeshTransform;
    [SerializeField] Animator animator;
    [SerializeField] AnimationClip slideAnimation;
    [SerializeField] PlayerInput playerInput;

    private readonly float initialPlayerSpeed = 10f;
    private readonly float maxPlayerSpeed = 15f;
    private readonly float playerSpeedIncreaseRate = 0.1f;
    private readonly float jumpHeight = 1f;
    private readonly float initialGravityValue = -9.81f;
    private readonly float scoreMultiplier = 10f;
    private readonly float groundDeathDistance = 20f;
    private readonly float jumpCoefficient = -3f;
    private readonly float maxAnimatorSpeed = 1.25f;
    private readonly Vector3 initialPlayerPosition = new Vector3(0, 2, 0);

    private float gravity;
    private float playerSpeed;
    private Vector3 playerVelocity;
    private Vector3 movementDirection = Vector3.forward;

    private bool sliding = false;
    private float score = 0;
    private float characterHeight;

    private InputAction turnAction;
    private InputAction jumpAction;
    private InputAction slideAction;
    private int slidingAnimationID;
    private bool isMovementEnabled = true;

    private void Awake()
    {
        turnAction = playerInput.actions["Turn"];
        jumpAction = playerInput.actions["Jump"];
        slideAction = playerInput.actions["Slide"];
        gravity = initialGravityValue;
        playerSpeed = initialPlayerSpeed;
        characterHeight = characterController.height;
        slidingAnimationID = Animator.StringToHash("Sliding");
    }

    private void Update()
    {
        if (isMovementEnabled == false)
            return;

        if(IsGrounded(groundDeathDistance) == false)
        {
            GameOver();
            return;
        }

        score += scoreMultiplier * Time.deltaTime;
        ScoreUpdateEvent?.Invoke((int)score);

        UpdateMovement();
    }

    private void OnEnable()
    {
        turnAction.performed +=  PlayerTurn;
        slideAction.performed += PlayerSlide;
        jumpAction.performed += PlayerJump;
    }

    private void OnDisable()
    {
        turnAction.performed -= PlayerTurn;
        slideAction.performed -= PlayerSlide;
        jumpAction.performed -= PlayerJump;
    }

    public void ResetPlayer()
    {
        gravity = initialGravityValue;
        playerSpeed = initialPlayerSpeed;
        transform.SetPositionAndRotation(initialPlayerPosition, Quaternion.identity);
        playerVelocity = Vector3.zero;
        movementDirection = Vector3.forward;
        score = 0;
        animator.speed = 1f;
        sliding = false;
        animator.enabled = true;
        playerMeshTransform.rotation = Quaternion.identity;
    }

    public void SetPlayerMovementState(bool state)
    {
        isMovementEnabled = state;
    }

    private void PlayerJump(InputAction.CallbackContext context)
    {
        if (isMovementEnabled == false)
            return;

        if (IsGrounded() == !sliding)
        {
            AudioManager.Instance.Play("Jump");

            playerVelocity.y += Mathf.Sqrt(jumpHeight * gravity * jumpCoefficient);
            characterController.Move(playerVelocity * Time.deltaTime);
        }
    }

    private void PlayerSlide(InputAction.CallbackContext context)
    {
        if (isMovementEnabled == false)
            return;

        if (!sliding && IsGrounded())
        {
            StartCoroutine(Slide());
        }
    }

    private IEnumerator Slide()
    {
        sliding = true;
        SetUpPlayerColliderSize();
        AudioManager.Instance.Play("Slide");

        animator.Play(slidingAnimationID);
        yield return new WaitForSeconds(slideAnimation.length / animator.speed);

        sliding = false;
        SetUpPlayerColliderSize();
    }

    private void PlayerTurn(InputAction.CallbackContext context)
    {
        if (isMovementEnabled == false)
            return;

        float turnValue = context.ReadValue<float>();
        Vector3? turnPosition = CheckTurn(turnValue);
        if (turnPosition.HasValue == false)
            return;

        AudioManager.Instance.Play("Turn");
        Vector3 targetDirection = Quaternion.AngleAxis(90 * turnValue, Vector3.up) * movementDirection;
        TurnEvent?.Invoke(targetDirection);
        Turn(turnValue, turnPosition.Value);
    }

    private Vector3? CheckTurn(float turnValue)
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 0.1f, turnLayer);
        if(hitColliders.Length != 0)
        {
            Tile tile = hitColliders[0].transform.parent.GetComponent<Tile>();
            if (tile.Turned == true)
                return null;

            TileType type = tile.Type;
            if ((type == TileType.LEFT && turnValue == -1) || (type == TileType.RIGHT && turnValue == 1) || (type == TileType.SIDEWAYS))
            {
                tile.Turned = true;
                return tile.Pivot.position;
            }
        }
        return null;
    }

    private void Turn(float turnValue, Vector3 turnPosition)
    {
        turnPosition.y = transform.position.y;
        Quaternion targetRotation = transform.rotation * Quaternion.Euler(0, 90 * turnValue, 0);
        characterController.enabled = false;
        transform.SetPositionAndRotation(turnPosition, targetRotation);
        characterController.enabled = true;
        movementDirection = transform.forward.normalized;
    }


    private void UpdateMovement()
    {
        float deltaTime = Time.deltaTime;
        characterController.Move(playerSpeed * deltaTime * transform.forward);

        if (IsGrounded() && playerVelocity.y < 0)
        {
            playerVelocity.y = 0;
        }

        playerVelocity.y += gravity * deltaTime;
        characterController.Move(playerVelocity * deltaTime);

        if (playerSpeed < maxPlayerSpeed)
        {
            playerSpeed += deltaTime * playerSpeedIncreaseRate;
            gravity = initialGravityValue - playerSpeed;

            if(animator.speed < maxAnimatorSpeed)
            {
                animator.speed += 1 / playerSpeed * deltaTime;
            }
        }
    }

    private bool IsGrounded(float length = 0.2f)
    {
        Vector3 raycastOriginOne = transform.position;
        raycastOriginOne.y += 0.1f - characterController.height / 2;
        return Physics.Raycast(raycastOriginOne, Vector3.down, out RaycastHit hit, length, groundLayer);
    } 
    
    private void GameOver()
    {
        StopAllCoroutines();  
        animator.enabled = false;
        sliding = false;
        SetUpPlayerColliderSize();
        GameOverEvent?.Invoke((int)score);
        gameObject.SetActive(false);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (((1 << hit.collider.gameObject.layer) & obstacleLayer) != 0)
        {
            AudioManager.Instance.Play("ObstacleHit");
            GameOver();
        }
    }

    private void SetUpPlayerColliderSize()
    {
        characterController.height = sliding ? characterHeight/2 : characterHeight;
        Vector3 slidingCenter = Vector3.zero;
        slidingCenter.y -= characterController.height / 2;
        characterController.center = sliding ? slidingCenter : Vector3.zero;
    }
}
