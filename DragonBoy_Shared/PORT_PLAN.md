# Ke Hoach Port Mod

## Da hoan thanh

- Auto Point, Skill Freeze, Auto Item whitelist/HP/MP.
- Auto Chat, Auto Box Sort, Schedule, Auto Revive.
- Auto Pet status, Auto Fusion/Kaio-ken.
- Next Map safe/fast, pathfinding, waypoint watchdog, cache theo gender.
- Tan Sat, nhat do, dau, bua, GoBack, Boss/Kite Broly.
- Set Kich Hoat, ban rac, reconnect, HUD, dich thuat, do hoa/FPS, tab dong.

## Con thieu

- Auto doi set theo HP/Boss/farm: can packet mac/thao item.
- Auto Pet tan cong/ne Boss/co den: can bang ma trang thai server.
- Auto nhiem vu Karin/Tau Pay Pay: can packet menu/quest.
- Auto giam tai khi treo: an nguoi choi, tat hieu ung, ha FPS.
- Auto Boss hoan chinh: danh sach Boss, map, target va quay ve farm.
- Buff dau cho nguoi khac: can packet nhom.
- AutoCaptcha JSON va tao tai khoan clone: can API/quy trinh ben ngoai.

## Thu tu tiep tuc

1. Bat packet mac/thao item, xac minh dong bo server, sau do lam Auto Set.
2. Hoan thien Auto Boss ket hop `nextmap=safe:ID` va GoBack.
3. Them Auto Item theo Boss/map va cooldown rieng.
4. Them che do treo nick va khoi phuc khi thao tac.
5. Bat packet Pet/nhiem vu, lap bang ma va watchdog.

## Tieu chi

- Khong tao packet moi khi chua co log server tuong ung.
- Moi module co toggle, luu config, cooldown, guard khi chua vao game va nut dung.
- Co regression check cho command parser, timeout, reconnect va Next Map.
- Build `SharedCore` va `DragonBoy_Net8_Native` khong warning/error.
