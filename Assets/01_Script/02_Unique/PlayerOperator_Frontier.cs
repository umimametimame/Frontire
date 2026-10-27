using AYellowpaper.SerializedCollections;
using UnityEngine;

public class PlayerOperator_Frontier : PlayerOperator_BaseTPS
{
    [field: SerializeField, NonEditable] public MotionStateBehaviour_Frontier motionBehaviou { get; private set; }
    public void Initialize()
    {
        motionBehaviou.Initialize();
        motionBehaviou.Assign_Player(this);
    }

    public void AssignReference()
    {
        motionBehaviou = GetComponent<MotionStateBehaviour_Frontier>();
    }

    private void Awake()
    {
        AssignReference();
    }

    private void Start()
    {
        Initialize();
    }

    protected override void Update()
    {
        base.Update();
    }



    public SerializedDictionary<InputCommandState_F, AdvancedInput> InputCommandDic
    {
        get
        {
            return inputParam.inputValues.inputDic;
        }
    }

    /// <summary>
    /// 移動可能状態で移動入力がある
    /// </summary>
    public bool IsMoveInputtingInMovable
    {
        get
        {
            bool _ret = false;

            if (motionBehaviou.IsNeutral == true)
            {
                if (IsMoveInputting == true)
                {
                    _ret = true;
                }
            }

            return _ret;
        }
    }
}
