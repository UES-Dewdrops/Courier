using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace CourierMod.Content
{
    public class CourierMainState : GenericCharacterMain
    {
        private EntityStateMachine slideStateMachine;

        public override void OnEnter()
        {
            base.OnEnter();
            slideStateMachine = EntityStateMachine.FindByCustomName(base.gameObject, "Slide");
        }

        public override void ProcessJump()
        {
            base.ProcessJump();
            if (hasCharacterMotor && hasInputBank && base.isAuthority)
            {
                bool num = base.inputBank.jump.down && base.characterMotor.velocity.y < 0f && !base.characterMotor.isGrounded;
                bool flag = slideStateMachine.state.GetType() == typeof(FlutterOn);
                if (num && !flag)
                {
                    slideStateMachine.SetNextState(new FlutterOn());
                }
                if (!num && flag)
                {
                    slideStateMachine.SetNextState(new Idle());
                }
            }
        }

        public override void OnExit()
        {
            if (base.isAuthority && slideStateMachine)
            {
                slideStateMachine.SetNextState(new Idle());
            }
            base.OnExit();
        }
    }
}
