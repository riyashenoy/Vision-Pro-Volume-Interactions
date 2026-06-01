// force Unity to always use its Vector3 type
using Vector3 = UnityEngine.Vector3;

using System;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

namespace PolySpatial.Template
{
    public class BoundedObjectBehavior : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] float k_ToVel = 10f;
        [SerializeField] float k_MaxVel = 20f;
        [SerializeField] float k_MaxForce = 40.0f;
        [SerializeField] float k_Gain = 5f;
        [SerializeField] float k_StartingTorque = 0.15f;

        [Header("Return Behavior")]
        [SerializeField] float m_LerpReturnTime = 1.0f;
        [SerializeField] float m_Delay = 3.0f;

        [Tooltip("If true, object will not return to its starting position after release")]
        public bool disableReturn = false;

        [Header("Materials")]
        [SerializeField] Material m_DefaultMaterial;
        [SerializeField] Material m_SelectedMaterial;

        Vector3 m_StartingPosition;
        Vector3 m_StartingLerpPosition;
        Transform m_Transform;
        Rigidbody m_Rigidbody;
        MeshRenderer m_MeshRenderer;
        bool m_Selected;
        bool m_Return;
        float m_CurrentTime;
        float m_DelayTime;

        const float k_LerpBaseValue = 2.0f;
        const float k_LerpExponentValue = -10.0f;

        void Start()
        {
            m_Transform = transform;
            m_StartingPosition = m_Transform.position;
            m_MeshRenderer = GetComponent<MeshRenderer>();
            m_Rigidbody = GetComponent<Rigidbody>();

            if (m_Rigidbody != null)
                m_Rigidbody.AddRelativeTorque(k_StartingTorque, k_StartingTorque, k_StartingTorque);
        }

        void Update()
        {
            // always tick delay time so physics / logic continue working
            m_DelayTime -= Time.deltaTime;

            // only run the snap-back code if disableReturn is false
            if (!disableReturn)
            {
                // after a delay, start lerping back to original position
                if (m_DelayTime <= 0.0f && !m_Return)
                {
                    m_StartingLerpPosition = m_Transform.position;
                    m_CurrentTime = 0.0f;
                    m_Return = true;
                }

                // exponential ease-out return motion
                if (!m_Selected && m_Return)
                {
                    m_CurrentTime += Time.deltaTime;
                    if (m_CurrentTime > m_LerpReturnTime)
                        m_CurrentTime = m_LerpReturnTime;

                    var time = m_CurrentTime / m_LerpReturnTime;
                    time = 1.0f - Mathf.Pow(k_LerpBaseValue, k_LerpExponentValue * time);
                    m_Transform.position = Vector3.Lerp(m_StartingLerpPosition, m_StartingPosition, time);
                }
            }
        }

        public void Select(bool selected)
        {
            m_Return = false;
            m_Selected = selected;

            if (m_Rigidbody != null)
                m_Rigidbody.linearVelocity = Vector3.zero;

            if (m_MeshRenderer != null)
                m_MeshRenderer.material = selected ? m_SelectedMaterial : m_DefaultMaterial;

            if (!selected)
                m_DelayTime = m_Delay;
        }

        public void MoveWithPhysics(Vector3 worldPosition)
        {
            if (m_Rigidbody == null) return;

            var distance = worldPosition - m_Transform.position;
            var targetVelocity = Vector3.ClampMagnitude(k_ToVel * distance, k_MaxVel);
            var error = targetVelocity - m_Rigidbody.linearVelocity;
            var force = Vector3.ClampMagnitude(k_Gain * error, k_MaxForce);
            m_Rigidbody.AddForce(force);
        }

        public void MoveDirectly(SpatialPointerState worldTouch)
        {
            m_Transform.SetPositionAndRotation(worldTouch.interactionPosition, worldTouch.inputDeviceRotation);
        }
    }
}