using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
    private List<Pickup> _pulledPickups = new List<Pickup>();
    private List<BulletBehaviour> _pulledBullets = new List<BulletBehaviour>();
    public bool IsEnemy;

    private AudioSource sound;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsEnemy)
        {
            if ((collision.gameObject.layer == 9) && collision.GetComponent<BulletBehaviour>() is BulletBehaviour b2 && !b2.data.isLaser)
            {
                b2.gameObject.layer = 8;
                _pulledBullets.Add(b2);
            }
            return;
        }
        if ((collision.gameObject.layer == 8 || collision.gameObject.layer == 7) && collision.GetComponent<BulletBehaviour>() is BulletBehaviour b && !b.data.isLaser)
        {
            _pulledBullets.Add(b);
            return;
        }

        if (collision.GetComponent<Pickup>() is Pickup p)
        {
            _pulledPickups.Add(p);
            p._speed = Vector2.zero;
            return;
        }
    }

    private void OnDisable()
    {
        _pulledBullets.Clear();
        _pulledPickups.Clear();
        if (sound != null && sound.isPlaying)
        {
            sound.Stop();
        }
    }

    // Update is called once per frame
    void Update()
    {
        var newSound =  SFX.PlayerBlackHole.Play();
        if (newSound != null)
        {
            sound = newSound;
        }
        for (int i=0; i < _pulledBullets.Count; i++)
        {
            if (_pulledBullets[i] == null || !_pulledBullets[i].gameObject.activeSelf)
            {
                _pulledBullets.RemoveAt(i);
                continue;
            }
            Vector2 vec = _pulledBullets[i].transform.position - transform.position;
            Vector2 vec2 = Quaternion.Euler(0,0,25) * vec * 0.92f;
            _pulledBullets[i].ForceSpeed((vec2 - vec) * 12);
        }
        for (int i = 0; i < _pulledPickups.Count; i++)
        {
            if (_pulledPickups[i] == null)
            {
                _pulledPickups.RemoveAt(i);
                continue;
            }
            Vector2 vec = _pulledPickups[i].transform.position - transform.position;
            Vector2 vec2 = Quaternion.Euler(0, 0, 25) * vec * 0.92f;
            _pulledPickups[i]._speed = ((vec2 - vec) * 12);
        }
    }
}
