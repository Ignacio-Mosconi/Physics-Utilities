using UnityEngine;

namespace PhysicsUtilities
{
    public class BoundingBox : CustomCollider2D
    {
        float width;
        float height;
        float outerRadius;
        float innerRadius;

        override protected void Awake()
        {
            base.Awake();

            width = spriteRenderer.bounds.size.x;
            height = spriteRenderer.bounds.size.y;

            outerRadius = Mathf.Sqrt((width * width) + (height * height));
            innerRadius = Mathf.Max(width, height) * 0.5f;
        }

        public override ColliderType GetColliderType()
        {
            return ColliderType.Box;
        }

        public float Width
        {
            get { return width; }
        }

        public float Height
        {
            get { return height; }
        }

        public float OuterRadius
        {
            get { return outerRadius; }
        }

        public float InnerRadius
        {
            get { return innerRadius; }
        }
    }
}
