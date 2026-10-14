# Danh Sách Tính Năng Mod DragonBoy

Tài liệu này liệt kê các mod hiện có trong core dùng chung tại `DragonBoy_Shared/Src/Mod`. Mô tả được đối chiếu với mã nguồn hiện tại, không chỉ với README cũ.

## 1. Menu Và Điều Khiển

- Menu Mod chính: mở bằng `~`, `F2`, logo hoặc nút mũi tên.
- HUB nổi: bong bóng kéo thả, vùng an toàn, panel modal chống click xuyên xuống game.
- Menu HUB có danh sách tab kéo ngang, nút thêm, chọn tab, `Mở`, `Dừng`, `Xóa` và `PiP`.
- Tab đang hoạt động hiển thị màu xanh; tab được chọn có viền nhận diện.
- Menu Nhân Bản dùng `ModTabPanel` chung với HUB.
- PiP/thu nhỏ cửa sổ trên PC và PiP trên Android.
- Fullscreen bằng `F11` hoặc `Alt+Enter`.
- `Home`: giải kẹt nhân vật.
- FPS HUD, tự điều chỉnh FPS theo thiết bị và cập nhật độ phân giải khi resize.

## 2. Tàn Sát Và Chiến Đấu

- Auto Tàn Sát (`ModTanSat`).
- Chọn quái theo template ID hoặc chọn toàn bộ.
- Chọn kỹ năng đánh hoặc chọn toàn bộ kỹ năng.
- Lọc kỹ năng đánh, kỹ năng hỗ trợ và kiểm tra MP.
- Tự chọn vị trí đánh an toàn theo tile map.
- Teleport tới mục tiêu khi được bật.
- Ghi log phiên tàn sát và thống kê quái đã diệt.
- Auto né Broly bằng kite, giữ khoảng cách an toàn.
- Khinh công Broly.
- Tự động đánh Broly khi vị trí an toàn.

## 3. Nhặt Đồ Và Vật Phẩm

- Auto nhặt vật phẩm.
- Nhặt vàng, trang bị, ngọc hoặc toàn bộ vật phẩm.
- Lọc vật phẩm theo ID.
- Lọc vật phẩm theo tên/từ khóa.
- Xóa toàn bộ bộ lọc bằng `xnhat`.
- Hút đồ tức thì/Instant Pick.
- Hiển thị tên và ID item trên ô vật phẩm.
- Thống kê quái chết, item rơi, Set Kích Hoạt và số sao.

## 4. Set Kích Hoạt Và Bán Rác

- Auto Úp Set Kích Hoạt.
- Nhận diện item Set Kích Hoạt.
- Chọn số sao tối thiểu cần giữ.
- Phân biệt item quan trọng và item rác.
- Theo dõi số ô túi còn trống.
- Lưu vị trí farm trước khi đi bán.
- Tự đi tới khu vực bán rác, bán item không cần thiết rồi quay lại farm.
- Hiển thị ID item để hỗ trợ lọc và cấu hình.

## 5. HP, MP, Đậu Và Bùa

- Tự dùng đậu theo phần trăm HP (`autoPeanHpPercent`).
- Tùy chọn khóa HP/MP.
- Tự thu hoạch cây Đậu Thần.
- Tự xin đậu hoặc cho đậu trong bang.
- Tự cho đệ tử ăn đậu khi đệ tử yêu cầu.
- Đi mua bùa tại Bà Hạt Mít.
- Chọn loại bùa: Thu Hút, Trí Tuệ, Oai Hùng, Mạnh Mẽ, Da Trâu, Dẻo Dai.
- Chọn gói bùa: 1 giờ, 8 giờ hoặc 1 tháng.
- Tự mua lại bùa khi hết hạn.

## 6. GoBack Và Chuyển Map

- Lưu map, khu, tọa độ farm.
- Tự ghi nhận vị trí cuối cùng trước khi chết.
- Tự gửi lệnh hồi sinh khi nhân vật kiệt sức.
- Tự đi về map farm cũ sau khi hồi sinh.
- Tự đổi khu và dịch chuyển về tọa độ cũ.
- Có watchdog khi đổi khu hoặc chuyển map bị kẹt.
- Auto chuyển map theo map đích.
- Tìm đường qua waypoint.
- Hiển thị map đích và trạng thái chuyển map trên HUD.
- Dừng Next Map khi người chơi thao tác thủ công.

## 7. Boss Và HUD Thực Thể

- Đọc thông báo Boss từ dữ liệu server.
- HUD Boss hiển thị tên, map, thời gian và trạng thái đã bị hạ.
- Nhấp HUD Boss để xử lý tương tác liên quan.
- HUD thực thể trên map.
- HUD map đích của Next Map.

## 8. Đa Tab Và Dữ Liệu

- Danh sách tab động, không còn cố định ở 6 tab trong core.
- Tab 1 là tab chính và không thể xóa.
- Nút `+` tạo ID tab mới; ID đã xóa không bị tái sử dụng.
- Kéo ngang hoặc cuộn danh sách để xem nhiều tab.
- Chạm tab chỉ chọn tab, không tự mở.
- Nút `Mở` mới chuyển sang tab đã chọn.
- `Dừng` dừng tab đã chọn; tab khác có thể nhận lệnh dừng qua file lệnh dùng chung.
- `Xóa` xóa tab khỏi danh sách nhưng không xóa thư mục dữ liệu tài khoản.
- Mỗi tab dùng thư mục RMS riêng `tabN`.
- Danh sách tab lưu trong `tabs.xml` ở thư mục RMS chung.
- Trạng thái tab gồm online, đang kết nối, đang chạy auto, tên nhân vật, map, khu, HP và mô tả auto.
- `tabN`: mở tab theo ID, ví dụ `tab12`.
- `tab` hoặc `nexttab`: chuyển tới tab tiếp theo trong danh sách.
- `alltab`: mở các tab đã đăng ký.

## 9. Hình Ảnh, Âm Thanh Và Ngôn Ngữ

- Quản lý hình nền phong cảnh tải từ nguồn Git.
- Chọn, áp dụng, reset và xóa hình nền đã tải.
- Pixel Perfect hoặc Smooth HD.
- Điều chỉnh chất lượng đồ họa và bit depth theo backend.
- Scale canvas động theo kích thước cửa sổ.
- Căn chỉnh Safe Area cho tai thỏ/camera/notch.
- Font TrueType Unicode hỗ trợ tiếng Việt.
- Việt hóa tên map, NPC, quái, item, option và mô tả.
- `dich`, `vietnam`, `trans`: đổi giữa Việt hóa và ngôn ngữ server.

## 10. Đăng Nhập, Bảo Mật Và Cập Nhật

- Tự đăng nhập lại khi mất kết nối.
- Lưu và khôi phục trạng thái các tính năng auto sau reconnect.
- Giới hạn retry và backoff để tránh spam server.
- Watchdog timeout đăng nhập.
- Watchdog timeout đổi khu.
- Mã hóa mật khẩu lưu cục bộ bằng device key.
- Tách credential theo thư mục tab.
- HWID/license validation.
- Security watchdog.
- Kiểm tra và tải cập nhật mod từ GitHub bằng `update`.

## 11. Lệnh Chat

| Lệnh | Chức năng |
|---|---|
| `upset` | Bật/tắt Auto Úp Set Kích Hoạt |
| `banrac` | Đi bán rác rồi quay lại farm |
| `iditem` | Bật/tắt tên và ID item |
| `nhat_id` | Cấu hình lọc ID nhặt |
| `nhat_ten` | Cấu hình lọc tên nhặt |
| `xnhat` | Xóa bộ lọc nhặt |
| `roido` | Bật/tắt hút đồ tức thì |
| `tkrd` | Xem thống kê rơi đồ |
| `kbroly`, `broly` | Bật/tắt Auto Né Broly |
| `kc` | Bật/tắt Khinh Công Broly |
| `muabua`, `bua` | Mua bùa ngay |
| `autobua` | Bật/tắt tự gia hạn bùa |
| `gb`, `goback` | Bật/tắt GoBack |
| `nextmap=safe:ID` | Tự tìm đường và chuyển đến map ID bằng di chuyển bình thường |
| `nextmap=fast:ID` | Next Map nhanh bằng đồng bộ tọa độ hiện tại |
| `stopmap` | Dừng Next Map |
| `td` | Thu hoạch Đậu Thần |
| `cd` | Xin/cho đậu trong bang |
| `cde` | Bật/tắt cho đệ tử ăn đậu |
| `dich`, `vietnam`, `trans` | Đổi ngôn ngữ |
| `bg`, `phongcanh` | Mở quản lý hình nền |
| `nhanban`, `multitab`, `clone` | Mở menu quản lý tab |
| `hub`, `hud` | Bật/tắt HUB nổi |
| `tabN` | Mở tab theo ID |
| `tab`, `nexttab` | Chuyển tab kế tiếp |
| `alltab` | Mở các tab đã đăng ký |
| `update` | Kiểm tra cập nhật |
| `autochat` | Bật/tắt Auto Chat |
| `autochat=Nội dung\|30000` | Đặt nội dung và chu kỳ gửi (ms, tối thiểu 5000) |
| `boxsort` | Sắp xếp rương ngay |
| `autoboxsort` | Bật/tắt tự động sắp xếp rương |
| `autopoint` | Bật/tắt tự động cộng điểm tiềm năng |
| `autopoint=type|amount|intervalMs` | Cấu hình type 0-3, số điểm mỗi lần (1-100), chu kỳ (500-60000ms) |
| `autofreeze=ID` | Khóa Tàn Sát dùng skill template ID; `autofreeze` bỏ khóa |
| `autoitem=ID1,ID2|intervalMs` | Tự dùng item trong túi theo whitelist ID; `autoitem` bật/tắt |
| `autoitemhp=ID|percent|intervalMs` | Tự dùng item khi HP thấp hơn ngưỡng |
| `autoitemmp=ID|percent|intervalMs` | Tự dùng item khi MP thấp hơn ngưỡng |
| `schedule=HH:mm-HH:mm` | Lập lịch bật/tắt Tàn Sát, Nhặt đồ và Auto Item; `schedule` bật/tắt lịch |
| `autorevive` | Bật/tắt tự hồi sinh khi nhân vật chết |
| `autopet=status|intervalMs` | Tự gửi trạng thái pet qua packet chuẩn; `autopet` bật/tắt |
| `autofusion=type|intervalMs` | Tự kích hoạt fusion type 4-6; `autofusion` bật/tắt |

## 12. Phím Tắt

| Phím | Chức năng |
|---|---|
| `~`, `F2` | Bật/tắt Menu Mod |
| `F11`, `Alt+Enter` | Bật/tắt fullscreen |
| `Home` | Giải kẹt nhân vật |
| Click logo/bubble | Mở Menu Mod hoặc HUB |

## 13. File Triển Khai Chính

- `Src/Mod/Core/ModMenu.cs`: vòng lặp cập nhật và điều phối mod.
- `Src/Mod/UI/ModUI.cs`: Menu Mod chính.
- `Src/Mod/UI/ModFloatingHUD.cs`: HUB nổi.
- `Src/Mod/UI/ModTabPanel.cs`: panel tab động dùng chung.
- `Src/Core/App/GameTabRegistry.cs`: lưu/thêm/xóa danh sách tab.
- `Src/Core/App/GameTabManager.cs`: mở, dừng và chuyển tab.
- `Src/Core/App/GameTabStatus.cs`: trạng thái tab.
- `Src/Mod/Core/ModConfig.cs`: lưu cấu hình.

## 14. Giới Hạn Hiện Tại

- PC hiện chuyển phiên trong một tiến trình; danh sách tab động không tự tạo nhiều process PC.
- Android vẫn có Activity cố định cho tab 1-6; tab lớn hơn dùng luồng chuyển phiên chung.
- Android build cần workload `android` và `wasm-tools-net8`.
- Một số mô tả cũ trong tài liệu lịch sử vẫn ghi “6 tab”; mã core hiện đã dùng danh sách tab động.

## 15. Tính Năng Bản J2ME Đã Dump

Hai tài liệu tham chiếu `C:\ModNRO\DragonBoy1_full\MOD_FEATURES.md` và `README_DUMP.md` mô tả bản DragonBoy 2.4.7 J2ME đã decompile/dump. Các mục dưới đây là tính năng được phát hiện trong package mod J2ME (`a.a.b`) và cần phân biệt với phần đã port sang core .NET hiện tại.

### Module J2ME Được Xác Nhận

| Module J2ME | Chức năng |
|---|---|
| `a.a.b.aj` | Menu trung tâm, Menu Pro, Setting, HUD và điều phối mod |
| `a.a.b.T` | Auto Train/Tàn Sát, tránh siêu quái, kiểm tra quái lag, đổi khu |
| `a.a.b.z` | Auto Đệ Tử/Pet, tự đánh, Kaio-ken, cờ đen, né Boss, hồi sinh bằng ngọc |
| `a.a.b.G` | Auto nhặt và auto vứt, whitelist/blacklist, khoảng cách nhặt |
| `a.a.b.u` | Auto Đậu Thần, xin/cho đậu, thu hoạch, ăn theo HP/MP |
| `a.a.b.r` | Auto mua bùa 1 giờ, 8 giờ và 1 tháng |
| `a.a.b.af` | GoBack về map, khu và tọa độ cũ |
| `a.a.b.P` | Auto Skill, delay từng ô skill, đóng băng skill, đổi target |
| `a.a.b.o` | Lập lịch tự động bật/tắt hoặc kết nối lại game |
| `a.a.b.L` | Auto SKH, bán đồ rác, cất đồ SKH, mua TDLT, dùng item |
| `a.a.b.c` | Auto săn Boss, ghim, dịch chuyển và tấn công Boss |
| `a.a.b.J` | Auto cộng điểm tiềm năng vào Sức Đánh, HP, MP, Giáp, Chí Mạng |
| `a.a.b.m` | Auto dùng item, capsule, máy dò, cuồng nộ, bổ huyết, bổ khí, giáp Xên |
| `a.a.b.k` | Auto Chat theo nội dung và khoảng delay |
| `a.a.b.b` | AutoCaptcha qua ảnh và JSON API |
| `a.a.b.ai` | Đồ họa siêu thấp và HUD phần trăm HP/KI/chỉ số |
| `a.a.b.N` | Lọc/bán đồ theo cấp, sao, đồ thần, hủy diệt, thiên sứ và SKH |
| `a.a.b.ad` | Báo Boss toàn server và nút Go |
| `a.a.b.ab` | Auto nhiệm vụ phụ, gồm Thần Mèo Karin và Tàu Pảy Pảy |
| `a.a.b.ae` | Chế độ giảm CPU khi treo nick |
| `a.a.b.ao` | Tự sắp xếp rương/hành trang |
| `a.a.b.F` | Trợ thủ trị thương và buff đậu cho đồng đội |
| `a.a.b.a` | Lời chào và khởi tạo phiên bản mod |
| `a.a.b.i` | Định dạng bộ đếm thời gian |
| `a.a.b.ac` | Hỗ trợ khởi tạo tân thủ |

### Cài Đặt Và Tiện Ích Chỉ Được Xác Nhận Trong Dump J2ME

- Game Speed từ khoảng 5ms đến 100ms.
- Move Speed từ 1 đến 20.
- Delay Xmap và delay dùng capsule.
- Hiển thị thông tin đệ tử và sư phụ.
- Danh sách người chơi trong map.
- Theo dõi 13 bộ Set Kích Hoạt.
- Capsule bay nhanh.
- Tự ăn đùi gà khi thiếu thể lực.
- Ẩn người chơi khác để giảm lag.
- Tăng tốc chuyển tới khu vực tương lai.
- Khóa mục tiêu quái/Boss.
- Tự đánh thường và bật cờ đen khi úp đệ.
- Đổi nhanh nhiều bộ trang bị.
- Auto tạo tài khoản clone mới để up vàng.

### Đối Chiếu Với Core Hiện Tại

- Đã có tương đương hoặc bản port: Tàn Sát, Nhặt Đồ, Đậu, Bùa, GoBack, Boss, Set Kích Hoạt/bán rác, đồ họa thấp, dịch thuật, nền, đa tab, reconnect và HUD.
- Có bản core gần tương đương nhưng không hoàn toàn trùng J2ME: Auto Pet nâng cao, Auto Skill đóng băng, Auto Point, Auto Item, Auto Chat và lập lịch On/Off.
- Chưa thấy module tương đương đầy đủ trong core hiện tại: AutoCaptcha JSON, Auto nhiệm vụ phụ Karin/Tàu Pảy Pảy, Auto tạo tài khoản clone và trợ thủ buff đậu cho người khác.
- Đã port thêm Auto Point từ packet `upPotential`: chỉ gửi khi nhân vật còn `powerPoint`, giới hạn tốc độ và số điểm mỗi lần để tránh spam server. Type 0-3 phụ thuộc thứ tự stat của server (thường Sức đánh/HP/MP/Giáp); Chí mạng nếu server dùng type riêng cần xác nhận packet.
- Đã port Skill Freeze: khóa skill theo template ID, vẫn kiểm tra danh sách skill được phép, cooldown và MP trước khi đánh.
- Đã port Auto Item whitelist: dùng `Service.useItem` trên item trong túi, mỗi chu kỳ tối đa một item và không gửi khi chưa vào game.
- Đã port lịch Auto theo giờ địa phương, hỗ trợ khung giờ qua nửa đêm; lịch chỉ bật/tắt các module auto trong core.
- Đã port Auto Revive bằng `Service.wakeUpFromDead`, giới hạn một lần mỗi 10 giây; chỉ chạy khi trạng thái nhân vật là chết.
- Đã port bộ điều khiển Auto Pet theo trạng thái server, giới hạn mã 0-20 và chu kỳ tối thiểu 1 giây.
- Đã port Auto Fusion/Kaio-ken qua `Service.funsion`, chỉ gửi khi nhân vật chưa ở trạng thái fusion.
- `README_DUMP.md` là nhật ký phân tích/dump runtime; nó xác nhận dữ liệu J2ME, RMS, server và worker timer, nhưng không phải danh sách tính năng đã được port sang .NET.
