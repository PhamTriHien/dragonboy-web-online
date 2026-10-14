using System;

/// <summary>
/// Thành phần phân mảnh (partial) của lớp Panel: Đảm nhiệm phân hệ chuyên trách Inventory (Split).
/// </summary>
public partial class Panel : IActionListener, IChatable
{
	private bool invSplitIsBody = true;

	private int invSplitBodySel = 0;

	private int invSplitBagSel = 0;

	private const int INV_SPLIT_HEADER_H = 18;

	private const int INV_SPLIT_DIV = 2;

	private const int BODY_SLOT_SIZE = 32;

	private const int BODY_SLOT_GAP = 5;

	/// <summary>
	/// Thành phần phân mảnh (partial) của lớp PaperdollSlotDef: Đảm nhiệm phân hệ chuyên trách Inventory (Split).
	/// </summary>
	private struct PaperdollSlotDef
	{
		public int slotIdx;

		public string ghostName;

		public int col;

		public int row;

		/// <summary>
		/// Thực thi phương thức nghiệp vụ PaperdollSlotDef.
		/// </summary>
		public PaperdollSlotDef(int slotIdx, string ghostName, int col, int row)
		{
			this.slotIdx = slotIdx;
			this.ghostName = ghostName;
			this.col = col;
			this.row = row;
		}
	}

	private static readonly PaperdollSlotDef[] paperdollSlots = new PaperdollSlotDef[]
	{
		/// <summary>
		/// Thực thi phương thức nghiệp vụ PaperdollSlotDef.
		/// </summary>
		new PaperdollSlotDef(0, "Áo", 0, 0),
		/// <summary>
		/// Thực thi phương thức nghiệp vụ PaperdollSlotDef.
		/// </summary>
		new PaperdollSlotDef(1, "Quần", 0, 1),
		/// <summary>
		/// Thực thi phương thức nghiệp vụ PaperdollSlotDef.
		/// </summary>
		new PaperdollSlotDef(3, "Giày", 0, 2),
		/// <summary>
		/// Thực thi phương thức nghiệp vụ PaperdollSlotDef.
		/// </summary>
		new PaperdollSlotDef(2, "Găng", 1, 0),
		/// <summary>
		/// Thực thi phương thức nghiệp vụ PaperdollSlotDef.
		/// </summary>
		new PaperdollSlotDef(4, "Rada", 1, 1),
		/// <summary>
		/// Thực thi phương thức nghiệp vụ PaperdollSlotDef.
		/// </summary>
		new PaperdollSlotDef(5, "C.Trang", 1, 2),
		/// <summary>
		/// Thực thi phương thức nghiệp vụ PaperdollSlotDef.
		/// </summary>
		new PaperdollSlotDef(6, "Pet", 0, 3),
		/// <summary>
		/// Thực thi phương thức nghiệp vụ PaperdollSlotDef.
		/// </summary>
		new PaperdollSlotDef(7, "Phụ kiện", 1, 3)
	};

	/// <summary>
	/// Truy xuất và trả về dữ liệu của GetBagGridLayout.
	/// </summary>
	private static void GetBagGridLayout(int rightW, out int cols, out int slotW, out int slotH, out int gap, out int startPad)
	{
		cols = (rightW >= 190) ? 5 : 4;
		gap = 2;
		int pad = 2;
		int availW = rightW - pad * 2 - (cols - 1) * gap;
		slotW = availW / cols;
		int remainder = availW % cols;
		startPad = pad + remainder / 2;
		slotH = slotW;
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của GetPaperdollY.
	/// </summary>
	private static int GetPaperdollY(int listY, int listH)
	{
		int totalSlotsH = 3 * BODY_SLOT_SIZE + 2 * BODY_SLOT_GAP;
		int py = listY + (listH - totalSlotsH) / 2;
		if (py < listY + 6)
		{
			py = listY + 6;
		}
		return py;
	}

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của IsInventorySplit.
	/// </summary>
	private bool IsInventorySplit()
	{
		try
		{
			if (type != 0 || currentTabIndex != 1)
			{
				return false;
			}
			Char me = Char.myCharz();
			if (me == null || me.arrItemBody == null || me.arrItemBag == null)
			{
				return false;
			}
			return true;
		}
		catch
		{
			return false;
		}
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của GetInventorySplitLayout.
	/// </summary>
	private void GetInventorySplitLayout(out int leftX, out int leftW, out int rightX, out int rightW, out int listY, out int listH)
	{
		int totalW = wScroll;
		leftW = totalW * 44 / 100;
		if (leftW < 136)
		{
			leftW = 136;
		}
		if (leftW > 185)
		{
			leftW = 185;
		}
		if (totalW <= 220)
		{
			leftW = totalW / 2 - 2;
		}
		leftX = xScroll;
		rightX = leftX + leftW + INV_SPLIT_DIV;
		rightW = xScroll + totalW - rightX;
		if (rightW < 80)
		{
			rightW = 80;
		}
		listY = yScroll + INV_SPLIT_HEADER_H + 2;
		listH = hScroll - INV_SPLIT_HEADER_H - 2;
		if (listH < 24)
		{
			listH = 24;
		}
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của GetPaperdollSlotX.
	/// </summary>
	private static int GetPaperdollSlotX(PaperdollSlotDef def, int leftX, int leftW)
	{
		if (def.col == 0)
		{
			return leftX + 8;
		}
		if (def.col == 1)
		{
			return leftX + leftW - BODY_SLOT_SIZE - 8;
		}
		if (def.col == 2)
		{
			return leftX + 28;
		}
		return leftX + leftW - BODY_SLOT_SIZE - 28;
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của GetPaperdollSlotY.
	/// </summary>
	private static int GetPaperdollSlotY(PaperdollSlotDef def, int paperdollY)
	{
		return paperdollY + def.row * (BODY_SLOT_SIZE + BODY_SLOT_GAP);
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ TryGetPaperdollDef.
	/// </summary>
	private static bool TryGetPaperdollDef(int slotIdx, out PaperdollSlotDef def)
	{
		for (int i = 0; i < paperdollSlots.Length; i++)
		{
			if (paperdollSlots[i].slotIdx == slotIdx)
			{
				def = paperdollSlots[i];
				return true;
			}
		}
		def = default(PaperdollSlotDef);
		return false;
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của GetPaperdollSlotYForIndex.
	/// </summary>
	private int GetPaperdollSlotYForIndex(int slotIdx, int paperdollY)
	{
		PaperdollSlotDef def;
		if (TryGetPaperdollDef(slotIdx, out def))
		{
			return GetPaperdollSlotY(def, paperdollY);
		}
		return paperdollY;
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của GetItemUpgradeLevel.
	/// </summary>
	private static int GetItemUpgradeLevel(Item item)
	{
		if (item == null || item.itemOption == null)
		{
			return 0;
		}
		for (int i = 0; i < item.itemOption.Length; i++)
		{
			if (item.itemOption[i] != null && item.itemOption[i].optionTemplate.id == 72)
			{
				return item.itemOption[i].param;
			}
		}
		return 0;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ ClampInventorySplit.
	/// </summary>
	private void ClampInventorySplit()
	{
		try
		{
			Char me = Char.myCharz();
			if (me == null || me.arrItemBody == null || me.arrItemBag == null)
			{
				return;
			}
			int bodyLen = me.arrItemBody.Length;
			int bagLen = me.arrItemBag.Length;
			if (bodyLen <= 0)
			{
				invSplitBodySel = 0;
			}
			else
			{
				if (invSplitBodySel < 0)
				{
					invSplitBodySel = 0;
				}
				if (invSplitBodySel >= bodyLen)
				{
					invSplitBodySel = bodyLen - 1;
				}
			}
			if (bagLen <= 0)
			{
				invSplitBagSel = 0;
			}
			else
			{
				if (invSplitBagSel < 0)
				{
					invSplitBagSel = 0;
				}
				if (invSplitBagSel >= bagLen)
				{
					invSplitBagSel = bagLen - 1;
				}
			}
			if (bodyLen > 0 && bagLen == 0)
			{
				invSplitIsBody = true;
			}
			else if (bodyLen == 0 && bagLen > 0)
			{
				invSplitIsBody = false;
			}
			int lx, lw, rx, rw, ly, lh;
			GetInventorySplitLayout(out lx, out lw, out rx, out rw, out ly, out lh);
			int cols, slotW, slotH, gap, startPad;
			GetBagGridLayout(rw, out cols, out slotW, out slotH, out gap, out startPad);
			int bagRows = (bagLen + cols - 1) / cols;
			int totalBagH = bagRows * (slotH + gap) + 6;
			cmyLim = totalBagH - lh;
			if (cmyLim < 0)
			{
				cmyLim = 0;
			}
			if (cmtoY < 0)
			{
				cmtoY = 0;
			}
			if (cmtoY > cmyLim)
			{
				cmtoY = cmyLim;
			}
			if (cmy < 0)
			{
				cmy = 0;
			}
			if (cmy > cmyLim)
			{
				cmy = cmyLim;
			}
			size_tab = 0;
			newSelected = 0;
			if (!invSplitIsBody)
			{
				selected = bodyLen + invSplitBagSel + 1;
			}
			else
			{
				selected = invSplitBodySel + 1;
			}
			if (GameCanvas.isTouch && selected < 1)
			{
				selected = -1;
			}
			currentListLength = bodyLen + bagLen + 1;
			if (currentListLength < 1)
			{
				currentListLength = 1;
			}
		}
		catch
		{
		}
	}

	/// <summary>
	/// Khởi tạo và nạp dữ liệu cấu hình cho InitInventorySplit.
	/// </summary>
	private void InitInventorySplit(bool resetSelect)
	{
		try
		{
			ITEM_HEIGHT = 34;
			if (resetSelect)
			{
				if (GameCanvas.isTouch)
				{
					selected = -1;
				}
				invSplitIsBody = false;
				invSplitBodySel = 0;
				invSplitBagSel = 0;
				cmy = (cmtoY = 0);
				cmRun = 0;
				currItem = null;
				cp = null;
			}
			ClampInventorySplit();
			if (!GameCanvas.isTouch && selected < 1)
			{
				invSplitIsBody = false;
				invSplitBagSel = 0;
				ClampInventorySplit();
			}
		}
		catch
		{
		}
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của GetInventorySplitCurrItem.
	/// </summary>
	private Item GetInventorySplitCurrItem()
	{
		try
		{
			Char me = Char.myCharz();
			if (me == null)
			{
				return null;
			}
			if (invSplitIsBody)
			{
				if (me.arrItemBody == null || invSplitBodySel < 0 || invSplitBodySel >= me.arrItemBody.Length)
				{
					return null;
				}
				return me.arrItemBody[invSplitBodySel];
			}
			if (me.arrItemBag == null || invSplitBagSel < 0 || invSplitBagSel >= me.arrItemBag.Length)
			{
				return null;
			}
			return me.arrItemBag[invSplitBagSel];
		}
		catch
		{
			return null;
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ SyncInventorySplitSelected.
	/// </summary>
	private void SyncInventorySplitSelected()
	{
		try
		{
			Char me = Char.myCharz();
			if (me == null)
			{
				return;
			}
			int bodyLen = (me.arrItemBody != null) ? me.arrItemBody.Length : 0;
			if (invSplitIsBody)
			{
				selected = invSplitBodySel + 1;
			}
			else
			{
				selected = bodyLen + invSplitBagSel + 1;
			}
			newSelected = 0;
			lastSelect[currentTabIndex] = selected;
		}
		catch
		{
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ EnsureInventorySplitBagVisible.
	/// </summary>
	private void EnsureInventorySplitBagVisible()
	{
		try
		{
			int lx, lw, rx, rw, ly, lh;
			GetInventorySplitLayout(out lx, out lw, out rx, out rw, out ly, out lh);
			int cols, slotW, slotH, gap, startPad;
			GetBagGridLayout(rw, out cols, out slotW, out slotH, out gap, out startPad);
			int row = invSplitBagSel / cols;
			int rowY = ly + 2 + row * (slotH + gap);
			if (rowY < ly + cmy)
			{
				cmtoY = row * (slotH + gap);
			}
			else if (rowY + slotH > ly + cmy + lh)
			{
				cmtoY = (row + 1) * (slotH + gap) - lh + 2;
			}
			if (cmtoY < 0)
			{
				cmtoY = 0;
			}
			if (cmtoY > cmyLim)
			{
				cmtoY = cmyLim;
			}
			cmy = cmtoY;
		}
		catch
		{
		}
	}

	/// <summary>
	/// Thực hiện vẽ và hiển thị thành phần DrawEquipmentPaperdollSlot lên canvas đồ họa.
	/// </summary>
	private void DrawEquipmentPaperdollSlot(mGraphics g, Item item, int slotIdx, string ghostName, int x, int y, int size, bool isSelected)
	{
		try
		{
			int baseBg = isSelected ? 16383818 : ((item != null) ? 15723751 : 15196114);
			int upLv = GetItemUpgradeLevel(item);
			if (item != null && !isSelected && upLv > 0)
			{
				sbyte upId = GetColor_Item_Upgrade(upLv);
				int upBg = GetColor_ItemBg(upId);
				if (upBg != -1)
				{
					baseBg = upBg;
				}
			}
			g.setColor(baseBg);
			g.fillRect(x, y, size, size);
			g.setColor(isSelected ? 13524492 : 11837316);
			g.drawRect(x + 1, y + 1, size - 3, size - 3);
			g.setColor(isSelected ? 13524492 : 9993045);
			g.drawRect(x, y, size - 1, size - 1);
			if (isSelected)
			{
				g.setColor(16383818);
				g.drawRect(x + 1, y + 1, size - 3, size - 3);
			}
			if (item == null)
			{
				mFont.tahoma_7_grey.drawString(g, ghostName, x + size / 2, y + size / 2 - 4, mFont.CENTER);
				return;
			}
			SmallImage.drawSmallImage(g, item.template.iconID, x + size / 2, y + size / 2, 0, 3);
			if (item.itemOption != null)
			{
				for (int a = 0; a < item.itemOption.Length; a++)
				{
					if (item.itemOption[a] != null)
					{
						paintOptItem(g, item.itemOption[a].optionTemplate.id, item.itemOption[a].param, x, y, size, size);
						paintOptSlotItem(g, item.itemOption[a].optionTemplate.id, item.itemOption[a].param, x, y, size, size);
					}
				}
			}
			if (upLv > 0)
			{
				mFont.tahoma_7b_green.drawString(g, "+" + upLv, x + size - 2, y + 1, mFont.RIGHT);
			}
			if (item.quantity > 1)
			{
				string qStr = (item.quantity > 999) ? (item.quantity / 1000 + "k") : (string.Empty + item.quantity);
				mFont.tahoma_7_yellow.drawString(g, qStr, x + size - 2, y + size - 8, mFont.RIGHT);
			}
		}
		catch
		{
		}
	}

	/// <summary>
	/// Thực hiện vẽ và hiển thị thành phần DrawGridBagSlot lên canvas đồ họa.
	/// </summary>
	private void DrawGridBagSlot(mGraphics g, Item item, int x, int y, int w, int h, bool isSelected)
	{
		try
		{
			int baseBg = isSelected ? 16383818 : ((item != null) ? 15723751 : 15196114);
			int upLv = GetItemUpgradeLevel(item);
			if (item != null && !isSelected && upLv > 0)
			{
				sbyte upId = GetColor_Item_Upgrade(upLv);
				int upBg = GetColor_ItemBg(upId);
				if (upBg != -1)
				{
					baseBg = upBg;
				}
			}
			g.setColor(baseBg);
			g.fillRect(x, y, w, h);
			g.setColor(isSelected ? 13524492 : 11837316);
			g.drawRect(x + 1, y + 1, w - 3, h - 3);
			g.setColor(isSelected ? 13524492 : 9993045);
			g.drawRect(x, y, w - 1, h - 1);
			if (isSelected)
			{
				g.setColor(16383818);
				g.drawRect(x + 1, y + 1, w - 3, h - 3);
			}
			if (item == null)
			{
				return;
			}
			SmallImage.drawSmallImage(g, item.template.iconID, x + w / 2, y + h / 2, 0, 3);
			if (item.itemOption != null)
			{
				for (int a = 0; a < item.itemOption.Length; a++)
				{
					if (item.itemOption[a] != null)
					{
						paintOptItem(g, item.itemOption[a].optionTemplate.id, item.itemOption[a].param, x, y, w, h);
						paintOptSlotItem(g, item.itemOption[a].optionTemplate.id, item.itemOption[a].param, x, y, w, h);
					}
				}
			}
			if (upLv > 0)
			{
				mFont.tahoma_7b_green.drawString(g, "+" + upLv, x + w - 2, y + 1, mFont.RIGHT);
			}
			if (item.quantity > 1)
			{
				string qStr = (item.quantity > 999) ? (item.quantity / 1000 + "k") : (string.Empty + item.quantity);
				mFont.tahoma_7_yellow.drawString(g, qStr, x + w - 2, y + h - 8, mFont.RIGHT);
			}
			if (item.newItem && GameCanvas.gameTick % 10 > 4)
			{
				if (imgNew != null)
				{
					g.drawImage(imgNew, x + 2, y + 2, 0);
				}
				else
				{
					g.setColor(16711680);
					g.fillRect(x + 2, y + 2, 4, 4);
				}
			}
		}
		catch
		{
		}
	}

	/// <summary>
	/// Thực hiện vẽ và hiển thị thành phần paintInventorySplit lên canvas đồ họa.
	/// </summary>
	private void paintInventorySplit(mGraphics g)
	{
		try
		{
			Char me = Char.myCharz();
			if (me == null || me.arrItemBody == null || me.arrItemBag == null)
			{
				return;
			}
			ClampInventorySplit();
			int leftX, leftW, rightX, rightW, listY, listH;
			GetInventorySplitLayout(out leftX, out leftW, out rightX, out rightW, out listY, out listH);
			Item[] arrBody = me.arrItemBody;
			Item[] arrBag = me.arrItemBag;
			int hlY = yScroll + 1;
			bool leftFocus = invSplitIsBody;

			g.setColor(13524492);
			g.fillRect(xScroll, yScroll, wScroll, 1);

			g.setColor(leftFocus ? 16383818 : 15723751);
			g.fillRect(leftX, hlY, leftW, INV_SPLIT_HEADER_H - 2);
			g.setColor(9993045);
			g.drawRect(leftX, hlY, leftW, INV_SPLIT_HEADER_H - 2);

			g.setColor(!leftFocus ? 16383818 : 15723751);
			g.fillRect(rightX, hlY, rightW, INV_SPLIT_HEADER_H - 2);
			g.setColor(9993045);
			g.drawRect(rightX, hlY, rightW, INV_SPLIT_HEADER_H - 2);

			string heroTitle = (me.cName != null && me.cName.Length > 0) ? me.cName : mResources.trangbi;
			if (mFont.tahoma_7b_dark.getWidth(heroTitle) > leftW - 6)
			{
				heroTitle = mResources.trangbi;
			}
			mFont.tahoma_7b_dark.drawString(g, heroTitle, leftX + leftW / 2, hlY + 3, mFont.CENTER);

			int usedCount = 0;
			for (int b = 0; b < arrBag.Length; b++)
			{
				if (arrBag[b] != null)
				{
					usedCount++;
				}
			}
			mFont.tahoma_7b_dark.drawString(g, "HÀNH TRANG", rightX + 8, hlY + 3, mFont.LEFT);
			mFont.tahoma_7_grey.drawString(g, usedCount + "/" + arrBag.Length, rightX + rightW - 8, hlY + 3, mFont.RIGHT);

			g.setColor(13524492);
			g.fillRect(leftX + leftW, hlY, INV_SPLIT_DIV, INV_SPLIT_HEADER_H - 2 + listH + 2);

			g.setColor(15196114);
			g.fillRect(leftX, listY, leftW, listH);
			g.setColor(9993045);
			g.drawRect(leftX, listY, leftW, listH);

			int paperdollY = GetPaperdollY(listY, listH);
			int avatarX = leftX + leftW / 2;
			int avatarFeetY = paperdollY + 2 * (BODY_SLOT_SIZE + BODY_SLOT_GAP) + BODY_SLOT_SIZE - 2;

			g.setColor(11837316);
			g.fillRoundRect(avatarX - 22, avatarFeetY - 3, 44, 7, 4, 4);
			g.setColor(9993045);
			g.drawRoundRect(avatarX - 22, avatarFeetY - 3, 44, 7, 4, 4);

			try
			{
				me.paintCharBody(g, avatarX, avatarFeetY, 1, 0, isPaintBag: true);
			}
			catch
			{
			}

			for (int s = 0; s < paperdollSlots.Length; s++)
			{
				PaperdollSlotDef pDef = paperdollSlots[s];
				if (pDef.slotIdx >= arrBody.Length)
				{
					continue;
				}
				if (pDef.row == 3 && listH < 170)
				{
					continue;
				}
				int sx = GetPaperdollSlotX(pDef, leftX, leftW);
				int sy = GetPaperdollSlotY(pDef, paperdollY);
				bool isSel = leftFocus && (pDef.slotIdx == invSplitBodySel);
				Item bItem = (pDef.slotIdx < arrBody.Length) ? arrBody[pDef.slotIdx] : null;
				DrawEquipmentPaperdollSlot(g, bItem, pDef.slotIdx, pDef.ghostName, sx, sy, BODY_SLOT_SIZE, isSel);
			}

			g.setColor(15196114);
			g.fillRect(rightX, listY, rightW, listH);
			g.setColor(9993045);
			g.drawRect(rightX, listY, rightW, listH);

			int cols, slotW, slotH, gap, startPad;
			GetBagGridLayout(rightW, out cols, out slotW, out slotH, out gap, out startPad);
			int gridStartX = rightX + startPad;

			g.setClip(rightX + 1, listY + 1, rightW - 2, listH - 2);
			g.translate(0, -cmy);

			int bagLen = arrBag.Length;
			for (int j = 0; j < bagLen; j++)
			{
				int row = j / cols;
				int col = j % cols;
				int sx2 = gridStartX + col * (slotW + gap);
				int sy2 = listY + 2 + row * (slotH + gap);
				if (sy2 - cmy > listY + listH || sy2 - cmy < listY - slotH)
				{
					continue;
				}
				bool sel2 = !leftFocus && (j == invSplitBagSel);
				DrawGridBagSlot(g, arrBag[j], sx2, sy2, slotW, slotH, sel2);
			}

			g.translate(0, cmy);
			g.setClip(xScroll, yScroll, wScroll, hScroll);

			if (cmyLim > 0)
			{
				int trackH = listH - 8;
				int bagRows = (bagLen + cols - 1) / cols;
				int totalBagH = bagRows * (slotH + gap) + 6;
				int thumbH = trackH * listH / totalBagH;
				if (thumbH < 14)
				{
					thumbH = 14;
				}
				int thumbY = listY + 4 + (cmy * (trackH - thumbH) / cmyLim);
				g.setColor(15196114);
				g.fillRect(rightX + rightW - 4, listY + 4, 3, trackH);
				g.setColor(13524492);
				g.fillRect(rightX + rightW - 4, thumbY, 3, thumbH);
			}
		}
		catch (Exception)
		{
		}
	}

	/// <summary>
	/// Cập nhật trạng thái logic của UpdateInventorySplit theo chu kỳ khung hình.
	/// </summary>
	private void UpdateInventorySplit()
	{
		try
		{
			Char me = Char.myCharz();
			if (me == null || me.arrItemBody == null || me.arrItemBag == null)
			{
				return;
			}
			ClampInventorySplit();
			int leftX, leftW, rightX, rightW, listY, listH;
			GetInventorySplitLayout(out leftX, out leftW, out rightX, out rightW, out listY, out listH);
			int bodyLen = me.arrItemBody.Length;
			int bagLen = me.arrItemBag.Length;
			int cols, slotW, slotH, gap, startPad;
			GetBagGridLayout(rightW, out cols, out slotW, out slotH, out gap, out startPad);
			int gridStartX = rightX + startPad;
			int paperdollY = GetPaperdollY(listY, listH);
			bool handledKey = false;

			if (GameCanvas.keyPressed[23] || GameCanvas.keyPressed[4])
			{
				handledKey = true;
				if (!invSplitIsBody)
				{
					int col = invSplitBagSel % cols;
					if (col > 0)
					{
						invSplitBagSel--;
					}
					else
					{
						invSplitIsBody = true;
						int r = invSplitBagSel / cols;
						if (r == 0)
						{
							invSplitBodySel = (bodyLen > 2) ? 2 : 0;
						}
						else if (r == 1)
						{
							invSplitBodySel = (bodyLen > 4) ? 4 : 1;
						}
						else
						{
							invSplitBodySel = (bodyLen > 5) ? 5 : ((bodyLen > 3) ? 3 : 0);
						}
					}
				}
				else
				{
					if (invSplitBodySel == 2)
					{
						invSplitBodySel = 0;
					}
					else if (invSplitBodySel == 4)
					{
						invSplitBodySel = (bodyLen > 1) ? 1 : 0;
					}
					else if (invSplitBodySel == 5)
					{
						invSplitBodySel = (bodyLen > 3) ? 3 : ((bodyLen > 1) ? 1 : 0);
					}
				}
				SoundMn.gI().panelClick();
				SyncInventorySplitSelected();
				EnsureInventorySplitBagVisible();
				currItem = GetInventorySplitCurrItem();
				if (currItem != null)
				{
					currItem.compare = getCompare(currItem);
					addItemDetail(currItem);
				}
			}
			else if (GameCanvas.keyPressed[24] || GameCanvas.keyPressed[6])
			{
				handledKey = true;
				if (invSplitIsBody)
				{
					if (invSplitBodySel == 0 && bodyLen > 2)
					{
						invSplitBodySel = 2;
					}
					else if (invSplitBodySel == 1 && bodyLen > 4)
					{
						invSplitBodySel = 4;
					}
					else if (invSplitBodySel == 3 && bodyLen > 5)
					{
						invSplitBodySel = 5;
					}
					else
					{
						invSplitIsBody = false;
						int targetR = 0;
						if (invSplitBodySel == 4 || invSplitBodySel == 1)
						{
							targetR = 1;
						}
						else if (invSplitBodySel == 5 || invSplitBodySel == 3)
						{
							targetR = 2;
						}
						invSplitBagSel = targetR * cols;
						if (invSplitBagSel >= bagLen)
						{
							invSplitBagSel = (bagLen > 0) ? (bagLen - 1) : 0;
						}
					}
				}
				else
				{
					if (invSplitBagSel < bagLen - 1)
					{
						invSplitBagSel++;
					}
				}
				SoundMn.gI().panelClick();
				SyncInventorySplitSelected();
				EnsureInventorySplitBagVisible();
				currItem = GetInventorySplitCurrItem();
				if (currItem != null)
				{
					currItem.compare = getCompare(currItem);
					addItemDetail(currItem);
				}
			}

			if (GameCanvas.keyPressed[21] || GameCanvas.keyPressed[2])
			{
				handledKey = true;
				if (invSplitIsBody)
				{
					if (invSplitBodySel == 1)
					{
						invSplitBodySel = 0;
					}
					else if (invSplitBodySel == 3)
					{
						invSplitBodySel = 1;
					}
					else if (invSplitBodySel == 4)
					{
						invSplitBodySel = (bodyLen > 2) ? 2 : 0;
					}
					else if (invSplitBodySel == 5)
					{
						invSplitBodySel = (bodyLen > 4) ? 4 : ((bodyLen > 2) ? 2 : 0);
					}
					else if (invSplitBodySel == 6)
					{
						invSplitBodySel = 3;
					}
					else if (invSplitBodySel == 7)
					{
						invSplitBodySel = 5;
					}
				}
				else
				{
					if (invSplitBagSel - cols >= 0)
					{
						invSplitBagSel -= cols;
					}
					EnsureInventorySplitBagVisible();
				}
				SoundMn.gI().panelClick();
				SyncInventorySplitSelected();
				currItem = GetInventorySplitCurrItem();
				if (currItem != null)
				{
					currItem.compare = getCompare(currItem);
					addItemDetail(currItem);
				}
			}
			else if (GameCanvas.keyPressed[22] || GameCanvas.keyPressed[8])
			{
				handledKey = true;
				if (invSplitIsBody)
				{
					if (invSplitBodySel == 0 && bodyLen > 1)
					{
						invSplitBodySel = 1;
					}
					else if (invSplitBodySel == 1 && bodyLen > 3)
					{
						invSplitBodySel = 3;
					}
					else if (invSplitBodySel == 2 && bodyLen > 4)
					{
						invSplitBodySel = 4;
					}
					else if (invSplitBodySel == 4 && bodyLen > 5)
					{
						invSplitBodySel = 5;
					}
					else if (invSplitBodySel == 3 && bodyLen > 6)
					{
						invSplitBodySel = 6;
					}
					else if (invSplitBodySel == 5 && bodyLen > 7)
					{
						invSplitBodySel = 7;
					}
				}
				else
				{
					if (invSplitBagSel + cols < bagLen)
					{
						invSplitBagSel += cols;
					}
					EnsureInventorySplitBagVisible();
				}
				SoundMn.gI().panelClick();
				SyncInventorySplitSelected();
				currItem = GetInventorySplitCurrItem();
				if (currItem != null)
				{
					currItem.compare = getCompare(currItem);
					addItemDetail(currItem);
				}
			}

			if ((GameCanvas.keyPressed[5] || GameCanvas.keyPressed[25] || GameCanvas.keyPressed[12]) && GetInventorySplitCurrItem() != null)
			{
				currItem = GetInventorySplitCurrItem();
				pointerDownTime = 0;
				waitToPerform = 2;
			}

			if (pointerIsDowning && !GameCanvas.isPointerDown && !GameCanvas.isPointerJustRelease)
			{
				pointerIsDowning = false;
				pointerDownTime = 0;
				cmtoY = cmy;
			}

			if (GameCanvas.isPointerDown)
			{
				justRelease = false;
				bool inRight = GameCanvas.isPointer(rightX, listY, rightW, listH);
				bool inLeft = GameCanvas.isPointer(leftX, listY, leftW, listH);
				if (!pointerIsDowning && (inRight || inLeft))
				{
					for (int i = 0; i < pointerDownLastX.Length; i++)
					{
						pointerDownLastX[i] = GameCanvas.py;
					}
					pointerDownFirstX = GameCanvas.py;
					pointerIsDowning = true;
					isDownWhenRunning = cmRun != 0;
					cmRun = 0;
				}
				else if (pointerIsDowning)
				{
					pointerDownTime++;
					int dy = GameCanvas.py - pointerDownLastX[0];
					if (GameCanvas.isPointer(rightX, listY, rightW, listH))
					{
						if (dy != 0)
						{
							cmtoY -= dy;
							if (cmtoY < 0)
							{
								cmtoY = 0;
							}
							if (cmtoY > cmyLim)
							{
								cmtoY = cmyLim;
							}
							if (cmy < 0 || cmy > cmyLim)
							{
								dy /= 2;
							}
							cmy -= dy;
							if (cmy < 0)
							{
								cmy = 0;
							}
							if (cmy > cmyLim)
							{
								cmy = cmyLim;
							}
						}
					}
					for (int n2 = pointerDownLastX.Length - 1; n2 > 0; n2--)
					{
						pointerDownLastX[n2] = pointerDownLastX[n2 - 1];
					}
					pointerDownLastX[0] = GameCanvas.py;
					if (dy != 0 && selected >= 0)
					{
						selected = -1;
					}
				}
			}

			if (GameCanvas.isPointerJustRelease && pointerIsDowning)
			{
				justRelease = true;
				int dy2 = GameCanvas.py - pointerDownLastX[0];
				GameCanvas.isPointerJustRelease = false;
				bool isTap = Res.abs(dy2) < 20 && Res.abs(GameCanvas.py - pointerDownFirstX) < 20 && !isDownWhenRunning;
				if (isTap)
				{
					cmRun = 0;
					cmtoY = cmy;
					pointerDownFirstX = -1000;
					int px = GameCanvas.px;
					int py = GameCanvas.py;
					bool hit = false;

					if (px >= leftX && px < leftX + leftW && py >= listY && py < listY + listH)
					{
						for (int s2 = 0; s2 < paperdollSlots.Length; s2++)
						{
							PaperdollSlotDef pDef2 = paperdollSlots[s2];
							if (pDef2.slotIdx >= bodyLen)
							{
								continue;
							}
							if (pDef2.row == 3 && listH < 170)
							{
								continue;
							}
							int sx3 = GetPaperdollSlotX(pDef2, leftX, leftW);
							int sy3 = GetPaperdollSlotY(pDef2, paperdollY);
							if (px >= sx3 && px < sx3 + BODY_SLOT_SIZE && py >= sy3 && py < sy3 + BODY_SLOT_SIZE)
							{
								invSplitIsBody = true;
								invSplitBodySel = pDef2.slotIdx;
								SyncInventorySplitSelected();
								currItem = GetInventorySplitCurrItem();
								if (currItem != null)
								{
									currItem.compare = getCompare(currItem);
									addItemDetail(currItem);
								}
								hit = true;
								break;
							}
						}
					}
					else if (px >= rightX && px < rightX + rightW && py >= listY && py < listY + listH)
					{
						int relX = px - gridStartX;
						int relY = py - (listY + 2) + cmy;
						if (relX >= 0 && relY >= 0)
						{
							int col2 = relX / (slotW + gap);
							int row2 = relY / (slotH + gap);
							int inX = relX % (slotW + gap);
							int inY = relY % (slotH + gap);
							if (col2 >= 0 && col2 < cols && inX < slotW && inY < slotH)
							{
								int idx2 = row2 * cols + col2;
								if (idx2 >= 0 && idx2 < bagLen)
								{
									invSplitIsBody = false;
									invSplitBagSel = idx2;
									SyncInventorySplitSelected();
									EnsureInventorySplitBagVisible();
									currItem = GetInventorySplitCurrItem();
									if (currItem != null)
									{
										currItem.compare = getCompare(currItem);
										addItemDetail(currItem);
									}
									hit = true;
								}
							}
						}
					}

					if (hit)
					{
						pointerDownTime = 0;
						waitToPerform = 10;
						SoundMn.gI().panelClick();
					}
				}
				else
				{
					if (cmy < 0)
					{
						cmtoY = 0;
					}
					else if (cmy > cmyLim)
					{
						cmtoY = cmyLim;
					}
					else
					{
						int fl = GameCanvas.py - pointerDownLastX[0] + (pointerDownLastX[0] - pointerDownLastX[1]) + (pointerDownLastX[1] - pointerDownLastX[2]);
						fl = ((fl > 10) ? 10 : ((fl < -10) ? (-10) : 0));
						cmRun = -fl * 100;
					}
				}
				pointerIsDowning = false;
				pointerDownTime = 0;
				GameCanvas.isPointerJustRelease = false;
			}

			if (handledKey)
			{
				cmtoY = cmy;
				if (cmtoY > cmyLim)
				{
					cmtoY = cmyLim;
				}
				if (cmtoY < 0)
				{
					cmtoY = 0;
				}
			}
		}
		catch
		{
		}
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của GetInventorySplitMenuY.
	/// </summary>
	private int GetInventorySplitMenuY()
	{
		try
		{
			int leftX, leftW, rightX, rightW, listY, listH;
			GetInventorySplitLayout(out leftX, out leftW, out rightX, out rightW, out listY, out listH);
			if (invSplitIsBody)
			{
				int paperdollY = GetPaperdollY(listY, listH);
				int sy = GetPaperdollSlotYForIndex(invSplitBodySel, paperdollY);
				return sy + BODY_SLOT_SIZE / 2;
			}
			int cols, slotW, slotH, gap, startPad;
			GetBagGridLayout(rightW, out cols, out slotW, out slotH, out gap, out startPad);
			int row = invSplitBagSel / cols;
			int sy2 = listY + 2 + row * (slotH + gap) - cmy;
			return sy2 + slotH / 2;
		}
		catch
		{
			return yScroll;
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ DoFireInventorySplit.
	/// </summary>
	private void DoFireInventorySplit()
	{
		try
		{
			if (GameCanvas.menu != null && GameCanvas.menu.showMenu)
			{
				return;
			}
			if (Char.myCharz().statusMe == 14)
			{
				GameCanvas.startOKDlg(mResources.can_not_do_when_die);
				return;
			}
			ClampInventorySplit();
			Item ci = GetInventorySplitCurrItem();
			if (ci == null)
			{
				cp = null;
				currItem = null;
				return;
			}
			currItem = ci;
			SyncInventorySplitSelected();
			MyVector menu = new MyVector();
			if (invSplitIsBody)
			{
				menu.addElement(new Command(mResources.GETOUT, this, 2002, currItem));
			}
			else
			{
				if (GameCanvas.panel.type == 12)
				{
					menu.addElement(new Command(mResources.use_for_combine, this, 6000, currItem));
				}
				else if (GameCanvas.panel.type == 13)
				{
					menu.addElement(new Command(mResources.use_for_trade, this, 7000, currItem));
				}
				else if (currItem.isTypeBody())
				{
					menu.addElement(new Command(mResources.USE, this, 2000, currItem));
					if (Char.myCharz().havePet)
					{
						menu.addElement(new Command(mResources.MOVEFORPET, this, 2005, currItem));
					}
				}
				else
				{
					menu.addElement(new Command(mResources.USE, this, 2001, currItem));
				}
			}
			Char.myCharz().setPartTemp(currItem.headTemp, currItem.bodyTemp, currItem.legTemp, currItem.bagTemp);
			if (GameCanvas.panel.type != 12 && GameCanvas.panel.type != 13)
			{
				if (position == 0)
				{
					menu.addElement(new Command(mResources.MOVEOUT, this, 2003, currItem));
				}
				if (position == 1)
				{
					menu.addElement(new Command(mResources.SALE, this, 3002, currItem));
				}
			}
			int menuY = GetInventorySplitMenuY();
			if (menuY < yScroll)
			{
				menuY = yScroll;
			}
			if (menuY > yScroll + hScroll - 20)
			{
				menuY = yScroll + hScroll - 20;
			}
			GameCanvas.menu.startAt(menu, X, menuY);
			addItemDetail(currItem);
		}
		catch (Exception ex)
		{
			Res.outz("SplitFire ex " + ex.StackTrace);
		}
	}
}
