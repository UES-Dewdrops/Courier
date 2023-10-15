using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace CourierMod.Content
{
    public class FlutterOn : BaseState
    {
        public static float hoverVelocity;

        public static float hoverAcceleration;

        private Transform jetOnEffect;

        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.isAuthority)
            {
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
