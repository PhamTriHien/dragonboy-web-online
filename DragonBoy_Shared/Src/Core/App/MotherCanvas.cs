using System;
using UnityEngine;

/// <summary>
/// Canvas gốc chứa toàn bộ màn hình game: Tính toán tỉ lệ co giãn (ScaleGUI) và quản lý cấp độ zoom (zoomLevel).
/// </summary>
public class MotherCanvas
{
	public static MotherCanvas instance;

	public GameCanvas tCanvas;

	public int zoomLevel = 2;

	public Image imgCache;

	private int[] imgRGBCache;

	private int newWidth;

	private int newHeight;

	private int[] output;

	private int OUTPUTSIZE = 20;

	/// <summary>
	/// Thực thi phương thức nghiệp vụ MotherCanvas.
	/// </summary>
	public MotherCanvas()
	{
		checkZoomLevel(getWidth(), getHeight());
	}

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của checkZoomLevel.
	/// </summary>
	public void checkZoomLevel(int w, int h)
	{
		mGraphics.zoomLevel = 2;
		this.zoomLevel = 2;
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của getWidth.
	/// </summary>
	public int getWidth()
	{
		return (int)ScaleGUI.WIDTH;
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của getHeight.
	/// </summary>
	public int getHeight()
	{
		return (int)ScaleGUI.HEIGHT;
	}

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho setChildCanvas.
	/// </summary>
	public void setChildCanvas(GameCanvas tCanvas)
	{
		this.tCanvas = tCanvas;
	}

	/// <summary>
	/// Thực hiện vẽ toàn bộ đối tượng và các lớp hiển thị lên canvas mGraphics.
	/// </summary>
	protected void paint(mGraphics g)
	{
		tCanvas.paint(g);
	}

	/// <summary>
	/// Tiếp nhận và xử lý sự kiện khi một phím trên bàn phím được nhấn xuống.
	/// </summary>
	protected void keyPressed(int keyCode)
	{
		tCanvas.keyPressedz(keyCode);
	}

	/// <summary>
	/// Tiếp nhận và xử lý sự kiện khi một phím trên bàn phím được nhả ra.
	/// </summary>
	protected void keyReleased(int keyCode)
	{
		tCanvas.keyReleasedz(keyCode);
	}

	/// <summary>
	/// Tiếp nhận sự kiện khi chuột hoặc điểm chạm cảm ứng đang được kéo rê.
	/// </summary>
	protected void pointerDragged(int x, int y)
	{
		x /= mGraphics.zoomLevel;
		y /= mGraphics.zoomLevel;
		tCanvas.pointerDragged(x, y);
	}

	/// <summary>
	/// Tiếp nhận sự kiện khi chuột hoặc điểm chạm cảm ứng nhấn xuống màn hình.
	/// </summary>
	protected void pointerPressed(int x, int y)
	{
		x /= mGraphics.zoomLevel;
		y /= mGraphics.zoomLevel;
		tCanvas.pointerPressed(x, y);
	}

	/// <summary>
	/// Tiếp nhận sự kiện khi chuột hoặc điểm chạm cảm ứng được nhả khỏi màn hình.
	/// </summary>
	protected void pointerReleased(int x, int y)
	{
		x /= mGraphics.zoomLevel;
		y /= mGraphics.zoomLevel;
		tCanvas.pointerReleased(x, y);
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của getWidthz.
	/// </summary>
	public int getWidthz()
	{
		int width = getWidth();
		int zoom = (mGraphics.zoomLevel > 0) ? mGraphics.zoomLevel : 1;
		return width / zoom + ((width % zoom != 0) ? 1 : 0);
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của getHeightz.
	/// </summary>
	public int getHeightz()
	{
		int height = getHeight();
		int zoom = (mGraphics.zoomLevel > 0) ? mGraphics.zoomLevel : 1;
		return height / zoom + ((height % zoom != 0) ? 1 : 0);
	}
}
