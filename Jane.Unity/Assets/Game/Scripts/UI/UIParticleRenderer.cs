using UnityEngine;
using UnityEngine.UI;

namespace Runner.UI
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(CanvasRenderer))]
    [RequireComponent(typeof(ParticleSystem))]
    public class UIParticleRenderer : MaskableGraphic
    {
        [Tooltip("Having this enabled run the system in LateUpdate rather than in Update making it faster but less precise (more clunky)")]
        public bool fixedTime = true;

        [SerializeField] private ParticleSystem particle;
        [SerializeField] private ParticleSystemRenderer particleRenderer;
        private ParticleSystem.MainModule mainModule;
        private ParticleSystem.Particle[] particles;

        private UIVertex[] _quad = new UIVertex[4];
        private Vector4 imageUV = Vector4.zero;
        private ParticleSystem.TextureSheetAnimationModule textureSheetAnimation;
        private int textureSheetAnimationFrames;
        private Vector2 textureSheetAnimationFrameSize;

        private Material currentMaterial;
        private Texture currentTexture;
        public override Texture mainTexture => currentTexture;

        protected override void Awake()
        {
            base.Awake();

            if (!Initialize()) { enabled = false; }
        }

        private void Update()
        {
            if (!fixedTime && Application.isPlaying)
            {
                particle.Simulate(Time.unscaledDeltaTime, false, false, true);
                SetAllDirty();

                if ((currentMaterial != null && currentTexture != currentMaterial.mainTexture) 
                 || (material != null && currentMaterial != null && material.shader != currentMaterial.shader))
                {
                    particle = null;
                    Initialize();
                }
            }
        }

        private void LateUpdate()
        {
            if (!Application.isPlaying) { SetAllDirty(); }
            else
            {
                if (fixedTime)
                {
                    particle.Simulate(Time.unscaledDeltaTime, false, false, true);
                    SetAllDirty();

                    if ((currentMaterial != null && currentTexture != currentMaterial.mainTexture) ||
                        (material != null && currentMaterial != null && material.shader != currentMaterial.shader))
                    {
                        particle = null;
                        Initialize();
                    }
                }
            }

            if (material == currentMaterial) { return; }

            particle = null;
            Initialize();
        }

        protected bool Initialize()
        {
            if (particle == null)
            {
                if (!TryGetComponent<ParticleSystem>(out particle)) { return false; }
                
                mainModule = particle.main;
                if (particle.main.maxParticles > 14000) { mainModule.maxParticles = 14000; }

                particleRenderer = particle.GetComponent<ParticleSystemRenderer>();
                if (particleRenderer is not null) { particleRenderer.enabled = false; }

                Shader foundShader = Shader.Find("Legacy Shaders/Particles/Additive");
                Material pMaterial = new(foundShader);
                if (material == null) { material = pMaterial; }

                currentMaterial = material;
                if (currentMaterial && currentMaterial.HasProperty("_MainTex"))
                {
                    currentTexture = currentMaterial.mainTexture;
                    if (currentTexture == null) { currentTexture = Texture2D.whiteTexture; }
                }
                material = currentMaterial;

                mainModule.scalingMode = ParticleSystemScalingMode.Hierarchy;
                particles = null;
            }

            if (particles == null) { particles = new ParticleSystem.Particle[particle.main.maxParticles]; }

            imageUV = new Vector4(0, 0, 1, 1);

            textureSheetAnimation = particle.textureSheetAnimation;
            textureSheetAnimationFrames = 0;
            textureSheetAnimationFrameSize = Vector2.zero;

            if (textureSheetAnimation.enabled)
            {
                textureSheetAnimationFrames = textureSheetAnimation.numTilesX * textureSheetAnimation.numTilesY;
                textureSheetAnimationFrameSize = new Vector2(1f / textureSheetAnimation.numTilesX, 1f / textureSheetAnimation.numTilesY);
            }

            return true;
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) { if (!Initialize()) { return; } }
#endif

            vh.Clear();

            if (!gameObject.activeInHierarchy) { return; }

            Vector2 temp = Vector2.zero;
            Vector2 corner1 = Vector2.zero;
            Vector2 corner2 = Vector2.zero;
            
            int count = particle.GetParticles(particles);

            for (int i = 0; i < count; i++)
            {
                ParticleSystem.Particle part = particles[i];

                Vector2 position = mainModule.simulationSpace == ParticleSystemSimulationSpace.Local ? part.position 
                                                                                                     : transform.InverseTransformPoint(part.position);

                float rotation = -part.rotation * Mathf.Deg2Rad;
                float rotation90 = rotation + Mathf.PI / 2;
                Color32 color = part.GetCurrentColor(particle);
                float size = part.GetCurrentSize(particle) * 0.5f;

                if (mainModule.scalingMode == ParticleSystemScalingMode.Shape) { position /= canvas.scaleFactor; }

                Vector4 particleUV = imageUV;
                if (textureSheetAnimation.enabled)
                {
                    float frameProgress = textureSheetAnimation.frameOverTime.curveMin.Evaluate(1 - (part.remainingLifetime / part.startLifetime));

                    frameProgress = Mathf.Repeat(frameProgress * textureSheetAnimation.cycleCount, 1);
                    int frame = 0;

                    switch (textureSheetAnimation.animation)
                    {

                        case ParticleSystemAnimationType.WholeSheet:
                            frame = Mathf.FloorToInt(frameProgress * textureSheetAnimationFrames);
                            break;

                        case ParticleSystemAnimationType.SingleRow:
                            frame = Mathf.FloorToInt(frameProgress * textureSheetAnimation.numTilesX);

                            int row = textureSheetAnimation.rowIndex;
                            frame += row * textureSheetAnimation.numTilesX;
                            break;

                    }

                    frame %= textureSheetAnimationFrames;
                    particleUV.x = (frame % textureSheetAnimation.numTilesX) * textureSheetAnimationFrameSize.x;
                    particleUV.y = Mathf.FloorToInt(frame / textureSheetAnimation.numTilesX) * textureSheetAnimationFrameSize.y;
                    particleUV.z = particleUV.x + textureSheetAnimationFrameSize.x;
                    particleUV.w = particleUV.y + textureSheetAnimationFrameSize.y;
                }

                temp.x = particleUV.x;
                temp.y = particleUV.y;

                _quad[0] = UIVertex.simpleVert;
                _quad[0].color = color;
                _quad[0].uv0 = temp;

                temp.x = particleUV.x;
                temp.y = particleUV.w;
                _quad[1] = UIVertex.simpleVert;
                _quad[1].color = color;
                _quad[1].uv0 = temp;

                temp.x = particleUV.z;
                temp.y = particleUV.w;
                _quad[2] = UIVertex.simpleVert;
                _quad[2].color = color;
                _quad[2].uv0 = temp;

                temp.x = particleUV.z;
                temp.y = particleUV.y;
                _quad[3] = UIVertex.simpleVert;
                _quad[3].color = color;
                _quad[3].uv0 = temp;

                if (rotation == 0)
                {
                    corner1.x = position.x - size;
                    corner1.y = position.y - size;
                    corner2.x = position.x + size;
                    corner2.y = position.y + size;

                    temp.x = corner1.x;
                    temp.y = corner1.y;
                    _quad[0].position = temp;
                    temp.x = corner1.x;
                    temp.y = corner2.y;
                    _quad[1].position = temp;
                    temp.x = corner2.x;
                    temp.y = corner2.y;
                    _quad[2].position = temp;
                    temp.x = corner2.x;
                    temp.y = corner1.y;
                    _quad[3].position = temp;
                }

                else
                {
                    Vector2 right = new Vector2(Mathf.Cos(rotation), Mathf.Sin(rotation)) * size;
                    Vector2 up = new Vector2(Mathf.Cos(rotation90), Mathf.Sin(rotation90)) * size;

                    _quad[0].position = position - right - up;
                    _quad[1].position = position - right + up;
                    _quad[2].position = position + right + up;
                    _quad[3].position = position + right - up;
                }

                vh.AddUIVertexQuad(_quad);
            }
        }
    }
}