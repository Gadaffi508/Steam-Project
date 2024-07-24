using Mirror;
using UnityEngine;

namespace Steamworks
{
    public class PlayerMove : NetworkBehaviour
    {
        public float speed;
        public float rotationSpeed;
        public float sprintMultiplier;
        
        private float _X, _Y, currentSpeed;
        private Vector3 _movement;
        private bool isSprinting;
        private int sprinting;
        
        public Rigidbody _rb;
        public Animator _animator;
        private Vector3 direction;
        
        private void Start()
        {
            _rb = GetComponent<Rigidbody>();

            RigidbodyConstraints a = RigidbodyConstraints.None;
            a = RigidbodyConstraints.FreezeRotation;

            _rb.constraints = a;

            _animator = GetComponentInChildren<Animator>();
        }
        
        private void Update()
        {
            if(!isLocalPlayer) return;

            #region Inputs

            _X = Input.GetAxis("Horizontal");
            _Y = Input.GetAxis("Vertical");
        
            isSprinting = Input.GetKey(KeyCode.LeftShift);

            #endregion
        
            currentSpeed = isSprinting ? speed * sprintMultiplier : speed;

            sprinting = isSprinting ? 1 : 0;

            Animations();
        
            CmdMove(_X,_Y,currentSpeed);
        }
        
        [Command]
        void CmdMove(float x, float y,float _currentspeed) => RpcMove(x,y,_currentspeed);
        
        [ClientRpc]
        void RpcMove(float x, float y, float _currentspeed)
        {
            direction = new Vector3(x, 0, y).normalized;
        
            Vector3 cameraForward = Camera.main.transform.forward;
        
            cameraForward.y = 0;
        
            Vector3 moveDirection = cameraForward * direction.z + Camera.main.transform.right * direction.x;
        
            Vector3 dir = new Vector3(moveDirection.x* _currentspeed * Time.deltaTime,_rb.linearVelocity.y,moveDirection.z* _currentspeed * Time.deltaTime);

            _rb.linearVelocity = dir;

            if (moveDirection != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
            }
        }
        
        void Animations()
        {
            _animator.SetFloat("velocity",direction.magnitude);
        
            _animator.SetFloat("Sprint", sprinting);
        }
    }
}
