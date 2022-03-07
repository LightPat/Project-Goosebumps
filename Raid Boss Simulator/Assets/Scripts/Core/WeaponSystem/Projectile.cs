using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LightPat.Core.WeaponSystem
{
    public class Projectile : MonoBehaviour
    {
        [HideInInspector]
        public int damage;
        [HideInInspector]
        public int speed;
        [HideInInspector]
        public GameObject player;

        private float distanceFromPlayer;
        private Vector3 forwardVector;
        private bool collisionHappened;
        private Rigidbody rb;


        void Start()
        {
            forwardVector = player.transform.forward;
            collisionHappened = false;
            rb = GetComponent<Rigidbody>();
        }

        void Update()
        {
            distanceFromPlayer = Vector3.Distance(transform.position, player.transform.position);

            // Destroy bullet if it is far away from player
            if (distanceFromPlayer > 200)
            {
                Destroy(gameObject);
            }

            //transform.Translate(Vector3.forward * speed * Time.deltaTime);
            rb.MovePosition(transform.position + rb.rotation * Vector3.forward * speed * Time.deltaTime);
        }

        void OnCollisionEnter(Collision collision)
        {
            if (!collisionHappened)
            {
                // If a projectile is hitting an object, deduct the damage from their HP
                if (collision.gameObject.GetComponent<Attributes>())
                {
                    DisplayLogger.Instance.LogInfo("Hitting " + collision.gameObject.name + " for " + damage.ToString());
                    collision.gameObject.GetComponent<Attributes>().changeHealth(damage);
                }
                collisionHappened = true;
                Destroy(gameObject);
            }
        }
    }
}