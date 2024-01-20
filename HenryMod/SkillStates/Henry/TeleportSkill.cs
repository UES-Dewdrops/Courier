using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using EntityStates;
using UnityEngine.AddressableAssets;

namespace HenryMod.SkillStates
{
    public class TeleportSkill : BaseSkillState
    {
        //Teleport
        public static float baseDuration = 0.25f;
        public static float smallHopVelocity = 24f;
        public static float teleportYBonus = 1.35f;
        public static float teleportInFrontDistance = 4f;

        private float duration;
        private bool fail;
        private Vector3 teleportTarget;
        public HurtBox target;
        private Vector3 teleportStartPosition;

        private Vector3 ToTeleportTarget;
        private Vector3 lastKnownTargetPosition;

        // kaboom
        public static float baseRadius = 6f;
        public static float baseForce = 100f;
        public static float dmgMod = 30f;
        

        public static GameObject impactEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Bandit2/Bandit2SmokeBomb.prefab").WaitForCompletion();

        private Vector3 hopVector;
        public override void OnEnter()
        {
            base.OnEnter();

            this.duration = TeleportSkill.baseDuration;
            this.fail = false;

            Util.PlaySound(Roll.dodgeSoundString, base.gameObject);

            TeleportTracker tracker = base.GetComponent<TeleportTracker>();
            this.target = tracker.GetTrackingTarget();
            if (!this.target)
            {
                this.outer.SetNextStateToMain();
                this.fail = true;
                return;
            }

            //sound            
            base.StartAimMode();

            this.teleportStartPosition = base.transform.position;
            this.teleportTarget = this.target.transform.position;
            this.lastKnownTargetPosition = this.teleportTarget;

            base.characterDirection.forward = this.teleportTarget - base.transform.position;
            base.characterMotor.velocity = Vector3.zero;


            if (NetworkServer.active)
            {
                base.characterBody.AddBuff(RoR2Content.Buffs.HiddenInvincibility);
            }
        }

        private void UpdateTarget()
        {
            if (this.target)
            {
                this.lastKnownTargetPosition = this.target.transform.position;
            }
            Vector3 between = this.lastKnownTargetPosition - this.teleportStartPosition;
            float distance = between.magnitude;
            Vector3 direction = between.normalized;
            this.teleportTarget = direction * (distance - TeleportSkill.teleportInFrontDistance) + base.transform.position;
            if (Physics.Raycast(this.lastKnownTargetPosition, direction, out RaycastHit hit, TeleportSkill.teleportInFrontDistance, LayerIndex.world.mask))
            {
                this.teleportTarget = hit.point;
            }
            this.teleportTarget.y = Mathf.Max(this.lastKnownTargetPosition.y + teleportYBonus, teleportTarget.y);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            this.UpdateTarget();
            if (this.fail == false)
            {
                base.characterMotor.velocity = Vector3.zero;
            }

            if (base.isAuthority && base.fixedAge >= this.duration)
            {
                TeleportHelper.TeleportBody(base.characterBody, this.teleportTarget);
                base.characterDirection.forward = this.ToTeleportTarget != Vector3.zero ? this.ToTeleportTarget : this.teleportStartPosition - base.transform.position;

                this.outer.SetNextStateToMain();
                this.fail = false;
                return;
            }            
        }

        public override void OnExit()
        {
            base.OnExit();
            if (this.fail == false)
            {
                var result = new BlastAttack
                {
                    attacker = base.gameObject,
                    baseDamage = damageStat * dmgMod,
                    baseForce = baseForce,
                    bonusForce = Vector3.down,
                    crit = false,
                    damageType = DamageType.Stun1s,
                    falloffModel = BlastAttack.FalloffModel.None,
                    procCoefficient = 0.5f,
                    radius = baseRadius,
                    position = base.characterBody.footPosition,
                    attackerFiltering = AttackerFiltering.NeverHitSelf,
                    impactEffect = EffectCatalog.FindEffectIndexFromPrefab(impactEffect),
                    teamIndex = base.teamComponent.teamIndex,

                }.Fire();

            }
            
            base.SmallHop(base.characterMotor, TeleportSkill.smallHopVelocity);
            var aimDirection = GetAimRay().direction;
            characterMotor?.ApplyForce((2000f * this.characterDirection.forward), false, false);

            if (NetworkServer.active)
            {
                base.characterBody.RemoveBuff(RoR2Content.Buffs.HiddenInvincibility);
            }
        }
    }
}


        