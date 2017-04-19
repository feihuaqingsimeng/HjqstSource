using UnityEngine;
using System.Collections;
namespace Common.Animators
{
    public static class AnimatorUtil
    {
        #region  animator field
        public const string BASE_LAYER = "Base Layer.";
        //public const string IDLE = "Idle";
        public const string DEAD = "Dead";
        public const string RUN = "Run";
        public const string ENDRUN = "EndRun";
        public const string VICOTRY = "Victory";
        //public const string GETHIT_1 = "GetHit_1";
        //public const string GETHIT_2 = "GetHit_2";
        //public const string HIT_1 = "Hit_1";
        //public const string HIT_2 = "Hit_2";
        //public const string FLOATSTART = "FloatStart";
        //public const string FLOATGETHIT = "FloatGetHit";
        //public const string FLOATDOWN = "FloatDown";
        public const string FLOATING = "Floating";
        //public const string SKILL_1 = "Skill_1";
        //public const string SKILL_2 = "Skill_2";
        public const string TUMBLE = "Tumble";
        //public const string GETUP = "Getup";
        //public const string TUMBLESTART = "TumbleStart";
        //public const string TUMBLEGETHIT = "TumbleGetHit";
        //public const string BOOT = "Boot";
        //public const string STUN = "STUN";
        public const string SWIMMY = "Swimmy";
        //public const string TRANSFORM = "Transform";
        //public const string VICTORY_02 = "Victory_02";

        #region state id
        public static int IDLE_ID;
        public static int DEAD_ID;
        public static int RUN_ID;
        public static int ENDRUN_ID;
        public static int VICOTRY_ID;
        public static int GETHIT_1_ID;
        public static int GETHIT_2_ID;
        public static int HIT_1_ID;
        public static int HIT_2_ID;
        public static int FLOATSTART_ID;
        public static int FLOATGETHIT_ID;
        public static int FLOATDOWN_ID;
        public static int FLOATING_ID;
        public static int SKILL_1_ID;
        public static int SKILL_2_ID;
        public static int TUMBLE_ID;
        public static int GETUP_ID;
        public static int TUMBLESTART_ID;
        public static int TUMBLEGETHIT_ID;
        public static int BOOT_ID;
        public static int STUN_ID;
        public static int SWIMMY_ID;
        public static int TRANSFORM_ID;
        public static int VICTORY_02_ID;
        #endregion
        #endregion

        static AnimatorUtil()
        {
            IDLE_ID = Animator.StringToHash(BASE_LAYER + "Idle");
            DEAD_ID = Animator.StringToHash(BASE_LAYER + "Dead");
            RUN_ID = Animator.StringToHash(BASE_LAYER + "Run");
            ENDRUN_ID = Animator.StringToHash(BASE_LAYER + "EndRun");
            VICOTRY_ID = Animator.StringToHash(BASE_LAYER + "Victory");
            GETHIT_1_ID = Animator.StringToHash(BASE_LAYER + "GetHit_1");
            GETHIT_2_ID = Animator.StringToHash(BASE_LAYER + "GetHit_2");
            HIT_1_ID = Animator.StringToHash(BASE_LAYER + "Hit_1");
            HIT_2_ID = Animator.StringToHash(BASE_LAYER + "Hit_2");
            FLOATSTART_ID = Animator.StringToHash(BASE_LAYER + "FloatStart");
            FLOATGETHIT_ID = Animator.StringToHash(BASE_LAYER + "FloatGetHit");
            FLOATDOWN_ID = Animator.StringToHash(BASE_LAYER + "FloatDown");
            FLOATING_ID = Animator.StringToHash(BASE_LAYER + "Floating");
            SKILL_1_ID = Animator.StringToHash(BASE_LAYER + "Skill_1");
            SKILL_2_ID = Animator.StringToHash(BASE_LAYER + "Skill_2");
            TUMBLE_ID = Animator.StringToHash(BASE_LAYER + "Tumble");
            GETUP_ID = Animator.StringToHash(BASE_LAYER + "Getup");
            TUMBLESTART_ID = Animator.StringToHash(BASE_LAYER + "TumbleStart");
            TUMBLEGETHIT_ID = Animator.StringToHash(BASE_LAYER + "TumbleGetHit");
            BOOT_ID = Animator.StringToHash(BASE_LAYER + "Boot");
            STUN_ID = Animator.StringToHash(BASE_LAYER + "STUN");
            SWIMMY_ID = Animator.StringToHash(BASE_LAYER + "Swimmy");
            TRANSFORM_ID = Animator.StringToHash(BASE_LAYER + "Transform");
            VICTORY_02_ID = Animator.StringToHash(BASE_LAYER + "Victory_02");
        }

        public static bool isTargetState(Animator anim, int stateNameHash, int layerIndex = 0)
        {
            if (anim)
                return anim.GetCurrentAnimatorStateInfo(layerIndex).fullPathHash == stateNameHash;
            return false;
        }

        public static void Play(Animator anim, int stateNameHash, int layer, float normalizedTime)
        {
            if (anim)
                anim.Play(stateNameHash, layer, normalizedTime);
        }

        public static void CrossFade(Animator anim, int stateNameHash, float transitionDuration)
        {
            if (anim)
                anim.CrossFade(stateNameHash, transitionDuration);
        }

        public static void SetTrigger(Animator anim, string argName)
        {
            if (anim)
                anim.SetTrigger(argName);
        }

        public static void SetBool(Animator anim, string argName, bool value)
        {
            if (anim)
                anim.SetBool(argName, value);
        }

        public static bool GetBool(Animator anim, string argName)
        {
            if (anim)
                return anim.GetBool(argName);
            return false;
        }
    }
}