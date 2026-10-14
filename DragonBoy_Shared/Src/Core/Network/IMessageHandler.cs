/// <summary>
/// INTERFACE IMessageHandler: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public interface IMessageHandler
{
	void onMessage(Message message);

	void onConnectionFail(bool isMain);

	void onDisconnected(bool isMain);

	void onConnectOK(bool isMain);
}
