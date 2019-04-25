using UnityEngine;
using UnityEngine.Events;

namespace PhysicsUtilities
{
    public enum ColliderType
    {
        Box,
        Circle
    }

    public class TriggerEvent : UnityEvent<CustomCollider2D> { }
    public class CollisionEvent : UnityEvent<CustomCollider2D, Vector2, float> { }

    [RequireComponent(typeof(SpriteRenderer))]
    public abstract class CustomCollider2D : MonoBehaviour
    {
        [SerializeField] float mass = 1f;
        [SerializeField] bool collisionEnabled = true;
        [SerializeField] bool isTrigger = false;

        [HideInInspector] public TriggerEvent OnTrigger = new TriggerEvent();
        [HideInInspector] public CollisionEvent OnCollision = new CollisionEvent();

        protected SpriteRenderer spriteRenderer;

        virtual protected void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            CollisionManager.Instance.RegisterCollider2D(gameObject.layer, this);
        }

        public void ResolveCollision(CustomCollider2D collider, Vector2 normal, float penetration)
        {
            if (!isTrigger)
            {
                float massRatio = collider.Mass / collider.Mass;
                float penetrationMult = 1f / (1f + massRatio);

                transform.Translate(normal * penetration * penetrationMult);

                OnCollision.Invoke(collider, normal, penetration);
            }
            else
                OnTrigger.Invoke(collider);      
        }

        public abstract ColliderType GetColliderType();

        public float Mass
        {
            get { return mass; }
        }

        public bool CollisionEnabled
        {
            get { return collisionEnabled; }
            set { collisionEnabled = value; }
        }
    }
}
