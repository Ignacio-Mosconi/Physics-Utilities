using System.Collections;
using UnityEngine;

namespace PhysicsUtilities
{
    public enum AccelerationAxis
    {
        Horizontal,
        Vertical,
        Forward
    }

    public static class PhysicalMovements
    {
        public static void LinearMotion(Transform transform, Vector3 direction, float speed)
        {
            direction.Normalize();

            Vector3 newPosition = transform.position;
            newPosition.x += direction.x * speed * Time.deltaTime;
            newPosition.y += direction.y * speed * Time.deltaTime;
            newPosition.z += direction.z * speed * Time.deltaTime;

            transform.position = newPosition;
        }

        public static void ConstantAccelerationMotion(Transform transform, AccelerationAxis axis, ref float initialAxisSpeed, 
                                                    float acceleration, float maxSpeed)
        {
            Vector3 newPosition = transform.position;
            float currentAxisSpeed = acceleration * Time.deltaTime + initialAxisSpeed;

            currentAxisSpeed = Mathf.Clamp(currentAxisSpeed, -maxSpeed, maxSpeed);
            
            switch (axis)
            {
                case AccelerationAxis.Horizontal:
                    newPosition.x += currentAxisSpeed * Time.deltaTime;
                    break;
                case AccelerationAxis.Vertical:
                    newPosition.y += currentAxisSpeed * Time.deltaTime;
                    break;
                case AccelerationAxis.Forward:
                    newPosition.z += currentAxisSpeed * Time.deltaTime;
                    break;
            }

            initialAxisSpeed = currentAxisSpeed;

            transform.position = newPosition;
        }

        public static IEnumerator PerformObliqueMotion2D(Transform transform, float speed, float angle, float gravity)
        {
            float time = 0f;
            float initialSpeedX = Mathf.Cos(angle * Mathf.Deg2Rad) * speed;
            float initialSpeedY = Mathf.Sin(angle * Mathf.Deg2Rad) * speed;
            float initialPosX = transform.position.x;
            float initialPosY = transform.position.y;

            while (transform.gameObject.activeInHierarchy)
            {
                time += Time.deltaTime;

                Vector3 newPosition;
                float newPosX = initialSpeedX * time + initialPosX;
                float newPosY = gravity * time * time * 0.5f + initialSpeedY * time + initialPosY;

                newPosition = new Vector3(newPosX, newPosY, transform.position.z);

                transform.position = newPosition;

                yield return new WaitForEndOfFrame();
            }
        }
    }
}
