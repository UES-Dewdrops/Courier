using EntityStates;
using R2API.Utils;
using RoR2;
using RoR2.Projectile;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace HenryMod.SkillStates
{
    public class PushSkill : BaseSkillState
    {
        public static GameObject effectPrefab2 = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Bandit2/Bandit2SmokeBomb.prefab").WaitForCompletion();
        public static GameObject effectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidJailer/VoidJailerDeathBombExplosion.prefab").WaitForCompletion();
        public float damageCoefficient = 1f;
        public float blastRadius = 12f;
        public float baseDuration = 0.25f;
        public static float knockbackForce = 0.14f;

        private float duration;
        private float dmgMod;
        private float baseRadius;
        private Ray aimRay;
        public override void OnEnter()
        {
            base.OnEnter();
            
            this.aimRay = base.GetAimRay();
            this.duration = this.baseDuration;
            this.dmgMod = this.damageCoefficient;
            this.baseRadius = this.blastRadius;

            base.characterMotor.disableAirControlUntilCollision = false;

            if (base.isAuthority) 
            {
                Vector3 blastLocation = (aimRay.origin + 12f * aimRay.direction);
                new BlastAttack
                {
                    attacker = base.gameObject,
                    baseDamage = damageStat * dmgMod,
                    baseForce = 0f,
                    crit = Util.CheckRoll(base.characterBody.crit, base.characterBody.master),
                    damageType = DamageType.Stun1s,
                    falloffModel = BlastAttack.FalloffModel.None,
                    procCoefficient = 0.5f,
                    radius = this.baseRadius,
                    position = blastLocation,
                    attackerFiltering = AttackerFiltering.NeverHitSelf,
                    teamIndex = base.teamComponent.teamIndex,

                }.Fire();

                EffectData effectData = new EffectData();
                effectData.origin = blastLocation;
                effectData.scale = 15;

                EffectManager.SpawnEffect(PushSkill.effectPrefab, effectData, true);

                Util.PlaySound(Roll.dodgeSoundString, base.gameObject);

                if (NetworkServer.active) 
                {
                    Push(blastLocation);
                }
            }

        }

        private void Push(Vector3 position)
        {
            Vector3 pushForce = ((aimRay.origin + aimRay.direction) + (30 * Vector3.up));

            List<CharacterBody> affectedEnemies = new List<CharacterBody>();
            foreach (HurtBox hurtBox in new SphereSearch
            {
                origin = position,
                radius = blastRadius,
                mask = LayerIndex.entityPrecise.mask
            }.RefreshCandidates().FilterCandidatesByHurtBoxTeam(TeamMask.GetEnemyTeams(base.GetTeam())).OrderCandidatesByDistance().FilterCandidatesByDistinctHurtBoxEntities().GetHurtBoxes())
            {
                Debug.Log("\nEnemy found!!");
                CharacterBody body = hurtBox.healthComponent.body;
                if (body && !affectedEnemies.Contains(body) && !HGMath.IsVectorNaN(hurtBox.transform.position))
                {
                    affectedEnemies.Add(body);

                    Vector3 force = pushForce * 25f;

                    //Apparently I need this NaN check?
                    if (!HGMath.IsVectorNaN(force))
                    {
                        Debug.Log("\nCan you please apply the damn force");
                        Debug.Log(force);
                        DamageInfo damageInfo = new DamageInfo
                        {
                            attacker = base.gameObject,
                            inflictor = base.gameObject,
                            crit = false,
                            damage = 0f,
                            damageColorIndex = DamageColorIndex.Default,
                            damageType = DamageType.NonLethal | DamageType.Silent,
                            force = force,
                            position = hurtBox.transform.position,
                            procChainMask = default,
                            procCoefficient = 0f
                        };
                        hurtBox.healthComponent.TakeDamageForce(damageInfo, true, false);
                    }

                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if ((base.fixedAge >= this.duration && base.isAuthority)) 
            {
                this.outer.SetNextStateToMain();
                return;
            }
            
        }

        public override void OnExit()
        {
            base.OnExit();

        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
