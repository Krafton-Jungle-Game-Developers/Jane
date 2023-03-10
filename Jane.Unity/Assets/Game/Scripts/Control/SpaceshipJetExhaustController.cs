using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipJetExhaustController : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private SpaceshipEngine engines;

    [SerializeField] float effectMultiplier = 1;
    public float EffectMultiplier
    {
        get { return effectMultiplier; }
        set { effectMultiplier = value; }
    }

    [Header("Visual Elements")]
    [SerializeField] List<MeshRenderer> exhaustGlowRenderers = new();
    List<Material> exhaustGlowMaterials = new();

    [SerializeField] string exhaustGlowShaderColorName = "_BaseColor";

    // Halo renderers
    [SerializeField] List<MeshRenderer> exhaustHaloRenderers = new();
    List<Material> exhaustHaloMaterials = new();
    [SerializeField] string exhaustHaloShaderColorName = "_BaseColor";

    // Particle systems
    [SerializeField] List<ParticleSystem> exhaustParticleSystems = new();
    ParticleSystem.MainModule[] exhaustParticleSystemMainModules;
    ParticleSystem.EmissionModule[] exhaustParticleSystemEmissionModules;
    List<Material> exhaustParticleMaterials = new();
    List<float> exhaustParticleStartSpeeds = new();

    [SerializeField] string exhaustParticleShaderColorName = "_BaseColor";


    [Header("Trail Renderers")]
    [SerializeField] List<TrailRenderer> exhaustTrailRenderers = new();
    List<Material> exhaustTrailMaterials = new();

    [SerializeField] string exhaustTrailShaderColorName = "_BaseColor";
    [SerializeField] bool disableExhaustTrailsOnAwake = false;

    [Header("Cruising")]
    [SerializeField] AnimationCurve throttleValueToEffectsCurve = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] Gradient exhaustColorGradient = new();
    [SerializeField] float maxCruisingGlowAlpha = 0.8f;
    [SerializeField] float maxCruisingHaloAlpha = 0.3f;
    [SerializeField] float maxCruisingParticleAlpha = 0.2f;
    [SerializeField] float maxCruisingParticleSpeedFactor = 1f;
    [SerializeField] float maxCruisingTrailAlpha = 0.75f;
    [SerializeField] float cruisingColorMultiplier = 3;

    [Header("Boost")]
    [SerializeField] Color boostColor = Color.white;
    [SerializeField] float boostGlowAlpha = 1f;
    [SerializeField] float boostHaloAlpha = 0.4f;
    [SerializeField] float boostParticleAlpha = 0.3f;
    [SerializeField] float boostParticleSpeedFactor = 2f;
    [SerializeField] float boostTrailAlpha = 1f;
    [SerializeField] float boostColorMultiplier = 3;

    float cruisingEffectsAmount = Mathf.Infinity, boostEffectsAmount = Mathf.Infinity;
    float targetCruisingEffectsAmount = 0, targetBoostEffectsAmount = 0;

    void Reset()
    {
        exhaustColorGradient.colorKeys = new GradientColorKey[] { new (new Color(1f, 0.5f, 0f, 1f), 0) };
    }

    void Awake()
    {
        foreach (MeshRenderer exhaustGlowRenderer in exhaustGlowRenderers)
        {
            foreach (Material mat in exhaustGlowRenderer.materials)
            {
                exhaustGlowMaterials.Add(mat);
            }
        }

        foreach (MeshRenderer exhaustHaloRenderer in exhaustHaloRenderers)
        {
            foreach (Material mat in exhaustHaloRenderer.materials)
            {
                exhaustHaloMaterials.Add(mat);
            }
        }

        exhaustParticleSystemMainModules = new ParticleSystem.MainModule[exhaustParticleSystems.Count];
        exhaustParticleSystemEmissionModules = new ParticleSystem.EmissionModule[exhaustParticleSystems.Count];
        for (int i = 0; i < exhaustParticleSystems.Count; ++i)
        {
            ParticleSystemRenderer particleSystemRenderer = exhaustParticleSystems[i].GetComponent<ParticleSystemRenderer>();
            foreach (Material mat in particleSystemRenderer.materials)
            {
                exhaustParticleMaterials.Add(mat);
            }

            // Store the particle speed for dynamic control via throttle (0-1) value
            exhaustParticleSystemMainModules[i] = exhaustParticleSystems[i].main;
            exhaustParticleSystemEmissionModules[i] = exhaustParticleSystems[i].emission;
            exhaustParticleStartSpeeds.Add(exhaustParticleSystemMainModules[i].startSpeed.constant);
        }

        foreach (TrailRenderer exhaustTrailRenderer in exhaustTrailRenderers)
        {
            foreach (Material mat in exhaustTrailRenderer.materials)
            {
                exhaustTrailMaterials.Add(mat);
            }
        }

        if (disableExhaustTrailsOnAwake)
        {
            SetExhaustTrailsEnabled(false);
        }
    }

    public void ResetExhaust()
    {
        for (int i = 0; i < exhaustTrailRenderers.Count; ++i)
        {
            exhaustTrailRenderers[i].Clear();
        }
    }

    public void SetExhaustTrailsEnabled(bool setEnabled)
    {
        for (int i = 0; i < exhaustTrailRenderers.Count; ++i)
        {
            exhaustTrailRenderers[i].enabled = setEnabled;
        }
    }

    void CalculateEffectsParameters()
    {
        if (engines == null) return;

        // If engines assigned, use it to drive the effects
        if (!engines.EnginesActivated)
        {
            targetCruisingEffectsAmount = 0;
            targetBoostEffectsAmount = 0;
        }
        else
        {
            targetCruisingEffectsAmount = effectMultiplier * throttleValueToEffectsCurve.Evaluate(engines.MovementInputs.z);
            targetBoostEffectsAmount = effectMultiplier * engines.BoostInputs.z;
        }
    }

    public void UpdateEffects()
    {

        for (int i = 0; i < exhaustParticleSystemEmissionModules.Length; ++i)
        {
            exhaustParticleSystemEmissionModules[i].enabled = !Mathf.Approximately(effectMultiplier * (cruisingEffectsAmount + boostEffectsAmount), 0);
        }

        for (int i = 0; i < exhaustGlowRenderers.Count; ++i)
        {
            exhaustGlowRenderers[i].enabled = !Mathf.Approximately(effectMultiplier * (cruisingEffectsAmount + boostEffectsAmount), 0);
        }

        for (int i = 0; i < exhaustHaloRenderers.Count; ++i)
        {
            exhaustHaloRenderers[i].enabled = !Mathf.Approximately(effectMultiplier * (cruisingEffectsAmount + boostEffectsAmount), 0);
        }

        if (Mathf.Approximately(cruisingEffectsAmount, targetCruisingEffectsAmount) && Mathf.Approximately(boostEffectsAmount, targetBoostEffectsAmount)) return;

        cruisingEffectsAmount = targetCruisingEffectsAmount;
        boostEffectsAmount = targetBoostEffectsAmount;


        Color c = (1 - boostEffectsAmount) * cruisingEffectsAmount * exhaustColorGradient.Evaluate(cruisingEffectsAmount) * cruisingColorMultiplier +
                    boostEffectsAmount * boostColor * boostColorMultiplier;


        float particleAlpha = (1 - boostEffectsAmount) * cruisingEffectsAmount * maxCruisingParticleAlpha +
                            boostEffectsAmount * boostParticleAlpha;

        float particleSpeedFactor = (1 - boostEffectsAmount) * cruisingEffectsAmount * maxCruisingParticleSpeedFactor +
                                boostEffectsAmount * boostParticleSpeedFactor;

        float haloAlpha = (1 - boostEffectsAmount) * cruisingEffectsAmount * maxCruisingHaloAlpha +
                        boostEffectsAmount * boostHaloAlpha;

        float glowAlpha = (1 - boostEffectsAmount) * cruisingEffectsAmount * maxCruisingGlowAlpha +
                        boostEffectsAmount * boostGlowAlpha;

        float trailAlpha = (1 - boostEffectsAmount) * cruisingEffectsAmount * maxCruisingTrailAlpha +
                        boostEffectsAmount * boostTrailAlpha;

        // Update halo materials
        for (int i = 0; i < exhaustHaloMaterials.Count; ++i)
        {
            c.a = haloAlpha;
            exhaustHaloMaterials[i].SetColor(exhaustHaloShaderColorName, c);
        }

        // Update glow materials
        for (int i = 0; i < exhaustGlowMaterials.Count; ++i)
        {
            c.a = glowAlpha;
            exhaustGlowMaterials[i].SetColor(exhaustGlowShaderColorName, c);
        }

        // Update particle effects
        for (int i = 0; i < exhaustParticleMaterials.Count; ++i)
        {
            c.a = particleAlpha;
            exhaustParticleMaterials[i].SetColor(exhaustParticleShaderColorName, c);
        }

        // Update particle speed
        for (int i = 0; i < exhaustParticleSystemMainModules.Length; ++i)
        {
            exhaustParticleSystemMainModules[i].startSpeed = particleSpeedFactor * exhaustParticleStartSpeeds[i];
        }

        // Update trail renderer materials
        for (int i = 0; i < exhaustTrailMaterials.Count; ++i)
        {
            c.a = trailAlpha;
            exhaustTrailMaterials[i].SetColor(exhaustTrailShaderColorName, c);
        }
    }

    // Called every frame
    void LateUpdate()
    {
        CalculateEffectsParameters();
        UpdateEffects();
    }
}

