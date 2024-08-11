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
                return new GameObject[] { newGameObject };
            }
            else
            {
                GameObject[] newArray = new GameObject[originalArray.Length + 1];

                for (int i = 0; i < originalArray.Length; i++)
                {
                    newArray[i] = originalArray[i];
                }

                newArray[originalArray.Length] = newGameObject;

                return newArray;
            }
        }
    }

}