using System;
using System.Collections.Generic;
using UnityEngine;

struct LayerKey
{
    public LayerMask layer;
    public int index;

    public LayerKey(LayerMask l, int i)
    {
        layer = l;
        index = i;
    }
}

namespace PhysicsUtilities
{
    public class CollisionManager : MonoBehaviour
    {
        static CollisionManager instance;

        [SerializeField] LayerMask[] collisionLayers;

        Dictionary<LayerKey, List<CustomCollider2D>> colliders = new Dictionary<LayerKey, List<CustomCollider2D>>();

        void Awake()
        {
            if (Instance != this)
                Destroy(gameObject);
        }

        void Update()
        {
            foreach (LayerKey layerA in colliders.Keys)
            {
                foreach (LayerKey layerB in colliders.Keys)
                {
                    if (layerB.index >= layerA.index)
                    {
                        foreach (CustomCollider2D colliderA in colliders[layerA])
                        {
                            if (colliderA.CollisionEnabled)
                            {
                                ColliderType colliderAType = colliderA.GetColliderType();

                                foreach (CustomCollider2D colliderB in colliders[layerB])
                                {
                                    if (colliderB.CollisionEnabled)
                                    {
                                        ColliderType colliderBType = colliderB.GetColliderType();

                                        if (colliderAType == ColliderType.Box && colliderBType == ColliderType.Box)
                                            CheckBoxAndBoxCollision(colliderA as CustomBoxCollider2D, colliderB as CustomBoxCollider2D);
                                        if (colliderAType == ColliderType.Circle && colliderBType == ColliderType.Box)
                                            CheckCircleAndBoxCollision(colliderA as CustomCircleCollider2D, colliderB as CustomBoxCollider2D);
                                        if (colliderAType == ColliderType.Box && colliderBType == ColliderType.Circle)
                                            CheckCircleAndBoxCollision(colliderB as CustomCircleCollider2D, colliderA as CustomBoxCollider2D);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        void CheckBoxAndBoxCollision(CustomBoxCollider2D boxA, CustomBoxCollider2D boxB)
        {
            Vector2 posA = boxA.transform.position;
            Vector2 posB = boxB.transform.position;
            Vector2 diff = posB - posA;

            float minDistX = (boxA.Width + boxB.Width) * 0.5f;
            float minDistY = (boxA.Height + boxB.Height) * 0.5f;

            float deltaX = Mathf.Abs(diff.x);
            float deltaY = Mathf.Abs(diff.y);

            if (deltaX < minDistX && deltaY < minDistY)
            {
                Vector2 normalA = Vector2.zero;
                Vector2 normalB = Vector2.zero;
                float horPenetration = minDistX - deltaX;
                float verPenetration = minDistY - deltaY;
                float distanceToMove = Mathf.Min(horPenetration, verPenetration);

                if (horPenetration > verPenetration)
                {
                    normalB.x = normalA.x = 0f;
                    normalB.y = (diff.y > 0f) ? 1f : -1f;
                    normalA.y = -normalB.y;
                }
                else
                {
                    normalB.y = normalA.y = 0f;
                    normalB.x = (diff.x > 0f) ? 1f : -1f;
                    normalA.x = -normalB.x;
                }

                boxA.ResolveCollision(boxB, normalA, distanceToMove);
                boxB.ResolveCollision(boxA, normalB, distanceToMove);
            }
        }

        void CheckCircleAndBoxCollision(CustomCircleCollider2D circle, CustomBoxCollider2D box)
        {
            Vector2 circlePos = circle.transform.position;
            Vector2 boxPos = box.transform.position;
            Vector2 diff = circlePos - boxPos;
            bool collisionDetected = false;
            float distOuterRadius = circle.Radius + box.OuterRadius;
            float distInnerRadius = circle.Radius + box.InnerRadius;
            float sqrDistance = Vector2.SqrMagnitude(diff);

            if (sqrDistance < distOuterRadius)
            {
                if (sqrDistance > distInnerRadius)
                {
                    Vector2[] boxCorners = new Vector2[4];
                    boxCorners[0] = new Vector2(boxPos.x - box.Width * 0.5f, boxPos.y + box.Height * 0.5f);
                    boxCorners[1] = new Vector2(boxPos.x + box.Width * 0.5f, boxPos.y + box.Height * 0.5f);
                    boxCorners[2] = new Vector2(boxPos.x + box.Width * 0.5f, boxPos.y - box.Height * 0.5f);
                    boxCorners[3] = new Vector2(boxPos.x - box.Width * 0.5f, boxPos.y - box.Height * 0.5f);

                    foreach (Vector2 boxCorner in boxCorners)
                    {
                        Vector2 cornerDiff = circlePos - boxCorner;

                        if (Vector2.SqrMagnitude(cornerDiff) <= circle.Radius)
                        {
                            collisionDetected = true;
                            break;
                        }
                    }
                }
                else
                    collisionDetected = true;
            }

            if (collisionDetected)
            {
                circle.OnTrigger.Invoke(box);
                box.OnTrigger.Invoke(circle);
            }
        }

        public void RegisterCollider2D(LayerMask layer, CustomCollider2D collider)
        {
            LayerKey[] keys = new LayerKey[colliders.Keys.Count];
            colliders.Keys.CopyTo(keys, 0);

            LayerKey layerKey = Array.Find(keys, k => k.layer == layer);

            if (!colliders.ContainsKey(layerKey))
            {
                int nextIndex = keys.GetLength(0);
                layerKey = new LayerKey(layer, nextIndex);
                List<CustomCollider2D> colliderList = new List<CustomCollider2D>();

                colliders.Add(layerKey, colliderList);
            }

            colliders[layerKey].Add(collider);
        }

        public static CollisionManager Instance
        {
            get
            {
                if (!instance)
                {
                    instance = FindObjectOfType<CollisionManager>();
                    if (!instance)
                    {
                        GameObject gameObj = new GameObject("CollisionManager");
                        instance = gameObj.AddComponent<CollisionManager>();
                    }
                }

                return instance;
            }
        }
    }
}
