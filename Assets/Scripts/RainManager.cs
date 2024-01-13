using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainManager : NoteReceiver
{
    public float _EmissionRampUpTime = 10;

    ParticleSystem[] _particleSystems;
    Coroutine _emissionRampUpCoro;
    float[] _targetEmissions;

    // Start is called before the first frame update
    void Start()
    {
        _particleSystems = GetComponentsInChildren<ParticleSystem>();
        _targetEmissions = new float[_particleSystems.Length];
    }
    public override void HandleNote(int noteNumber)
    {
        if (_emissionRampUpCoro != null)
        {
            return;
        }

        _emissionRampUpCoro = StartCoroutine(rampUpEmissionCoro());
    }
    IEnumerator rampUpEmissionCoro()
    {
        for (int i = 0; i < _particleSystems.Length; i++)
        {
            ParticleSystem p = _particleSystems[i];
            ParticleSystem.EmissionModule em = p.emission;
            _targetEmissions[i] = em.rateOverTime.constant;
            em.rateOverTime = 0;
            p.Play();
        }

        double currentTime = Time.timeAsDouble;
        double startTime = currentTime;
        double endTime = startTime + _EmissionRampUpTime;
        while (currentTime < endTime)
        {
            double x = (currentTime - startTime) / _EmissionRampUpTime;  
            for (int i = 0; i < _particleSystems.Length; i++)
            {
                ParticleSystem p = _particleSystems[i];
                ParticleSystem.EmissionModule em = p.emission;
                em.rateOverTime = Mathf.Lerp(0f, _targetEmissions[i], (float)x);
            }
            yield return new WaitForEndOfFrame();
            currentTime = Time.timeAsDouble;
        }
    }
}
