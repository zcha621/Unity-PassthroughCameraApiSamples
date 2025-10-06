using UnityEngine;
using UnityEngine.UI;
using PassthroughCameraSamples;
public class OnVuforiaTargetFound : MonoBehaviour
{
    [SerializeField] private Text m_debugText;
    [SerializeField] private WebCamTextureManager m_webCamTextureManager;
    private PassthroughCameraEye CameraEye => m_webCamTextureManager.Eye;
    [SerializeField] private GameObject m_centerEyeAnchor;
    [SerializeField] private GameObject m_target;
    // Store default world pose
    private Vector3 m_defaultWorldPosition;
    private Quaternion m_defaultWorldRotation;

    private void Start()
    {
        // Store the original tracked pose as default
        m_defaultWorldPosition = m_target.transform.position;
        m_defaultWorldRotation = m_target.transform.rotation;
    }

    public void OnTargetFound()
    {
        //m_debugText.text = $"Vuforia target found: \n Pos: {m_target.transform.position:F3} \nRot: {m_target.transform.rotation.eulerAngles:F3}";
        m_debugText.text = $"Vuforia target found!";
        //ConvertTargetToCenterEye(m_target.transform);
    }

    public void OnTargetLost()
    {
        m_debugText.text = "Vuforia target lost!";
        //ResetTargetToDefault();
    }
    /// <summary>
    /// Converts a target that is currently positioned based on the passthrough camera (used by Vuforia)
    /// into the centerEyeAnchor coordinate space.
    /// </summary>
    public void ConvertTargetToCenterEye(Transform targetTransform)
    {
        Pose cameraPose = PassthroughCameraUtils.GetCameraPoseInWorld(CameraEye);
        Vector3 centerPos = m_centerEyeAnchor.transform.position;
        Quaternion centerRot = m_centerEyeAnchor.transform.rotation;

        Vector3 localPosToCamera = Quaternion.Inverse(cameraPose.rotation) * (targetTransform.position - cameraPose.position);
        Quaternion localRotToCamera = Quaternion.Inverse(cameraPose.rotation) * targetTransform.rotation;

        Vector3 newWorldPos = centerRot * localPosToCamera + centerPos;
        Quaternion newWorldRot = centerRot * localRotToCamera;

        targetTransform.SetPositionAndRotation(newWorldPos, newWorldRot);

        m_debugText.text += $"\nAdjusted to CenterEye Pos: {newWorldPos:F3} \nRot: {newWorldRot.eulerAngles:F3}";
    }

    private void ResetTargetToDefault()
    {
        m_target.transform.SetPositionAndRotation(m_defaultWorldPosition, m_defaultWorldRotation);
        m_debugText.text += "\nReset ImageTarget to default pose";
    }
}
