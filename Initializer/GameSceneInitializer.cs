using static Define;
public class GameSceneInitializer : Initializer
{
    public override void Init()
    {
        DataManager.instance.Init();
        
        GameManager.instance.Init();
        
        UIManager.instance.Init();
        UIManager.instance.UIActiveSetting(UI.GAMEUI, true);
        UIManager.instance.UIActiveSetting(UI.HANDUI, false);
        UIManager.instance.UIActiveSetting(UI.DECKUI, false);

        PathManager.instance.Init();

        BattleManager.instance.Init();
    }
}