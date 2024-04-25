using EntityStates;
using R2API.Utils;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HenryMod.SkillStates
{
    public class PushSkill : BaseSkillState
    {
        public static GameObject hitPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/SlowOnHit/SlowDownTime.prefab").WaitForCompletion();
        public static GameObject effectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidJailer/VoidJailerDeathBombExplosion.prefab").WaitForCompletion();
        public float damageCoefficient = 20f;
        private float blastRadius = 15f;
        public float baseDuration = 0.25f;

        private float duration;
        private float dmgMod;
        private float baseRadius;
        public override void OnEnter()
        {
            base.OnEnter();
            
            Ray aimRay = base.GetAimRay();
            this.duration = this.baseDuration;
            this.dmgMod = this.damageCoefficient;
            this.baseRadius = this.blastRadius;

            base.characterMotor.disableAirControlUntilCollision = false;


            if (base.isAuthority) 
            {
                Vector3 blastLocation = aimRay.origin + 20 * aimRay.direction;
                new BlastAttack
                {
                    attacker = base.gameObject,
                    baseDamage = damageStat * dmgMod,
                    baseForce = 0f,
                    bonusForce = Vector3.down,
                    crit = Util.CheckRoll(base.characterBody.crit, base.characterBody.master),
                    damageType = DamageType.Stun1s,
                    falloffModel = BlastAttack.FalloffModel.None,
                    procCoefficient = 0.5f,
                    radius = this.baseRadius,
                    position = blastLocation,
                    attackerFiltering = AttackerFiltering.NeverHitSelf,
                    impactEffect = EffectCatalog.FindEffectIndexFromPrefab(hitPrefab),
                    teamIndex = base.teamComponent.teamIndex,

                }.Fire();

                EffectData effectData = new EffectData();
                effectData.origin = blastLocation;
                effectData.scale = 15;

                EffectManager.SpawnEffect(PushSkill.effectPrefab, effectData, true);

                Util.PlaySound(Roll.dodgeSoundString, base.gameObject);


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
