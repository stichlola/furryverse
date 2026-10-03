using System.Collections.Generic;
using UnityEngine;

namespace Project.Player
{
    /// <summary>
    /// Lato visivo del player: rotazione verso la direzione (con leggero 3/4 verso la camera),
    /// inclinazione in corsa, squash &amp; stretch procedurale e parametri dell'Animator.
    /// Va sul figlio "Visual" che contiene il modello.
    /// </summary>
    public class PlayerVisuals : MonoBehaviour
    {
        public PlayerMotor motor;
        public Animator animator;

        [Header("Rotazione")]
        [Tooltip("Gradi verso la camera: 0 = profilo puro, 20-30 = 3/4 alla Ori.")]
        public float cameraFacingBias = 25f;
        public float turnSpeed = 14f;

        [Header("Inclinazione")]
        public float leanAngle = 8f;
        public float leanSpeed = 10f;

        [Header("Squash & Stretch")]
        public Vector3 jumpStretch = new Vector3(0.85f, 1.2f, 0.85f);
        public Vector3 dashStretch = new Vector3(1.25f, 0.85f, 1f);
        public float landSquashPerSpeed = 0.012f;
        public float maxLandSquash = 0.3f;
        public float springStiffness = 260f;
        public float springDamping = 16f;

        static readonly int SpeedHash = Animator.StringToHash("Speed");
        static readonly int VelYHash = Animator.StringToHash("VelocityY");
        static readonly int GroundedHash = Animator.StringToHash("Grounded");
        static readonly int WallSlideHash = Animator.StringToHash("WallSlide");
        static readonly int JumpHash = Animator.StringToHash("Jump");
        static readonly int LandHash = Animator.StringToHash("Land");
        static readonly int DashHash = Animator.StringToHash("Dash");

        readonly HashSet<int> animParams = new HashSet<int>();
        Vector3 baseScale, scale, scaleVel;
        float lean;
        Quaternion yaw;

        void Awake()
        {
            if (motor == null) motor = GetComponentInParent<PlayerMotor>();
            if (animator == null) animator = GetComponentInChildren<Animator>();
            if (animator != null)
                foreach (var p in animator.parameters) animParams.Add(p.nameHash);

            baseScale = transform.localScale;
            scale = Vector3.one;
            yaw = TargetYaw();
        }

        void OnEnable()
        {
            if (motor == null) return;
            motor.Jumped += OnJump;
            motor.WallJumped += OnJump;
            motor.Dashed += OnDash;
            motor.Landed += OnLand;
        }

        void OnDisable()
        {
            if (motor == null) return;
            motor.Jumped -= OnJump;
            motor.WallJumped -= OnJump;
            motor.Dashed -= OnDash;
            motor.Landed -= OnLand;
        }

        void LateUpdate()
        {
            if (motor == null) return;
            float dt = Time.deltaTime;

            // Rotazione: Slerp tra destra e sinistra passa davanti alla camera
            yaw = Quaternion.Slerp(yaw, TargetYaw(), 1f - Mathf.Exp(-turnSpeed * dt));

            // Inclinazione proporzionale alla velocità orizzontale
            float targetLean = motor.IsGrounded ? motor.Velocity.x / Mathf.Max(0.01f, motor.maxSpeed) * leanAngle : 0f;
            lean = Mathf.Lerp(lean, targetLean, 1f - Mathf.Exp(-leanSpeed * dt));
            transform.localRotation = Quaternion.Euler(0f, 0f, -lean) * yaw;

            // Molla smorzata che riporta la scala a 1
            Vector3 force = (Vector3.one - scale) * springStiffness - scaleVel * springDamping;
            scaleVel += force * dt;
            scale += scaleVel * dt;
            transform.localScale = Vector3.Scale(baseScale, scale);

            UpdateAnimator();
        }

        Quaternion TargetYaw()
        {
            int f = motor != null ? motor.Facing : 1;
            return Quaternion.Euler(0f, f > 0 ? 90f + cameraFacingBias : -90f - cameraFacingBias, 0f);
        }

        void UpdateAnimator()
        {
            if (animator == null) return;
            SetFloat(SpeedHash, Mathf.Abs(motor.Velocity.x) / Mathf.Max(0.01f, motor.maxSpeed));
            SetFloat(VelYHash, motor.Velocity.y);
            SetBool(GroundedHash, motor.IsGrounded);
            SetBool(WallSlideHash, motor.IsWallSliding);
        }

        void OnJump() { scale = jumpStretch; scaleVel = Vector3.zero; SetTrigger(JumpHash); }
        void OnDash() { scale = dashStretch; scaleVel = Vector3.zero; SetTrigger(DashHash); }

        void OnLand(float impactSpeed)
        {
            float s = Mathf.Min(maxLandSquash, impactSpeed * landSquashPerSpeed);
            scale = new Vector3(1f + s, 1f - s, 1f + s);
            scaleVel = Vector3.zero;
            SetTrigger(LandHash);
        }

        // I parametri mancanti nell'Animator vengono ignorati (niente warning)
        void SetFloat(int h, float v) { if (animParams.Contains(h)) animator.SetFloat(h, v); }
        void SetBool(int h, bool v) { if (animParams.Contains(h)) animator.SetBool(h, v); }
        void SetTrigger(int h) { if (animParams.Contains(h)) animator.SetTrigger(h); }
    }
}
