using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform cameraTransform; // 相机的 Transform
    public float zoomSpeed;      // 缩放速度
    public float targetDistance; // 拉近距离
    public float targetHeight; // NPC 的目标高度
    public float maxZoomTime;    // 最大缩放时间（决定最大放大倍数）

    private Transform targetNPC;      // 当前聚焦的 NPC
    private Vector3 originalPosition; // 相机初始位置
    private Quaternion originalRotation; // 相机初始旋转
    private bool isZoomedIn = false;  // 是否已经拉近
    private float zoomTimer = 0f;     // 缩放计时器
    private bool isZoomFinished = false; // 是否已完成缩放

    private void Start()
    {
        originalPosition = cameraTransform.position;
        originalRotation = cameraTransform.rotation;
    }

    private void Update()
    {
        // 按下Esc恢复
        if (isZoomedIn && Input.GetKeyDown(KeyCode.Escape))
        {
            ResetCamera();
            return;
        }

        if (targetNPC != null && isZoomedIn && !isZoomFinished)
        {
            zoomTimer += Time.deltaTime * zoomSpeed;
            float easeProgress = Mathf.Clamp01(zoomTimer / maxZoomTime);

            // 计算目标位置和旋转
            Vector3 focusPosition = targetNPC.position + new Vector3(0, targetHeight, 0);
            Vector3 targetPosition = focusPosition - cameraTransform.forward * targetDistance;

            cameraTransform.position = Vector3.Lerp(originalPosition, targetPosition, easeProgress);
            cameraTransform.rotation = Quaternion.Slerp(originalRotation, Quaternion.LookRotation(focusPosition - targetPosition), easeProgress);

            if (easeProgress >= 1f)
            {
                isZoomFinished = true; // 缩放完成
            }
        }
    }

    public void FocusOnNPC(Transform npcTransform)
    {
        targetNPC = npcTransform;
        isZoomedIn = true;
        isZoomFinished = false;
        zoomTimer = 0f; // 开始缩放
    }

    public void ResetCamera()
    {
        targetNPC = null;
        isZoomedIn = false;
        isZoomFinished = false;
        zoomTimer = 0f;

        cameraTransform.position = originalPosition;
        cameraTransform.rotation = originalRotation;
    }
}