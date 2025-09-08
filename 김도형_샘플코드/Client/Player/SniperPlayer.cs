using UnityEngine;

public class SniperPlayer : BasePlayer
{
    public override bool CanAttackWhileJumping => false;
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }
}