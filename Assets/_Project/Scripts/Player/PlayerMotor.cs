using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Project.Player
{
    /// <summary>
    /// Controller cinematico 2.5D: corsa, salto variabile, coyote time, jump buffer,
    /// doppio salto, scivolata e salto a muro, dash. Movimento sul piano X/Y, Z bloccata a 0.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMotor : MonoBehaviour
    {
        [Header("Corsa")]
        public float maxSpeed = 8f;
        public float groundAccel = 70f;
        public float groundDecel = 90f;
        public float airAccel = 45f;

        [Header("Salto")]
        public float jumpHeight = 3.2f;
        public float timeToApex = 0.38f;
        [Tooltip("Gravità extra in caduta: salto più 'pesante' e leggibile.")]
        public float fallGravityMultiplier = 1.8f;
        [Tooltip("Gravità extra se rilasci il tasto in salita: salto variabile.")]
        public float jumpCutMultiplier = 2.5f;
        public float maxFallSpeed = 22f;
        public float coyoteTime = 0.12f;
        public float jumpBuffer = 0.12f;
        public int airJumps = 1;

        [Header("Muro")]
        public LayerMask wallMask = ~0;
        public float wallCheckDistance = 0.15f;
        public float wallSlideMaxSpeed = 3f;
        public Vector2 wallJumpVelocity = new Vector2(7f, 11f);
        [Tooltip("Secondi in cui l'input orizzontale è ignorato dopo il salto a muro.")]
        public float wallJumpControlLock = 0.15f;

        [Header("Dash")]
        public float dashSpeed = 20f;
        public float dashDuration = 0.15f;
        public float dashCooldown = 0.4f;

        // Stato leggibile da altri componenti (es. PlayerVisuals)
        public Vector2 Velocity => velocity;
        public bool IsGrounded { get; private set; }
        public bool IsWallSliding { get; private set; }
        public bool IsDashing => dashTimer > 0f;
        public int Facing { get; private set; } = 1;
        public float MoveInput => moveInput;

        public event Action Jumped;
        public event Action WallJumped;
        public event Action Dashed;
        /// <summary>Parametro: velocità di caduta all'impatto (positiva).</summary>
        public event Action<float> Landed;

        CharacterController cc;
        Vector2 velocity;
        float gravity, jumpVelocity;
        float coyoteTimer, bufferTimer, controlLockTimer, dashTimer, dashCooldownTimer;
        int airJumpsLeft, wallDir, dashDir;
        bool dashAvailable = true;
        float moveInput;
        bool jumpHeld;

        InputAction moveAction, jumpAction, dashAction;

        void Awake()
        {
            cc = GetComponent<CharacterController>();

            moveAction = new InputAction("Move", InputActionType.Value, expectedControlType: "Axis");
            moveAction.AddCompositeBinding("1DAxis").With("Negative", "<Keyboard>/a").With("Positive", "<Keyboard>/d");
            moveAction.AddCompositeBinding("1DAxis").With("Negative", "<Keyboard>/leftArrow").With("Positive", "<Keyboard>/rightArrow");
            moveAction.AddBinding("<Gamepad>/leftStick/x").WithProcessor("axisDeadzone");

            jumpAction = new InputAction("Jump", InputActionType.Button);
            jumpAction.AddBinding("<Keyboard>/space");
            jumpAction.AddBinding("<Gamepad>/buttonSouth");

            dashAction = new InputAction("Dash", InputActionType.Button);
            dashAction.AddBinding("<Keyboard>/leftShift");
            dashAction.AddBinding("<Gamepad>/rightTrigger");
        }

        void OnEnable() { moveAction.Enable(); jumpAction.Enable(); dashAction.Enable(); }
        void OnDisable() { moveAction.Disable(); jumpAction.Disable(); dashAction.Disable(); }
        void OnDestroy() { moveAction.Dispose(); jumpAction.Dispose(); dashAction.Dispose(); }

        void Update()
        {
            float dt = Time.deltaTime;

            // Gravità e velocità di salto derivate da altezza e tempo all'apice (tuning intuitivo)
            gravity = 2f * jumpHeight / (timeToApex * timeToApex);
            jumpVelocity = gravity * timeToApex;

            ReadInput(dt);
            UpdateGroundState(dt);

            wallDir = CheckWall();
            IsWallSliding = !IsGrounded && wallDir != 0 && velocity.y < 0f
                            && Mathf.Abs(moveInput) > 0.1f && (int)Mathf.Sign(moveInput) == wallDir;

            controlLockTimer -= dt;
            dashCooldownTimer -= dt;

            if (Mathf.Abs(moveInput) > 0.1f && controlLockTimer <= 0f && !IsDashing)
                Facing = moveInput > 0f ? 1 : -1;

            if (dashAction.WasPressedThisFrame() && dashAvailable && dashCooldownTimer <= 0f)
                StartDash();

            if (IsDashing)
            {
                dashTimer -= dt;
                velocity = new Vector2(dashDir * dashSpeed, 0f);
                if (!IsDashing) velocity.x = dashDir * maxSpeed; // uscita morbida dal dash
            }
            else
            {
                ApplyHorizontal(dt);
                if (bufferTimer > 0f) TryJump();
                ApplyGravity(dt);
            }

            // Mantiene il contatto col terreno su discese e gradini
            if (IsGrounded && velocity.y < 0f) velocity.y = -2f;

            CollisionFlags flags = cc.Move(new Vector3(velocity.x, velocity.y, 0f) * dt);
            if ((flags & CollisionFlags.Above) != 0 && velocity.y > 0f) velocity.y = 0f;
            if ((flags & CollisionFlags.Sides) != 0)
            {
                velocity.x = 0f;
                dashTimer = 0f; // il dash si interrompe contro un muro
            }

            LockDepth();
        }

        void ReadInput(float dt)
        {
            moveInput = moveAction.ReadValue<float>();
            jumpHeld = jumpAction.IsPressed();
            if (jumpAction.WasPressedThisFrame()) bufferTimer = jumpBuffer;
            else bufferTimer -= dt;
        }

        void UpdateGroundState(float dt)
        {
            bool wasGrounded = IsGrounded;
            IsGrounded = cc.isGrounded;

            if (IsGrounded)
            {
                if (!wasGrounded) Landed?.Invoke(Mathf.Max(0f, -velocity.y));
                coyoteTimer = coyoteTime;
                airJumpsLeft = airJumps;
                dashAvailable = true;
            }
            else
            {
                coyoteTimer -= dt;
            }
        }

        void ApplyHorizontal(float dt)
        {
            if (controlLockTimer > 0f) return;
            float target = moveInput * maxSpeed;
            float accel = IsGrounded
                ? (Mathf.Abs(target) > 0.01f ? groundAccel : groundDecel)
                : airAccel;
            velocity.x = Mathf.MoveTowards(velocity.x, target, accel * dt);
        }

        void ApplyGravity(float dt)
        {
            float g = gravity;
            if (velocity.y < 0f) g *= fallGravityMultiplier;
            else if (velocity.y > 0f && !jumpHeld) g *= jumpCutMultiplier;

            velocity.y -= g * dt;
            float maxFall = IsWallSliding ? wallSlideMaxSpeed : maxFallSpeed;
            velocity.y = Mathf.Max(velocity.y, -maxFall);
        }

        void TryJump()
        {
            if (coyoteTimer > 0f)
            {
                coyoteTimer = 0f;
                DoJump();
            }
            else if (wallDir != 0)
            {
                velocity = new Vector2(-wallDir * wallJumpVelocity.x, wallJumpVelocity.y);
                Facing = -wallDir;
                controlLockTimer = wallJumpControlLock;
                bufferTimer = 0f;
                airJumpsLeft = airJumps; // il muro ricarica il doppio salto
                WallJumped?.Invoke();
            }
            else if (airJumpsLeft > 0)
            {
                airJumpsLeft--;
                DoJump();
            }
        }

        void DoJump()
        {
            velocity.y = jumpVelocity;
            bufferTimer = 0f;
            Jumped?.Invoke();
        }

        void StartDash()
        {
            dashDir = Mathf.Abs(moveInput) > 0.1f ? (moveInput > 0f ? 1 : -1) : Facing;
            Facing = dashDir;
            dashTimer = dashDuration;
            dashCooldownTimer = dashCooldown + dashDuration;
            if (!IsGrounded) dashAvailable = false; // un solo dash in aria
            Dashed?.Invoke();
        }

        /// <summary>Restituisce -1 (muro a sinistra), 1 (a destra) o 0.</summary>
        int CheckWall()
        {
            if (IsGrounded) return 0;
            Vector3 center = transform.position + cc.center;
            float dist = cc.radius + wallCheckDistance;
            for (int dir = -1; dir <= 1; dir += 2)
            {
                if (Physics.Raycast(center, Vector3.right * dir, out RaycastHit hit, dist, wallMask, QueryTriggerInteraction.Ignore)
                    && Mathf.Abs(hit.normal.y) < 0.3f)
                    return dir;
            }
            return 0;
        }

        void LockDepth()
        {
            Vector3 p = transform.position;
            if (Mathf.Abs(p.z) < 0.0001f) return;
            cc.enabled = false;
            p.z = 0f;
            transform.position = p;
            cc.enabled = true;
        }

        /// <summary>Teletrasporto sicuro (checkpoint, respawn).</summary>
        public void Teleport(Vector3 position)
        {
            cc.enabled = false;
            position.z = 0f;
            transform.position = position;
            cc.enabled = true;
            velocity = Vector2.zero;
            dashTimer = 0f;
        }

#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            var c = GetComponent<CharacterController>();
            if (c == null) return;
            Gizmos.color = Color.cyan;
            Vector3 center = transform.position + c.center;
            float dist = c.radius + wallCheckDistance;
            Gizmos.DrawLine(center, center + Vector3.left * dist);
            Gizmos.DrawLine(center, center + Vector3.right * dist);
        }
#endif
    }
}
