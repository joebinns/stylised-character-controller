using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Presentation : MonoBehaviour
{
    public Transition[] transitions;

    public int step = 0;

    public void MakeNextTransition(InputAction.CallbackContext context)
    {
        if (context.started) // button down
        {
            int nextTransition = step + 1;
            nextTransition = Mathf.Min(nextTransition, transitions.Length);
            bool success = transitions[nextTransition - 1].MakeTransition();
            if (!success)
            {
                return;
            }
            step = nextTransition;
        }
    }

    public void RevertPrevTransition(InputAction.CallbackContext context)
    {
        if (context.started) // button down
        {
            int prevTransition = step - 1;
            prevTransition = Mathf.Max(0, prevTransition);
            bool success = transitions[prevTransition].RevertTransition();
            if (!success)
            {
                return;
            }
            step = prevTransition;
        }
    }
}
