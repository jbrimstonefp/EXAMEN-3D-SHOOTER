// Interface for state machine states used by enemy AI
public interface IState
{
    void Enter();
    void Execute();
    void Exit();
}
