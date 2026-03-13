using System.Runtime.CompilerServices;
using UnityEngine;

public class CollisionData
{
    public Vector3 Direction;
    public float Speed;
    public Collider OriginCollider;

    public CollisionData(Vector3 dir, float speed, Collider col)
    {
        this.Direction = dir;
        this.Speed = speed;
        this.OriginCollider = col;
    }
}
