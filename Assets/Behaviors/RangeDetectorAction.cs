using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Range Detector", story: "update Range [Detector] and assign [target]", category: "Action", id: "93b3a9a79a12a0495192caabcf42d02c")]
public partial class RangeDetectorAction : Action
{
    [SerializeReference] public BlackboardVariable<RangeDetector> Detector;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    protected override Status OnUpdate()
    {
        Target.Value = Detector.Value.UpdateDetector();
        return Detector.Value.UpdateDetector() == null ? Status.Failure : Status.Success;
    }
}

