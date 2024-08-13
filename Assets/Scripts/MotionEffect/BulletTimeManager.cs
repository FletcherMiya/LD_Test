using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTimeManager : MonoBehaviour
{
    public float bulletTimeScale = 0.1f; // �ӵ�ʱ������ű���
    public GameObject[] unaffectedObjects; // �����ӵ�ʱ��Ӱ��������б�

    private bool isBulletTime = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B)) // ���谴��B�������ӵ�ʱ��
        {
            ToggleBulletTime();
        }
    }

    public void ToggleBulletTime()
    {
        isBulletTime = !isBulletTime;

        if (isBulletTime)
        {
            // �����ӵ�ʱ��
            Time.timeScale = bulletTimeScale;
            Time.fixedDeltaTime = 0.02f * Time.timeScale; // ���������������

            /*
            foreach (GameObject obj in unaffectedObjects)
            {
                // Ϊ����Ӱ��������ṩһ�����⴦����
                var unaffectedScript = obj.GetComponent<UnaffectedByBulletTime>();
                if (unaffectedScript != null)
                {
                    unaffectedScript.EnableUnaffectedMode();
                }
            }
            */
        }
        else
        {
            // �˳��ӵ�ʱ��
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;

            /*
            foreach (GameObject obj in unaffectedObjects)
            {
                var unaffectedScript = obj.GetComponent<UnaffectedByBulletTime>();
                if (unaffectedScript != null)
                {
                    unaffectedScript.DisableUnaffectedMode();
                }
            }
            */
        }
    }

    public bool checkBulletTimeState()
    {
        return isBulletTime;
    }
}