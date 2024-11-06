using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ResourceManager : MonoBehaviour
{
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
        if (resourceCanvas != null)
        {
            resourceCanvas.enabled = false;
        }
    }

    private void Update()
    {
        if (isDepleting && currentResource > 0)
        {
            currentResource -= depletionRate * depletionMultiplier * Time.deltaTime;
            UpdateResourceUI();

            // 检查资源值是否低于 50，并开始播放循环音效
            if (currentResource < 50)
            {
                SoundManager.Instance.PlayLoopingSound();
            }
            else
            {
                SoundManager.Instance.StopLoopingSound();
            }

            // 当资源耗尽时停止
            if (currentResource <= 0)
            {
                SoundManager.Instance.StopLoopingSound();
                SoundManager.Instance.PlayGlassBrokenSound();
                currentResource = 0;
                isDepleting = false;
            }
        }
        else if (!isDepleting)
        {
            SoundManager.Instance.StopLoopingSound();
        }
    }

    public void StartResourceDepletion(float initialMaxResource)
    {
        if (!isInitialized)
        {
            maxResource = initialMaxResource;
            currentResource = maxResource;
            isInitialized = true;
        }

        resourceCanvas.enabled = true;
        isDepleting = true;
    }

    public void StopResourceDepletion()
    {
        isDepleting = false;
    }

    private void UpdateResourceUI()
    {
        if (resourceImage != null)
        {
            resourceImage.fillAmount = currentResource / maxResource;
        }
    }

    public void ChangeUIColor(Color targetColor)
    {
        resourceImage.color = targetColor;
    }

    public void SetDepletionMultiplier(float multiplier)
    {
        depletionMultiplier = multiplier;
    }

}
