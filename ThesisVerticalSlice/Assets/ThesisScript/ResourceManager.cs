using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceManager : MonoBehaviour
{
    // signleton pattern
    public static ResourceManager Instance { get; private set; }

    [Header("Resource Settings")]
    public float currentResource = 100f;  // current resource value
    public float maxResource = 100f;      // max resource value
    public float depletionRate = 5f;      // resource depletion rate
    public float depletionMultiplier = 1f; // resource depletion multiplier

    [Header("UI Settings")]
    public Canvas resourceCanvas;         // reference to the resource UI canvas
    public Image resourceImage;           

    private bool isDepleting = false;     // flag to check if the resource is depleting
     private bool isInitialized = false;   // flag to check if the resource is initialized


    private void Awake()
    {
        
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // keep the resource manager alive between scenes
        }
        else
        {
            Destroy(gameObject);  // destroy the duplicate resource manager, if any
        }

        if (resourceCanvas != null) // disable the resource UI on start
        {
            resourceCanvas.enabled = false; // disable the resource UI
        }
    }

     private void Update()
    {
        // deplete the resource over time
    if (isDepleting && currentResource > 0)
        {
            currentResource -= depletionRate * depletionMultiplier * Time.deltaTime;
            UpdateResourceUI();

            // 检查资源值是否低于50，并开始播放循环音效
            if (currentResource < 50)
            {
                SoundManager.Instance.PlayLoopingSound();
            }
            else
            {
                // 如果资源值高于50，停止循环音效
                SoundManager.Instance.StopLoopingSound();
            }

            // Stop depleting the resource and sound if it reaches zero
            if (currentResource <= 0)
            {
                SoundManager.Instance.StopLoopingSound(); // 停止循环音效
                SoundManager.Instance.PlayGlassBrokenSound(); // 播放玻璃破碎音效
                currentResource = 0;
                isDepleting = false;
            }
        }
        else if (!isDepleting)
        {
            // 当不再耗减资源时，停止循环音效
            SoundManager.Instance.StopLoopingSound();
        }
        
    }


public void StartResourceDepletion(float initialMaxResource)
    {
        if (!isInitialized)
        {
            maxResource = initialMaxResource;  // initialize the max resource value
            currentResource = maxResource;     // set the current resource value to max
            isInitialized = true;
        }

        resourceCanvas.enabled = true;         // enable the resource UI
        isDepleting = true;                    // start depleting the resource
    }

    public void StopResourceDepletion()
    {
        isDepleting = false;            // stop depleting the resource
        //resourceCanvas.enabled = false; // disable the resource UI
    }

    private void UpdateResourceUI()
    {
        if (resourceImage != null)
        {
            resourceImage.fillAmount = currentResource / maxResource;
            // change the color of the resource image when get detected
        }
    }

    public void ChangeUIColor(Color targetColor)
    {
        resourceImage.color = targetColor; // 设置为目标颜色
    }

    public void SetDepletionMultiplier(float multiplier)
    {
        depletionMultiplier = multiplier;
    }
}
