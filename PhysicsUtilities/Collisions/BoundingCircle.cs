using UnityEngine;

namespace PhysicsUtilities
{
    public class BoundingCircle : CustomCollider2D
    {
        float radius;

        override protected void Awake()
        {
            base.Awake();

            radius = Mathf.Max(spriteRenderer.bounds.size.x, spriteRenderer.bounds.size.y) * 0.5f;
        }

        public override ColliderType GetColliderType()
        {
            return ColliderType.Circle;
        }

        public float Radius
        {
            get { return radius; }
        }
    }
}
