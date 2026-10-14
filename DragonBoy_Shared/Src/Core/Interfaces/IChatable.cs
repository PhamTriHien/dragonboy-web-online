/// <summary>
/// INTERFACE IChatable: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public interface IChatable
{
	void onChatFromMe(string text, string to);

	void onCancelChat();
}
