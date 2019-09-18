using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicCreep : Creep {

    public string projectileName;
    public float projectileSpeed;
    public void ShootProjectile()
    {
        GameObject projectile = GameObject.Instantiate((GameObject)Resources.Load(projectileName));
        CreepProjectile cproj = projectile.GetComponent<CreepProjectile>();
        cproj.damage = damage;
        cproj.speed = projectileSpeed;
        cproj.playerTarget = player.gameObject;
        projectile.transform.position = transform.position;
    }
}
