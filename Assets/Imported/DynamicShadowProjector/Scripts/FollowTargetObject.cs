using UnityEngine;
using System.Collections;

namespace DynamicShadowProjector {
	public class FollowTargetObject : MonoBehaviour {
		public enum TextureAlignment {
			None = 0,
			TargetAxisX,
			TargetAxisY,
			TargetAxisZ,
		}
		public enum UpdateFunction {
			OnPreCull = 0,
			LateUpdate,
			UpdateTransform,
		}
		// Serialize Fields
		[SerializeField]
		private Transform m_target;
		[SerializeField]
		private Transform m_targetDirection = null;
		[SerializeField]
		private TextureAlignment m_textureAlignment = TextureAlignment.None;
		[SerializeField]
		private UpdateFunction m_updateFunction = UpdateFunction.LateUpdate;

		// public properties
		public Transform target
		{
			get { return m_target; }
			set { m_target = value; }
		}
		public Transform targetDirection
		{
			get { return m_targetDirection; }
			set { m_targetDirection = value; }
		}
		public TextureAlignment textureAlignment
		{
			get { return m_textureAlignment; }
			set { m_textureAlignment = value; }
		}
		public UpdateFunction updateFunction
		{
			get { return m_updateFunction; }
			set { m_updateFunction = value; }
		}

		public void UpdateTransform()
		{
			if (m_textureAlignment != TextureAlignment.None || m_targetDirection != null) {
				// rotate projector to align texture parallel to target object.
				Vector3 up;
				switch (m_textureAlignment) {
				case TextureAlignment.TargetAxisX:
					up = m_target.right;
					break;
				case TextureAlignment.TargetAxisZ:
					up = m_target.forward;
					break;
				default:
					up = m_target.up;
					break;
				}
				Vector3 z = m_targetDirection != null ? m_targetDirection.forward : transform.forward;
				transform.LookAt(transform.position + z, up);
			}
			Vector3 targetPos = transform.TransformPoint(m_localTargetPosition);
			transform.position += m_target.position - targetPos;
		}

		private Vector3 m_localTargetPosition;

		// message handlers
		void Awake()
		{
			if (m_target != null) {
				m_localTargetPosition = transform.InverseTransformPoint(m_target.position);
			}
		}

		void LateUpdate()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying) {
				return;
			}
#endif
			if (m_updateFunction == UpdateFunction.LateUpdate) {
				UpdateTransform();
			}
		}

		void OnPreCull()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying) {
				return;
			}
#endif
			if (m_updateFunction == UpdateFunction.OnPreCull) {
				UpdateTransform();
			}
		}
	}
}
