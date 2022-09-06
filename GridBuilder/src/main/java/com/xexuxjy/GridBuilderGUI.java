package com.xexuxjy;

import java.awt.BorderLayout;
import java.awt.Color;
import java.awt.Dimension;
import java.awt.Point;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.awt.event.KeyEvent;
import java.io.File;
import java.util.ArrayList;

import javax.swing.BoxLayout;
import javax.swing.JCheckBox;
import javax.swing.JComboBox;
import javax.swing.JFileChooser;
import javax.swing.JFrame;
import javax.swing.JLabel;
import javax.swing.JMenu;
import javax.swing.JMenuBar;
import javax.swing.JMenuItem;
import javax.swing.JPanel;
import javax.swing.WindowConstants;

import org.apache.commons.lang3.StringUtils;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

public class GridBuilderGUI
{
	private GridPanel m_gridPanel;
	private JPanel m_toolsPanel;
	private JPanel m_slotPanel;
	private JPanel m_infoPanel;

	private JLabel m_fileInfoLabel;
	private JLabel m_positionLabel;
	private JLabel m_squareInfoLabel;

	private GridFile m_gridFile;

	private JFrame m_frame;

	private static int NumTags = 12;

	private String m_fileName;

	private int m_currentDrawMask;

	private ArrayList<JComboBox<String>> m_comboBoxes = new ArrayList<JComboBox<String>>();
	private ArrayList<JCheckBox> m_checkBoxes = new ArrayList<JCheckBox>();

	public GridFile getGridFile()
	{
		return m_gridFile;
	}

	public void setGridFile(String fileName, GridFile gridFile)
	{
		m_fileName = fileName;

		m_frame.setTitle(m_fileName);

		m_gridFile = gridFile;
		refreshGUI();
	}

	public void refreshGUI()
	{

		for (int i = 0; i < NumTags; ++i)
		{
			if (m_gridFile.m_activeTags[i])
			{
				int index = matchIndex(m_comboBoxes.get(i), m_gridFile.m_gridTags[i]);
				if (index != -1)
				{
					m_comboBoxes.get(i).setSelectedIndex(index);
				}
			}
			else
			{
				m_comboBoxes.get(i).setSelectedItem(GridFile.NO_VALUE);
			}
		}

		m_gridPanel.repaint();

	}

	private int matchIndex(JComboBox<String> cb, String val)
	{
		int size = cb.getModel().getSize();
		for (int i = 0; i < size; ++i)
		{
			String cbval = cb.getModel().getElementAt(i);

			if (StringUtils.compare(cbval, val) == 0)
			{
				return i;
			}
		}
		return -1;
	}

	public void buildGUI(JFrame frame, GridFile gridFile)
	{
		m_frame = frame;
		m_gridFile = gridFile;
		buildMenu(frame);

		BorderLayout borderLayout = new BorderLayout(20, 20);
		frame.setLayout(borderLayout);

		buildInfoPanel();
		buildToolsPanel();
		buildSlotsPanel();

		m_gridPanel = new GridPanel();
		m_gridPanel.initialise(this, m_positionLabel, m_squareInfoLabel);

		JPanel rightHandSide = new JPanel();
		rightHandSide.setLayout(new BoxLayout(rightHandSide, BoxLayout.Y_AXIS));
		rightHandSide.add(m_toolsPanel);
		rightHandSide.add(m_slotPanel);

		frame.add(m_gridPanel, BorderLayout.CENTER);
		frame.add(m_infoPanel, BorderLayout.SOUTH);
		//frame.add(m_toolsPanel, BorderLayout.EAST);
		frame.add(rightHandSide, BorderLayout.EAST);

		//frame.addMouseListener(mouseHandler);

	}

	private void buildMenu(JFrame frame)
	{
		JMenuBar menuBar = new JMenuBar();
		frame.setJMenuBar(menuBar);

		JMenu fileMenu = new JMenu("File");
		fileMenu.setMnemonic(KeyEvent.VK_F);

		JMenuItem newMenuItem = new JMenuItem("New");
		newMenuItem.setMnemonic(KeyEvent.VK_N);
		newMenuItem.addActionListener(new newActionListener());

		JMenuItem loadMenuItem = new JMenuItem("Load");
		loadMenuItem.setMnemonic(KeyEvent.VK_L);
		loadMenuItem.addActionListener(new LoadActionListener());

		JMenuItem saveMenuItem = new JMenuItem("Save");
		saveMenuItem.setMnemonic(KeyEvent.VK_S);
		saveMenuItem.addActionListener(new SaveActionListener());

		JMenuItem saveAsMenuItem = new JMenuItem("Save As");
		saveAsMenuItem.setMnemonic(KeyEvent.VK_A);
		saveAsMenuItem.addActionListener(new SaveAsActionListener());

		menuBar.add(fileMenu);

		fileMenu.add(newMenuItem);
		fileMenu.add(loadMenuItem);
		fileMenu.add(saveMenuItem);
		fileMenu.add(saveAsMenuItem);

	}

	private void buildInfoPanel()
	{
		m_infoPanel = new JPanel();
		m_infoPanel.setLayout(new BoxLayout(m_infoPanel, BoxLayout.Y_AXIS));
		m_infoPanel.setPreferredSize(new Dimension(600, 200));
		m_infoPanel.setBackground(Color.lightGray);

		m_fileInfoLabel = new JLabel();
		m_infoPanel.add(m_fileInfoLabel);

		m_positionLabel = new JLabel();
		m_positionLabel.setText("Position x 0 y 0 ");

		m_infoPanel.add(m_positionLabel);

		m_squareInfoLabel = new JLabel();
		m_infoPanel.add(m_squareInfoLabel);

	}

	private void buildSlotsPanel()
	{
		m_slotPanel = new JPanel();
		m_slotPanel.setLayout(new BoxLayout(m_slotPanel, BoxLayout.Y_AXIS));
		m_slotPanel.setPreferredSize(new Dimension(300, 300));

		for (int i = 0; i < NumTags; ++i)
		{
			JPanel linePanel = new JPanel();
			m_slotPanel.add(linePanel);

			JComboBox<String> comboBox = buildComboBox();
			JCheckBox checkBox = new JCheckBox();
			checkBox.addActionListener(new ActionListener()
			{

				@Override
				public void actionPerformed(ActionEvent e)
				{
					updateDrawMask();
				}
			});

			m_comboBoxes.add(comboBox);
			m_checkBoxes.add(checkBox);

			linePanel.add(checkBox);
			linePanel.add(comboBox);
		}

	}

	private JComboBox<String> buildComboBox()
	{
		ArrayList<String> values = new ArrayList<String>();
		values.add(GridFile.NO_VALUE);
		for (String val : GridFile.DefaultTags)
		{
			values.add(val);
		}

		for (String val : GridFile.DefaultTeamTags)
		{
			values.add(val);
		}

		JComboBox<String> comboBox = new JComboBox<String>(values.toArray(new String[0]));
		comboBox.setPreferredSize(new Dimension(200, 80));

		return comboBox;
	}

	private void buildToolsPanel()
	{
		m_toolsPanel = new JPanel();
		m_toolsPanel.add(new JLabel("Tools"));
	}

	public String getInfoForSquare(Point arenaPoint)
	{
		return m_gridFile.getInfoForSquare(arenaPoint);

	}

	public int getCurrentDrawMask()
	{
		return m_currentDrawMask;
	}

	public void updateDrawMask()
	{
		m_currentDrawMask = 0;
		for (int i = 0; i < m_checkBoxes.size(); ++i)
		{
			if (m_checkBoxes.get(i).isSelected())
			{
				m_currentDrawMask |= 1 << i;
			}
		}

	}

	private class newActionListener implements ActionListener
	{
		@Override
		public void actionPerformed(ActionEvent e)
		{
			setGridFile("New", new GridFile());
		}

	}

	private class LoadActionListener implements ActionListener
	{

		@Override
		public void actionPerformed(ActionEvent ae)
		{
			JFileChooser fileChooser = new JFileChooser();
			int option = fileChooser.showOpenDialog(m_frame);
			if (option == JFileChooser.APPROVE_OPTION)
			{
				try
				{
					File file = fileChooser.getSelectedFile();
					GridFile gridFile = GridFile.loadData(file);
					setGridFile(file.getName(), gridFile);

				}
				catch (Exception e)
				{
					logger.error("", e);
				}
			}
			else
			{
			}
		}

	}

	private class SaveActionListener implements ActionListener
	{

		@Override
		public void actionPerformed(ActionEvent ae)
		{
			try
			{
				File file = new File(m_fileName);
				if (file.exists())
				{
					m_gridFile.saveData(file);
				}
			}
			catch (Exception e)
			{
				logger.error("", e);
			}
		}

	}

	private class SaveAsActionListener implements ActionListener
	{

		@Override
		public void actionPerformed(ActionEvent ae)
		{
			JFileChooser fileChooser = new JFileChooser();
			int option = fileChooser.showSaveDialog(m_frame);
			if (option == JFileChooser.APPROVE_OPTION)
			{
				try
				{
					File file = fileChooser.getSelectedFile();
					m_gridFile.saveData(file);
					setGridFile(file.getName(), m_gridFile);

				}
				catch (Exception e)
				{
					logger.error("", e);
				}
			}
			else
			{
			}

		}

	}

	public static void main(String[] args)
	{
		try
		{
			GridFile gridFile = new GridFile();

			JFrame jframe = new JFrame();

			GridBuilderGUI gui = new GridBuilderGUI();
			jframe.setDefaultCloseOperation(WindowConstants.EXIT_ON_CLOSE);
			jframe.setSize(new Dimension(1024, 1024));

			gui.buildGUI(jframe, gridFile);

			jframe.setVisible(true);
		}
		catch (Exception e)
		{
			logger.error("", e);
		}
	}

	public static final Logger logger = LogManager.getLogger(GridBuilderGUI.class);

}
