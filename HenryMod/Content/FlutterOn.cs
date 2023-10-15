using EntityStates;
using HenryMod;
using UnityEngine;

namespace CourierMod.Content
{
    public class FlutterOn : BaseState
    {
        public static float hoverVelocity = 20.0f;

        public static float hoverAcceleration = 20.0f;

        public override void OnEnter()
        {
            base.OnEnter();
            Log.Info("flutteron onenter");
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.isAuthority)
            {
                Log.Info("flutteron fixedupdate hover");
                float y = base.characterMotor.velocity.y;
                y = Mathf.MoveTowards(y, hoverVelocity, hoverAcceleration * Time.fixedDeltaTime);
                base.characterMotor.velocity = new Vector3(base.characterMotor.velocity.x, y, base.characterMotor.velocity.z);
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}
