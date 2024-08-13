using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTimeManager : MonoBehaviour
{
    public float bulletTimeScale = 0.1f; // 子弹时间的缩放比例
    public GameObject[] unaffectedObjects; // 不受子弹时间影响的物体列表

    private bool isBulletTime = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B)) // 假设按下B键启动子弹时间
        {
            ToggleBulletTime();
        }
    }

    public void ToggleBulletTime()
    {
        isBulletTime = !isBulletTime;

        if (isBulletTime)
        {
            // 启动子弹时间
            Time.timeScale = bulletTimeScale;
            Time.fixedDeltaTime = 0.02f * Time.timeScale; // 调整物理更新速率

            /*
            foreach (GameObject obj in unaffectedObjects)
            {
                // 为不受影响的物体提供一种特殊处理方法
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
            // 退出子弹时间
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