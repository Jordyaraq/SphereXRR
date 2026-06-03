using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using TMPro;
using SpaceGraphicsToolkit.Backdrop;
using SpaceGraphicsToolkit.Galaxy;

public static class VisualUpgradeTool
{
    private const string UpgradeFolder = "Assets/VisualUpgrade";

    [MenuItem("Tools/SphereXRR/Apply Visual Upgrade")]
    public static void Apply()
    {
        EnsureFolder("Assets", "VisualUpgrade");

        Shader lit = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
        Shader unlit = Shader.Find("Universal Render Pipeline/Unlit") ?? Shader.Find("Unlit/Texture");
        Shader particles = Shader.Find("Universal Render Pipeline/Particles/Unlit") ?? unlit;

        Texture2D nebulaTex = LoadTex("Assets/Plugins/CW/SpaceGraphicsToolkit/Features/Nebula/Examples/Textures/Nebula.png");
        Texture2D galaxyTex = LoadTex("Assets/Plugins/CW/SpaceGraphicsToolkit/Features/Galaxy/Examples/Textures/SpiralGalaxy.png");
        Texture2D planetTex = LoadTex("Assets/Plugins/CW/SpaceGraphicsToolkit/Features/Planet/Examples/Textures/Planet_Albedo.png");
        Texture2D glowyTex = LoadTex("Assets/Plugins/CW/SpaceGraphicsToolkit/Features/Shared/Examples/Textures/Glowy.png");
        Texture2D starTex = LoadTex("Assets/Plugins/CW/SpaceGraphicsToolkit/Features/Galaxy/Examples/Textures/Galaxy_Stars.png");
        Texture2D spiralStarsTex = LoadTex("Assets/Plugins/CW/SpaceGraphicsToolkit/Features/Galaxy/Examples/Textures/SpiralGalaxy_Stars.png");
        Texture2D thrusterTex = LoadTex("Assets/Plugins/CW/SpaceGraphicsToolkit/Features/Thruster/Examples/Textures/Thruster.png");

        Material coreCelestial = MakeMat("VX_Core_Celestial", lit, new Color(0.36f, 0.95f, 1f, 1f), new Color(0.10f, 1.6f, 1.9f, 1f), 0f, 0.9f, planetTex, false);
        Material coreNebular = MakeMat("VX_Core_Nebular", lit, new Color(0.95f, 0.18f, 1f, 1f), new Color(1.7f, 0.18f, 2.2f, 1f), 0f, 0.85f, nebulaTex, false);
        Material coreTemporal = MakeMat("VX_Core_Temporal", lit, new Color(1f, 0.78f, 0.22f, 1f), new Color(2.2f, 1.25f, 0.25f, 1f), 0f, 0.85f, galaxyTex, false);
        Material centralEnergy = MakeMat("VX_Central_Energy", lit, new Color(0.86f, 0.94f, 1f, 1f), new Color(1.5f, 2.6f, 3.4f, 1f), 0f, 0.95f, glowyTex, false);
        Material platformMat = MakeMat("VX_Obsidian_Platform", lit, new Color(0.035f, 0.043f, 0.055f, 1f), new Color(0.025f, 0.13f, 0.18f, 1f), 0.55f, 0.78f, null, false);
        Material ringMat = MakeMat("VX_Accretion_Ring", lit, new Color(0.06f, 0.44f, 0.55f, 1f), new Color(0.1f, 1.1f, 1.4f, 1f), 0.1f, 0.72f, null, false);
        Material pillarMat = MakeMat("VX_Energy_Pillars", lit, new Color(0.02f, 0.86f, 1f, 1f), new Color(0.05f, 2.8f, 3.4f, 1f), 0f, 0.75f, glowyTex, false);
        Material lineCyan = MakeMat("VX_Line_Cyan", unlit, new Color(0.1f, 0.95f, 1f, 0.86f), new Color(0.1f, 1.6f, 2.1f, 1f), 0f, 0.5f, null, true);
        Material lineMagenta = MakeMat("VX_Line_Magenta", unlit, new Color(1f, 0.1f, 0.95f, 0.82f), new Color(2.0f, 0.1f, 1.8f, 1f), 0f, 0.5f, null, true);
        Material lineGold = MakeMat("VX_Line_Gold", unlit, new Color(1f, 0.72f, 0.16f, 0.82f), new Color(2.1f, 1.15f, 0.15f, 1f), 0f, 0.5f, null, true);
        Material nebulaBillboard = MakeMat("VX_Nebula_Billboard", unlit, new Color(0.8f, 0.55f, 1f, 0.30f), Color.black, 0f, 0.5f, nebulaTex, true);
        Material galaxyBillboard = MakeMat("VX_Galaxy_Billboard", unlit, new Color(0.55f, 0.9f, 1f, 0.34f), Color.black, 0f, 0.5f, galaxyTex, true);
        Material particleMat = MakeMat("VX_Spark_Particles", particles, new Color(0.2f, 0.95f, 1f, 0.7f), new Color(0.2f, 1.2f, 1.6f, 1f), 0f, 0.5f, glowyTex, true);
        Material deepStars = MakeMat("VX_Deep_Starfield", unlit, new Color(0.72f, 0.9f, 1f, 0.52f), Color.black, 0f, 0.5f, starTex, true);
        Material spiralStars = MakeMat("VX_Spiral_Stars", unlit, new Color(0.75f, 0.82f, 1f, 0.42f), Color.black, 0f, 0.5f, spiralStarsTex, true);
        Material thrusterMat = MakeMat("VX_Rocket_Thruster", particles, new Color(1f, 0.42f, 0.08f, 0.9f), new Color(2.8f, 0.8f, 0.08f, 1f), 0f, 0.5f, thrusterTex, true);
        Material imageHaloMat = MakeMat("VX_Final_Image_Halo", unlit, new Color(0.35f, 0.92f, 1f, 0.24f), new Color(0.25f, 1.3f, 1.8f, 1f), 0f, 0.5f, glowyTex, true);
        Material fallbackPlanetMat = MakeMat("VX_Distant_Planet_Fallback", lit, new Color(0.28f, 0.42f, 0.62f, 1f), new Color(0.01f, 0.025f, 0.04f, 1f), 0f, 0.82f, planetTex, false);

        Assign("EnergySphere", centralEnergy);
        Assign("CentralPlatform", platformMat);
        Assign("MainRing", ringMat);
        Assign("CelestialCore", coreCelestial);
        Assign("NebularCore", coreNebular);
        Assign("TemporalCore", coreTemporal);
        Assign("EnergyPillar_01", pillarMat);
        Assign("EnergyPillar_02", pillarMat);
        Assign("EnergyPillar_03", pillarMat);
        Assign("EnergyPillar_04", pillarMat);

        MeshRenderer[] meshRenderers = Object.FindObjectsByType<MeshRenderer>(FindObjectsInactive.Exclude);
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            if (meshRenderers[i].gameObject.name == "RingMesh")
            {
                meshRenderers[i].sharedMaterial = ringMat;
            }
        }

        GameObject oldRoot = GameObject.Find("VisualUpgradeRoot");
        if (oldRoot != null)
        {
            Object.DestroyImmediate(oldRoot);
        }

        GameObject root = new GameObject("VisualUpgradeRoot");

        AddPointLight(root.transform, "VX_Central_Core_Light", new Vector3(0f, 1.5f, 0f), new Color(0.55f, 0.95f, 1f), 5.0f, 9f);
        AddPointLight(root.transform, "VX_Celestial_Light", new Vector3(0f, 1.1f, 4f), new Color(0.2f, 0.95f, 1f), 3.5f, 5.5f);
        AddPointLight(root.transform, "VX_Nebular_Light", new Vector3(-4f, 1.1f, 0f), new Color(1f, 0.25f, 0.95f), 3.0f, 5.5f);
        AddPointLight(root.transform, "VX_Temporal_Light", new Vector3(4f, 1.1f, 0f), new Color(1f, 0.72f, 0.2f), 3.0f, 5.5f);

        AddCircle(root.transform, "VX_Main_Energy_Orbit", new Vector3(0f, 0.28f, 0f), Vector3.zero, 4.15f, 0.035f, lineCyan);
        AddCircle(root.transform, "VX_Inner_Energy_Orbit", new Vector3(0f, 1.25f, 0f), new Vector3(18f, 0f, 0f), 1.35f, 0.028f, lineCyan);
        AddCircle(root.transform, "VX_Outer_Tilted_Orbit", new Vector3(0f, 1.15f, 0f), new Vector3(68f, 0f, 22f), 3.0f, 0.022f, lineGold);

        GameObject celestialCore = GameObject.Find("CelestialCore");
        GameObject nebularCore = GameObject.Find("NebularCore");
        GameObject temporalCore = GameObject.Find("TemporalCore");
        if (celestialCore != null) AddCircle(celestialCore.transform, "VX_Celestial_SGT_Halo", Vector3.zero, new Vector3(90f, 0f, 0f), 0.95f, 0.026f, lineCyan);
        if (nebularCore != null) AddCircle(nebularCore.transform, "VX_Nebular_SGT_Halo", Vector3.zero, new Vector3(90f, 0f, 0f), 0.95f, 0.026f, lineMagenta);
        if (temporalCore != null) AddCircle(temporalCore.transform, "VX_Temporal_SGT_Halo", Vector3.zero, new Vector3(90f, 0f, 0f), 0.95f, 0.026f, lineGold);

        AddParticles(root.transform, "VX_Central_Stardust", new Vector3(0f, 1.2f, 0f), new Color(0.3f, 0.95f, 1f, 0.45f), 3.3f, 18f, 0.08f, particleMat);
        AddParticles(root.transform, "VX_Background_Sparks", new Vector3(0f, 2f, 3f), new Color(0.8f, 0.55f, 1f, 0.28f), 8f, 10f, 0.12f, particleMat);

        AddBillboard(root.transform, "VX_Nebula_Veil_Left", nebulaBillboard, new Vector3(-5.4f, 2.6f, 5.6f), new Vector3(0f, 180f, -12f), new Vector3(5.6f, 3.2f, 1f));
        AddBillboard(root.transform, "VX_Galaxy_Veil_Right", galaxyBillboard, new Vector3(5.4f, 3.2f, 5.8f), new Vector3(0f, 180f, 15f), new Vector3(4.7f, 4.7f, 1f));
        AddBillboard(root.transform, "VX_Nebula_Ceiling", nebulaBillboard, new Vector3(0f, 6.1f, 2.0f), new Vector3(64f, 180f, 0f), new Vector3(8.5f, 3.8f, 1f));
        AddDeepSpaceBackdrop(root.transform, deepStars, spiralStars, nebulaBillboard, galaxyBillboard, particleMat);
        AddSgtProceduralBackground(root.transform);
        AddGalaxySetPieces(root.transform, fallbackPlanetMat, lineGold, lineCyan);
        AddShootingStar(root.transform, particleMat, lineCyan);

        AddSgtPrefabs(root.transform, thrusterMat);
        PolishFinalImage(imageHaloMat);
        PolishCameraAndLight();
        PolishPostProcessing(root.transform);
        PolishArrowText();
        PolishSequenceFeedback(root.transform, particleMat);

        AssetDatabase.SaveAssets();
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();
        Debug.Log("SphereXRR visual upgrade applied successfully.");
    }

    private static void EnsureFolder(string parent, string folder)
    {
        if (!AssetDatabase.IsValidFolder(parent + "/" + folder))
        {
            AssetDatabase.CreateFolder(parent, folder);
        }
    }

    private static Texture2D LoadTex(string path)
    {
        return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
    }

    private static Material MakeMat(string name, Shader shader, Color baseColor, Color emission, float metallic, float smoothness, Texture tex, bool transparent)
    {
        string path = UpgradeFolder + "/" + name + ".mat";
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (mat == null)
        {
            mat = new Material(shader);
            AssetDatabase.CreateAsset(mat, path);
        }

        mat.shader = shader;
        if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", baseColor);
        if (mat.HasProperty("_Color")) mat.SetColor("_Color", baseColor);
        if (mat.HasProperty("_EmissionColor")) mat.SetColor("_EmissionColor", emission);
        if (mat.HasProperty("_Metallic")) mat.SetFloat("_Metallic", metallic);
        if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", smoothness);
        if (tex != null)
        {
            if (mat.HasProperty("_BaseMap")) mat.SetTexture("_BaseMap", tex);
            if (mat.HasProperty("_MainTex")) mat.SetTexture("_MainTex", tex);
        }
        if (emission.maxColorComponent > 0.01f) mat.EnableKeyword("_EMISSION");
        if (transparent)
        {
            if (mat.HasProperty("_Surface")) mat.SetFloat("_Surface", 1f);
            if (mat.HasProperty("_Blend")) mat.SetFloat("_Blend", 0f);
            if (mat.HasProperty("_ZWrite")) mat.SetFloat("_ZWrite", 0f);
            mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            mat.renderQueue = (int)RenderQueue.Transparent;
        }
        EditorUtility.SetDirty(mat);
        return mat;
    }

    private static void Assign(string objectName, Material mat)
    {
        GameObject go = GameObject.Find(objectName);
        if (go == null) return;
        Renderer renderer = go.GetComponent<Renderer>();
        if (renderer != null) renderer.sharedMaterial = mat;
    }

    private static void AddPointLight(Transform parent, string name, Vector3 pos, Color color, float intensity, float range)
    {
        GameObject lightGo = new GameObject(name);
        lightGo.transform.SetParent(parent, false);
        lightGo.transform.position = pos;
        Light light = lightGo.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = color;
        light.intensity = intensity;
        light.range = range;
        light.shadows = LightShadows.None;
    }

    private static void AddCircle(Transform parent, string name, Vector3 localPos, Vector3 localRot, float radius, float width, Material mat)
    {
        const int segments = 144;
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.transform.localPosition = localPos;
        go.transform.localEulerAngles = localRot;
        LineRenderer line = go.AddComponent<LineRenderer>();
        line.useWorldSpace = false;
        line.loop = true;
        line.positionCount = segments;
        line.widthMultiplier = width;
        line.numCapVertices = 8;
        line.numCornerVertices = 8;
        line.shadowCastingMode = ShadowCastingMode.Off;
        line.receiveShadows = false;
        line.sharedMaterial = mat;
        for (int i = 0; i < segments; i++)
        {
            float angle = Mathf.PI * 2f * i / segments;
            line.SetPosition(i, new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius));
        }
    }

    private static void AddParticles(Transform parent, string name, Vector3 pos, Color color, float radius, float rate, float size, Material mat)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.transform.position = pos;
        ParticleSystem ps = go.AddComponent<ParticleSystem>();
        ParticleSystem.MainModule main = ps.main;
        main.loop = true;
        main.startLifetime = new ParticleSystem.MinMaxCurve(2.5f, 5.5f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.05f, 0.35f);
        main.startSize = new ParticleSystem.MinMaxCurve(size * 0.55f, size);
        main.startColor = color;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.maxParticles = 350;
        ParticleSystem.EmissionModule emission = ps.emission;
        emission.rateOverTime = rate;
        ParticleSystem.ShapeModule shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = radius;
        ParticleSystemRenderer renderer = ps.GetComponent<ParticleSystemRenderer>();
        renderer.sharedMaterial = mat;
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        renderer.sortMode = ParticleSystemSortMode.Distance;
    }

    private static void AddBillboard(Transform parent, string name, Material mat, Vector3 pos, Vector3 rot, Vector3 scale)
    {
        GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        quad.name = name;
        quad.transform.SetParent(parent, false);
        quad.transform.position = pos;
        quad.transform.eulerAngles = rot;
        quad.transform.localScale = scale;
        Object.DestroyImmediate(quad.GetComponent<Collider>());
        MeshRenderer renderer = quad.GetComponent<MeshRenderer>();
        renderer.sharedMaterial = mat;
        renderer.shadowCastingMode = ShadowCastingMode.Off;
        renderer.receiveShadows = false;
    }

    private static void AddDeepSpaceBackdrop(Transform parent, Material deepStars, Material spiralStars, Material nebula, Material galaxy, Material particleMat)
    {
        AddBillboard(parent, "VX_Deep_Starfield_Back", deepStars, new Vector3(0f, 3.4f, 9.2f), new Vector3(0f, 180f, 0f), new Vector3(15.5f, 8.6f, 1f));
        AddBillboard(parent, "VX_Deep_Starfield_Left", spiralStars, new Vector3(-8.8f, 3.0f, 2.7f), new Vector3(0f, 105f, -8f), new Vector3(8.5f, 6.0f, 1f));
        AddBillboard(parent, "VX_Deep_Starfield_Right", deepStars, new Vector3(8.8f, 2.8f, 2.2f), new Vector3(0f, 255f, 9f), new Vector3(8.2f, 5.9f, 1f));
        AddBillboard(parent, "VX_Distant_Spiral_Core", galaxy, new Vector3(0f, 5.4f, 8.4f), new Vector3(0f, 180f, 22f), new Vector3(5.8f, 5.8f, 1f));
        AddBillboard(parent, "VX_Low_Nebula_Haze", nebula, new Vector3(0f, 0.9f, 7.2f), new Vector3(0f, 180f, 0f), new Vector3(12.0f, 3.2f, 1f));
        AddParticles(parent, "VX_Distant_Parallax_Dust", new Vector3(0f, 2.8f, 4.8f), new Color(0.45f, 0.8f, 1f, 0.20f), 9.5f, 22f, 0.045f, particleMat);
    }

    private static void AddSgtProceduralBackground(Transform parent)
    {
        Material backdropMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Plugins/CW/SpaceGraphicsToolkit/Features/Backdrop/Examples/Materials/Backdrop + Power RGB + Pulse.mat");
        Material galaxyMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Plugins/CW/SpaceGraphicsToolkit/Features/Galaxy/Examples/Materials/Galaxy.mat");

        if (backdropMaterial != null)
        {
            SgtBackdrop backdrop = SgtBackdrop.Create(0, parent);
            backdrop.name = "VX_SGT_Procedural_Star_Backdrop";
            backdrop.SourceMaterial = backdropMaterial;
            backdrop.Color = new Color(0.78f, 0.9f, 1f, 1f);
            backdrop.Brightness = 1.35f;
            backdrop.Radius = 42f;
            backdrop.Seed = 250603;
            backdrop.StarCount = 2600;
            backdrop.StarRadiusMin = 0.006f;
            backdrop.StarRadiusMax = 0.035f;
            backdrop.StarRadiusBias = 2.2f;
            backdrop.StarPulseSpeedMin = 0.08f;
            backdrop.StarPulseSpeedMax = 0.38f;
            backdrop.Squash = 0.05f;
            backdrop.DirtyMesh();
        }

        if (galaxyMaterial != null)
        {
            GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Mesh sourceMesh = temp.GetComponent<MeshFilter>().sharedMesh;
            Object.DestroyImmediate(temp);

            SgtGalaxy galaxy = SgtGalaxy.Create(0, parent);
            galaxy.name = "VX_SGT_Distant_Procedural_Galaxy";
            galaxy.transform.localPosition = new Vector3(0f, 5.5f, 15f);
            galaxy.transform.localEulerAngles = new Vector3(12f, 180f, -18f);
            galaxy.SourceMaterial = galaxyMaterial;
            galaxy.SourceMesh = sourceMesh;
            galaxy.Radius = 13f;
            galaxy.Flattening = 0.72f;
        }
    }

    private static void AddGalaxySetPieces(Transform parent, Material fallbackPlanetMat, Material ringLineMat, Material cyanLineMat)
    {
        Material jovianMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Plugins/CW/SpaceGraphicsToolkit/Features/Jovian/Examples/Materials/Jovian + Lighting + Scattering + Flow.mat");
        Material terrestrialMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Plugins/CW/SpaceGraphicsToolkit/Packs/Basic Pack/Materials/Earth Sized Planet.mat");
        Material starMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Plugins/CW/SpaceGraphicsToolkit/Features/Star/Examples/Materials/Star (03).mat");
        Material ringMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Plugins/CW/SpaceGraphicsToolkit/Features/Ring/Examples/Materials/Ring + Scattering + Detail.mat");
        Material atmosphereMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Plugins/CW/SpaceGraphicsToolkit/Features/Atmosphere/Examples/Materials/Atmosphere + Lighting + Scattering.mat");

        GameObject jovian = AddDecorativeSphere(parent, "VX_Background_Jovian_Planet", new Vector3(-7.1f, 4.9f, 7.8f), new Vector3(0f, -18f, 0f), Vector3.one * 1.55f, jovianMat != null ? jovianMat : fallbackPlanetMat, true);
        AddCircle(jovian.transform, "VX_Background_Jovian_Ring_A", Vector3.zero, new Vector3(63f, 0f, -18f), 1.52f, 0.01f, ringLineMat);
        AddCircle(jovian.transform, "VX_Background_Jovian_Ring_B", Vector3.zero, new Vector3(63f, 0f, -18f), 1.76f, 0.007f, ringLineMat);

        GameObject terrestrial = AddDecorativeSphere(parent, "VX_Background_Terrestrial_Planet", new Vector3(7.1f, 4.55f, 7.5f), new Vector3(0f, 25f, -8f), Vector3.one * 1.05f, terrestrialMat != null ? terrestrialMat : fallbackPlanetMat, true);
        if (atmosphereMat != null)
        {
            AddDecorativeSphere(terrestrial.transform, "VX_Background_Terrestrial_Atmosphere", Vector3.zero, Vector3.zero, Vector3.one * 1.06f, atmosphereMat, false);
        }
        AddCircle(terrestrial.transform, "VX_Background_Terrestrial_Orbit_Glint", Vector3.zero, new Vector3(72f, 0f, 14f), 1.08f, 0.008f, cyanLineMat);

        GameObject star = AddDecorativeSphere(parent, "VX_Distant_Blue_White_Star", new Vector3(4.8f, 6.2f, 11.0f), Vector3.zero, Vector3.one * 0.7f, starMat != null ? starMat : fallbackPlanetMat, true);
        Light starLight = star.AddComponent<Light>();
        starLight.type = LightType.Point;
        starLight.color = new Color(0.55f, 0.78f, 1f);
        starLight.intensity = 2.8f;
        starLight.range = 12f;
        starLight.shadows = LightShadows.None;

        AddPointLight(parent, "VX_Cool_Rim_Light", new Vector3(3.8f, 4.6f, -4.0f), new Color(0.36f, 0.78f, 1f), 2.9f, 11f);
        AddPointLight(parent, "VX_Warm_Planet_Bounce", new Vector3(-4.5f, 2.0f, 3.8f), new Color(1f, 0.42f, 0.18f), 1.4f, 7f);
    }

    private static GameObject AddDecorativeSphere(Transform parent, string name, Vector3 position, Vector3 rotation, Vector3 scale, Material material, bool addDrift)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.name = name;
        sphere.transform.SetParent(parent, false);
        sphere.transform.localPosition = position;
        sphere.transform.localEulerAngles = rotation;
        sphere.transform.localScale = scale;
        Object.DestroyImmediate(sphere.GetComponent<Collider>());
        MeshRenderer renderer = sphere.GetComponent<MeshRenderer>();
        renderer.sharedMaterial = material;
        renderer.shadowCastingMode = ShadowCastingMode.Off;
        renderer.receiveShadows = false;
        if (addDrift)
        {
            SpaceDriftAnimator drift = sphere.AddComponent<SpaceDriftAnimator>();
            drift.orbitCenter = position;
            drift.orbitRadius = 0.05f;
            drift.verticalAmplitude = 0.015f;
            drift.orbitSpeed = 0.35f;
            drift.spinAxis = new Vector3(0.2f, 1f, 0.1f);
            drift.spinSpeed = 2.2f;
        }
        return sphere;
    }

    private static void AddSgtPrefabs(Transform parent, Material thrusterMat)
    {
        GameObject asteroidPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Plugins/CW/SpaceGraphicsToolkit/Features/Shared/Examples/Models/Asteroid.prefab");
        GameObject rocketPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Plugins/CW/SpaceGraphicsToolkit/Packs/Basic Pack/Models/Rocket Ship.prefab");
        if (asteroidPrefab != null)
        {
            Vector3[] positions = new Vector3[]
            {
                new Vector3(-6.5f, 2.2f, 4.5f), new Vector3(6.6f, 1.7f, 4.2f),
                new Vector3(-5.1f, 4.6f, -0.7f), new Vector3(5.0f, 4.1f, -1.4f),
                new Vector3(-7.4f, 0.6f, -2.4f), new Vector3(7.1f, 0.9f, -2.1f),
                new Vector3(-2.7f, 5.5f, 5.8f), new Vector3(3.1f, 5.2f, 5.3f)
            };
            for (int i = 0; i < positions.Length; i++)
            {
                GameObject asteroid = (GameObject)PrefabUtility.InstantiatePrefab(asteroidPrefab);
                asteroid.name = "VX_SGT_Asteroid_" + (i + 1).ToString("00");
                asteroid.transform.SetParent(parent, false);
                asteroid.transform.position = positions[i];
                asteroid.transform.eulerAngles = new Vector3(23f * i, 51f * i, 17f * i);
                asteroid.transform.localScale = Vector3.one * (0.22f + (i % 3) * 0.08f);
                StripGameplayComponents(asteroid);
                SpaceDriftAnimator drift = asteroid.AddComponent<SpaceDriftAnimator>();
                drift.orbitCenter = Vector3.zero;
                drift.orbitAxis = new Vector3(0.12f + i * 0.03f, 1f, 0.18f - i * 0.02f);
                drift.orbitRadius = Mathf.Max(4.9f, new Vector2(positions[i].x, positions[i].z).magnitude);
                drift.verticalAmplitude = 0.35f + (i % 4) * 0.16f;
                drift.orbitSpeed = 2.8f + (i % 5) * 0.55f;
                drift.phase = i * 0.78f;
                drift.spinAxis = new Vector3(0.35f + i * 0.11f, 0.7f, 0.22f + i * 0.07f);
                drift.spinSpeed = 18f + (i % 4) * 9f;
            }
        }
        if (rocketPrefab != null)
        {
            GameObject ship = (GameObject)PrefabUtility.InstantiatePrefab(rocketPrefab);
            ship.name = "VX_SGT_Display_Rocket";
            ship.transform.SetParent(parent, false);
            ship.transform.position = new Vector3(-3.6f, 1.0f, -2.3f);
            ship.transform.eulerAngles = new Vector3(0f, 38f, -8f);
            ship.transform.localScale = Vector3.one * 0.35f;
            StripGameplayComponents(ship);
            MeshRenderer[] renderers = ship.GetComponentsInChildren<MeshRenderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].shadowCastingMode = ShadowCastingMode.On;
                renderers[i].receiveShadows = true;
            }
            RocketPatrolAnimator patrol = ship.AddComponent<RocketPatrolAnimator>();
            patrol.center = new Vector3(0f, 0f, -1.6f);
            patrol.radius = new Vector3(6.0f, 1.1f, 2.15f);
            patrol.finalImageSafeCenter = new Vector3(0f, 0f, -3.15f);
            patrol.finalImageSafeRadius = new Vector3(6.9f, 1.25f, 1.05f);
            patrol.speed = 0.045f;
            patrol.bankAmount = 24f;
            GameObject image = GameObject.Find("Imagen25Anios");
            if (image != null)
            {
                patrol.avoidWhenActive = image.transform;
            }
            AddRocketThruster(ship.transform, thrusterMat);
        }
    }

    private static void AddRocketThruster(Transform rocket, Material mat)
    {
        GameObject thrust = new GameObject("VX_Rocket_Fire_Trail");
        thrust.transform.SetParent(rocket, false);
        thrust.transform.localPosition = new Vector3(0f, 0f, -1.3f);
        thrust.transform.localEulerAngles = new Vector3(0f, 180f, 0f);

        ParticleSystem ps = thrust.AddComponent<ParticleSystem>();
        ParticleSystem.MainModule main = ps.main;
        main.loop = true;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.18f, 0.42f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.55f, 1.15f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.05f, 0.14f);
        main.startColor = new ParticleSystem.MinMaxGradient(new Color(1f, 0.86f, 0.28f, 0.95f), new Color(1f, 0.18f, 0.02f, 0.55f));
        main.simulationSpace = ParticleSystemSimulationSpace.Local;
        main.maxParticles = 190;

        ParticleSystem.EmissionModule emission = ps.emission;
        emission.rateOverTime = 68f;

        ParticleSystem.ShapeModule shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 10f;
        shape.radius = 0.045f;
        shape.length = 0.24f;

        ParticleSystem.ColorOverLifetimeModule colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[]
            {
                new GradientColorKey(new Color(1f, 0.92f, 0.34f), 0f),
                new GradientColorKey(new Color(1f, 0.25f, 0.04f), 0.35f),
                new GradientColorKey(new Color(0.35f, 0.06f, 0.02f), 1f)
            },
            new GradientAlphaKey[]
            {
                new GradientAlphaKey(0.95f, 0f),
                new GradientAlphaKey(0.55f, 0.45f),
                new GradientAlphaKey(0f, 1f)
            }
        );
        colorOverLifetime.color = gradient;

        ParticleSystemRenderer renderer = ps.GetComponent<ParticleSystemRenderer>();
        renderer.sharedMaterial = mat;
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        renderer.sortMode = ParticleSystemSortMode.Distance;

        GameObject smoke = new GameObject("VX_Rocket_Trail_Shadow_Smoke");
        smoke.transform.SetParent(rocket, false);
        smoke.transform.localPosition = new Vector3(0f, 0f, -1.42f);
        smoke.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
        ParticleSystem smokePs = smoke.AddComponent<ParticleSystem>();
        ParticleSystem.MainModule smokeMain = smokePs.main;
        smokeMain.loop = true;
        smokeMain.startLifetime = new ParticleSystem.MinMaxCurve(0.32f, 0.68f);
        smokeMain.startSpeed = new ParticleSystem.MinMaxCurve(0.18f, 0.44f);
        smokeMain.startSize = new ParticleSystem.MinMaxCurve(0.08f, 0.2f);
        smokeMain.startColor = new ParticleSystem.MinMaxGradient(new Color(0f, 0f, 0f, 0.24f), new Color(0.12f, 0.03f, 0.01f, 0.08f));
        smokeMain.simulationSpace = ParticleSystemSimulationSpace.Local;
        smokeMain.maxParticles = 100;
        ParticleSystem.EmissionModule smokeEmission = smokePs.emission;
        smokeEmission.rateOverTime = 38f;
        ParticleSystem.ShapeModule smokeShape = smokePs.shape;
        smokeShape.shapeType = ParticleSystemShapeType.Cone;
        smokeShape.angle = 15f;
        smokeShape.radius = 0.055f;
        smokeShape.length = 0.18f;
        ParticleSystemRenderer smokeRenderer = smokePs.GetComponent<ParticleSystemRenderer>();
        smokeRenderer.sharedMaterial = mat;
        smokeRenderer.renderMode = ParticleSystemRenderMode.Billboard;
        smokeRenderer.sortMode = ParticleSystemSortMode.Distance;

        TrailRenderer trail = thrust.AddComponent<TrailRenderer>();
        trail.time = 0.28f;
        trail.minVertexDistance = 0.025f;
        trail.widthMultiplier = 0.11f;
        trail.shadowCastingMode = ShadowCastingMode.On;
        trail.receiveShadows = false;
        trail.sharedMaterial = mat;
        Gradient trailGradient = new Gradient();
        trailGradient.SetKeys(
            new GradientColorKey[]
            {
                new GradientColorKey(new Color(1f, 0.88f, 0.35f), 0f),
                new GradientColorKey(new Color(1f, 0.22f, 0.05f), 0.55f),
                new GradientColorKey(new Color(0.02f, 0.01f, 0f), 1f)
            },
            new GradientAlphaKey[]
            {
                new GradientAlphaKey(0.75f, 0f),
                new GradientAlphaKey(0.42f, 0.45f),
                new GradientAlphaKey(0f, 1f)
            }
        );
        trail.colorGradient = trailGradient;

        Light light = thrust.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = new Color(1f, 0.42f, 0.08f);
        light.intensity = 1.75f;
        light.range = 2.25f;
        light.shadows = LightShadows.None;
    }

    private static void StripGameplayComponents(GameObject go)
    {
        MonoBehaviour[] behaviours = go.GetComponentsInChildren<MonoBehaviour>();
        for (int i = behaviours.Length - 1; i >= 0; i--) Object.DestroyImmediate(behaviours[i]);
        Collider[] colliders = go.GetComponentsInChildren<Collider>();
        for (int i = 0; i < colliders.Length; i++) Object.DestroyImmediate(colliders[i]);
        Rigidbody[] bodies = go.GetComponentsInChildren<Rigidbody>();
        for (int i = 0; i < bodies.Length; i++) Object.DestroyImmediate(bodies[i]);
        AudioSource[] audioSources = go.GetComponentsInChildren<AudioSource>();
        for (int i = 0; i < audioSources.Length; i++) Object.DestroyImmediate(audioSources[i]);
    }

    private static void PolishCameraAndLight()
    {
        Camera cam = Camera.main;
        if (cam != null)
        {
            cam.transform.position = new Vector3(0f, 5.55f, -10.4f);
            cam.transform.eulerAngles = new Vector3(20.5f, 0f, 0f);
            cam.fieldOfView = 55f;
            cam.allowHDR = true;
            UniversalAdditionalCameraData urpCam = cam.GetComponent<UniversalAdditionalCameraData>();
            if (urpCam != null) urpCam.renderPostProcessing = true;
        }

        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
        RenderSettings.ambientSkyColor = new Color(0.055f, 0.075f, 0.105f);
        RenderSettings.ambientEquatorColor = new Color(0.018f, 0.026f, 0.038f);
        RenderSettings.ambientGroundColor = new Color(0.002f, 0.004f, 0.008f);
        RenderSettings.reflectionIntensity = 0.38f;
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Exponential;
        RenderSettings.fogColor = new Color(0.015f, 0.026f, 0.04f);
        RenderSettings.fogDensity = 0.018f;

        GameObject sun = GameObject.Find("Directional Light");
        if (sun != null)
        {
            Light light = sun.GetComponent<Light>();
            if (light != null)
            {
                light.color = new Color(0.82f, 0.9f, 1f);
                light.intensity = 2.25f;
                light.shadows = LightShadows.Soft;
                light.shadowStrength = 0.65f;
                light.shadowBias = 0.035f;
            }
            sun.transform.eulerAngles = new Vector3(42f, 315f, 12f);
        }
    }

    private static void PolishPostProcessing(Transform parent)
    {
        Volume volume = Object.FindAnyObjectByType<Volume>();
        if (volume == null)
        {
            GameObject volumeGo = new GameObject("Global Volume");
            volumeGo.transform.SetParent(parent, false);
            volume = volumeGo.AddComponent<Volume>();
            volume.isGlobal = true;
            volume.priority = 0f;
        }
        volume.isGlobal = true;
        if (volume.profile == null) volume.profile = ScriptableObject.CreateInstance<VolumeProfile>();

        Bloom bloom;
        if (!volume.profile.TryGet(out bloom)) bloom = volume.profile.Add<Bloom>(true);
        bloom.active = true;
        bloom.threshold.Override(0.58f);
        bloom.intensity.Override(1.05f);
        bloom.scatter.Override(0.82f);

        Vignette vignette;
        if (!volume.profile.TryGet(out vignette)) vignette = volume.profile.Add<Vignette>(true);
        vignette.active = true;
        vignette.intensity.Override(0.34f);
        vignette.smoothness.Override(0.52f);

        ColorAdjustments colorAdjustments;
        if (!volume.profile.TryGet(out colorAdjustments)) colorAdjustments = volume.profile.Add<ColorAdjustments>(true);
        colorAdjustments.active = true;
        colorAdjustments.postExposure.Override(-0.18f);
        colorAdjustments.contrast.Override(36f);
        colorAdjustments.saturation.Override(12f);

        Tonemapping tonemapping;
        if (!volume.profile.TryGet(out tonemapping)) tonemapping = volume.profile.Add<Tonemapping>(true);
        tonemapping.active = true;
        tonemapping.mode.Override(TonemappingMode.ACES);

        ChromaticAberration chromaticAberration;
        if (!volume.profile.TryGet(out chromaticAberration)) chromaticAberration = volume.profile.Add<ChromaticAberration>(true);
        chromaticAberration.active = true;
        chromaticAberration.intensity.Override(0.04f);

        FilmGrain filmGrain;
        if (!volume.profile.TryGet(out filmGrain)) filmGrain = volume.profile.Add<FilmGrain>(true);
        filmGrain.active = true;
        filmGrain.intensity.Override(0.08f);
        filmGrain.response.Override(0.75f);
    }

    private static void PolishArrowText()
    {
        TMP_Text[] texts = Object.FindObjectsByType<TMP_Text>(FindObjectsInactive.Include);
        for (int i = 0; i < texts.Length; i++)
        {
            if (texts[i] == null || !texts[i].name.ToLowerInvariant().Contains("arrow"))
            {
                continue;
            }

            texts[i].characterSpacing = 0f;
            texts[i].fontStyle |= FontStyles.Bold;
            texts[i].color = new Color(0.78f, 1f, 1f, 0.96f);
            texts[i].outlineWidth = 0.18f;
            texts[i].outlineColor = new Color(0f, 0.34f, 0.44f, 1f);
            AddOrConfigureArrowAnimator(texts[i].gameObject);
            EditorUtility.SetDirty(texts[i]);
        }

        LayoutArrowHud();
    }

    private static void LayoutArrowHud()
    {
        string[] names =
        {
            "Arrow1", "Arrow2", "Arrow3",
            "nebularArrow1", "nebularArrow2", "nebularArrow3",
            "temporalArrow1", "temporalArrow2", "temporalArrow3"
        };

        TMP_Text[] arrows = new TMP_Text[names.Length];
        for (int i = 0; i < names.Length; i++)
        {
            GameObject arrowObject = FindSceneObjectByName(names[i]);
            if (arrowObject == null)
            {
                continue;
            }

            arrows[i] = arrowObject.GetComponent<TMP_Text>();
        }

        Canvas canvas = null;
        for (int i = 0; i < arrows.Length; i++)
        {
            if (arrows[i] == null) continue;
            canvas = arrows[i].GetComponentInParent<Canvas>(true);
            if (canvas != null) break;
        }

        if (canvas == null)
        {
            return;
        }

        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        canvas.transform.localPosition = Vector3.zero;
        canvas.transform.localRotation = Quaternion.identity;
        canvas.transform.localScale = Vector3.one;

        CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
        if (scaler != null)
        {
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(880f, 434f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
        }

        Vector2[] positions =
        {
            new Vector2(-72f, 74f), new Vector2(0f, 74f), new Vector2(72f, 74f),
            new Vector2(-72f, 74f), new Vector2(0f, 74f), new Vector2(72f, 74f),
            new Vector2(-72f, 74f), new Vector2(0f, 74f), new Vector2(72f, 74f)
        };

        for (int i = 0; i < arrows.Length; i++)
        {
            if (arrows[i] == null) continue;

            RectTransform rect = arrows[i].GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchorMin = new Vector2(0.5f, 0f);
                rect.anchorMax = new Vector2(0.5f, 0f);
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.anchoredPosition = positions[i];
                rect.sizeDelta = new Vector2(76f, 76f);
                rect.localRotation = Quaternion.identity;
                rect.localScale = Vector3.one;
            }

            CreateOrUpdateArrowSlot(arrows[i], positions[i], i < 3);

            arrows[i].text = GetArrowSymbolForIndex(i);
            arrows[i].alignment = TextAlignmentOptions.Center;
            arrows[i].fontSize = 38f;
            arrows[i].textWrappingMode = TextWrappingModes.NoWrap;
            arrows[i].overflowMode = TextOverflowModes.Overflow;
            arrows[i].raycastTarget = false;
            EditorUtility.SetDirty(arrows[i]);
        }

        for (int i = 0; i < arrows.Length; i++)
        {
            if (arrows[i] == null) continue;
            arrows[i].gameObject.SetActive(i < 3);
        }

        EditorUtility.SetDirty(canvas);
    }

    private static string GetArrowSymbolForIndex(int index)
    {
        switch (index)
        {
            case 0: return "\u2191";
            case 1: return "\u2193";
            case 2: return "\u2191";
            case 3: return "\u2190";
            case 4: return "\u2192";
            case 5: return "\u2190";
            case 6: return "\u2191";
            case 7: return "\u2192";
            case 8: return "\u2193";
            default: return "?";
        }
    }

    private static void CreateOrUpdateArrowSlot(TMP_Text arrow, Vector2 position, bool active)
    {
        if (arrow == null || arrow.transform.parent == null)
        {
            return;
        }

        string slotName = arrow.name + "_Slot";
        Transform existing = arrow.transform.parent.Find(slotName);
        GameObject slotObject;
        if (existing != null)
        {
            slotObject = existing.gameObject;
        }
        else
        {
            slotObject = new GameObject(slotName, typeof(RectTransform), typeof(Image), typeof(Outline));
            slotObject.transform.SetParent(arrow.transform.parent, false);
        }

        RectTransform rect = slotObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0f);
        rect.anchorMax = new Vector2(0.5f, 0f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(52f, 50f);
        rect.localRotation = Quaternion.identity;
        rect.localScale = Vector3.one;

        Image image = slotObject.GetComponent<Image>();
        image.color = new Color(0.02f, 0.12f, 0.18f, 0.28f);
        image.raycastTarget = false;

        Outline outline = slotObject.GetComponent<Outline>();
        outline.effectColor = new Color(0.2f, 1f, 1f, 0.32f);
        outline.effectDistance = new Vector2(1.1f, -1.1f);

        slotObject.transform.SetSiblingIndex(Mathf.Max(0, arrow.transform.GetSiblingIndex()));
        arrow.transform.SetSiblingIndex(slotObject.transform.GetSiblingIndex() + 1);
        slotObject.SetActive(active);

        EditorUtility.SetDirty(slotObject);
        EditorUtility.SetDirty(rect);
        EditorUtility.SetDirty(image);
        EditorUtility.SetDirty(outline);
    }

    private static void PolishSequenceFeedback(Transform visualRoot, Material particleMaterial)
    {
        Canvas canvas = null;
        GameObject firstArrow = FindSceneObjectByName("Arrow1");
        if (firstArrow != null)
        {
            canvas = firstArrow.GetComponentInParent<Canvas>(true);
        }

        if (canvas != null)
        {
            GameObject feedbackObject = FindSceneObjectByName("VX_Sequence_Feedback_Text");
            if (feedbackObject == null)
            {
                feedbackObject = new GameObject("VX_Sequence_Feedback_Text", typeof(RectTransform), typeof(TextMeshProUGUI));
                feedbackObject.transform.SetParent(canvas.transform, false);
            }

            RectTransform rect = feedbackObject.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0f);
            rect.anchorMax = new Vector2(0.5f, 0f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = new Vector2(0f, 154f);
            rect.sizeDelta = new Vector2(320f, 74f);
            rect.localRotation = Quaternion.identity;
            rect.localScale = Vector3.one;

            TextMeshProUGUI text = feedbackObject.GetComponent<TextMeshProUGUI>();
            TMP_Text arrowText = firstArrow != null ? firstArrow.GetComponent<TMP_Text>() : null;
            if (arrowText != null && arrowText.font != null)
            {
                text.font = arrowText.font;
            }

            text.text = "Muy bien!";
            text.alignment = TextAlignmentOptions.Center;
            text.fontSize = 38f;
            text.fontStyle = FontStyles.Bold;
            text.color = new Color(0.45f, 1f, 0.6f, 1f);
            text.outlineWidth = 0.18f;
            text.outlineColor = new Color(0f, 0.22f, 0.18f, 1f);
            text.textWrappingMode = TextWrappingModes.NoWrap;
            text.raycastTarget = false;

            feedbackObject.SetActive(false);
            EditorUtility.SetDirty(feedbackObject);
            EditorUtility.SetDirty(rect);
            EditorUtility.SetDirty(text);
        }

        AddBurstParticles(visualRoot, "VX_Sequence_Success_Particles", new Vector3(0f, 2.35f, -0.25f), new Color(0.28f, 1f, 0.48f, 1f), 0.52f, 0.19f, 54, particleMaterial);
        AddBurstParticles(visualRoot, "VX_Sequence_Error_Particles", new Vector3(0f, 2.05f, -0.25f), new Color(1f, 0.08f, 0.04f, 1f), 0.42f, 0.16f, 38, particleMaterial);
    }

    private static void AddBurstParticles(Transform parent, string name, Vector3 position, Color color, float radius, float size, int burstCount, Material material)
    {
        Transform existing = parent.Find(name);
        GameObject go;
        if (existing != null)
        {
            go = existing.gameObject;
        }
        else
        {
            go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.AddComponent<ParticleSystem>();
        }

        go.transform.position = position;

        ParticleSystem ps = go.GetComponent<ParticleSystem>();
        ParticleSystem.MainModule main = ps.main;
        main.loop = false;
        main.playOnAwake = false;
        main.duration = 0.75f;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.45f, 0.9f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.35f, 1.25f);
        main.startSize = new ParticleSystem.MinMaxCurve(size * 0.45f, size);
        main.startColor = new ParticleSystem.MinMaxGradient(color, new Color(color.r, color.g, color.b, 0.25f));
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.maxParticles = 120;

        ParticleSystem.EmissionModule emission = ps.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new[] { new ParticleSystem.Burst(0f, burstCount) });

        ParticleSystem.ShapeModule shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = radius;

        ParticleSystemRenderer renderer = ps.GetComponent<ParticleSystemRenderer>();
        renderer.sharedMaterial = material;
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        renderer.sortMode = ParticleSystemSortMode.Distance;

        EditorUtility.SetDirty(go);
        EditorUtility.SetDirty(ps);
    }

    private static void AddShootingStar(Transform parent, Material particleMaterial, Material lineMaterial)
    {
        Transform existing = parent.Find("VX_Shooting_Star");
        GameObject star;
        if (existing != null)
        {
            Object.DestroyImmediate(existing.gameObject);
        }

        star = new GameObject("VX_Shooting_Star", typeof(LineRenderer), typeof(ParticleSystem), typeof(Light));
        star.transform.SetParent(parent, false);

        LineRenderer line = star.GetComponent<LineRenderer>();
        line.positionCount = 2;
        line.useWorldSpace = true;
        line.numCapVertices = 8;
        line.numCornerVertices = 4;
        line.sharedMaterial = lineMaterial;
        line.shadowCastingMode = ShadowCastingMode.Off;
        line.receiveShadows = false;
        Gradient lineGradient = new Gradient();
        lineGradient.SetKeys(
            new GradientColorKey[]
            {
                new GradientColorKey(new Color(0.9f, 1f, 1f), 0f),
                new GradientColorKey(new Color(0.35f, 0.9f, 1f), 0.65f),
                new GradientColorKey(new Color(0.05f, 0.1f, 0.16f), 1f)
            },
            new GradientAlphaKey[]
            {
                new GradientAlphaKey(0.95f, 0f),
                new GradientAlphaKey(0.45f, 0.45f),
                new GradientAlphaKey(0f, 1f)
            }
        );
        line.colorGradient = lineGradient;
        line.widthMultiplier = 0.07f;

        ParticleSystem ps = star.GetComponent<ParticleSystem>();
        ParticleSystem.MainModule main = ps.main;
        main.loop = true;
        main.playOnAwake = false;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.18f, 0.48f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.04f, 0.18f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.035f, 0.09f);
        main.startColor = new ParticleSystem.MinMaxGradient(new Color(0.8f, 1f, 1f, 0.85f), new Color(0.25f, 0.8f, 1f, 0.2f));
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.maxParticles = 80;
        ParticleSystem.EmissionModule emission = ps.emission;
        emission.rateOverTime = 32f;
        ParticleSystem.ShapeModule shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.04f;
        ParticleSystemRenderer renderer = ps.GetComponent<ParticleSystemRenderer>();
        renderer.sharedMaterial = particleMaterial;
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        renderer.sortMode = ParticleSystemSortMode.Distance;

        Light light = star.GetComponent<Light>();
        light.type = LightType.Point;
        light.color = new Color(0.58f, 0.92f, 1f);
        light.range = 4f;
        light.intensity = 0f;
        light.shadows = LightShadows.None;

        System.Type shootingStarType = FindType("ShootingStarAnimator");
        if (shootingStarType != null)
        {
            Component animator = star.AddComponent(shootingStarType);
            SetVector3Field(animator, "startPoint", new Vector3(-8.4f, 5.7f, 6.4f));
            SetVector3Field(animator, "endPoint", new Vector3(8.6f, 3.9f, 6.2f));
            SetFloatField(animator, "minDelay", 6f);
            SetFloatField(animator, "maxDelay", 13f);
            SetFloatField(animator, "travelDuration", 1.28f);
        }
    }

    private static GameObject FindSceneObjectByName(string objectName)
    {
        GameObject[] objects = Resources.FindObjectsOfTypeAll<GameObject>();
        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i] == null || objects[i].name != objectName)
            {
                continue;
            }

            if (!objects[i].scene.IsValid())
            {
                continue;
            }

            return objects[i];
        }

        return null;
    }

    private static void AddOrConfigureArrowAnimator(GameObject target)
    {
        System.Type animatorType = FindType("HolographicArrowAnimator");
        if (animatorType == null)
        {
            return;
        }

        Component animator = target.GetComponent(animatorType);
        if (animator == null)
        {
            animator = target.AddComponent(animatorType);
        }

        SetFloatField(animator, "pressScale", 1.12f);
        SetFloatField(animator, "successScale", 1.18f);
        SetFloatField(animator, "errorShake", 10f);
        SetColorField(animator, "idleColor", new Color(0.2f, 1f, 1f, 1f));
        SetColorField(animator, "pressColor", new Color(0.45f, 1f, 0.82f, 1f));
        SetColorField(animator, "successColor", new Color(0.38f, 1f, 0.42f, 1f));
        SetColorField(animator, "errorColor", new Color(1f, 0.2f, 0.12f, 1f));
        EditorUtility.SetDirty(animator);
    }

    private static void SetFloatField(Component component, string fieldName, float value)
    {
        System.Reflection.FieldInfo field = component.GetType().GetField(fieldName);
        if (field != null)
        {
            field.SetValue(component, value);
        }
    }

    private static void SetVector3Field(Component component, string fieldName, Vector3 value)
    {
        System.Reflection.FieldInfo field = component.GetType().GetField(fieldName);
        if (field != null)
        {
            field.SetValue(component, value);
        }
    }

    private static void SetColorField(Component component, string fieldName, Color value)
    {
        System.Reflection.FieldInfo field = component.GetType().GetField(fieldName);
        if (field != null)
        {
            field.SetValue(component, value);
        }
    }

    private static System.Type FindType(string typeName)
    {
        System.Type type = System.Type.GetType(typeName);
        if (type != null)
        {
            return type;
        }

        System.Reflection.Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
        for (int i = 0; i < assemblies.Length; i++)
        {
            type = assemblies[i].GetType(typeName);
            if (type != null)
            {
                return type;
            }
        }

        return null;
    }

    private static void PolishFinalImage(Material haloMaterial)
    {
        GameObject image = GameObject.Find("Imagen25Anios");
        if (image == null)
        {
            return;
        }

        image.transform.position = new Vector3(0.2096f, 1.55f, 3.2f);
        image.transform.eulerAngles = new Vector3(90f, 180f, 0f);
        image.transform.localScale = Vector3.one * 0.58f;

        ImageReveal reveal = image.GetComponent<ImageReveal>();
        if (reveal == null) reveal = image.AddComponent<ImageReveal>();
        reveal.riseDistance = 3.4f;
        reveal.revealDuration = 3.8f;
        reveal.overshootScale = 1.075f;
        reveal.settleSpin = 8f;
        reveal.revealCurve = new AnimationCurve(
            new Keyframe(0f, 0f, 0f, 0.6f),
            new Keyframe(0.62f, 0.88f, 1.6f, 1.2f),
            new Keyframe(1f, 1f, 0.2f, 0f)
        );

        Renderer renderer = image.GetComponent<Renderer>();
        if (renderer != null && renderer.sharedMaterial != null)
        {
            Material imageMaterial = renderer.sharedMaterial;
            imageMaterial.EnableKeyword("_EMISSION");
            if (imageMaterial.HasProperty("_EmissionColor"))
            {
                imageMaterial.SetColor("_EmissionColor", new Color(0.08f, 0.16f, 0.2f, 1f));
            }
            EditorUtility.SetDirty(imageMaterial);
        }

        Transform oldHalo = image.transform.Find("VX_Final_Image_Reveal_Halo");
        if (oldHalo != null)
        {
            Object.DestroyImmediate(oldHalo.gameObject);
        }

        Transform oldLight = image.transform.Find("VX_Final_Image_Reveal_Light");
        if (oldLight != null)
        {
            Object.DestroyImmediate(oldLight.gameObject);
        }

        GameObject lightGo = new GameObject("VX_Final_Image_Reveal_Light");
        lightGo.transform.SetParent(image.transform, false);
        lightGo.transform.localPosition = new Vector3(0f, 0.08f, -0.65f);
        Light light = lightGo.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = new Color(0.42f, 0.88f, 1f);
        light.range = 4.5f;
        light.intensity = 0f;
        light.shadows = LightShadows.None;
    }
}
