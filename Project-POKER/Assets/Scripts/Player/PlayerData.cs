[System.Serializable]
public class PlayerData
{
    public string username;
    public int cash;

    public PlayerData(string _username, int _cash)
    {
        username = _username;
        cash = _cash;
    }

    public PlayerData(int _cash)
    {
        cash = _cash;
    }
}
