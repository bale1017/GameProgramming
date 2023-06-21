namespace BasePatterns
{
    public enum EnemyState
    {
        Idle,
        Roaming,
        ChaseTarget,
        AttackTarget
    }

    interface IController
    {
        public Health health { get; }
    }
}
