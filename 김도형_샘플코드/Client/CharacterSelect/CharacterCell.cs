using CharacterSelect;
using FancyScrollView;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCell : FancyCell<CharacterData, Context>
{
    [SerializeField] private UICharacterAnimator animator;
    [SerializeField] private Image image;
    [SerializeField] private Animator scrollAnimator;

    float currentPosition = 0;
    
    static class AnimatorHash
    {
        public static readonly int Scroll = Animator.StringToHash("scroll");
    }

    public override void Initialize()
    {
    }

    public override void UpdateContent(CharacterData data)
    {
        animator.SetJob(data.data.job_type);

        var selected = Context.SelectedIndex == Index;
        image.color = selected ? Color.white : new Color(1, 1, 1, 0.5f);
    }

    public override void UpdatePosition(float position)
    {
        currentPosition = position;
        
        if (scrollAnimator.isActiveAndEnabled)
        {
            scrollAnimator.Play(AnimatorHash.Scroll, -1, position);
        }

        scrollAnimator.speed = 0;
    }

    void OnEnable() => UpdatePosition(currentPosition);
}
