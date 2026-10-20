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
        Initialize();
    }


    private void Start()
    {
        chara.followLanding.AssignDisableGravityFunc(EnableGravityFunc);
    }

    protected override void Update()
    {
        base.Update();
        if (CurrentMotionStateProgressEnum == AnimationProgress.None)
        {
            ThinkNextMotion();
        }
    }

    [ContextMenu(nameof(Initialize))]
    private void Initialize()
    {
        Initialize_Base();
        AssignMotionStateProfile<MotionState_Player>();
        InitializeMotionState();
        AssignTransitional();
    }


    public void Assign_Player(PlayerOperator_Frontier _player)
    {
        player = _player;
        foreach (var _m in motionDic)
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
        foreach (var _d in motionDic)
        {
            for (int i = 0; i < _d.Value.progressList.Count; ++i)
            {
                AnimationProgress_Base _currentProgress = _d.Value.progressList[i];
                List<TransitionalRange> _transitionalList = _currentProgress.transitionalRangeList;
                for (int j = 0; j < _transitionalList.Count; ++j)
                {
                    _transitionalList[j].Initialize(this);
                }
            }
        }
    }

    private void AssignTransitional()
    {
        var _transitionnalDic = new List<List<TransitionalRange>>();
        foreach (var _d in motionDic)
        {
            for (int i = 0; i < _d.Value.progressList.Count; ++i)
            {
                AnimationProgress_Base _currentProgress = _d.Value.progressList[i];
                List<TransitionalRange> _transitionalList = _currentProgress.transitionalRangeList;
                _transitionnalDic.Add(_transitionalList);
            }
        }

        //motionDic[PlayerMotion.Sprint_Value].progressDic[AnimationProgress.In].toFinishFunc = Sprint_ToFinishFunc;
        motionDic[PlayerMotion.Sprint_Value].progressDic[AnimationProgress.Loop].toFinishFunc = Sprint_ToFinishFunc;
        motionDic[PlayerMotion.Jump_Value].progressDic[AnimationProgress.Loop].toFinishFunc = Jump_ToFinishFunc; ;

        foreach (var _t in _transitionnalDic)
        {
            TransitionalRange _transisitionalRange = new TransitionalRange();
            for (int i = 0; i < _t.Count; ++i)
            {
                _transisitionalRange = _t[i];
                PlayerMotion _convert = Converter.GetEnumFromInt<PlayerMotion>(_transisitionalRange.profile.destinationState.Value);
                switch (_convert)
                {
                    case PlayerMotion.Idle_Stick:
                    _transisitionalRange.transitionalInputFunc = TransitionalFunc_ToIdle;
                    break;
                    case PlayerMotion.Walk_Stick:
                    _transisitionalRange.transitionalInputFunc = TransitionalFunc_ToWalk;
                    break;
                    case PlayerMotion.Sprint_Value:
                    _transisitionalRange.transitionalInputFunc = TransitionalFunc_ToSprint;
                    break;
                    case PlayerMotion.Jump_Value:
                    _transisitionalRange.transitionalInputFunc = TransitionalFunc_ToJump;
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
            if (player.InputCommandDic[InputCommandState_F.Sprint_Value].Executable == false)
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
            if (player.InputCommandDic[InputCommandState_F.Sprint_Value].Executable == true)
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
        if (player.InputCommandDic[InputCommandState_F.Jump_Button].Executable == true)
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
            if (player.InputCommandDic[InputCommandState_F.Sprint_Value].Executable == false)
            {
                _ret = true;
            }
            else if (player.IsMoveInputting == false)
            {
                _ret = true;
            }
            //Debug.LogWarning($"{nameof(Sprint_ToFinishFunc)} {currentMotionStateEnum} {currentMotionStateProgressEnum}");
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
        PlayerMotion _ret = PlayerMotion.Idle_Stick;
        if (TransitionalFunc_ToSprint() == true)
        {
            _ret = PlayerMotion.Sprint_Value;
        }
        else if (TransitionalFunc_ToWalk() == true)
        {
            _ret = PlayerMotion.Walk_Stick;
        }

        //Debug.Log($"{nameof(ThinkNextMotion)} {_ret}");
        AssignNextStateAndProgress(_ret, motionDic[_ret].FirstProgress);
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
                case PlayerMotion.Idle_Stick:
                    {
                        _ret = true;
                    }
                break;

                case PlayerMotion.Walk_Stick:
                    {
                        _ret = true;
                    }
                break;

                case PlayerMotion.Sprint_Value:
                    {
                        _ret = true;
                    }
                break;

                case PlayerMotion.Jump_Value:
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
            if (currentMotionStateEnum == PlayerMotion.Jump_Value)
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
            _ret = motionDic[PlayerMotion.Walk_Stick].progressDic[AnimationProgress.Loop].profile.speedProfile.MultiplyValue;
            return _ret;
        }
    }
}


public enum PlayerMotion
{
    Idle_Stick = 0,
    Walk_Stick,
    Sprint_Value,
    Jump_Value,
    Guard,
    Fall,
    BackStub,
    GuardKnockBack,

    Down,

    Think,
}