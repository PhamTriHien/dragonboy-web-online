
/// <summary>
/// Thành phần phân mảnh (partial) của lớp mResources: Đảm nhiệm phân hệ chuyên trách VN ().
/// </summary>
public partial class mResources
{
	/// <summary>
	/// Khởi tạo và nạp dữ liệu cấu hình cho loadLanguague.
	/// </summary>
	public static void loadLanguague()
		{
			loadLanguague(0);
		}

	/// <summary>
	/// Khởi tạo và nạp dữ liệu cấu hình cho loadLanguague.
	/// </summary>
	public static void loadLanguague(sbyte newLanguage)
		{
			language = 0;
			if (LoginScr.imgTitle == null)
			{
				Image customLogo = GameCanvas.loadCustomImage("custom_logo.png");
				if (customLogo != null)
				{
					LoginScr.imgTitle = customLogo;
				}
				else
				{
					LoginScr.imgTitle = GameCanvas.loadImage("/mainImage/logo1.png");
				}
			}
			T1.load();
			ServerListScreen.linkweb = "http://ngocrongonline.com";
		}

}
