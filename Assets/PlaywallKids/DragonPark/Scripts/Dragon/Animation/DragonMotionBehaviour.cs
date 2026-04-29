using UnityEngine;

public class DragonMotionBehaviour : StateMachineBehaviour
{
    private DragonMotionList _motionList;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_motionList == null)
            _motionList = animator.GetComponent<DragonMotionList>();

        int dance = -1;
        if (_motionList != null)
        {
            _motionList.index++;
            if (_motionList.index < _motionList.motions.Count)
                dance = _motionList.motions[_motionList.index];
        }

        if (dance != 0)
        {
            animator.SetInteger("motion_num", dance);
            animator.SetTrigger("motion");
        }

        if (_motionList.index == _motionList.motions.Count)
        {
            if (_motionList != null && _motionList.onFinished != null)
            {
                _motionList.onFinished();
                _motionList.onFinished = null;
            }
        }
    }
}