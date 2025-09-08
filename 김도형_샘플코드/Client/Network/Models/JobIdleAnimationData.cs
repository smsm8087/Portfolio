using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Job/IdleAnimationData", fileName = "JobIdleAnimationData")]
public class JobIdleAnimationData : ScriptableObject
{
    public string jobType;             // 예: "warrior", "mage"
    public List<FrameData> idleFrames; // Sprite + duration      // 해당 직업의 idle 프레임
}
[System.Serializable]
public class FrameData
{
    public Sprite sprite;
    public float duration = 0.1f;
}