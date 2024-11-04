using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // 在Inspector中可以添加的物体列表
    public List<GameObject> CCTVs;
    
    // Start is called before the first frame update
    void Start()
    {
        // 隐藏鼠标光标
        Cursor.visible = false;
        // 将光标锁定在屏幕中心
        Cursor.lockState = CursorLockMode.Locked;
        
        // 在游戏开始时，将列表中的所有物体设为不激活
        foreach (GameObject obj in CCTVs)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnTriggerEnter(Collider other)
    {
        // 检查进入的物体是否带有"Animal"标签
        if (other.CompareTag("Animal"))
        {
            // 将列表中的所有物体设为激活
            foreach (GameObject obj in CCTVs)
            {
                if (obj != null)
                {
                    obj.SetActive(true);
                }
            }
        }
    }
}
