using UnityEngine;

public class BulletTimeManager : MonoBehaviour
{
    public float bulletTimeScale = 0.1f;
    public GameObject[] unaffectedObjects;
    public bool isBulletTime = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            StartBulletTime();
        }

        if (Input.GetKeyUp(KeyCode.F))
        {
            EndBulletTime();
        }
    }

    public void StartBulletTime()
    {
        if (!isBulletTime)
        {
            isBulletTime = true;
            Time.timeScale = bulletTimeScale;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;

            /*
            foreach (GameObject obj in unaffectedObjects)
            {
                var unaffectedScript = obj.GetComponent<UnaffectedByBulletTime>();
                if (unaffectedScript != null)
                {
                    unaffectedScript.EnableUnaffectedMode();
                }
            }
            */
        }
    }

    public void EndBulletTime()
    {
        if (isBulletTime)
        {
            isBulletTime = false;
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