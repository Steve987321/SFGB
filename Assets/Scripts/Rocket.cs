using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    /// <summary>
    /// this will make the rotation of the vfx rotate towards the player
    /// </summary>
    public Transform PlayerTransform;

    public List<Transform> ExcludeObj = new List<Transform>();

    private float timer = 0;
    void Update()
    {
        // destroy rocket if nothing has been hit for amount of time
        timer += Time.deltaTime;
        if (timer > 7)
        {
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

        VFXManager.Instance.apply_force(hitcontact, 1200, 20);
        VFXManager.Instance.AddBulletHole(hitcontact, hitnormal, VFXManager.BULLET_HOLE_TYPE.EXPLOSION, col.transform);
        VFXManager.Instance.play_sparkHitBig(hitcontact, Quaternion.LookRotation(PlayerTransform.rotation.eulerAngles));
        Destroy(this.gameObject);
    }
}
