using System.Collections.Generic;
using UnityEngine;
using static BulletData;

public class BulletRenderer : MonoBehaviour
{
    public static BulletRenderer instance;
    private void Awake()
    {
        instance = this;
    }

    private struct BulletData
    {
        public Vector3 position;
        public Vector3 direction;
        public float speed;
        public float acceleration;
        public float timePassed;
        public Quaternion rotation;
        public BulletType bulletType;
        public bool active;
    }

    private List<BulletData> bullets = new List<BulletData>();
    private List<Matrix4x4> matrices = new List<Matrix4x4>();

    private Mesh bulletMesh;
    private Material bulletMaterial;

    private float fixedDeltaTime;

    void Start()
    {
        // Create simple quad mesh for bullets
        bulletMesh = CreateBulletMesh();
        bulletMaterial = new Material(Shader.Find("Sprites/Default"));
        fixedDeltaTime = Time.fixedDeltaTime;
    }

    Mesh CreateBulletMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = new Vector3[]
        {
            new Vector3(-0.5f, -0.5f, 0),
            new Vector3(0.5f, -0.5f, 0),
            new Vector3(0.5f, 0.5f, 0),
            new Vector3(-0.5f, 0.5f, 0),
        };
        mesh.triangles = new int[] { 0, 2, 1, 0, 3, 2 };
        mesh.RecalculateNormals();
        return mesh;
    }

    public void AddBullet(Vector3 position, Vector3 direction, float speed,
        float acceleration, BulletType bulletType)
    {
        bullets.Add(new BulletData
        {
            position = position,
            direction = direction.normalized,
            speed = speed,
            acceleration = acceleration,
            timePassed = 0,
            rotation = Quaternion.identity,
            bulletType = bulletType,
            active = true
        });
    }

    void FixedUpdate()
    {
        // Update all bullets
        for (int i = 0; i < bullets.Count; i++)
        {
            if (!bullets[i].active) continue;

            BulletData b = bullets[i];
            b.timePassed += fixedDeltaTime;
            b.speed = b.speed + b.acceleration * b.timePassed;

            if (b.speed <= 0 && b.acceleration < 0)
                b.speed = 0;

            b.position += b.direction * b.speed * fixedDeltaTime;
            bullets[i] = b;
        }

        // Build matrices for rendering
        matrices.Clear();
        foreach (var b in bullets)
        {
            if (b.active)
                matrices.Add(Matrix4x4.TRS(b.position, b.rotation, Vector3.one * 0.5f));
        }
    }

    void LateUpdate()
    {
        if (matrices.Count == 0) return;

        Graphics.DrawMeshInstanced(bulletMesh, 0, bulletMaterial, matrices);
    }

    public List<Vector3> GetActiveBulletPositions(BulletType[] types)
    {
        List<Vector3> positions = new List<Vector3>();
        foreach (var b in bullets)
        {
            if (b.active && System.Array.Exists(types, t => t == b.bulletType))
                positions.Add(b.position);
        }
        return positions;
    }

    public void ClearBullets()
    {
        bullets.Clear();
        matrices.Clear();
    }
}