using UnityEngine;
using System.Collections;
using Common.GameTime.Controller;
using Logic.Character;

namespace Common.Components.Effect
{
    public class ParticlePlayer : MonoBehaviour
    {
        public System.Action<GameObject> onDestroyEvent;
        private float _duration;
        public bool isLoop = false;
        public bool isUI = false;
        private float _currentTime;
        public CharacterEntity character;
        void Awake()
        {
            particle = GetComponentInChildren<ParticleSystem>();
            anims = GetComponentsInChildren<Animator>();
            //trailRenderers = GetComponentsInChildren<TrailRenderer>();
            //if (particle == null && (trailRenderers == null || trailRenderers.Length == 0))
            //    UnityEngine.Object.Destroy(this);
        }

        // Use this for initialization
        void Start()
        {
            if (particle)
                _duration = particle.duration;
            //if ((trailRenderers != null && trailRenderers.Length >= 0))
            //{
            //    trailRendererTimes = new float[trailRenderers.Length];
            //    trailRendererStartWidths = new float[trailRenderers.Length];
            //    for (int i = 0, length = trailRenderers.Length; i < length; i++)
            //    {
            //        TrailRenderer tr = trailRenderers[i];
            //        trailRendererTimes[i] = tr.time;
            //        trailRendererStartWidths[i] = tr.startWidth;
            //    }
            //}
            if (anims != null && anims.Length > 0)
            {
                for (int i = 0, length = anims.Length; i < length; i++)
                {
                    Animator anim = anims[i];
                    anim.updateMode = AnimatorUpdateMode.UnscaledTime;
                    anim.speed = speed;
                }
            }
            initTime = Time.realtimeSinceStartup;
            lastTime = initTime;
            _currentTime = lastTime;
        }

        // Update is called once per frame
        void Update()
        {
            if (Time.realtimeSinceStartup - initTime >= delay)
            {
                float realDeltaTime = Time.realtimeSinceStartup - lastTime;
                float deltaTime = realDeltaTime * speed;
                deltaTime = deltaTime % _duration;
                if (particle)
                    particle.Simulate(deltaTime, true, false); //last must be false!!
                lastTime = Time.realtimeSinceStartup;
            }
            if (!isLoop)
            {
                if (Time.realtimeSinceStartup - initTime >= duration + delay)
                {
                    particle = null;
                    if (onDestroyEvent != null)
                        onDestroyEvent(gameObject);
                    onDestroyEvent = null;
                    Object.Destroy(gameObject);
                }
            }
            if (!isUI)
            {
                if (!TimeController.instance.IgnorePause(character))
                {
                    float lastFrameTime = Time.realtimeSinceStartup - _currentTime;
                    initTime += lastFrameTime;
                    lastTime += lastFrameTime;
                    if (anims != null && anims.Length > 0)
                    {
                        for (int i = 0, length = anims.Length; i < length; i++)
                        {
                            Animator anim = anims[i];
                            anim.speed = 0;
                        }
                    }
                }
                else
                {
                    if (anims != null && anims.Length > 0)
                    {
                        for (int i = 0, length = anims.Length; i < length; i++)
                        {
                            Animator anim = anims[i];
                            anim.speed = speed;
                        }
                    }
                }
            }
            _currentTime = Time.realtimeSinceStartup;
        }

        void OnDestroy()
        {
            particle = null;
            if (onDestroyEvent != null)
                onDestroyEvent = null;
        }

        private float lastTime;
        private ParticleSystem particle;
        //private TrailRenderer[] trailRenderers;
        //private float[] trailRendererTimes;
        //private float[] trailRendererStartWidths;
        private Animator[] anims;
        public float delay;
        public float duration;
        private float initTime;
        private float _speed = 1f;
        public float speed 
        {
            set 
            {
                _speed = value;
            }
            get 
            {
                if (!Logic.Fight.Controller.FightController.instance.isWaitingCombo)
                    return Logic.Game.GameSetting.instance.speed;
                return _speed;
            }
        }
    }
}