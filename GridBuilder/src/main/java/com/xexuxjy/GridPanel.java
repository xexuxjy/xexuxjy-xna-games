package com.xexuxjy;

import java.awt.Color;
import java.awt.Graphics;
import java.awt.Point;
import java.awt.event.MouseEvent;

import javax.swing.JLabel;
import javax.swing.JPanel;
import javax.swing.event.MouseInputAdapter;

public class GridPanel extends JPanel
{
	public static final int GRIDSIZE = 20;

	GridBuilderGUI  m_gridBuilderGUI;
	GridFileMouseHandler m_gridMouseHandler;


	
	
	public GridBuilderGUI getGridBuilderGUI()
	{
		return m_gridBuilderGUI;
	}

	public void initialise(GridBuilderGUI gridBuilderGUI, JLabel positionLabel,JLabel squareInfoLabel)
	{
		m_gridBuilderGUI = gridBuilderGUI;

		setSize(GridFile.ArenaSize * GRIDSIZE, GridFile.ArenaSize * GRIDSIZE);

		m_gridMouseHandler = new GridFileMouseHandler(this, positionLabel,squareInfoLabel);

		addMouseListener(m_gridMouseHandler);
		addMouseMotionListener(m_gridMouseHandler);
	}

	private void drawRect(Point start, Point end, Color color, Graphics graphics)
	{
		int xstart = Math.min(start.x, end.x);
		int ystart = Math.min(start.y, end.y);

		int width = Math.abs(start.x - end.x);
		int height = Math.abs(start.y - end.y);

		graphics.setColor(color);
		graphics.drawRect(xstart, ystart, width, height);

	}

	public void paint(Graphics g)
	{

		for (int x = 0; x < GridFile.ArenaSize; ++x)
		{
			for (int y = 0; y < GridFile.ArenaSize; ++y)
			{
				int val = m_gridBuilderGUI.getGridFile().getValue(x, y);
				Color squareColour = m_gridBuilderGUI.getGridFile().getColor(x, y);
				
				
				g.setColor(Color.black);
				g.drawRect(x * GRIDSIZE, y * GRIDSIZE, GRIDSIZE, GRIDSIZE);
				g.setColor(val != 0 ? squareColour : Color.white);
				g.fillRect((x * GRIDSIZE) + 1, (y * GRIDSIZE) + 1, GRIDSIZE - 1, GRIDSIZE - 1);
			}

		}

		if (m_gridMouseHandler.isDragging())
		{
			drawRect(m_gridMouseHandler.getDragStartPoint(), m_gridMouseHandler.getDragEndPoint(), Color.black, g);
		}

	}

	public void finishPaint(Point start, Point end,boolean erase)
	{

		int pointValue = m_gridBuilderGUI.getCurrentDrawMask();
		
		if(erase)
		{
			pointValue = 0;
		}
		
		if (start != null && end != null)
		{

			if (start.equals(end))
			{
				Point gridStart = new Point(start.x / GridPanel.GRIDSIZE, start.y/ GridPanel.GRIDSIZE);
				m_gridBuilderGUI.getGridFile().setValue(gridStart.x, gridStart.y, pointValue);
			}
			else
			{
				int xstart = Math.min(start.x, end.x);
				int ystart = Math.min(start.y, end.y);

				int width = Math.abs(start.x - end.x);
				int height = Math.abs(start.y - end.y);

				Point gridStart = new Point(xstart / GridPanel.GRIDSIZE, ystart / GridPanel.GRIDSIZE);
				Point gridEnd = new Point((xstart + width) / GridPanel.GRIDSIZE,
						(ystart + height) / GridPanel.GRIDSIZE);


				for (int x = gridStart.x; x < gridEnd.x; ++x)
				{
					for (int y = gridStart.y; y < gridEnd.y; ++y)
					{
						m_gridBuilderGUI.getGridFile().setValue(x, y, pointValue);
					}
				}
			}
			repaint();
		}
	}

	private static class GridFileMouseHandler extends MouseInputAdapter
	{
		public GridPanel m_gridPanel;
		public JLabel m_positionInfoLabel;
		public JLabel m_squareInfoLabel;
		
		private boolean m_dragging;
		private Point m_dragStartPoint;
		private Point m_dragEndPoint;

		public boolean isDragging()
		{
			return m_dragging;
		}

		public Point getDragStartPoint()
		{
			return m_dragStartPoint;
		}

		public void setDragStartPoint(Point p)
		{
			m_dragStartPoint = limitPoint(p);
		}
		
		private Point limitPoint(Point p)
		{
			if(p.x < 0)
			{
				p.x = 0;
			}
			if(p.x > GridFile.ArenaSize * GRIDSIZE)
			{
				p.x = GridFile.ArenaSize * GRIDSIZE;
			}
			if(p.y < 0)
			{
				p.y = 0;
			}
			if(p.y > GridFile.ArenaSize * GRIDSIZE)
			{
				p.y = GridFile.ArenaSize * GRIDSIZE;
			}
			return p;
		}
		
		
		public Point getDragEndPoint()
		{
			return m_dragEndPoint;
		}

		public void setDragEndPoint(Point p)
		{
			m_dragEndPoint = limitPoint(p);
		}
		
		
		public GridFileMouseHandler(GridPanel gridPanel, JLabel positionInfoLabel,JLabel squareInfoLabel)
		{
			m_gridPanel = gridPanel;
			m_positionInfoLabel = positionInfoLabel;
			m_squareInfoLabel = squareInfoLabel;
		}

		@Override
		public void mousePressed(MouseEvent e)
		{
			m_dragging = true;
			setDragStartPoint(e.getPoint());
		}

		@Override
		public void mouseReleased(MouseEvent e)
		{
			setDragEndPoint(e.getPoint());
			m_dragging = false;

			boolean erase = e.getButton() == MouseEvent.BUTTON3;
			
			if (isValidPoint(m_dragStartPoint) && isValidPoint(m_dragEndPoint))
			{
				m_gridPanel.finishPaint(m_dragStartPoint, m_dragEndPoint,erase);
			}
		}

		@Override
		public void mouseDragged(MouseEvent e)
		{
			setDragEndPoint(e.getPoint());
			m_gridPanel.repaint();
		}

		private boolean isValidPoint(Point p)
		{
			Point gridPoint = new Point(p.x / GridPanel.GRIDSIZE, p.y / GridPanel.GRIDSIZE);
			boolean valid = gridPoint.x >= 0 && gridPoint.x < GridFile.ArenaSize && gridPoint.y >= 0
					&& gridPoint.y < GridFile.ArenaSize;
			return valid;
		}

		@Override
		public void mouseMoved(MouseEvent e)
		{
			Point p = e.getPoint();
			Point gridPoint = new Point(p.x / GridPanel.GRIDSIZE, p.y / GridPanel.GRIDSIZE);

//			String textString = String.format("Grid - Header %s  , Entries %s\n", m_gridPanel.getGridFile().getHeader(),
//					m_gridPanel.getGridFile().getNumEntries());
			String positionText = "";
			String squareText = "";
			if (isValidPoint(p))
			{

				positionText = String.format("Position x[%s] y[%s]  mouseX %s  mouseY %s   Draw Mask %s", gridPoint.x, gridPoint.y, p.x,
						p.y,m_gridPanel.getGridBuilderGUI().getGridFile().getInfoForValue(m_gridPanel.getGridBuilderGUI().getCurrentDrawMask()));
				
				
				squareText = m_gridPanel.getGridBuilderGUI().getInfoForSquare(gridPoint);
				
				
			}
			m_positionInfoLabel.setText(positionText);
			m_squareInfoLabel.setText(squareText);
		}

	}

}
