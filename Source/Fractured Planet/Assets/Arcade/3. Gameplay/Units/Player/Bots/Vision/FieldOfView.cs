// Copyright (c) Peter Vrenken. All rights reserved. See the license on https://github.com/vrenken/EtAlii.RemoteMesh

namespace EtAlii.FracturedPlanet
{

    using System.Collections;
    using UnityEngine;

    public class FieldOfView : MonoBehaviour
    {
        public float radius;

        [Range(0, 360)] public float angle;

        public GameObject playerRef;

        public LayerMask targetMask;
        public LayerMask obstructionMask;

        public bool canSeePlayer;

        private void Start()
        {
            //playerRef = GameObject.FindGameObjectWithTag("Player");
            StartCoroutine(FOVRoutine());
        }

        private IEnumerator FOVRoutine()
        {
            var wait = new WaitForSeconds(0.2f);

            while (true)
            {
                yield return wait;
                FieldOfViewCheck();
            }
        }

        private void FieldOfViewCheck()
        {
            var rangeChecks = Physics.OverlapSphere(playerRef.transform.position, radius, targetMask);

            if (rangeChecks.Length != 0)
            {
                var playerPosition = playerRef.transform.position;
                var target = rangeChecks[0].transform;
                var directionToTarget = (target.position - playerPosition).normalized;

                if (Vector3.Angle(playerRef.transform.forward, directionToTarget) < angle / 2)
                {
                    var distanceToTarget = Vector3.Distance(playerPosition, target.position);

                    canSeePlayer = !Physics.Raycast(playerPosition, directionToTarget, distanceToTarget, obstructionMask);
                }
                else
                    canSeePlayer = false;
            }
            else if (canSeePlayer)
                canSeePlayer = false;
        }
    }
}
