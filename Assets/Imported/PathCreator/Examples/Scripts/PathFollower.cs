using UnityEngine;

namespace PathCreation.Examples
{
    // Moves along a path Loops.
    // Depending on the end of path instruction, will either loop, reverse, or stop at the end of the path.
    public class PathFollower : MonoBehaviour
    {
        public PathCreator pathCreator;
        public EndOfPathInstruction endOfPathInstruction;
        public Vector3 rotateScale = Vector3.one;
        public float period = 10f;
        private float _t;
        private float distanceTravelled = 0f;

        void Start() {
            if (pathCreator != null)
            {
                // Subscribed to the pathUpdated event so that we're notified if the path changes during the game
                pathCreator.pathUpdated += OnPathChanged;
            }
        }

        void FixedUpdate()
        {
            _t += Time.fixedDeltaTime;
            if (pathCreator != null)
            {
                float ratio = _t / period;
                float theta = ratio * 2f * Mathf.PI;

                if (endOfPathInstruction == EndOfPathInstruction.Reverse)
                {
                    distanceTravelled = pathCreator.path.length * 0.5f * (1 - Mathf.Sin(theta + Mathf.PI / 2f));
                }
                else
                {
                    distanceTravelled += Time.fixedDeltaTime / period;
                }

                Vector3 pos = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
                Vector3 rot = pathCreator.path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction).eulerAngles;

                Vector3 tempRot = Vector3.zero;
                for (int i = 0; i < 3; i++)
                {
                    if (rotateScale[i] == 1)
                    {
                        tempRot[i] = rot[i];
                    }
                    else
                    {
                        tempRot[i] = transform.rotation[i];
                    }
                }
                transform.rotation = Quaternion.Euler(tempRot);
                if (GetComponent<Oscillator>())
                {
                    Oscillator oscillator = GetComponent<Oscillator>();
                    oscillator.localEquilibriumPosition = pos;
                    Vector3 tempPos = Vector3.zero;
                    for (int i = 0; i < 3; i++)
                    {
                        if (oscillator.forceScale[i] == 0)
                        {
                            tempPos[i] = pos[i];
                        }
                        else
                        {
                            tempPos[i] = transform.position[i];
                        }
                    }
                    transform.position = tempPos;
                }
                else
                {
                    transform.position = pos;
                }
            }
        }

        // If the path changes during the game, update the distance travelled so that the follower's position on the new path
        // is as close as possible to its position on the old path
        void OnPathChanged() {
            distanceTravelled = pathCreator.path.GetClosestDistanceAlongPath(transform.position);
        }
    }
}