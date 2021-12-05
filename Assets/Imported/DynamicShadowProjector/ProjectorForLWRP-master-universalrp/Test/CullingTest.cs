using UnityEngine;

namespace ProjectorForLWRP.Test
{
	[ExecuteInEditMode]
	public class CullingTest : MonoBehaviour
	{
		public new Camera camera;
		public float cameraSpeed = 5.0f;
		public Transform cameraTarget;
		public bool createTestCubes = false;
		public float cubeSize = 0.5f;
		public float gridInterval = 1.0f;
		public int gridSizeX = 20;
		public int gridSizeY = 10;
		public int gridSizeZ = 20;

		public float layerCullingDistance = 0.0f;
		private float[] m_layerCullingDistances = null;
		private void AddCubes()
		{
			Vector3 offset = new Vector3(-0.5f * gridInterval * gridSizeX, -0.5f * gridInterval * gridSizeY, -0.5f * gridInterval * gridSizeZ);
			for (int z = 0; z < gridSizeZ; ++z)
			{
				for (int y = 0; y < gridSizeY; ++y)
				{
					for (int x = 0; x < gridSizeX; ++x)
					{
						GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
						DestroyImmediate(cube.GetComponent<Collider>());
						cube.gameObject.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
						cube.layer = 0;
						cube.transform.parent = transform;
						cube.transform.localScale = new Vector3(cubeSize, cubeSize, cubeSize);
						cube.transform.transform.position = offset + new Vector3(gridInterval * x, gridInterval * y, gridInterval * z);
						cube.transform.localRotation = Quaternion.identity;
					}
				}
			}
		}
		private void Update()
		{
			if (createTestCubes && transform.childCount == 0)
			{
				AddCubes();
			}
			else if (!createTestCubes)
			{
				int i = transform.childCount;
				while (0 < i--)
				{
					DestroyImmediate(transform.GetChild(i).gameObject);
				}
			}
		}
		private float m_cameraAngleX;
		private float m_cameraAngleY;
		private float m_distance;
		private Vector3 m_mousePosition;
		enum MouseButtonState
		{
			Normal,
			LeftButtonDown,
			RightButtonDown
		}
		private MouseButtonState m_mouseButtonState = MouseButtonState.Normal;
		private void Start()
		{
			if (camera != null)
			{
				Vector3 eulerAngles = camera.transform.eulerAngles;
				m_cameraAngleX = eulerAngles.y;
				m_cameraAngleY = eulerAngles.x;
				m_mouseButtonState = MouseButtonState.Normal;
				m_mousePosition = Input.mousePosition;
				if (cameraTarget != null)
				{
					m_distance = Vector3.Distance(cameraTarget.position, camera.transform.position);
				}
			}
		}
		private void LateUpdate()
		{
			if (!Application.isPlaying)
			{
				return;
			}
			if (camera != null)
			{
				if (0.0f < layerCullingDistance || m_layerCullingDistances != null)
				{
					if (m_layerCullingDistances == null)
					{
						m_layerCullingDistances = camera.layerCullDistances;
					}
					m_layerCullingDistances[0] = layerCullingDistance;
					camera.layerCullDistances = m_layerCullingDistances;
				}
				// controll camera
				if (m_mouseButtonState == MouseButtonState.Normal)
				{
					m_mousePosition = Input.mousePosition;
				}
				if (Input.GetMouseButton(0))
				{
					m_mouseButtonState = MouseButtonState.LeftButtonDown;
				}
				else if (Input.GetMouseButton(1))
				{
					m_mouseButtonState = MouseButtonState.RightButtonDown;
				}
				else
				{
					m_mouseButtonState = MouseButtonState.Normal;
				}
				if (m_mouseButtonState != MouseButtonState.Normal)
				{
					if (Input.GetKey(KeyCode.LeftShift))
					{
						m_mouseButtonState = MouseButtonState.RightButtonDown;
					}
					if (Input.touchCount == 3)
					{
						m_mouseButtonState = MouseButtonState.RightButtonDown;
					}
					else if (Input.touchCount == 2)
					{
						m_mouseButtonState = MouseButtonState.Normal;
					}
				}
				if (m_mouseButtonState == MouseButtonState.RightButtonDown)
				{
					Vector3 deltaPos = Input.mousePosition - m_mousePosition;
					float fov = camera.fieldOfView;
					float d = -2.0f * Mathf.Tan(0.5f * fov * Mathf.Deg2Rad) / camera.pixelHeight;
					if (cameraTarget == null)
					{
						camera.transform.position += cameraSpeed * camera.transform.TransformVector(deltaPos * d);
					}
					else
					{
						cameraTarget.position += m_distance * camera.transform.TransformVector(deltaPos * d);
					}
				}
				else if (m_mouseButtonState == MouseButtonState.LeftButtonDown)
				{
					Vector3 deltaPos = Input.mousePosition - m_mousePosition;
					m_cameraAngleX -= deltaPos.x * 180.0f / Screen.width;
					m_cameraAngleY += deltaPos.y * 180.0f / Screen.width;
					if (180.0f < m_cameraAngleX)
					{
						m_cameraAngleX -= 360.0f;
					}
					else if (m_cameraAngleX < -180.0f)
					{
						m_cameraAngleX += 360.0f;
					}
					if (180.0f < m_cameraAngleY)
					{
						m_cameraAngleY -= -360.0f;
					}
					else if (m_cameraAngleY < -180.0f)
					{
						m_cameraAngleY += 360.0f;
					}
					camera.transform.rotation = Quaternion.Euler(m_cameraAngleY, m_cameraAngleX, 0);
				}
				float dz = Input.GetAxis("Mouse ScrollWheel");
				if (Input.touchCount == 2)
				{
					Vector2 p1 = (Input.touches[0].position - Input.touches[1].position);
					Vector2 p2 = p1 - (Input.touches[0].deltaPosition - Input.touches[1].deltaPosition);
					float zoom = p2.magnitude / p1.magnitude;
					zoom = Mathf.Max(0.5f, zoom);
					zoom = Mathf.Min(2.0f, zoom);
					dz = Mathf.Log(zoom);
				}
				if (dz != 0.0f)
				{
					if (cameraTarget == null)
					{
						camera.transform.position += cameraSpeed * dz * camera.transform.forward;
					}
					else
					{
						m_distance *= Mathf.Exp(dz);
						if (m_distance < 0.01f)
						{
							m_distance = 0.01f;
						}
					}
				}
				float x = Input.GetAxis("Horizontal");
				if (x != 0.0f)
				{
					if (cameraTarget == null)
					{
						camera.transform.position += cameraSpeed * x * Time.deltaTime * camera.transform.right;
					}
					else
					{
						Vector3 v = camera.transform.right;
						v.y = 0.0f;
						if (0.0001f < v.sqrMagnitude)
						{
							v.Normalize();
						}
						v *= cameraSpeed * x * Time.deltaTime;
						cameraTarget.position += v;
					}
				}
				float y = Input.GetAxis("Vertical");
				if (y != 0.0f)
				{
					if (cameraTarget == null)
					{
						camera.transform.position += cameraSpeed * y * Time.deltaTime * camera.transform.up;
					}
					else
					{
						Vector3 v = Vector3.Cross(camera.transform.right, Vector3.up);
						v.y = 0.0f;
						if (0.0001f < v.sqrMagnitude)
						{
							v.Normalize();
						}
						v *= cameraSpeed * y * Time.deltaTime;
						cameraTarget.position += v;
					}
				}
				if (cameraTarget != null)
				{
					camera.transform.position = cameraTarget.position - m_distance * camera.transform.forward;
				}
				m_mousePosition = Input.mousePosition;
			}
		}
	}
}
