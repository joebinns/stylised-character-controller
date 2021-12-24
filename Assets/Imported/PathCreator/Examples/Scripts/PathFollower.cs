using UnityEngine;

namespace PathCreation.Examples
{
    // Moves along a path at constant speed.
    // Depending on the end of path instruction, will either loop, reverse, or stop at the end of the path.
    public class PathFollower : MonoBehaviour
    {
        public PathCreator pathCreator;
        public EndOfPathInstruction endOfPathInstruction;
        public float speed = 5;
        float distanceTravelled;
        public bool rotate;

        void Start() {
            if (pathCreator != null)
            {
                // Subscribed to the pathUpdated event so that we're notified if the path changes during the game
                pathCreator.pathUpdated += OnPathChanged;
            }
        }

        void FixedUpdate()
        {
            if (pathCreator != null)
            {
                distanceTravelled += speed * Time.deltaTime;
                Vector3 pos = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);

                if (rotate)
                {
                    transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction);
                }
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