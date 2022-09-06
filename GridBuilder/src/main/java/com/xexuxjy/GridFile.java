package com.xexuxjy;

import java.awt.Color;
import java.awt.Point;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.IOException;
import java.nio.charset.StandardCharsets;
import java.util.Arrays;

import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import com.google.common.io.LittleEndianDataInputStream;
import com.google.common.io.LittleEndianDataOutputStream;

public class GridFile
{
	public static final String NO_VALUE = "**NONE**";

	public static String[] DefaultTags = new String[] { "NoMoveNoCursor", "NoMove", "Gate", "NoLargeUnits", "MapCenter",
			"LineOfSightPass" };

	public static String[] DefaultTeamTags = new String[] { "Start1", "Start2", "Start3", "Start4", "Start5", "Start6",
			"Start7", "Start8" };

	public static final int NumSlots = 30;
	public static final int ArenaSize = 32;

	private String[] m_rawGridTags = new String[NumSlots];
	public String[] m_gridTags = new String[NumSlots];
	public boolean[] m_activeTags = new boolean[NumSlots];

	private int[] m_gridData = new int[ArenaSize * ArenaSize];

	static byte[] TagData = new byte[32];

	private int m_header;

	private boolean m_dirty;

	public boolean isDirty()
	{
		return m_dirty;
	}

	public void setDirty(boolean dirty)
	{
		m_dirty = dirty;
	}

	public int getHeader()
	{
		return m_header;
	}

	public void setHeader(int header)
	{
		m_header = header;
	}

	public int getNumEntries()
	{
		return m_numEntries;
	}

	public void setNumEntries(int numEntries)
	{
		m_numEntries = numEntries;
	}

	private int m_numEntries;

	public GridFile()
	{
		for (int i = 0; i < NumSlots; ++i)
		{
			setSlot(i, NO_VALUE);
		}

		setSlot(0, DefaultTags[0]);
		setSlot(1, DefaultTags[1]);

	}

	public void setSlot(int i, String value)
	{
		if (NO_VALUE.equals(value))
		{
			m_activeTags[i] = false;
			m_rawGridTags[i] = "E" + value;
			m_gridTags[i] = value;
		}
		else
		{
			m_activeTags[i] = true;
			m_rawGridTags[i] = "F" + value;
			m_gridTags[i] = value;

		}

	}

	public static GridFile loadData(String filename) throws IOException
	{
		try (LittleEndianDataInputStream ds = new LittleEndianDataInputStream(new FileInputStream(filename)))
		{
			return loadData(ds);
		}

	}

	public static GridFile loadData(File file) throws IOException
	{
		try (LittleEndianDataInputStream ds = new LittleEndianDataInputStream(new FileInputStream(file)))
		{
			return loadData(ds);
		}

	}

	public static GridFile loadData(LittleEndianDataInputStream dataInputStream)
	{

		GridFile gridFile = null;

		try
		{
			gridFile = new GridFile();

			gridFile.setHeader(dataInputStream.readInt());
			gridFile.setNumEntries(dataInputStream.readInt());

			for (int i = 0; i < NumSlots; ++i)
			{
				int numRead = dataInputStream.read(TagData);
				assert (numRead == TagData.length);

				String s = new String(TagData, StandardCharsets.UTF_8);
				s = s.trim();

				gridFile.m_rawGridTags[i] = s;
				gridFile.m_activeTags[i] = false;

				if (gridFile.m_rawGridTags[i] != null)
				{
					if (gridFile.m_rawGridTags[i].startsWith("F"))
					{
						gridFile.m_activeTags[i] = true;
						gridFile.m_gridTags[i] = gridFile.m_rawGridTags[i].substring(1);

					}
				}

			}

			for (int i = 0; i < ArenaSize * ArenaSize; ++i)
			{
				gridFile.m_gridData[i] = dataInputStream.readInt();
			}

		}

		catch (IOException ioe)
		{
			logger.error("", ioe);
		}

		return gridFile;
	}

	public int getValue(int row, int column)
	{
		return m_gridData[(row * ArenaSize) + column];
	}

	public Color getColor(int row, int column)
	{
		int value = getValue(row, column);
		for (int i = 0; i < m_rawGridTags.length; ++i)
		{
			if (m_rawGridTags[i].startsWith("F"))
			{
				int mask = 1 << i;
				if ((value & (mask)) != 0)
				{
					if (i == 0)
					{
						return Color.red;
					}
					if (i == 1)
					{
						return Color.green;
					}
					if (i == 2)
					{
						return Color.blue;
					}
					if (i == 3)
					{
						return Color.pink;
					}
					if (i == 4)
					{
						return Color.orange;
					}
					if (i == 6)
					{
						return Color.cyan;
					}
				}
			}
		}
		return Color.black;
	}

	public void setValue(int row, int column, int value)
	{
		m_gridData[(row * ArenaSize) + column] = value;
		setDirty(true);
	}

	public String getInfoForSquare(Point point)
	{
		int value = getValue(point.x, point.y);
		return getInfoForValue(value);
	}

	public String getInfoForValue(int value)
	{
		String result = "";
		for (int i = 0; i < m_rawGridTags.length; ++i)
		{
			if (m_rawGridTags[i].startsWith("F"))
			{
				int mask = 1 << i;
				if ((value & (mask)) != 0)
				{
					result += m_gridTags[i] + ",";
				}
			}
		}

		return result;

	}

	public void saveData(String filename) throws IOException
	{
		saveData(new File(filename));
	}

	public void saveData(File file) throws IOException
	{
		try (LittleEndianDataOutputStream ds = new LittleEndianDataOutputStream(new FileOutputStream(file)))
		{
			saveData(ds);
		}

	}

	public void saveData(LittleEndianDataOutputStream dataOutputStream) throws IOException
	{
		dataOutputStream.writeInt(m_header);

		int activeCount = 0;
		for (boolean val : m_activeTags)
		{
			if (val)
			{
				activeCount++;
			}
		}
		dataOutputStream.writeInt(activeCount);

		for (int i = 0; i < NumSlots; ++i)
		{
			Arrays.fill(TagData, (byte) 0);

			TagData[0] = (byte) (m_activeTags[i] ? 'F' : 'E');
			byte[] stringBytes = m_gridTags[i].getBytes(StandardCharsets.UTF_8);
			for (int j = 0; j < stringBytes.length; ++j)
			{
				TagData[j + 1] = stringBytes[j];
			}
			dataOutputStream.write(TagData);
		}

		// now grid info

		for (int i = 0; i < m_gridData.length; ++i)
		{
			dataOutputStream.writeInt(m_gridData[i]);
		}

	}

	public static final Logger logger = LogManager.getLogger(GridFile.class);

}
