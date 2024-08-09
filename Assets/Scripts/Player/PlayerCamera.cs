using System;
using Mirror;
using UnityEngine;

namespace Steamworks
{
    public class PlayerCamera : NetworkBehaviour
    {
        private const float YMin = -50f;
        private const float YMax = 50f;

        public Transform lookAt;
        public float distance;
        public float sensitivity;

        private float currentX = 0;
        private float currentY = 25;

        private bool _clickEscape = false;

        [ClientCallback]
        void Update()
        {
            currentX += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
            currentY -= Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
            
            currentY = Mathf.Clamp(currentY, YMin, YMax);
            
            Cursor.lockState = _clickEscape ? CursorLockMode.None : CursorLockMode.Locked;

            if (Input.GetKeyDown(KeyCode.Escape))
                _clickEscape = !_clickEscape;
        }

        [ClientCallback]
        private void LateUpdate()
        {
            if(!isLocalPlayer) return;
            
            Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
            Vector3 direction = new Vector3(0, 0, -distance);
            Vector3 position = lookAt.position + rotation * direction;

            transform.position = position;
            transform.LookAt(lookAt.position);
        }
    }
}
