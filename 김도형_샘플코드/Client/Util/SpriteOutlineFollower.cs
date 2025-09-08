using UnityEngine;

namespace Util
{
    public class SpriteOutlineFollower : MonoBehaviour
    {
        public SpriteRenderer sourceRenderer;  // 본체 SpriteRenderer
        private SpriteRenderer outlineRenderer;

        void Awake()
        {
            outlineRenderer = GetComponent<SpriteRenderer>();
        }

        void LateUpdate()
        {
            if (sourceRenderer != null && outlineRenderer != null)
            {
                outlineRenderer.sprite = sourceRenderer.sprite;
                outlineRenderer.flipX = sourceRenderer.flipX;
                outlineRenderer.flipY = sourceRenderer.flipY;
                outlineRenderer.transform.localPosition = Vector3.zero;
            }
        }
    }
}