# PLAN Gom Core Native Chung — DragonBoy_Shared

> Scope: Chỉ Native (PC + Android + iOS). Loại trừ Unity PC cũ (`Dragonboy250_PC_projectbuild`, net35).

## 1. Hiện trạng Kiến trúc Đã Thống Nhất

- **Toàn bộ Core dùng chung duy nhất nằm tại `C:\ModNRO\DragonBoy_Shared\`**:
  - `Src/` (439 file C#: GameScr, GameCanvas, Char, Mod/UI 16 file, Service, Session_ME, TField Telex IME, SafeArea, ModFloatingHUD, ModUIMultiTab, GameTabManager, GameTabStatus...)
  - `Engine/Compatibility/UnityEngine/` (Shim tương thích đồ họa, toán học, âm thanh, clipboard và font TrueType Unicode)
  - `Engine/Graphics/RenderManager.cs` (PC Raylib Virtual resolution, Dynamic Scaling, Frame Pacing)
  - `Engine/Platform/IGraphicsDriver.cs`, `IInputDriver.cs`, `IPlatformBridge.cs`, `RaylibGraphicsDriver.cs`, `RaylibInputDriver.cs`
  - `Assets/`, `Backgrounds/`, `custom_logo.png`
  - `SharedCore.csproj` (.NET 8 Library dùng chung, chuẩn hóa IDE IntelliSense)
- **Các Host mỏng (Thin Shells)**:
  - PC (`C:\ModNRO\DragonBoy_Net8_Native\`): Chỉ giữ `Program.cs`, icon `trihienkun.ico`, và `DragonBoy_Net8_Native.csproj` link sang `DragonBoy_Shared`.
  - Android (`C:\ModNRO\DragonBoy_Mobile\Android\`): Chỉ giữ `MainActivity.cs`, `GameView.cs`, `AndroidGraphicsBackend.cs`, `AndroidInputBridge.cs`, `AndroidAssetLoader.cs`, `AndroidPlatformBridge.cs` và `DragonBoy_Android.csproj` link sang `DragonBoy_Shared`.
  - iOS (`C:\ModNRO\DragonBoy_Mobile\iOS\`): Plist manifests và `DragonBoy_iOS.csproj` link sang `DragonBoy_Shared`.

## 2. Cấu trúc Thư mục

```
C:\ModNRO\DragonBoy_Shared\          <- Core dùng chung duy nhất (Single Source of Truth)
  Src/                              <- 100% gameplay, screens, models, UI, Mod logic
  Engine/
    Compatibility/UnityEngine/      <- Unity API compatibility shims & TrueType font
    Graphics/                       <- RenderManager & Graphic pipelines
    Platform/                       <- Platform bridges & drivers
    Network/
  Assets/                           <- Game assets (x1, x2, x3, x4, sounds, fonts)
  Backgrounds/                      <- Dynamic Git landscapes
  custom_logo.png                   <- Logo TriHienKun
  SharedCore.csproj                 <- net8.0 Class Library

C:\ModNRO\DragonBoy_Net8_Native\      <- Host PC mỏng
  Program.cs, *.ico, *.csproj, README.md

C:\ModNRO\DragonBoy_Mobile\Android\   <- Host Android mỏng
  MainActivity.cs, GameView.cs, AndroidGraphicsBackend.cs, AndroidInputBridge.cs, AndroidAssetLoader.cs, AndroidPlatformBridge.cs, *.csproj

C:\ModNRO\DragonBoy_Mobile\iOS\       <- Host iOS mỏng
  Info*.plist, *.csproj
```

## 3. Tiến Độ Thực Hiện

- [x] B0. Đóng băng & ghi nhận lệnh build pass
- [x] B1. Thống nhất toàn bộ `Src/`, `Engine/`, `Assets/`, `Backgrounds/`, `custom_logo.png` vào `DragonBoy_Shared/`
- [x] B2. Thêm `DragonBoy_Shared/SharedCore.csproj` (.NET 8 Library, biên dịch 0 Error 0 Warning)
- [x] B3. Đổi link csproj của 3 host sang `DragonBoy_Shared`
- [x] B4. Xóa bỏ hoàn toàn mã nguồn trùng lặp/rời rạc cũ trong `DragonBoy_Net8_Native/`
- [x] B5. Thống nhất UI/UX 100% qua `mGraphics` trong `DragonBoy_Shared`:
  - `ModUI` (11 tabs, bao gồm Tab Nhân Bản Đa Tab 1-6)
  - `ModFloatingHUD` (Menu HUB nổi bong bóng tròn mượt mà, modal shield chống click-through)
  - `ModArrowButton`, `ModBossNotice`, `ModMapEntityHUD`, `ModLogo` (Căn chỉnh Safe Area mép màn hình)
  - TrueType font Unicode HD sắc nét
- [x] B6. Build và kiểm thử thực tế PC và Android đạt tiêu chuẩn Production-Ready.

## 4. Nhật Ký Phân Tích & Khắc Phục Lỗi Kỹ Thuật (Bug Post-Mortem & Fixes)

### Lỗi Render Khi Phóng To Cửa Sổ / Fullscreen Dạng Cửa Sổ (Windowed Maximize)
* **Hiện tượng**:
  * Khi phóng to cực đại cửa sổ (Maximize / Windowed Fullscreen ~1920x1030), nền trời, núi và sprite cây cối bị các ô vuông bậc thang đè lên hoặc bị cắt khúc, mặt đất có hiện tượng sọc gạch.
* **Nguyên nhân gốc rễ (Root Cause Analysis)**:
  1. **Tràn biên mảng Camera TileMap**:
     * Khi chiều rộng cửa sổ `gW` tăng lên (ví dụ 960px logic), `gssw = gW / 24 + 2 = 42` ô gạch.
     * Biến `gssxe = gssx + gssw` vượt quá chiều rộng bản đồ thực tế (`tmw = 30`).
     * Vòng lặp vẽ `maps[k * tmw + j]` đọc vượt ra ngoài kích thước mảng và ném ngoại lệ `IndexOutOfRangeException`, khiến tiến trình vẽ bản đồ bị ngắt giữa chừng và để lại các mảng trống dạng bậc thang.
  2. **Lỗi logic hòa trộn màu `mGraphics.blend()`**:
     * Giá trị kênh màu `Color.r, g, b` chạy trong thang `0.0f - 1.0f`, nhưng code cũ lại so sánh với `255f` và không kiểm tra ngưỡng alpha trong suốt (`isNotTranColor`).
     * Điều này làm các pixel trong suốt xung quanh sprite của cây cối và vật thể phụ (`BgItem`) bị tô đặc màu xám/xanh đè lên nền trời.
  3. **Lệch tọa độ nền mây/núi khi đổi độ phân giải**:
     * `getYBackground(typeBg)` không được gọi cập nhật lại khi kích thước khung nhìn `gH` thay đổi.
  4. **Hụt biên phải của tầng lặp hình nền**:
     * Vòng lặp `for (int i = -offset; i < maxDrawW; i += bgW[num])` bị dừng sớm ở mép phải trước khi phủ kín `maxDrawW + bgW[num]`.
* **Giải pháp đã thực hiện**:
  * **`Src/GameScr/GameScr.Camera.cs`**: Bổ sung kiểm tra chặn trên bắt buộc:
    ```csharp
    if (gssxe > TileMap.tmw) gssxe = TileMap.tmw;
    if (gssye > TileMap.tmh) gssye = TileMap.tmh;
    ```
  * **`Src/TileMap/TileMap.Paint.cs`**: Thêm điều kiện an toàn `if (j < 0 || j >= tmw || k < 0 || k >= tmh) continue;` trên toàn bộ các hàm vẽ map (`paintTilemap`, `paintTilemapLOW`, `paintTilemapSuperLow`, `paintOutTilemap`, `paintBorderTiles`).
  * **`Src/mGraphics/mGraphics.Image.cs`**: Viết lại hàm `blend()` với `System.Math.Clamp` và lọc bỏ pixel trong suốt `if (color2.a > 0.05f)`.
  * **`Src/Mod/Graphics/ModGraphics.cs`**: Tự động gọi `GameCanvas.getYBackground(GameCanvas.typeBg)` ngay khi phát hiện cửa sổ thay đổi kích thước.
  * **`Src/GameCanvas/GameCanvas.Paint.Part1.cs`**: Mở rộng vòng lặp vẽ nền phủ kín `maxDrawW + bgW[num]`.

## 5. Tiêu Chuẩn Xác Minh (Verification)

- `dotnet build DragonBoy_Shared/SharedCore.csproj -c Release` $\rightarrow$ **0 Warning(s), 0 Error(s)**
- `dotnet build DragonBoy_Net8_Native/DragonBoy_Net8_Native.csproj -c Release` $\rightarrow$ **0 Warning(s), 0 Error(s)**
- Phóng to cửa sổ, Maximize, Fullscreen hiển thị sắc nét, liền mạch, không còn lỗi cắt hình hay sọc gạch.
