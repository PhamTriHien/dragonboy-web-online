/// <summary>
/// CLASS NewPanel: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
internal class NewPanel : IActionListener, IChatable
{
	/// <summary>
	/// Thực hiện vẽ toàn bộ đối tượng và các lớp hiển thị lên canvas mGraphics.
	/// </summary>
	public void paint(mGraphics g)
	{
	}

	/// <summary>
	/// Cập nhật trạng thái logic của đối tượng theo từng chu kỳ khung hình.
	/// </summary>
	public void update()
	{
	}

	/// <summary>
	/// Thực thi hành động tương tác khi người dùng kích hoạt lệnh hoặc nút bấm.
	/// </summary>
	public void perform(int idAction, object p)
	{
	}

	/// <summary>
	/// Bộ xử lý sự kiện tiếp nhận và phản hồi onCancelChat.
	/// </summary>
	public void onCancelChat()
	{
	}

	/// <summary>
	/// Bộ xử lý sự kiện tiếp nhận và phản hồi onChatFromMe.
	/// </summary>
	public void onChatFromMe(string text, string to)
	{
	}
}
