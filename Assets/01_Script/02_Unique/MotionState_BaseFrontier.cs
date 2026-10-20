using System;
using UnityEngine;

[Serializable]
public class MotionState_BaseFrontier<T> : MotionState_Base<T> where T : Enum
{
    protected PlayerOperator_Frontier player;
    public override void Initialize_Base()
    {
        base.Initialize_Base();
    }
    
    public void AssignPlayer(PlayerOperator_Frontier _player)
    {
        player = _player;
    }
    public void AssignParameter()
    {

    }
}

[Serializable]
public class MotionState_Player : MotionState_BaseFrontier<PlayerMotion>
{
    public float Speed
    {
        get
        {
            float _ret = -100.0f;
            if (profile is MotionStateProfile_000_Player == true)
            {
                MotionStateProfile_000_Player _profile = (MotionStateProfile_000_Player)profile;
                if (CurrentAnimationProgress != null)
                {

                    _ret = CurrentAnimationProgress.profile.speedProfile.Evaluate(CurrentAnimationProgress.motionTime.CurrentRatio);

                }

            }
            return _ret;
        }
    }


    public override void Initialize_Base()
    {
        base.Initialize_Base();
        life.enableAction += UpdateSwitchState;
    }

    private void UpdateSwitchState()
    {
        switch (state)
        {
            case PlayerMotion.Idle_Stick:
                {
                    UpdateIdle();
                }
                break;
            case PlayerMotion.Walk_Stick:
                {
                    UpdateWalk();
                }
                break;
            case PlayerMotion.Sprint_Value:
                {
                    UpdateSprint();
                }
                break;
            case PlayerMotion.Jump_Value:
                {
                    UpdateJump();
                }
            break;
            case PlayerMotion.Guard:
            break;
            case PlayerMotion.Fall:
            break;
            case PlayerMotion.BackStub:
            break;
            case PlayerMotion.GuardKnockBack:
            break;
            case PlayerMotion.Down:
            break;
            case PlayerMotion.Think:
            break;
        }
    }

    private void UpdateIdle()
    {

    }
    private void UpdateWalk()
    {
        player.Update_CharaAngle();
        velocity = MoveVelocity;
    }

    private void UpdateSprint()
    {
        AssignVelocity_Sprint();
    }

    private void AssignVelocity_Sprint()
    {

        if (Speed <= 0.0f)
        {
            Debug.LogError($"sprintSpeed‚ª0ˆÈ‰º‚Å‚·");
        }
        //Debug.Log($"{state} {currentProgress}");

        player.Update_CharaAngle();
        velocity = player.GetMoveVelocity_ConvertCamera * Speed;
    }

    private void UpdateJump()
    {
        AssignVelocity_Jump();
    }

    private void AssignVelocity_Jump()
    {
        player.Update_CharaAngle();
        Vector3 _jumpVelocity = new Vector3(0, Speed, 0);
        velocity = _jumpVelocity + MoveVelocity;
    }

    public Vector3 MoveVelocity
    {
        get
        {
            Vector3 _ret = player.GetMoveVelocity_ConvertCamera * player.motionBehaviou.MoveSpeed;

            return _ret;
        }
    }

    public float Error
    {
        get
        {
            Debug.Log($"ƒGƒ‰[");
            return -100;
        }
    }

}