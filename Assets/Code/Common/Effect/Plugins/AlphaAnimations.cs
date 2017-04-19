using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AlphaKeyFrame
{
    public float _time = 0;
    public float _value = 0;
}

[RequireComponent(typeof(Animation))]

public class AlphaAnimations : MonoBehaviour
{
    public AnimationCurve _curve;
    public float[] _time;
    public float[] _value;
    private AnimationClip _clip;
    [HideInInspector]
    public List<Vector2> _nodes = new List<Vector2>();
    [HideInInspector]
    public int _count;
    //[HideInInspector]
    //public string _osmPath = "";

    public TextAsset _osmFile = null;
    void Awake()
    {
        if (_nodes.Count > 0)
        {
            foreach (Vector2 frame in _nodes)
            {
                _curve.AddKey(frame.x, frame.y);
            }
        }
        else
        {
            if (_time.Length == _value.Length)
            {
                for (int i = 0; i < _time.Length; i++)
                {
                    _curve.AddKey(_time[i], _value[i]);
                }
            }
        }
        _clip = new AnimationClip();
        _clip.SetCurve("", typeof(Material), "_Color.a", _curve);
        //clip.SetCurve("", typeof(Material), "_MainTex.offset.x", new AnimationCurve(new Keyframe(0, 0, 0, 0), new Keyframe(0, 0, 0, 0)));
        Animation animation = GetComponent<Animation>();
        _clip.legacy = true;
        animation.AddClip(_clip, _clip.name);
        animation.Play(_clip.name);
    }

    public bool playAnimation(bool bPlay)
    {
        if (GetComponent<Animation>() == null)
            return false;
        if (bPlay)
        {
            GetComponent<Animation>()[_clip.name].speed = 1.0f;
            GetComponent<Animation>()[_clip.name].time = 0.0f;
            GetComponent<Animation>().Play(_clip.name);
        }
        else
        {
            //animation[_clip.name].speed = 0.0f;
            //animation.Stop(_clip.name);
        }
        return true;
    }

    public void setAnimationSpeed(float speed)
    {
        if (GetComponent<Animation>() == null)
            return;
        GetComponent<Animation>()[_clip.name].speed = speed;
    }
}
