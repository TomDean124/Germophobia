using UnityEngine;
using System;
public class AttackHandler : MonoBehaviour
{
    private float _cooldownTimer = 0f;

    public AttackManager attackData;
    public MutationManager mutationManager;

    void Update()
    {
        _cooldownTimer += Time.deltaTime;
    }

    public void TryUseAttack(GameObject target, string applyToTypeStr)
    {
        if (_cooldownTimer < attackData.attack.cooldownTime) return;

        _cooldownTimer = 0f;

        float damage = attackData.attack.defaultDamage;

        if (Enum.TryParse(applyToTypeStr, out MutationManager.MutationTypes.applyToType applyTo))
        {
            var mutations = GameManager.CheckForValidMutations(mutationManager, applyTo);
            if (mutations.HasValue)
                GameManager.ApplyMutations(gameObject, mutations.Value);
        }

        attackData.Execute(gameObject, target, damage);
    }
}