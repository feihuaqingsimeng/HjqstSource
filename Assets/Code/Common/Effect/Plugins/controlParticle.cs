using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class controlParticle : MonoBehaviour
{

    public GameObject _particleObject;
    public float _startTime;
    public float _endTime;
    private float _time;
    private bool _enble;
    private int _state;
    private GameObject _myParticleObject;
    private ParticleSystem[] _particles;
    void Awake()
    {
        if (_particleObject != null)
        {
            _myParticleObject = Instantiate(_particleObject, transform.position, transform.rotation) as GameObject;
            _particles = _myParticleObject.GetComponentsInChildren<ParticleSystem>();
            for (int i = 0; i < _particles.Length; i++)
            {
                _particles[i].Stop();
            }

            _myParticleObject.SetActive(false);
            _myParticleObject.transform.parent = transform;
            _enble = true;
        }
        else
        {
            _enble = false;
        }
        _state = 0;
    }

    void Update()
    {
        if (_enble && _myParticleObject != null)
        {
            _time += Time.deltaTime;
            if (_time >= _startTime && _time < _endTime)
            {
                if (_state == 0)
                {
                    _state = 1;
                    _myParticleObject.SetActive(true);
                    for (int i = 0; i < _particles.Length; i++)
                    {
                        _particles[i].Play();
                    }

                }
            }
            else if (_time >= _endTime)
            {
                _enble = false;
                _time = 0.0f;
                for (int i = 0; i < _particles.Length; i++)
                {
                    _particles[i].Stop();
                }
                _myParticleObject.SetActive(false);
            }
        }
    }


    public void paly()
    {
        if (_particleObject != null)
        {
            _enble = true;
            _state = 0;
            _time = 0.0f;
            for (int i = 0; i < _particles.Length; i++)
            {
                _particles[i].Stop();
                _particles[i].time = 0.0f;
                _particles[i].Simulate(0.0f);
                //_particles[i].playOnAwake;
            }
            _myParticleObject.SetActive(false);
        }
    }

    public void stop()
    {
        if (_particleObject != null)
        {
            _enble = false;
            _state = 0;
            _time = 0.0f;
            for (int i = 0; i < _particles.Length; i++)
            {
                _particles[i].Stop();
            }
            _myParticleObject.SetActive(false);
        }
    }

    public void pause()
    {


    }

}
