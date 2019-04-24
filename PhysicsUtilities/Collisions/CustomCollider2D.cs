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
        [SerializeField] float mass;
        [SerializeField] bool collisionEnabled;

        [HideInInspector] public TriggerEvent OnTrigger = new TriggerEvent();
        [HideInInspector] public CollisionEvent OnCollision = new CollisionEvent();

        protected SpriteRenderer spriteRenderer;

        virtual protected void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            CollisionManager.Instance.RegisterCollider2D(gameObject.layer, this);
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
