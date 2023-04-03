using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class Rocket : MonoBehaviour
{
   
    public Transform PlayerTransform;

    public List<Transform> ExcludeObj = new List<Transform>();

    private float timer = 0;
    void Update()
    {
        // destroy rocket if nothing has been hit for amount of time
        timer += Time.deltaTime;
        if (timer > 2)
        {
            if (!gameObject.IsDestroyed())
                Destroy(this.gameObject);
        }
        VFXManager.Instance.play_smokepuff(transform.position, transform.rotation);
    }

    void OnCollisionEnter(Collision col)
    {
        if (ExcludeObj.Contains(col.transform))
        {
            return;
        }
        var hitcontact = col.GetContact(0).point;
        var hitnormal = col.GetContact(0).normal;

        // relative to Player position
        var relativePos = PlayerTransform.position - hitcontact;

        DoRocketDamage(PlayerTransform, hitcontact);

        VFXManager.Instance.apply_force(hitcontact, 1200, 20);
        VFXManager.Instance.add_bullet_hole(hitcontact, hitnormal, VFXManager.BULLET_HOLE_TYPE.EXPLOSION, col.transform);
        VFXManager.Instance.play_sparkHitBig(hitcontact, Quaternion.LookRotation(relativePos, Vector3.up));
        Destroy(this.gameObject);
    }

    public static void DoRocketDamage(Transform self, Vector3 point)
    {
        var collisionRoots = new List<Transform>();

        foreach (var collider in Physics.OverlapSphere(point, 10))
            if (collider.transform.root.TryGetComponent<Player>(out var player)
                && !collisionRoots.Contains(collider.transform.root))
            {
                player.DoDamage(collider.transform.root == self ? player.Health.Value / 5f : player.Health.Value / 2f);
                collisionRoots.Add(collider.transform.root);
            }

        AudioManager.Instance.PlayDeafningFX();
        AudioManager.Instance.UpFightMusic();
        AudioManager.Instance.Play_Explosion(point);

        CameraManager.Instance.DoShake(0.5f, 2f, 5);
    }
}
