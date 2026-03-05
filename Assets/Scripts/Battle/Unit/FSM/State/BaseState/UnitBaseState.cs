

public class UnitBaseState : IState
{
    protected UnitStateMachine stateMachine;

    public UnitBaseState(UnitStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;

        //ECharacterType characterType = stateMachine.Character.CharacterType;
        //EWeaponType weaponType = stateMachine.Character.WeaponType;

        //ChangeFloatType(stateMachine.Character.AnimationData.MeleeTypeParameterHash, (int)weaponType);
        //ChangeFloatType(stateMachine.Character.AnimationData.WeaponTypeParameterHash, (int)characterType);
    }

    public virtual void Enter()
    {

    }

    public virtual void Exit()
    {

    }

    public virtual void PhysicsUpdate()
    {

    }

    public virtual void Update()
    {

    }

    // 局聪皋捞记 包府
    protected void StartAnimation_Bool(int animationHash)
    {
        //stateMachine.Character.Animator.SetBool(animationHash, true);
    }
    protected void StopAnimation_Bool(int animationHash)
    {
        //stateMachine.Character.Animator.SetBool(animationHash, false);
    }
    protected void StartAnimation_Trigger(int animationHash)
    {
        //stateMachine.Character.Animator.SetTrigger(animationHash);
    }
    protected void ChangeFloatType(int animationHash, float type)
    {
        //stateMachine.Character.Animator.SetFloat(animationHash, type);
    }
}
