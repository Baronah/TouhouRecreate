using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Youmu_A : PlayerBase
{
    protected override void Shoot()
    {
        if (!CanShoot()) return;

        if (!focusing) CreateUnfocusedShots();
        else CreateFocusedShot();
    }

    void CreateUnfocusedShots()
    {
        Vector3[] shotDirections;

        shotDirections = GetUnfocusedShots();
        for (int i = 0; i < shotDirections.Length; i++)
        {
            CreateProjectileAndShoot(
                BulletData.BulletType.TRIANGLE_GREY,
                1f, 
                500f, 
                200f, 
                shotDirections[i]
            );
        }
    }

    readonly Vector3[] unfocus_default = new Vector3[] { Vector3.up };
    readonly Vector3[] unfocus_P1 = new Vector3[] { Vector3.up, new Vector3(0.3f, 1f), new Vector3(-0.3f, 1) };
    readonly Vector3[] unfocus_P2 = new Vector3[] { Vector3.up, new Vector3(0.25f, 1f), new Vector3(-0.25f, 1), new Vector3(0.5f, 1f), new Vector3(-0.5f, 1f) };

    Vector3[] GetUnfocusedShots()
    {
        if (firepower >= 3.0f) return unfocus_P2;
        if (firepower >= 2.0f) return unfocus_P1;
        return unfocus_default;
    }

    void CreateFocusedShot()
    {
        Vector3[] shotPositions = GetFocusedShots();

        for (int i = 0; i < shotPositions.Length; i++)
        {
            CreateProjectileAndShoot(
                BulletData.BulletType.TRIANGLE_GREY,
                1f,
                500f,
                200f,
                Vector3.up,
                transform.position + shotPositions[i]
            );
        }
    }

    readonly Vector3[] focus_default = new Vector3[] { Vector3.zero };
    readonly Vector3[] focus_P1 = new Vector3[] { new(-6f, 0f), new(6f, 0f) };
    readonly Vector3[] focus_P2 = new Vector3[] { Vector3.zero, new(-10f, -5f), new(10f, -5f) };
    Vector3[] GetFocusedShots()
    {
        if (firepower >= 3.0f) return focus_P2;
        if (firepower >= 2.0f) return focus_P1;
        return focus_default;
    }

    protected override void Boom()
    {

    }
}
