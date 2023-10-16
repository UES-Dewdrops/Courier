using EntityStates;
using R2API.Utils;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HenryMod.SkillStates
{
    public class ShootOrb : BaseSkillState
    {
        public static float damageCoefficient = Modules.StaticValues.gunDamageCoefficient;
        public static float procCoefficient = 1f;
        public static float baseDuration = 0.6f;
        public static float force = 800f;
        public static float recoil = 3f;
        public static float range = 256f;
        public GameObject tracerEffectPrefab = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/Tracers/TracerGoldGat");
        public GameObject projectilePrefab = EntityStates.Captain.Weapon.FireTazer.projectilePrefab;


        private float duration;
        private float fireTime;
        private bool hasFired;
        private string muzzleString;

        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = ShootOrb.baseDuration / this.attackSpeedStat;
            this.fireTime = 0.2f * this.duration;
            base.characterBody.SetAimTimer(2f);
            this.muzzleString = "Muzzle";


            base.PlayAnimation("LeftArm, Override", "ShootGun", "ShootGun.playbackRate", 1.8f);

        }

        public override void OnExit()
        {
            base.OnExit();
        }

        private void Fire()
        {
            if (!this.hasFired)
            {
                this.hasFired = true;

                base.characterBody.AddSpreadBloom(1.5f);
                EffectManager.SimpleMuzzleFlash(EntityStates.Captain.Weapon.FireTazer.muzzleflashEffectPrefab, base.gameObject, this.muzzleString, false);
                Util.PlaySound("HenryShootPistol", base.gameObject);

                if (base.isAuthority)
                {
                    Ray aimRay = base.GetAimRay();
                    base.AddRecoil(-1f * ShootOrb.recoil, -2f * ShootOrb.recoil, -0.5f * ShootOrb.recoil, 0.5f * ShootOrb.recoil);

                    FireProjectileInfo fireProjectileInfo = default(FireProjectileInfo);
                    fireProjectileInfo.projectilePrefab = projectilePrefab;
                    fireProjectileInfo.position = aimRay.origin;
                    fireProjectileInfo.rotation = Util.QuaternionSafeLookRotation(aimRay.direction);
                    fireProjectileInfo.owner = base.gameObject;
                    fireProjectileInfo.damage = damageStat * damageCoefficient;
                    fireProjectileInfo.force = force;
                    fireProjectileInfo.crit = Util.CheckRoll(critStat, base.characterBody.master);
                    ProjectileManager.instance.FireProjectile(fireProjectileInfo);
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.fixedAge >= this.fireTime)
            {
                this.Fire();
            }

            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}