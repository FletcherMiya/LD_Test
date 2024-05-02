using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Invector.vCharacterController
{
    public class ElevatorController : MonoBehaviour
    {
        public Transform leftDoor;
        public Transform rightDoor;
        public float openDistance = 2.0f; // 可调整的开门距离
        public float openSpeed = 1.0f; // 开门速度，单位是单位/秒
        private bool isOpening = false;
        private Vector3 leftDoorStartPosition;
        private Vector3 rightDoorStartPosition;
        private Vector3 leftDoorOpenPosition;
        private Vector3 rightDoorOpenPosition;
        private vThirdPersonInput input;


        void Start()
        {
            // 存储门的初始位置
            if (leftDoor != null)
                leftDoorStartPosition = leftDoor.position;
            if (rightDoor != null)
                rightDoorStartPosition = rightDoor.position;

            // 计算门打开时的目标位置
            leftDoorOpenPosition = leftDoorStartPosition - new Vector3(openDistance, 0, 0); // 左门向左移动
            rightDoorOpenPosition = rightDoorStartPosition + new Vector3(openDistance, 0, 0); // 右门向右移动
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && !isOpening)
            {
                input = other.GetComponent<vThirdPersonInput>();
                isOpening = true; // 确保门只打开一次
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