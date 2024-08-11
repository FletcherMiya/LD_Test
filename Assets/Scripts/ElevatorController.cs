using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Invector.vCharacterController
{
    public class ElevatorController : MonoBehaviour
    {
        public Transform leftDoor;
        public Transform rightDoor;
        public float openDistance = 2.0f;
        public float openSpeed = 1.0f;
        private bool isOpening = false;
        private Vector3 leftDoorStartPosition;
        private Vector3 rightDoorStartPosition;
        private Vector3 leftDoorOpenPosition;
        private Vector3 rightDoorOpenPosition;
        private vThirdPersonInput input;


        void Start()
        {
            if (leftDoor != null)
                leftDoorStartPosition = leftDoor.position;
            if (rightDoor != null)
                rightDoorStartPosition = rightDoor.position;

            leftDoorOpenPosition = leftDoorStartPosition - new Vector3(openDistance, 0, 0);
            rightDoorOpenPosition = rightDoorStartPosition + new Vector3(openDistance, 0, 0);
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && !isOpening)
            {
                input = other.GetComponent<vThirdPersonInput>();
                isOpening = true;
                StartCoroutine(OpenDoors());
            }
        }

        IEnumerator OpenDoors()
        {
            input.SetLockBasicInput(true);
            input.SetLockCameraInput(true);
            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime * openSpeed;
                if (leftDoor != null)
                    leftDoor.position = Vector3.Lerp(leftDoorStartPosition, leftDoorOpenPosition, t);
                if (rightDoor != null)
                    rightDoor.position = Vector3.Lerp(rightDoorStartPosition, rightDoorOpenPosition, t);
                yield return null;
            }
            input.SetLockBasicInput(false);
            input.SetLockCameraInput(false);
        }
    }
}