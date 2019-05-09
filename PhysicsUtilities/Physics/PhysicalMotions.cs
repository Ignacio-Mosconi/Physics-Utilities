using System.Collections;
using UnityEngine;

namespace PhysicsUtilities
{
    public static class PhysicalMotions
    {
        public static void Linear(Transform transform, Vector3 direction, float speed)
        {
            Vector3 newPosition = transform.position;

            direction.Normalize();

            newPosition.x += direction.x * speed * Time.deltaTime;
            newPosition.y += direction.y * speed * Time.deltaTime;
            newPosition.z += direction.z * speed * Time.deltaTime;

            transform.position = newPosition;
        }

        public static void ConstantAcceleration(Transform transform, ref Vector3 initialSpeed, Vector3 acceleration, float maxSpeed = 0f)
        {
            Vector3 newPosition = transform.position;
            float currentSpeedX = acceleration.x * Time.deltaTime + initialSpeed.x;
            float currentSpeedY = acceleration.y * Time.deltaTime + initialSpeed.y;
            float currentSpeedZ = acceleration.z * Time.deltaTime + initialSpeed.z;

            if (maxSpeed > 0f)
            {
                currentSpeedX = Mathf.Clamp(currentSpeedX, -maxSpeed, maxSpeed);
                currentSpeedY = Mathf.Clamp(currentSpeedY, -maxSpeed, maxSpeed);
                currentSpeedZ = Mathf.Clamp(currentSpeedZ, -maxSpeed, maxSpeed);
            }

            newPosition.x += currentSpeedX * Time.deltaTime;
            newPosition.y += currentSpeedY * Time.deltaTime;
            newPosition.z += currentSpeedZ * Time.deltaTime;

            initialSpeed.x = currentSpeedX;
            initialSpeed.y = currentSpeedY;
            initialSpeed.z = currentSpeedZ;

            transform.position = newPosition;
        }

        public static void UniformCircular2D(Transform pivot, Transform satellite, float radius, float angularSpeed, ref float angle)
        {
            Vector3 newPosition = satellite.position;

            angle += angularSpeed * Time.deltaTime;

            if (angle >= 360f)
                angle -= 360f;

            newPosition.x = pivot.position.x + pivot.right.x * Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
            newPosition.y = pivot.position.y + pivot.up.y * Mathf.Sin(angle * Mathf.Deg2Rad) * radius;

            satellite.position = newPosition;
        }

        public static void ConstantAccelerationCircular2D(Transform pivot, Transform satellite, float radius, float acceleration,
                                                        ref float initialAngularSpeed, ref float angle, float maxSpeed = 0f)
        {
            Vector3 newPosition = satellite.position;
            float currentAngularSpeed = 0f;

            currentAngularSpeed = acceleration * Time.deltaTime + initialAngularSpeed;

            if (maxSpeed > 0f)
                currentAngularSpeed = Mathf.Clamp(currentAngularSpeed, -maxSpeed, maxSpeed);

            angle += currentAngularSpeed * Time.deltaTime;

            if (angle >= 360f)
                angle -= 360f;

            newPosition.x = pivot.position.x + pivot.right.x * Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
            newPosition.y = pivot.position.y + pivot.up.y * Mathf.Sin(angle * Mathf.Deg2Rad) * radius;

            initialAngularSpeed = currentAngularSpeed;

            satellite.position = newPosition;
        }

        public static IEnumerator PerformObliqueShot2D(Transform transform, float speed, float angle, float gravity)
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
