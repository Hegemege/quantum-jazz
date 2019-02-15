using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PoolableParticleSystem : MonoBehaviour, IPoolable
{
    private ParticleSystem _ps;

    void Awake()
    {
        _ps = GetComponentInChildren<ParticleSystem>();
    }

    public void ResetState()
    {
        gameObject.SetActive(true);
    }

    public void ReturnToPool()
    {

    }

    void Update()
    {
        if (!_ps.IsAlive())
        {
            _ps.Clear();
            gameObject.SetActive(false);
            return;
        }

        if (!_ps.isPlaying)
        {
            _ps.Play();
        }
    }
}