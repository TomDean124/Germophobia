using UnityEngine;

[CreateAssetMenu(fileName = "NewAttack", menuName = "Combat/Attack")]
public class AttackManager : ScriptableObject
{
    public attackData attack;

    [System.Serializable]
    public struct attackData
    {
        public string attackName;
        public float defaultDamage;
        public float cooldownTime;
        public float Range;
        public bool isAreaEffect;
        public bool UseParticles;
        public ParticleSystem particle;

        public enum affectData { Germ, Player, Enemy, All };
        public affectData effectiveAgainst;
    }

    public virtual void Execute(GameObject attacker, GameObject target, float damage)
    {
        var health = target.GetComponent<Health>();
        if (health != null)
            health.TakeDamage(damage);

        if (attack.UseParticles && attack.particle != null && !attack.isAreaEffect)
        {
            Instantiate(attack.particle, target.transform.position, Quaternion.identity);
            Debug.Log("Execute Called! " + target.name);
        }
        else if (attack.isAreaEffect)
        {
            ParticleSystem Effect = Instantiate(attack.particle, target.transform.position, Quaternion.identity);
            Effect.Play();
            ApplyAreaEffect(target, damage);
        }
    }

    private void ApplyAreaEffect(GameObject _target, float damage)
    {
        Collider[] hits = Physics.OverlapSphere(_target.transform.position, attack.Range);

        foreach (var hit in hits)
        {
            if (hit.CompareTag(attack.effectiveAgainst.ToString()) || attack.effectiveAgainst == attackData.affectData.All)
            {
                var health = hit.GetComponent<Health>();
                if (health != null)
                {
                    health.TakeDamage(damage);
                    Debug.Log("Target Is In Effect Zone: " + _target.name + " Damage Applied: " + damage);
                }
            }
        }
    }
}
