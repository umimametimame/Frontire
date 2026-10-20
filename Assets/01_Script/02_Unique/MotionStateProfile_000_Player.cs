using UnityEngine;
using NaughtyAttributes;
using AYellowpaper.SerializedCollections;
[CreateAssetMenu(fileName = nameof(MotionStateProfile_000_Player) + "_", menuName = "ScriptableObject/" + nameof(MotionStateProfile_000_Player))]

public class MotionStateProfile_000_Player : MotionStateProfile_BaseFrontier<PlayerMotion>
{
    [EnableIf(nameof(state), PlayerMotion.Sprint_Value)]
    public float sprintSpeed;


}