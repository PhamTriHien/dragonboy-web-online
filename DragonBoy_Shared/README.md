# DRAGONBOY SHARED - UNIFIED CROSS-PLATFORM CORE (.NET 8)
> **Mã nguồn Core trò chơi DragonBoy (Ngọc Rồng Online) dùng chung duy nhất cho đa nền tảng: PC Windows, Android và iOS.**

---

## 1. Tổng Quan Kiến Trúc

Dự án áp dụng mô hình **Single Source of Truth** (Nguyên mẫu mã nguồn duy nhất):
* **`DragonBoy_Shared`**: Nơi lưu trữ 100% logic gameplay, hệ thống màn hình, models, giao diện người dùng (Mod UI), đồ họa `mGraphics`, và font TrueType Unicode.
* **Thin Shells (Host mỏng)**:
  * **PC Windows (`DragonBoy_Net8_Native`)**: Điểm khởi chạy `Program.cs` sử dụng backend đồ họa Raylib OpenGL hardware.
  * **Android (`DragonBoy_Mobile/Android`)**: `MainActivity.cs`, `GameView.cs` sử dụng backend 2D Hardware Canvas, JNI bridges và đa Tab tiến trình.
  * **iOS (`DragonBoy_Mobile/iOS`)**: Cấu hình và bundle manifests.

---

## 2. Cấu Trúc Thư Mục Dự Án

```
C:\ModNRO\
├── DragonBoy_Shared/                 # Core trò chơi dùng chung duy nhất (Single Source of Truth)
│   ├── Src/                         # 100% logic gameplay & UI components
│   │   ├── Core/                    # App, Input, Network, IO, Collections, Interfaces
│   │   │   └── App/                 # GameTabManager, GameTabStatus, SafeArea, MotherCanvas...
│   │   ├── Mod/                     # Toàn bộ hệ thống Mod tính năng & Mod UI
│   │   │   ├── UI/                  # ModUI, ModFloatingHUD, ModUIMultiTab, ModLogo...
│   │   │   ├── Automation/          # Tàn sát, Tự nhặt, Hồi máu, Tốc độ
│   │   │   ├── Boss/                # Báo Boss, Khinh công, Né Broly
│   │   │   ├── Graphics/            # Đồ họa, FPS, Hình nền phong cảnh Git
│   │   │   ├── NextMap/             # Auto chuyển map, Fast travel
│   │   │   ├── Security/            # Bảo mật thông tin, HWID license, Watchdogs
│   │   │   └── Translate/           # Dữ liệu dịch Việt hóa
│   │   ├── mGraphics/               # Engine dựng hình 2D độc lập
│   │   ├── mFont/                   # Hệ thống Typography TrueType Unicode HD
│   │   └── GameCanvas/, GameScr/... # Màn hình và giao diện trò chơi
│   ├── Engine/
│   │   ├── Compatibility/           # UnityEngine shims (Graphics, System, Math, Audio)
│   │   ├── Graphics/                # RenderManager (PC Raylib scaler & frame pacing)
│   │   └── Platform/                # IPlatformBridge, IGraphicsDriver, IInputDriver
│   ├── Assets/                      # Toàn bộ tài nguyên game (x1, x2, x3, x4, fonts, sfx)
│   ├── Backgrounds/                 # Bộ hình nền phong cảnh động
│   ├── custom_logo.png              # Icon Logo TriHienKun
│   └── SharedCore.csproj            # .NET 8 Class Library (Build: 0 Error, 0 Warning)
│
├── DragonBoy_Net8_Native/           # Host PC Windows mỏng (.NET 8 Native AOT)
│   ├── Program.cs                   # Điểm khởi chạy Windows (High Priority, 1ms Timer)
│   ├── trihienkun.ico               # Icon ứng dụng
│   └── DragonBoy_Net8_Native.csproj # Cấu hình liên kết vào DragonBoy_Shared
│
└── DragonBoy_Mobile/                # Host Mobile
    ├── Android/                     # Host Android mỏng (net8.0-android)
    │   ├── MainActivity.cs          # Multi-Tab Activities (:tab1 .. :tab6)
    │   ├── GameView.cs              # SurfaceView 60 FPS integer-scaling
    │   ├── AndroidGraphicsBackend.cs# Zero-GC Hardware Canvas 2D
    │   ├── AndroidInputBridge.cs    # Touch & IME Keyboard Bridge
    │   ├── AndroidPlatformBridge.cs # Cầu nối hệ thống Android
    │   └── DragonBoy_Android.csproj # Cấu hình liên kết vào DragonBoy_Shared
    └── iOS/                         # Host iOS mỏng (net8.0-ios)
        └── DragonBoy_iOS.csproj     # Cấu hình liên kết vào DragonBoy_Shared
```

---

## 3. Hệ Thống UI/UX & Tính Năng Đột Phá

### A. Menu HUB Nổi Đa Nhiệm Trên Màn Hình (`ModFloatingHUD`)
* **Bong bóng nổi thu gọn (Compact Circular Bubble)**:
  * Hình tròn vector mượt mà viền vàng cam hoàng kim NRO.
  * Tích hợp **Logo TriHienKun (`custom_logo.png`)** sắc nét tại tâm.
  * Hỗ trợ **kéo thả tự do mọi vị trí** trên màn hình cảm ứng di động và chuột PC.
  * Tự động căn chỉnh theo vùng an toàn (Safe Area) tránh camera nốt ruồi / tai thỏ.
* **Bảng điều khiển mở rộng (Expanded Panel)**:
  * **Danh sách tab kéo ngang**: Nút `+` thêm tab, chạm chỉ chọn tab; nút `Mở` bên dưới mới mở phiên. Tab đang hoạt động có nền xanh, tab được chọn có viền vàng.
  * **Hàng nút chức năng**:
    * `[ Dừng ]`: Dừng tab được chọn, ngắt kết nối và tắt auto.
    * `[ Xóa ]`: Xóa tab được chọn khỏi danh sách, giữ dữ liệu tài khoản trên đĩa. Tab 1 là tab chính, không xóa.
    * `[ Thu Nhỏ PiP ]`: Thu nhỏ cửa sổ về chế độ Mini (PC) hoặc kích hoạt PiP (Android). Bấm lại để phóng to về kích thước cũ.
  * **Tích hợp Modal Shield**: Chống click xuyên thấu 100% xuống nhân vật và thế giới game bên dưới.

### B. Tab "Nhân Bản" Trong Menu Mod (`ModUIMultiTab`) & Quản Lý Tab (`GameTabManager`)
* Mặc định khi khởi động chỉ chạy **1 Tab duy nhất (`Tab 1`)**.
* Người dùng tự thêm tab; core không giới hạn số lượng ở 6. Danh sách được lưu trong `tabs.xml` tại thư mục RMS chung.
* Mỗi tab có thư mục dữ liệu `tabN`. HUB và menu Nhân Bản dùng chung thành phần `ModTabPanel`.
* PC hiện chuyển phiên trong một tiến trình. Android giữ các Activity riêng cho tab 1-6; các ID lớn hơn dùng luồng chuyển phiên chung. Danh sách tab không đồng nghĩa với số phiên online đồng thời không giới hạn.
* Bảng giám sát trạng thái thời gian thực (`GameTabStatus`): Nhận biết trực quan Tab nào đang online, tab nào đang đăng nhập, tab nào đã dừng.

### C. Tự Động Scale Thích Ứng Màn Hình Thực Tế (Dynamic Adaptive Native Scaling)
* Tự động nhận diện kích thước cửa sổ vật lý (Windowed Maximize, 1080p, 2K, 4K, Ultra-wide).
* Tự động điều chỉnh `zoomLevel` thích ứng thông minh và tự động đồng bộ lại tọa độ tầng nền trời (`getYBackground`).
* Xóa bỏ hoàn toàn lỗi hổng mảng màu xanh lá, lỗi cắt hình bậc thang và lỗi sọc gạch chân bản đồ khi phóng to cửa sổ.

### D. Typography & Font Chữ Unicode HD Sắc Nét
* 100% văn bản trong game sử dụng font TrueType Unicode (`barmeneb.ttf`, `chelthm.ttf`, `staccato.ttf`) độ phân giải cao kết hợp bộ lọc Bilinear + Mipmaps.
* Hiển thị đầy đủ dấu Tiếng Việt, chữ số, và ký hiệu đặc biệt mà không bị mờ nhòe hay vỡ nét.

---

## 4. Lệnh Chat & Phím Tắt

### Phím tắt PC:
| Phím Tắt | Chức Năng |
| :---: | :--- |
| **`~`** hoặc **`F2`** | Bật / Tắt Menu Mod chính |
| **`F11`** hoặc **`Alt + Enter`** | Bật / Tắt chế độ Toàn màn hình (Fullscreen) |
| **`Home`** | Giải kẹt nhân vật khẩn cấp (Emergency Unstuck) |
| **Click Logo / Bubble** | Mở Menu Mod hoặc mở Menu HUB nổi |

### Lệnh Chat:
| Lệnh | Chức Năng |
| :--- | :--- |
| `tabN` | Mở tab theo ID, ví dụ `tab12` |
| `tab` / `nexttab` | Luân chuyển sang Tab kế tiếp trong cùng 1 game |
| `hub` / `hud` | Bật / Tắt Menu HUB Bong Bóng Nổi trên màn hình |
| `nhanban` / `multitab` | Mở trực tiếp Tab Nhân Bản trong Menu Mod |
| `upset` / `banrac` | Bật/tắt Auto Úp Set Kích Hoạt / Bán rác tự động |
| `dich` | Chuyển đổi ngôn ngữ Việt Hóa / Gốc Server |
| `bg` | Mở trình quản lý hình nền phong cảnh Git |

---

## 5. Hướng Dẫn Biên Dịch

### 1. Biên dịch Core Shared Library:
```bash
dotnet build C:\ModNRO\DragonBoy_Shared\SharedCore.csproj -c Release
```

### 2. Biên dịch & Xuất bản bản PC Windows:
```bash
# Biên dịch thông thường:
dotnet build C:\ModNRO\DragonBoy_Net8_Native\DragonBoy_Net8_Native.csproj -c Release

# Xuất bản Native AOT:
dotnet publish C:\ModNRO\DragonBoy_Net8_Native\DragonBoy_Net8_Native.csproj -c Release -r win-x64 --no-self-contained
```

### 3. Biên dịch bản Android APK:
```bash
dotnet build C:\ModNRO\DragonBoy_Mobile\Android\DragonBoy_Android.csproj -c Release
```
