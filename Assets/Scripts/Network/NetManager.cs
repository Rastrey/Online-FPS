using Mirror;
using Player;

public class NetManager : NetworkManager
{
	public static NetManager Singleton { get; private set; }
	public event OnAddPlayer OnAddPlayerEvent;
	public delegate void OnAddPlayer(PlayerMovement playerMovement);

	public override void Awake()
	{
		base.Awake();
		Singleton = this;
	}

	public void InvokeOnAddPlayer(PlayerMovement playerMovement)
	{
		OnAddPlayerEvent?.Invoke(playerMovement);
	}
}
