using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TriggerWithCamera : MonoBehaviour
{
    public TextMeshProUGUI gameText;
    public TextMeshProUGUI gameTextShading;
    public List<string> messages = new List<string>();
    private int currentMessageIndex = 0;
    private bool playerInRange = false;

    public Camera secondaryCamera;

    private CanvasGroup canvasGroup; // 用于控制渐入渐出效果

    [SerializeField] // 允许在 Inspector 中调整时间
    private float fadeDuration = 0.5f; // 渐入渐出的时间

    private bool isFading = false; // 是否正在进行渐入渐出

    private void Start()
    {
        gameText.text = "";
        gameTextShading.text = "";
        // 初始化 CanvasGroup 以控制透明度
        canvasGroup = gameText.gameObject.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f; // 初始时文字透明

        if (secondaryCamera != null)
        {
            secondaryCamera.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        gameTextShading.text = gameText.text;
        
        // 当不在渐入渐出状态且玩家在范围内时，允许按下 X 键
        if (playerInRange && Input.GetKeyDown(KeyCode.X) && !isFading)
        {
            // 如果是第一条消息，切换到第二相机
            if (currentMessageIndex == 0)
            {
                if (secondaryCamera != null)
                {
                    secondaryCamera.gameObject.SetActive(true);
                }
            }

            // 切换到下一条消息，并确保消息渐入渐出显示
            currentMessageIndex = (currentMessageIndex + 1) % messages.Count;
            StartCoroutine(FadeText(messages[currentMessageIndex])); // 开始渐入渐出效果

            // 如果切换回第一条消息，关闭第二相机
            if (currentMessageIndex == 0)
            {
                if (secondaryCamera != null)
                {
                    secondaryCamera.gameObject.SetActive(false);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Animal"))
        {
            playerInRange = true;
            Debug.Log("进入范围了");

            currentMessageIndex = 0;
            StartCoroutine(FadeText(messages[currentMessageIndex])); // 初始显示时渐入效果
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Animal"))
        {
            playerInRange = false;
            Debug.Log("离开范围了");

            StartCoroutine(FadeText("")); // 离开时文字淡出
            
            if (secondaryCamera != null)
            {
                secondaryCamera.gameObject.SetActive(false);
            }
        }
    }

    // 实现文字渐入渐出的协程
    private IEnumerator FadeText(string newText)
    {
        isFading = true; // 标记正在进行渐入渐出

        // 渐出效果
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 0f;
        gameText.text = newText;

        // 渐入效果
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;

        isFading = false; // 渐入渐出完成
    }
}
