using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AnsgarsAssets;
using UnityEngine.Animations;

namespace AnsgarsAssets
{
    public class GrapplingGun : MonoBehaviour
    {
        [Header("General")]
        public float projectileVelocity;

        public Transform muzzle;
        [Tooltip("The specified rigidbody and the source of the rope will be connected by a fixed joint.")]
        public Rigidbody rigidBodyToConnectChainLinkSourceTo;

        [Header("Prefabs")]
        public GameObject ropePrefab;
        public GrapplingGunProjectile projectilePrefab;
        public GrapplingGunChainLinkSource chainLinkSourcePrefab;

        [Header("Rope attaching")]
        public LayerMask raycastObstacles;
        public LayerMask possibleObjectsToAttachRopeTo;
        public float maximumDistanceToSurfaceToAttachRopeTo;

        [Header("Uptaking and expelling")]
        public float maximumRopeExpellingSpeed;
        public float expellingRopeForce;
        [Space(10)]
        public float maximumRopeUptakingSpeed;
        public float uptakingRopeForce;

        [Header("Optional bookkeeping")]
        public Transform ropeParent;
        public Transform chainLinkSourceParent;
        public Transform projectileParent;

        [Header("Events")]
        public GameEvent grappleDisconnected;

        GameObject rope;
        GrapplingGunProjectile projectile;
        GrapplingGunChainLinkSource chainLinkSource;

        RopeGunState gunState = RopeGunState.unloaded;

        enum RopeGunState
        {
            unloaded, // The initial state of the gun. There is no projectile, no chainLinkSource, and no rope.
            loaded, // The gun is ready to fire.
            shot, // The projectile is shot out and flying in the air.
            connected // The projectile has hit something.
        }

        private void OnEnable()
        {
            EnableInstantiatedObjectsIfExistent();

            SwitchToLoadedState();
        }

        private void OnDisable()
        {
            DisableInstantiatedObjectsIfExistent();
        }

        private void OnDestroy()
        {
            DestroyInstantiatedObjectsIfExistent();
        }

        void EnableInstantiatedObjectsIfExistent()
        {
            if (rope)
                rope.SetActive(true);

            if (projectile)
                projectile.gameObject.SetActive(true);

            if (chainLinkSource)
                chainLinkSource.gameObject.SetActive(true);
        }

        void DisableInstantiatedObjectsIfExistent()
        {
            if (rope)
                rope.SetActive(false);

            if (projectile)
                projectile.gameObject.SetActive(false);

            if (chainLinkSource)
                chainLinkSource.gameObject.SetActive(false);
        }

        void DestroyInstantiatedObjectsIfExistent()
        {
            if (rope)
                Destroy(rope);

            if (projectile)
                Destroy(projectile.gameObject);

            if (chainLinkSource)
                Destroy(chainLinkSource.gameObject);
        }

        // --- Buttons of the actual ropeGun that the player can press ---

        public void StartPressingTrigger()
        {
            if (gunState == RopeGunState.loaded)
                SwitchToShotState();
        }

        public void StopPressingTrigger()
        {

        }

        public void PressReloadButton()
        {
            if (gunState == RopeGunState.shot
                || gunState == RopeGunState.connected)
                SwitchToLoadedState();
        }

        public void PressAttachButton()
        {
            if (gunState == RopeGunState.connected)
            {
                RaycastHit hitInfo;
                if (Physics.Raycast(muzzle.position, muzzle.forward, out hitInfo, maximumDistanceToSurfaceToAttachRopeTo, raycastObstacles))
                {
                    GameObject hitObject = hitInfo.collider.gameObject;

                    if (Utility.LayerMaskContainsLayer(possibleObjectsToAttachRopeTo, hitObject.layer))
                    {
                        AttachRopeToObjectAtPositionWithNormal(hitObject, hitInfo.point, hitInfo.normal);

                        SwitchToUnloadedState();
                    }
                }
            }
        }

        public void StartPressingExpellRopeButton()
        {
            if (gunState == RopeGunState.connected)
            {
                chainLinkSource.GetChainLinkSource().maximumExpellSpeed = maximumRopeExpellingSpeed;
                chainLinkSource.GetChainLinkSource().maximumTakeUpSpeed = 0;

                chainLinkSource.GetChainLinkSource().pushOutForceAmount = expellingRopeForce;
            }
        }

        public void StopPressingExpellRopeButton()
        {
            if (gunState == RopeGunState.connected)
            {
                chainLinkSource.GetChainLinkSource().LockRopeLength();
                chainLinkSource.GetChainLinkSource().pushOutForceAmount = 0;
            }
        }

        public void StartPressingTakeUpRopeButton()
        {
            if (gunState == RopeGunState.connected)
            {
                chainLinkSource.GetChainLinkSource().maximumExpellSpeed = 0;
                chainLinkSource.GetChainLinkSource().maximumTakeUpSpeed = maximumRopeUptakingSpeed;

                chainLinkSource.GetChainLinkSource().pushOutForceAmount = -uptakingRopeForce;
            }
        }

        public void StopPressingTakeUpRopeButton()
        {
            if (gunState == RopeGunState.connected)
            {
                chainLinkSource.GetChainLinkSource().LockRopeLength();
                chainLinkSource.GetChainLinkSource().pushOutForceAmount = 0;
            }
        }

        // --- State transitions ---

        void SwitchToUnloadedState()
        {
            rope = null;
            projectile = null;
            chainLinkSource = null;

            gunState = RopeGunState.unloaded;

            // Automatic reloading
            SwitchToLoadedState();
        }

        void SwitchToLoadedState()
        {
            if (gunState == RopeGunState.unloaded)
            {
                InstantiateAndConfigureRopeIfNotExistent();
                InstantiateAndConfigureProjectileIfNotExistent();
                InstantiateAndConfigureChainLinkSourceIfNotExistent();
            }
            else if (gunState == RopeGunState.shot)
            {
                chainLinkSource.GetChainLinkSource().DestroyChainAndResetHookPosition();
            }
            else if (gunState == RopeGunState.connected)
            {
                chainLinkSource.GetChainLinkSource().DestroyChainAndResetHookPosition();

                grappleDisconnected.Raise();
            }

            chainLinkSource.GetParentConstraint().constraintActive = true;
            projectile.GetParentConstraint().constraintActive = true;

            chainLinkSource.GetChainLinkSource().GetRigidbody().isKinematic = true;
            projectile.GetRigidbody().isKinematic = true;

            chainLinkSource.GetDeactivatableFixedJoint().Deactivate();

            chainLinkSource.enabled = false;
            projectile.GetSticky().DisableStickiness();

            gunState = RopeGunState.loaded;
        }

        void SwitchToShotState()
        {
            chainLinkSource.GetParentConstraint().constraintActive = false;
            projectile.GetParentConstraint().constraintActive = false;

            chainLinkSource.GetChainLinkSource().GetRigidbody().isKinematic = false;
            projectile.GetRigidbody().isKinematic = false;

            chainLinkSource.GetDeactivatableFixedJoint().Activate();
            chainLinkSource.GetDeactivatableFixedJoint().GetJoint().connectedBody = rigidBodyToConnectChainLinkSourceTo;

            chainLinkSource.enabled = true;

            chainLinkSource.GetChainLinkSource().maximumTakeUpSpeed = 0;
            chainLinkSource.GetChainLinkSource().maximumExpellSpeed = Mathf.Infinity;

            projectile.GetSticky().EnableStickiness();

            ShootProjectile();

            gunState = RopeGunState.shot;
        }

        void SwitchToConnectedState()
        {
            chainLinkSource.GetChainLinkSource().LockRopeLength();

            gunState = RopeGunState.connected;
        }

        // --- Other methods ---

        void InstantiateAndConfigureRopeIfNotExistent()
        {
            if (!rope)
            {
                rope = Instantiate(ropePrefab, ropeParent);

                if (chainLinkSource)
                    chainLinkSource.GetChainLinkSource().chainLinkParent = rope.transform;
            }
        }

        void InstantiateAndConfigureProjectileIfNotExistent()
        {
            if (!projectile)
            {
                projectile = Instantiate(projectilePrefab, muzzle.position, muzzle.rotation, projectileParent);
                projectile.GetSticky().DisableStickiness();

                ConstraintSource source = new ConstraintSource();
                source.sourceTransform = muzzle;
                source.weight = 1;

                projectile.GetParentConstraint().AddSource(source);

                if (chainLinkSource)
                    chainLinkSource.GetChainLinkSource().SetHookToConnectChainLinkTo(projectile.GetComponent<ChainLinkHook>());
            }
        }

        void InstantiateAndConfigureChainLinkSourceIfNotExistent()
        {
            if (!chainLinkSource)
            {
                chainLinkSource = Instantiate(chainLinkSourcePrefab, muzzle.position, muzzle.rotation, chainLinkSourceParent);

                ConstraintSource source = new ConstraintSource();
                source.sourceTransform = muzzle;
                source.weight = 1;

                chainLinkSource.GetParentConstraint().AddSource(source);

                if (rope)
                    chainLinkSource.GetChainLinkSource().chainLinkParent = rope.transform;

                if (projectile)
                    chainLinkSource.GetChainLinkSource().SetHookToConnectChainLinkTo(projectile.GetComponent<ChainLinkHook>());
            }
        }

        private void AttachRopeToObjectAtPositionWithNormal(GameObject gameObject, Vector3 position, Vector3 direction)
        {
            chainLinkSource.transform.position = position;

            // TESTS
            chainLinkSource.GetComponent<Rigidbody>().MovePosition(position);

            //chainLinkSource.GetComponent<Rigidbody>().velocity = Vector3.zero;
            //chainLinkSource.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

            chainLinkSource.GetDeactivatableFixedJoint().Activate();

            Rigidbody rigidbodyToAttachTo = gameObject.GetComponent<Rigidbody>();

            chainLinkSource.GetDeactivatableFixedJoint().GetJoint().connectedBody = rigidbodyToAttachTo;

            // TODO: rotate it towards the diraction
        }

        void ShootProjectile()
        {
            projectile.GetRigidbody().angularVelocity = Vector3.zero;
            projectile.GetRigidbody().velocity = Vector3.zero;
            projectile.GetRigidbody().AddForce(muzzle.forward * projectileVelocity, ForceMode.VelocityChange);
        }

        public void OnGrappleConnected()
        {
            if (gunState == RopeGunState.shot)
                SwitchToConnectedState();
        }
    }
}