using System;

/// <summary>
/// Trình quản lý âm thanh: Nạp và phát các tệp âm thanh hiệu ứng (Sound Effect) và nhạc nền (BGM).
/// </summary>
public partial class SoundMn
{
	/// <summary>
	/// CLASS MediaPlayer: Định nghĩa và điều phối logic trong hệ thống game.
	/// </summary>
	public class MediaPlayer
		{
		}

	/// <summary>
	/// CLASS SoundPool: Định nghĩa và điều phối logic trong hệ thống game.
	/// </summary>
	public class SoundPool
		{
		}

	/// <summary>
	/// CLASS AssetManager: Định nghĩa và điều phối logic trong hệ thống game.
	/// </summary>
	public class AssetManager
		{
		}

	public static bool IsDelAcc;

	public static SoundMn gIz;

	public static bool isSound = true;

	public static float volume = 0.5f;

	private static int MAX_VOLUME = 10;

	public static MediaPlayer[] music;

	public static SoundPool[] sound;

	public static int[] soundID;

	public static int AIR_SHIP;

	public static int RAIN = 1;

	public static int TAITAONANGLUONG = 2;

	public static int GET_ITEM;

	public static int MOVE = 1;

	public static int LOW_PUNCH = 2;

	public static int LOW_KICK = 3;

	public static int FLY = 4;

	public static int JUMP = 5;

	public static int PANEL_OPEN = 6;

	public static int BUTTON_CLOSE = 7;

	public static int BUTTON_CLICK = 8;

	public static int MEDIUM_PUNCH = 9;

	public static int MEDIUM_KICK = 10;

	public static int PANEL_CLICK = 11;

	public static int EAT_PEAN = 12;

	public static int OPEN_DIALOG = 13;

	public static int NORMAL_KAME = 14;

	public static int NAMEK_KAME = 15;

	public static int XAYDA_KAME = 16;

	public static int EXPLODE_1 = 17;

	public static int EXPLODE_2 = 18;

	public static int TRAIDAT_KAME = 19;

	public static int HP_UP = 20;

	public static int THAIDUONGHASAN = 21;

	public static int HOISINH = 22;

	public static int GONG = 23;

	public static int KHICHAY = 24;

	public static int BIG_EXPLODE = 25;

	public static int NAMEK_LAZER = 26;

	public static int NAMEK_CHARGE = 27;

	public static int RADAR_CLICK = 28;

	public static int RADAR_ITEM = 29;

	public static int FIREWORK = 30;

	public static int KAMEX10_0 = 31;

	public static int KAMEX10_1 = 32;

	public static int DESTROY_0 = 33;

	public static int DESTROY_1 = 34;

	public static int MAFUBA_0 = 35;

	public static int MAFUBA_1 = 36;

	public static int MAFUBA_2 = 37;

	public static int DESTROY_2 = 38;

	public bool freePool;

	public int poolCount;

	public static int cout = 1;

	/// <summary>
	/// Khởi tạo và nạp dữ liệu cấu hình cho init.
	/// </summary>
	public static void init(AssetManager ac)
		{
			Sound.setActivity(ac);
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ gI.
	/// </summary>
	public static SoundMn gI()
		{
			if (gIz == null)
			{
				gIz = new SoundMn();
			}
			return gIz;
		}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của getStrOption.
	/// </summary>
	public void getStrOption()
		{
			string text = "[x]   ";
			string text2 = "[  ]   ";
			if (Main.isPC)
			{
				Panel.strCauhinh = new string[4]
				{
					(!Char.isPaintAura) ? (text2 + mResources.aura_off.Trim()) : (text + mResources.aura_off.Trim()),
					(!Char.isPaintAura2) ? (text2 + mResources.aura_off_2.Trim()) : (text + mResources.aura_off_2.Trim()),
					(!GameCanvas.isPlaySound) ? (text2 + mResources.turnOffSound) : (text + mResources.turnOffSound),
					(mGraphics.zoomLevel <= 1) ? (text2 + mResources.x2Screen) : (text + mResources.x1Screen)
				};
				return;
			}
			string text3 = ((GameScr.isAnalog != 0) ? (text + mResources.turnOffAnalog) : (text2 + mResources.turnOnAnalog));
			if (!GameCanvas.isTouch)
			{
				text3 = (GameScr.isPaintChatVip ? (text + mResources.serverchat_off) : (text2 + mResources.serverchat_off));
			}
			Panel.strCauhinh = new string[5]
			{
				(!Char.isPaintAura) ? (text2 + mResources.aura_off.Trim()) : (text + mResources.aura_off.Trim()),
				(!Char.isPaintAura2) ? (text2 + mResources.aura_off_2.Trim()) : (text + mResources.aura_off_2.Trim()),
				(!GameCanvas.isPlaySound) ? (text2 + mResources.turnOffSound) : (text + mResources.turnOffSound),
				(!GameCanvas.lowGraphic) ? (text2 + mResources.cauhinhthap) : (text + mResources.cauhinhthap),
				text3
			};
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ HP_MPup.
	/// </summary>
	public void HP_MPup()
		{
			Sound.playSound(HP_UP, 0.5f);
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ charPunch.
	/// </summary>
	public void charPunch(bool isKick, float volumn)
		{
			if (!Char.myCharz().me)
			{
				volumn /= 2f;
			}
			if (volumn <= 0f)
			{
				volumn = 0.01f;
			}
			int num = Res.random(0, 3);
			if (isKick)
			{
				Sound.playSound((num != 0) ? MEDIUM_KICK : LOW_KICK, 0.1f);
			}
			else
			{
				Sound.playSound((num != 0) ? MEDIUM_PUNCH : LOW_PUNCH, 0.1f);
			}
			poolCount++;
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ thaiduonghasan.
	/// </summary>
	public void thaiduonghasan()
		{
			Sound.playSound(THAIDUONGHASAN, 0.5f);
			poolCount++;
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ rain.
	/// </summary>
	public void rain()
		{
			Sound.playMus(RAIN, 0.3f, loop: true);
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ gongName.
	/// </summary>
	public void gongName()
		{
			Sound.playSound(NAMEK_CHARGE, 0.3f);
			poolCount++;
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ gong.
	/// </summary>
	public void gong()
		{
			Sound.playSound(GONG, 0.2f);
			poolCount++;
		}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của getItem.
	/// </summary>
	public void getItem()
		{
			Sound.playSound(GET_ITEM, 0.3f);
			poolCount++;
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ chatVipToolOption.
	/// </summary>
	public void chatVipToolOption()
		{
			GameScr.isPaintChatVip = !GameScr.isPaintChatVip;
			if (GameScr.isPaintChatVip)
			{
				Rms.saveRMSInt("serverchat", 0);
			}
			else
			{
				Rms.saveRMSInt("serverchat", 1);
			}
			getStrOption();
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ analogToolOption.
	/// </summary>
	public void analogToolOption()
		{
			if (GameScr.isAnalog == 0)
			{
				GameScr.isAnalog = 1;
				Rms.saveRMSInt("analog", GameScr.isAnalog);
				GameScr.setSkillBarPosition();
			}
			else
			{
				GameScr.isAnalog = 0;
				Rms.saveRMSInt("analog", GameScr.isAnalog);
				GameScr.setSkillBarPosition();
			}
			getStrOption();
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ CaseAnalog.
	/// </summary>
	public void CaseAnalog()
		{
			if (!Main.isPC)
			{
				if (!GameCanvas.isTouch)
				{
					chatVipToolOption();
				}
				else
				{
					analogToolOption();
				}
			}
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ CaseSizeScr.
	/// </summary>
	public void CaseSizeScr()
		{
			if (GameCanvas.lowGraphic)
			{
				Rms.saveRMSInt("lowGraphic", 0);
				GameCanvas.startOK(mResources.plsRestartGame, 8885, null);
			}
			else
			{
				Rms.saveRMSInt("lowGraphic", 1);
				GameCanvas.startOK(mResources.plsRestartGame, 8885, null);
			}
			getStrOption();
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ AuraToolOption.
	/// </summary>
	public void AuraToolOption()
		{
			if (Char.isPaintAura)
			{
				Rms.saveRMSInt("isPaintAura", 0);
				Char.isPaintAura = false;
			}
			else
			{
				Rms.saveRMSInt("isPaintAura", 1);
				Char.isPaintAura = true;
			}
			getStrOption();
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ AuraToolOption2.
	/// </summary>
	public void AuraToolOption2()
		{
			if (Char.isPaintAura2)
			{
				Rms.saveRMSInt("isPaintAura2", 0);
				Char.isPaintAura2 = false;
			}
			else
			{
				Rms.saveRMSInt("isPaintAura2", 1);
				Char.isPaintAura2 = true;
			}
			getStrOption();
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ HatToolOption.
	/// </summary>
	public void HatToolOption()
		{
			Service.gI().sendOptHat(0);
		}

	/// <summary>
	/// Cập nhật trạng thái logic của đối tượng theo từng chu kỳ khung hình.
	/// </summary>
	public void update()
		{
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ bigeExlode.
	/// </summary>
	public void bigeExlode()
		{
			Sound.playSound(BIG_EXPLODE, 0.5f);
			poolCount++;
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ explode_1.
	/// </summary>
	public void explode_1()
		{
			Sound.playSound(EXPLODE_1, 0.5f);
			poolCount++;
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ explode_2.
	/// </summary>
	public void explode_2()
		{
			Sound.playSound(EXPLODE_1, 0.5f);
			poolCount++;
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ traidatKame.
	/// </summary>
	public void traidatKame()
		{
			Sound.playSound(TRAIDAT_KAME, 1f);
			poolCount++;
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ namekKame.
	/// </summary>
	public void namekKame()
		{
			Sound.playSound(NAMEK_KAME, 0.3f);
			poolCount++;
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ nameLazer.
	/// </summary>
	public void nameLazer()
		{
			Sound.playSound(NAMEK_LAZER, 0.3f);
			poolCount++;
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ xaydaKame.
	/// </summary>
	public void xaydaKame()
		{
			Sound.playSound(XAYDA_KAME, 0.3f);
			poolCount++;
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ mobKame.
	/// </summary>
	public void mobKame(int type)
		{
			int id = XAYDA_KAME;
			if (type == 13)
			{
				id = NORMAL_KAME;
			}
			Sound.playSound(id, 0.1f);
			poolCount++;
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ charRun.
	/// </summary>
	public void charRun(float volumn)
		{
			if (!Char.myCharz().me)
			{
				volumn /= 2f;
				if (volumn <= 0f)
				{
					volumn = 0.01f;
				}
			}
			if (GameCanvas.gameTick % 8 == 0)
			{
				Sound.playSound(MOVE, volumn);
				poolCount++;
			}
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ monkeyRun.
	/// </summary>
	public void monkeyRun(float volumn)
		{
			if (GameCanvas.gameTick % 8 == 0)
			{
				Sound.playSound(KHICHAY, 0.2f);
				poolCount++;
			}
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ stopMove.
	/// </summary>
	public void stopMove()
		{
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ hoisinh.
	/// </summary>
	public void hoisinh()
		{
			Sound.playSound(HOISINH, 0.5f);
			poolCount++;
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ taitao.
	/// </summary>
	public void taitao()
		{
			Sound.playMus(TAITAONANGLUONG, 0.5f, loop: true);
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ taitaoPause.
	/// </summary>
	public void taitaoPause()
		{
		}

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của isPlayRain.
	/// </summary>
	public bool isPlayRain()
		{
			try
			{
				return Sound.isPlayingSound();
			}
			catch (Exception)
			{
				return false;
			}
		}

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của isPlayAirShip.
	/// </summary>
	public bool isPlayAirShip()
		{
			return false;
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ airShip.
	/// </summary>
	public void airShip()
		{
			cout++;
			if (cout % 2 == 0)
			{
				Sound.playMus(AIR_SHIP, 0.3f, loop: false);
			}
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ pauseAirShip.
	/// </summary>
	public void pauseAirShip()
		{
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ resumeAirShip.
	/// </summary>
	public void resumeAirShip()
		{
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ stopAll.
	/// </summary>
	public void stopAll()
		{
			Sound.stopAllz();
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ backToRegister.
	/// </summary>
	public void backToRegister()
		{
			Session_ME.gI().close();
			GameCanvas.panel.hide();
			GameCanvas.loginScr.actRegister();
			GameCanvas.loginScr.switchToMe();
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ newKame.
	/// </summary>
	public void newKame()
		{
			poolCount++;
			if (poolCount % 15 == 0)
			{
				Sound.playSound(TRAIDAT_KAME, 0.5f);
			}
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ radarItem.
	/// </summary>
	public void radarItem()
		{
			Sound.playSound(RADAR_ITEM, 0.5f);
		}

}
