using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


[RequireComponent(typeof(LineRenderer))]
public class EnemyStateManager : MonoBehaviour
{
    EnemyBaseState currentState;
    public EnemyPatrolState PatrolState = new EnemyPatrolState();
    public EnemySusState SusState = new EnemySusState();
    public EnemyAlertState AlertState = new EnemyAlertState();
    public EnemyCombatState CombatState = new EnemyCombatState();
    public EnemySearchState SearchState = new EnemySearchState();


    // Detection parameters
    public Transform item;  // 改为检测物品
    public float viewRadius = 10f;  // 视野范围
    [Range(0, 360)] public float horizontalViewAngle = 90f;  // 水平视野角度
    [Range(0, 180)] public float verticalViewAngle = 60f;  // 垂直视野角度    
    private LineRenderer lineRenderer;  // 显示射线
    private bool canSeeItem = false;  // 现在检测物品
    public Color defaultRayColor = Color.blue;  // 默认颜色
    public Color detectedRayColor = Color.red;  // 检测到物品的颜色

    // Waypoints for patrolling
    public Transform[] waypoints;  // 巡逻点数组
    public float waypointReachThreshold = 0.5f;  // 到达巡逻点的距离阈值

    // Speed
    public float patrolSpeed = 2f;  // 巡逻速度
    private NavMeshAgent navAgent;
    
    //alert system
    // 警戒参数
    
    public float alertMeter = 0;  // 当前警戒值
    
    public float alertMeterMax = 100;  // 警戒条最大值
    public float alertMeterIncreaseRate = 5f;  // 检测到玩家时警戒条增加速度
    public float alertMeterDecreaseRate = 3f;  // 未检测到玩家时警戒条减少速度

    // 警戒条 UI
    public Image alertBarUI;  // 警戒条 Image
    public Transform alertBarCanvas;  // 警戒条的 Canvas

    // 定义多个警戒值阈值
    public float[] alertThresholds = { 25f, 50f, 100f };

    private bool canDecreaseAlertMeter = true;

    public bool shouldRotate = false; 

    //CCTV Enemy
    public Vector3[] rotationPoints;  // 定义旋转目标角度（欧拉角）
    public float rotationSpeed = 30f;  // 旋转速度
    public float waitTimeAtPoint = 2f;  // 每个角度停留时间
    private int currentRotationIndex = 0;  // 当前旋转角度的索引
    private Quaternion targetRotation;  // 当前目标旋转
    private float waitTimer = 0f;  // 停留计时器
    private bool previousCanSeeItem = false; // 用于跟踪上一次的 canSeeItem 状态

    private Light alertSpotLight;  // 用于引用 SpotLight

    [Header("Light Colors")] // 在 Inspector 窗口中自定义颜色
    public Color lowAlertColor = Color.white;
    public Color mediumAlertColor = new Color(1f, 0.65f, 0f);   // 默认橙色
    public Color highAlertColor = new Color(1f, 0.4f, 0f);       // 默认深橙色
    public Color maxAlertColor =  new Color(1f, 0f, 0f);         // 默认红色

    private Color targetColor;     // 目标颜色
    public float colorLerpSpeed = 2f;  // 控制 Lerp 速度


    void Start()
    {
        // 初始化 NavMeshAgent
        navAgent = GetComponent<NavMeshAgent>();
        
        // 禁用 NavMeshAgent 的自动旋转，我们手动管理
        navAgent.updateRotation = false;

        currentState = PatrolState;
        currentState.EnterState(this);

        // 初始化 LineRenderer
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));  // 确保 LineRenderer 可以改变颜色

        // 初始化警戒条
        if (alertMeter >= alertMeterMax)
        {
            alertMeter = alertMeterMax;
            canDecreaseAlertMeter = false;
        }

        if (rotationPoints.Length > 0)
        {
            targetRotation = Quaternion.Euler(rotationPoints[currentRotationIndex]);
        }

        alertSpotLight = transform.Find("Spot Light").GetComponent<Light>();

        // 确保找到 SpotLight
        if (alertSpotLight == null)
        {
            Debug.LogError("SpotLight not found! Please ensure it is a child of this GameObject.");
        }
    }

     void Update()
    {
        UpdateAlertBarPosition();
        currentState.UpdateState(this);

        if (shouldRotate)
        {
            RotateBetweenPoints();
        }

        // 手动更新物体朝向
        UpdateRotation();

        // 更新 Raycast 方向和距离，改为检测物品
        Vector3 rayDirection = (item.position - transform.position).normalized;
        float distanceToItem = Vector3.Distance(transform.position, item.position);

        // 调用视野检测
        canSeeItem = DetectItem();

        lineRenderer.enabled = canSeeItem;

        // 仅在检测状态发生变化时调用更改方法
        if (canSeeItem != previousCanSeeItem)
        {

            if (canSeeItem) // 检测到物品
            {
                ResourceManager.Instance.ChangeUIColor(Color.yellow);
                ResourceManager.Instance.SetDepletionMultiplier(3f);
            }
            else
            {
                ResourceManager.Instance.ChangeUIColor(Color.white);
                ResourceManager.Instance.SetDepletionMultiplier(1f);
            }
            // 更新 previousCanSeeItem 状态
            previousCanSeeItem = canSeeItem;
        }

        UpdateAlertMeter();
        UpdateAlertLight();

        // 更新 LineRenderer 的位置和颜色
        // 先关掉射线
        // Vector3 endPoint = transform.position + rayDirection * viewRadius;
        // UpdateLineRenderer(transform.position, endPoint, canSeeItem ? detectedRayColor : defaultRayColor);
        
    }

     // CCTV旋转到多个目标角度
void RotateBetweenPoints()
{
    // 平滑旋转，确保考虑X、Y、Z轴的旋转
    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

    if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
    {
        waitTimer += Time.deltaTime;
        if (waitTimer >= waitTimeAtPoint)
        {
            waitTimer = 0f;
            currentRotationIndex = (currentRotationIndex + 1) % rotationPoints.Length;

            // 更新目标旋转角度，使用X、Y、Z的欧拉角
            targetRotation = Quaternion.Euler(rotationPoints[currentRotationIndex]);
        }
    }
}

    // 手动旋转物体朝向 NavMeshAgent 的前进方向
    void UpdateRotation()
    {
        if (navAgent.velocity.sqrMagnitude > 0.1f)  // 当有移动时
        {
            // 计算旋转方向
            Quaternion targetRotation = Quaternion.LookRotation(navAgent.velocity.normalized);

            // 使用 RotateTowards 使物体平滑地旋转到前进方向
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, navAgent.angularSpeed * Time.deltaTime);
        }
    }

    public void Rotate()
    {
        // 在这里实现旋转逻辑
        transform.Rotate(Vector3.up * 30f * Time.deltaTime);  // Y 轴旋转
    }


        // 基于物体当前朝向检测玩家
public bool DetectItem()
{
    Vector3 dirToItem = item.position - transform.position;
    float distanceToItem = dirToItem.magnitude;
    dirToItem.Normalize();

    float edgeBuffer = 2.5f;  // 视野边缘缓冲区

    if (distanceToItem <= viewRadius)
    {
        Vector3 horizontalDir = Vector3.ProjectOnPlane(dirToItem, transform.up).normalized;
        float dotHorizontal = Vector3.Dot(transform.forward, horizontalDir);
        float horizontalAngle = Mathf.Acos(dotHorizontal) * Mathf.Rad2Deg;

        float dotVertical = Vector3.Dot(transform.forward, dirToItem);
        float totalAngle = Mathf.Acos(dotVertical) * Mathf.Rad2Deg;
        float verticalAngle = totalAngle - horizontalAngle;

        if (Vector3.Dot(transform.up, dirToItem) < 0)
        {
            verticalAngle = -verticalAngle;
        }

        if (horizontalAngle <= (horizontalViewAngle / 2) - edgeBuffer && 
            Mathf.Abs(verticalAngle) <= (verticalViewAngle / 2) - edgeBuffer)
        {
            RaycastHit[] hits = Physics.RaycastAll(transform.position, dirToItem, distanceToItem);
            foreach (RaycastHit hit in hits)
            {
                // 检查碰撞物体是否具有 "Cover" 标签
                if (hit.collider.CompareTag("Cover"))
                {
                    return false;  // 被标记为“Cover”的物体阻挡了视线
                }
            }
            return true; // 未被阻挡，检测到物品
        }
    }
    return false; // 未检测到物品
}
    void OnDrawGizmos()
    {

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        // 绘制水平视野
        Vector3 leftDir = Quaternion.AngleAxis(-horizontalViewAngle / 2, transform.up) * transform.forward;
        Vector3 rightDir = Quaternion.AngleAxis(horizontalViewAngle / 2, transform.up) * transform.forward;
        
        // 绘制垂直视野
        Vector3 upDir = Quaternion.AngleAxis(verticalViewAngle / 2, transform.right) * transform.forward;
        Vector3 downDir = Quaternion.AngleAxis(-verticalViewAngle / 2, transform.right) * transform.forward;

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, leftDir * viewRadius);
        Gizmos.DrawRay(transform.position, rightDir * viewRadius);
        Gizmos.DrawRay(transform.position, upDir * viewRadius);
        Gizmos.DrawRay(transform.position, downDir * viewRadius);

        // draw view cone
        Gizmos.color = new Color(0, 1, 1, 0.2f); 
        DrawViewCone(leftDir, rightDir, upDir, downDir);

        if (DetectItem())
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, item.position);
        }
    }

    void DrawViewCone(Vector3 leftDir, Vector3 rightDir, Vector3 upDir, Vector3 downDir)
    {
        Vector3[] vertices = new Vector3[4];
        vertices[0] = transform.position + upDir * viewRadius;
        vertices[1] = transform.position + downDir * viewRadius;
        vertices[2] = transform.position + leftDir * viewRadius;
        vertices[3] = transform.position + rightDir * viewRadius;

        Gizmos.DrawLine(transform.position, vertices[0]);
        Gizmos.DrawLine(transform.position, vertices[1]);
        Gizmos.DrawLine(transform.position, vertices[2]);
        Gizmos.DrawLine(transform.position, vertices[3]);

        Gizmos.DrawLine(vertices[0], vertices[2]);
        Gizmos.DrawLine(vertices[0], vertices[3]);
        Gizmos.DrawLine(vertices[1], vertices[2]);
        Gizmos.DrawLine(vertices[1], vertices[3]);
    }

    // use this method to get the direction vector from an angle in degrees
    public Vector3 DirFromAngle(float horizontalAngleInDegrees, float verticalAngleInDegrees)
    {
        // transform the angle from degrees to radians
        float horizontalRadians = horizontalAngleInDegrees * Mathf.Deg2Rad;
        float verticalRadians = verticalAngleInDegrees * Mathf.Deg2Rad;

        // calculate the horizontal direction
        Vector3 direction = transform.forward * Mathf.Cos(horizontalRadians) + transform.right * Mathf.Sin(horizontalRadians);
        direction = direction * Mathf.Cos(verticalRadians) + transform.up * Mathf.Sin(verticalRadians);

        return direction;
    }

    void UpdateAlertBarPosition()
    {
        if (alertBarCanvas != null)
        {
            // 设置警戒条 Canvas 在敌人头顶上方
            alertBarCanvas.position = transform.position + Vector3.up * 3f;  // 2f 是头部高度，可根据需要调整
            alertBarCanvas.rotation = Camera.main.transform.rotation;  // 使警戒条始终面向相机
        }

        // 更新警戒条的填充
        if (alertBarUI != null)
        {
            alertBarUI.fillAmount = alertMeter / alertMeterMax;
        }
    }

void UpdateAlertMeter()
{
    if (canSeeItem)
    {
        // 检测到玩家时，警戒值逐渐增加
        alertMeter += Time.deltaTime * alertMeterIncreaseRate;
    }
    else if (canDecreaseAlertMeter)
    {
        // 如果允许，未检测到玩家时警戒值逐渐减少
        alertMeter -= Time.deltaTime * alertMeterDecreaseRate;
    }

    // 限制警戒值在 0 到最大值之间
    alertMeter = Mathf.Clamp(alertMeter, 0, alertMeterMax);

    // 检查警戒值是否达到最大值
    if (alertMeter >= alertMeterMax)
    {
        // 达到最大值后停止降低
        alertMeter = alertMeterMax;
        canDecreaseAlertMeter = false;
    }

    // 根据警戒值的不同阈值进行状态切换
    if (alertMeter >= alertThresholds[0] && alertMeter < alertThresholds[1])
    {
        // 切换到警戒状态 SusState
        Debug.Log("Switch to SusState");
    }
    else if (alertMeter >= alertThresholds[1] && alertMeter < alertThresholds[2])
    {
        // 切换到警戒状态 AlertState
        Debug.Log("Switch to AlertState");
    }
    else if (alertMeter >= alertThresholds[2])
    {
        // 切换到战斗状态 CombatState
        Debug.Log("Switch to CombatState");
        canDecreaseAlertMeter = false;  // 达到最大值后停止降低
    }
}

    void UpdateAlertLight()
    {
        if (alertSpotLight != null)  // 确保 SpotLight 存在
        {
            // 根据警戒值确定目标颜色
            if (alertMeter <= 25)
            {
                targetColor = lowAlertColor;
            }
            else if (alertMeter > 25 && alertMeter <= 50)
            {
                targetColor = mediumAlertColor;
            }
            else if (alertMeter > 50 && alertMeter < 100)
            {
                targetColor = highAlertColor;
            }
            else if (alertMeter >= 100)
            {
                targetColor = maxAlertColor;
                canDecreaseAlertMeter = false;  // 达到最大值后停止降低
            }

            // 使用 Lerp 平滑过渡到目标颜色
            alertSpotLight.color = Color.Lerp(alertSpotLight.color, targetColor, Time.deltaTime * colorLerpSpeed);
        }
    }

    // 更新 LineRenderer 的位置和颜色
    private void UpdateLineRenderer(Vector3 startPosition, Vector3 endPosition, Color color)
    {
        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, endPosition);
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
        lineRenderer.startWidth = 0.2f;
        lineRenderer.endWidth = 0.2f;
    }

    void SwitchState(EnemyBaseState state)
    {
        currentState = state;
        state.EnterState(this);
    }

}