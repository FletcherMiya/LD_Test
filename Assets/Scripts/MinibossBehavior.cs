using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Invector
{
    public class MinibossBehavior : MonoBehaviour
    {
        public GameObject key;
        public GameObject shields;
        public GameObject target;
        public GameObject spawn;

        public void minibossDeath()
        {
            disableShields();
            dropKey();
        }

        private void disableShields()
        {
            shields.SetActive(false);
        }

        private void dropKey()
        {
            GameObject droppedKey = Instantiate(key, target.transform.position, target.transform.rotation);
            droppedKey.GetComponent<Rigidbody>().isKinematic = false;
            if (spawn != null)
            {
                spawn.GetComponent<SpawnPointManager>().stufftoDeactivate = AddGameObjectToArray(spawn.GetComponent<SpawnPointManager>().stufftoDeactivate, droppedKey);
            }
        }

        GameObject[] AddGameObjectToArray(GameObject[] originalArray, GameObject newGameObject)
        {
            if (originalArray == null)
            {
                // 如果原数组为空，直接创建包含一个元素的数组
                return new GameObject[] { newGameObject };
            }
            else
            {
                // 创建新的数组，长度比原数组多1
                GameObject[] newArray = new GameObject[originalArray.Length + 1];

                // 复制原数组到新数组
                for (int i = 0; i < originalArray.Length; i++)
                {
                    newArray[i] = originalArray[i];
                }

                // 在新数组的最后位置添加新 GameObject
                newArray[originalArray.Length] = newGameObject;

                // 返回新数组
                return newArray;
            }
        }
    }

}