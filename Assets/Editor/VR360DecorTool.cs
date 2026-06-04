using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class VR360DecorTool
{
    private const string RootName = "VR360_Space_Decor_Root";
    private const string Folder = "Assets/VisualUpgrade/VR360";

    [MenuItem("Tools/SphereXRR/Add VR 360 Space Decor")]
    public static void AddDecor()
    {
        GameObject existing = GameObject.Find(RootName);
        if (existing != null)
        {
            Object.DestroyImmediate(existing);
        }

        EnsureFolders();

        Shader unlit = Shader.Find("Universal Render Pipeline/Unlit") ?? Shader.Find("Unlit/Texture");
        Shader lit = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
        Shader particleShader = Shader.Find("Universal Render Pipeline/Particles/Unlit") ?? Shader.Find("Particles/Standard Unlit");

        Material starMat = MakeMat("VR360_Star_Dots", particleShader, new Color(0.72f, 0.95f, 1f, 0.72f), null, true);
        Material dustMat = MakeMat("VR360_Dust_Points", particleShader, new Color(0.35f, 1f, 0.82f, 0.26f), null, true);
        Material ringMat = MakeMat("VR360_Planet_Rings", unlit, new Color(0.3f, 0.9f, 1f, 0.38f), null, true);
        Material fallbackPlanetMat = MakeMat("VR360_Fallback_Planet", lit, new Color(0.28f, 0.48f, 0.95f, 1f), null, false);

        Texture2D nebulaTex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Plugins/CW/SpaceGraphicsToolkit/Features/Nebula/Examples/Textures/Nebula.png");
        Texture2D nebulaTex2 = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Plugins/CW/SpaceGraphicsToolkit/Features/Starfield/Examples/Textures/Nebula1.png") ?? nebulaTex;
        Material nebulaMatA = MakeMat("VR360_Nebula_BlueViolet", unlit, new Color(0.55f, 0.42f, 1f, 0.26f), nebulaTex, true);
        Material nebulaMatB = MakeMat("VR360_Nebula_CyanRose", unlit, new Color(0.28f, 0.95f, 1f, 0.20f), nebulaTex2, true);

        Material planetMatA = AssetDatabase.LoadAssetAtPath<Material>("Assets/Plugins/CW/SpaceGraphicsToolkit/Features/Planet/Examples/Materials/Planet (03).mat") ?? fallbackPlanetMat;
        Material planetMatB = AssetDatabase.LoadAssetAtPath<Material>("Assets/Plugins/CW/SpaceGraphicsToolkit/Features/Planet/Examples/Materials/Planet (09).mat") ?? fallbackPlanetMat;
        Material planetMatC = AssetDatabase.LoadAssetAtPath<Material>("Assets/Plugins/CW/SpaceGraphicsToolkit/Features/Planet/Examples/Materials/Planet HD2.mat") ?? fallbackPlanetMat;
        GameObject asteroidPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Plugins/CW/SpaceGraphicsToolkit/Features/Shared/Examples/Models/Asteroid.prefab");

        GameObject root = new GameObject(RootName);
        root.transform.position = Vector3.zero;

        GameObject p1 = CreatePlanet(root.transform, "VR360_Distant_Teal_Planet", new Vector3(-27f, 9f, 19f), 4.8f, planetMatA, new Color(0.35f, 0.9f, 1f), 2.8f);
        CreateRing(p1.transform, "VR360_Distant_Teal_Planet_Ring", 1.35f, new Color(0.35f, 1f, 1f, 0.42f), new Vector3(18f, 0f, 24f), ringMat);
        GameObject p2 = CreatePlanet(root.transform, "VR360_Rear_Rose_Planet", new Vector3(24f, 6.5f, -24f), 3.9f, planetMatB, new Color(1f, 0.38f, 0.55f), -2.1f);
        CreateRing(p2.transform, "VR360_Rear_Rose_Planet_Orbit", 1.55f, new Color(1f, 0.35f, 0.75f, 0.32f), new Vector3(-8f, 0f, -31f), ringMat);
        CreatePlanet(root.transform, "VR360_High_Gold_Moon", new Vector3(-10f, 18f, -30f), 2.1f, planetMatC, new Color(1f, 0.78f, 0.36f), 3.7f);

        CreateNebula(root.transform, "VR360_Nebula_Back_Left", new Vector3(-34f, 7f, -18f), new Vector3(14f, 9f, 1f), nebulaMatA, 18f);
        CreateNebula(root.transform, "VR360_Nebula_Back_Right", new Vector3(31f, 10f, 13f), new Vector3(12f, 8f, 1f), nebulaMatB, -26f);
        CreateNebula(root.transform, "VR360_Nebula_Overhead", new Vector3(2f, 24f, 4f), new Vector3(18f, 9f, 1f), nebulaMatA, 92f);
        CreateNebula(root.transform, "VR360_Nebula_Low_Rear", new Vector3(5f, -3f, -34f), new Vector3(17f, 7f, 1f), nebulaMatB, 9f);

        CreateStarShell(root.transform, starMat);
        CreateCosmicDust(root.transform, dustMat);
        CreateAsteroids(root.transform, asteroidPrefab);
        CreateOrbitLine(root.transform, ringMat);

        EditorUtility.SetDirty(root);
        EditorSceneManager.MarkSceneDirty(root.scene);
        EditorSceneManager.SaveScene(root.scene);
        AssetDatabase.SaveAssets();
        Debug.Log("VR360 space decor created around the full scene.");
    }

    private static void EnsureFolders()
    {
        if (!AssetDatabase.IsValidFolder("Assets/VisualUpgrade"))
            AssetDatabase.CreateFolder("Assets", "VisualUpgrade");
        if (!AssetDatabase.IsValidFolder(Folder))
            AssetDatabase.CreateFolder("Assets/VisualUpgrade", "VR360");
    }

    private static Material MakeMat(string name, Shader shader, Color color, Texture texture, bool transparent)
    {
        string path = Folder + "/" + name + ".mat";
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (mat == null)
        {
            mat = new Material(shader);
            AssetDatabase.CreateAsset(mat, path);
        }

        mat.shader = shader;
        if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", color);
        if (mat.HasProperty("_Color")) mat.SetColor("_Color", color);
        if (texture != null)
        {
            if (mat.HasProperty("_BaseMap")) mat.SetTexture("_BaseMap", texture);
            if (mat.HasProperty("_MainTex")) mat.SetTexture("_MainTex", texture);
        }

        if (transparent)
        {
            mat.SetFloat("_Surface", 1f);
            mat.SetFloat("_Blend", 0f);
            mat.renderQueue = 3000;
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        }

        EditorUtility.SetDirty(mat);
        return mat;
    }

    private static GameObject CreatePlanet(Transform parent, string name, Vector3 position, float scale, Material mat, Color lightColor, float rotateSpeed)
    {
        GameObject planet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        planet.name = name;
        planet.transform.SetParent(parent, true);
        planet.transform.position = position;
        planet.transform.localScale = Vector3.one * scale;
        Object.DestroyImmediate(planet.GetComponent<Collider>());
        planet.GetComponent<Renderer>().sharedMaterial = mat;

        RotateObject rotate = planet.AddComponent<RotateObject>();
        rotate.speed = rotateSpeed;

        GameObject lightGo = new GameObject(name + "_Rim_Light");
        lightGo.transform.SetParent(planet.transform, false);
        lightGo.transform.localPosition = new Vector3(-0.4f, 0.35f, -0.9f);
        Light light = lightGo.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = lightColor;
        light.intensity = 0.65f;
        light.range = scale * 3.6f;
        light.shadows = LightShadows.None;

        return planet;
    }

    private static void CreateRing(Transform parent, string name, float radius, Color color, Vector3 euler, Material material)
    {
        GameObject ring = new GameObject(name);
        ring.transform.SetParent(parent, false);
        ring.transform.localRotation = Quaternion.Euler(euler);
        LineRenderer lr = ring.AddComponent<LineRenderer>();
        lr.useWorldSpace = false;
        lr.loop = true;
        lr.positionCount = 96;
        lr.widthMultiplier = 0.035f;
        lr.sharedMaterial = material;
        lr.startColor = color;
        lr.endColor = color;
        for (int i = 0; i < 96; i++)
        {
            float a = (i / 96f) * Mathf.PI * 2f;
            lr.SetPosition(i, new Vector3(Mathf.Cos(a) * radius, 0f, Mathf.Sin(a) * radius));
        }
    }

    private static void CreateNebula(Transform parent, string name, Vector3 position, Vector3 scale, Material mat, float roll)
    {
        GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        quad.name = name;
        quad.transform.SetParent(parent, true);
        quad.transform.position = position;
        quad.transform.localScale = scale;
        quad.transform.LookAt(Vector3.up * 2.2f);
        quad.transform.Rotate(0f, 0f, roll, Space.Self);
        Object.DestroyImmediate(quad.GetComponent<Collider>());
        quad.GetComponent<Renderer>().sharedMaterial = mat;
    }

    private static void CreateStarShell(Transform parent, Material mat)
    {
        GameObject stars = new GameObject("VR360_Star_Particle_Shell");
        stars.transform.SetParent(parent, false);
        ParticleSystem ps = stars.AddComponent<ParticleSystem>();
        ParticleSystem.MainModule main = ps.main;
        main.loop = true;
        main.playOnAwake = true;
        main.duration = 9999f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.maxParticles = 520;
        main.startLifetime = 9999f;
        main.startSpeed = 0f;
        main.startSize = new ParticleSystem.MinMaxCurve(0.035f, 0.095f);
        main.startColor = new ParticleSystem.MinMaxGradient(new Color(0.7f, 0.92f, 1f, 0.52f), new Color(1f, 0.92f, 0.72f, 0.62f));
        ParticleSystem.EmissionModule emission = ps.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new[] { new ParticleSystem.Burst(0f, 520) });
        ParticleSystem.ShapeModule shape = ps.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 42f;
        ParticleSystemRenderer renderer = ps.GetComponent<ParticleSystemRenderer>();
        renderer.sharedMaterial = mat;
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        ps.Emit(520);
    }

    private static void CreateCosmicDust(Transform parent, Material mat)
    {
        GameObject dust = new GameObject("VR360_Slow_Cosmic_Dust");
        dust.transform.SetParent(parent, false);
        ParticleSystem ps = dust.AddComponent<ParticleSystem>();
        ParticleSystem.MainModule main = ps.main;
        main.loop = true;
        main.playOnAwake = true;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.maxParticles = 140;
        main.startLifetime = new ParticleSystem.MinMaxCurve(18f, 32f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.02f, 0.09f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.04f, 0.14f);
        main.startColor = new ParticleSystem.MinMaxGradient(new Color(0.15f, 1f, 0.75f, 0.12f), new Color(0.8f, 0.35f, 1f, 0.1f));
        ParticleSystem.EmissionModule emission = ps.emission;
        emission.rateOverTime = 5f;
        ParticleSystem.ShapeModule shape = ps.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 24f;
        ParticleSystemRenderer renderer = ps.GetComponent<ParticleSystemRenderer>();
        renderer.sharedMaterial = mat;
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
    }

    private static void CreateAsteroids(Transform parent, GameObject prefab)
    {
        Vector3[] positions =
        {
            new Vector3(-17f, 4.2f, 14f), new Vector3(-22f, 7.5f, -9f), new Vector3(18f, 5.2f, -14f),
            new Vector3(25f, 8.5f, 5f), new Vector3(9f, 12f, 23f), new Vector3(-7f, 10f, -25f),
            new Vector3(31f, 3f, -2f), new Vector3(-29f, 5f, 1f), new Vector3(4f, -1.5f, 28f),
            new Vector3(-3f, 16f, 18f), new Vector3(16f, 14f, -22f), new Vector3(-20f, 13f, 22f)
        };

        for (int i = 0; i < positions.Length; i++)
        {
            GameObject asteroid = prefab != null ? (GameObject)PrefabUtility.InstantiatePrefab(prefab) : GameObject.CreatePrimitive(PrimitiveType.Sphere);
            asteroid.name = "VR360_Background_Asteroid_" + (i + 1).ToString("00");
            asteroid.transform.SetParent(parent, true);
            asteroid.transform.position = positions[i];
            asteroid.transform.localScale = Vector3.one * Random.Range(0.28f, 0.72f);
            asteroid.transform.rotation = Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
            foreach (Collider collider in asteroid.GetComponentsInChildren<Collider>())
                Object.DestroyImmediate(collider);
            RotateObject rotate = asteroid.AddComponent<RotateObject>();
            rotate.speed = Random.Range(-8f, 8f);
        }
    }

    private static void CreateOrbitLine(Transform parent, Material mat)
    {
        GameObject orbit = new GameObject("VR360_Faint_Galactic_Orbit_Line");
        orbit.transform.SetParent(parent, false);
        orbit.transform.position = new Vector3(0f, 5.5f, 0f);
        orbit.transform.rotation = Quaternion.Euler(8f, 0f, 17f);
        LineRenderer lr = orbit.AddComponent<LineRenderer>();
        lr.useWorldSpace = false;
        lr.loop = true;
        lr.positionCount = 160;
        lr.widthMultiplier = 0.025f;
        lr.sharedMaterial = mat;
        lr.startColor = new Color(0.35f, 1f, 1f, 0.22f);
        lr.endColor = new Color(0.85f, 0.45f, 1f, 0.16f);
        for (int i = 0; i < 160; i++)
        {
            float a = i / 160f * Mathf.PI * 2f;
            lr.SetPosition(i, new Vector3(Mathf.Cos(a) * 21f, 0f, Mathf.Sin(a) * 21f));
        }
    }
}
