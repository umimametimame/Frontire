using AddUnityClass;
using AYellowpaper.SerializedCollections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MotionStateBehaviour_Frontier : MotionStateBehaviour_Base<PlayerMotion>
{
    private PlayerOperator_Frontier player;

    private void Awake()
    {
        AssignReference();
    }
    protected override void Update()
    {
        base.Update();
        if (CurrentMotionStateProgressEnum == AnimationProgress.None)
        {
            ThinkNextMotion();
        }
    }

    private void AssignReference()
    {
        AssignReference_Base();
    }

    public void Initialize()
    {
        InitializeMotionState();
        chara.followLanding.AssignDisableGravityFunc(EnableGravityFunc);
        AssignToFinishFunc();
        AssignTransitionalRangeFunc();
    }


    public void Assign_Player(PlayerOperator_Frontier _player)
    {
        player = _player;
        foreach (var _m in motionStateDic)
        {
            if (_m.Value is MotionState_Player)
            {
                MotionState_Player _convertMotion = (MotionState_Player)_m.Value;
                _convertMotion.AssignPlayer(_player);
            }
        }
    }


    private void InitializeMotionState()
    {
        ProfileMotionStateDic<MotionState_Player>();
        foreach (var _motionStateIdx in motionStateDic)
        {
            for (int i = 0; i < _motionStateIdx.Value.progressList.Count; ++i)
            {
                AnimationProgress_Base _currentProgress = _motionStateIdx.Value.progressList[i];
                List<TransitionalRange> _transitionalList = _currentProgress.transitionalList;
                for (int j = 0; j < _transitionalList.Count; ++j)
                {
                    _transitionalList[j].Initialize(this);
                }
            }
        }
        //Debug.Log($"{nameof(InitializeMotionState)} {motionStateDic.Count}");
    }

    private void AssignToFinishFunc()
    {
        motionStateDic[PlayerMotion.Sprint].progressDic[AnimationProgress.Loop].toFinishFunc = Sprint_ToFinishFunc;
        motionStateDic[PlayerMotion.Jump].progressDic[AnimationProgress.Loop].toFinishFunc = Jump_ToFinishFunc; ;

    }
    private void AssignTransitionalRangeFunc()
    {

        Dictionary<PlayerMotion, Func<bool>> _transitionalRangeFuncDic = new Dictionary<PlayerMotion, Func<bool>>();
        _transitionalRangeFuncDic.Add(PlayerMotion.Idle, TransitionalFunc_ToIdle);
        _transitionalRangeFuncDic.Add(PlayerMotion.Walk, TransitionalFunc_ToWalk);
        _transitionalRangeFuncDic.Add(PlayerMotion.Sprint, TransitionalFunc_ToSprint);
        _transitionalRangeFuncDic.Add(PlayerMotion.Jump, TransitionalFunc_ToJump);

        foreach (var _transitionalRangeIdx in GetTransitionalRangeListList)
        {
            TransitionalRange _transisitionalRange = new TransitionalRange();
            for (int i = 0; i < _transitionalRangeIdx.Count; ++i)
            {
                _transisitionalRange = _transitionalRangeIdx[i];
                PlayerMotion _convertEnum = Converter.GetEnumFromInt<PlayerMotion>(_transisitionalRange.profile.destinationState.Value);
                _transisitionalRange.transitionalInputFunc = _transitionalRangeFuncDic[_convertEnum];
                
            }
        }
    }


    private bool TransitionalFunc_ToIdle()
    {
        bool _ret = false;
        if (player.IsMoveInputtingInMovable == false)
        {
            _ret = true;
            Debug.Log($"Idle Input");
        }
        return _ret;
    }
    private bool TransitionalFunc_ToWalk()
    {
        bool _ret = false;
        if (player.IsMoveInputtingInMovable == true)
        {
            if (player.InputCommandDic[InputCommandState_F.Sprint].Executable == false)
            {

                _ret = true;
                Debug.Log($"Walk Input");
            }
        }
        return _ret;
    }


    private bool TransitionalFunc_ToSprint()
    {
        bool _ret = false;
        if (player.IsMoveInputtingInMovable == true)
        {
            if (player.InputCommandDic[InputCommandState_F.Sprint].Executable == true)
            {
                _ret = true;
                Debug.Log($"Sprint Input");
            }
        }

        return _ret;
    }

    private bool TransitionalFunc_ToJump()
    {
        bool _ret = false;
        if (player.InputCommandDic[InputCommandState_F.Jump].Executable == true)
        {
            _ret = true;
            Debug.Log($"Jump Input");
        }
        return _ret;
    }

    private bool Sprint_ToFinishFunc()
    {
        bool _ret = false;

        // SprintモーションがOutでなければ
        if (CurrentMotionStateProgressEnum != AnimationProgress.Out)
        {
            if (player.InputCommandDic[InputCommandState_F.Sprint].Executable == false)
            {
                _ret = true;
            }
            else if (player.IsMoveInputting == false)
            {
                _ret = true;
            }
        }


        return _ret;
    }

    private bool Jump_ToFinishFunc()
    {
        bool _ret = false;

        if (chara.followLanding.isLanding == true)
        {
            _ret = true;
        }

        return _ret;
    }

    private void ThinkNextMotion()
    {
        PlayerMotion _ret = PlayerMotion.Idle;
        if (TransitionalFunc_ToSprint() == true)
        {
            _ret = PlayerMotion.Sprint;
        }
        else if (TransitionalFunc_ToWalk() == true)
        {
            _ret = PlayerMotion.Walk;
        }

        //Debug.Log($"{nameof(ThinkNextMotion)} {_ret}");
        AssignNextStateAndProgress(_ret, motionStateDic[_ret].FirstProgress);
    }

    private bool EnableGravityFunc()
    {
        bool _ret = true;
        if (chara.followLanding.isLanding == true)
        {
            _ret = false;
        }
        else if (IsJumping == true)
        {
            _ret = false;
        }

        return _ret;
    }

    public override bool IsNeutral
    {
        get
        {
            bool _ret = false;

            switch (currentMotionStateEnum)
            {
                case PlayerMotion.Idle:
                    {
                        _ret = true;
                    }
                break;

                case PlayerMotion.Walk:
                    {
                        _ret = true;
                    }
                break;

                case PlayerMotion.Sprint:
                    {
                        _ret = true;
                    }
                break;

                case PlayerMotion.Jump:
                    {
                        _ret = true;
                    }
                break;
            }
            return _ret;
        }
    }
    public bool IsJumping
    {
        get
        {
            bool _ret = false;
            if (currentMotionStateEnum == PlayerMotion.Jump)
            {
                if (CurrentMotionStateProgressEnum == AnimationProgress.In)
                {
                    _ret = true;
                }
            }

            return _ret;
        }
    }
    public float MoveSpeed
    {
        get
        {
            float _ret;
            _ret = motionStateDic[PlayerMotion.Walk].progressDic[AnimationProgress.Loop].profile.speedProfile.MultiplyValue;
            return _ret;
        }
    }
}


public enum PlayerMotion
{
    Idle = 0,
    Walk,
    Sprint,
    Jump,
    Guard,
    Fall,
    BackStub,
    GuardKnockBack,

    Down,

    Think,
}