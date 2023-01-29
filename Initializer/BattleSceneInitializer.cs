using static Define;

public class BattleSceneInitializer : Initializer
{
    public override void Init()
    {
        BattleManager.instance.Init();
    }
}
