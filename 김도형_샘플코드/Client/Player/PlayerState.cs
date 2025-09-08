public abstract class PlayerState
{
    protected BasePlayer player;

    protected PlayerState(BasePlayer player)
    {
        this.player = player;
    }

    public virtual void Enter() { }

    public virtual void Update() { }

    public virtual void Exit() { }
}