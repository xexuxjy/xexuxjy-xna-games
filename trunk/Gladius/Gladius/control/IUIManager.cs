using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Gladius.events;
using Gladius.gamestatemanagement.screenmanager;
using Microsoft.Xna.Framework.Graphics;
using Gladius.util;

namespace Gladius.control
{
	public interface IUIManager
	{
	
		IUIControl CurrentControl
		{get;set;}
        void UIControlChanged(IUIControl oldvp, IUIControl newvp, bool up);
		
	}

	public interface IUIControl
	{
		IUIControl Prev {get;set;}
		IUIControl Next {get;set;}
		String Text {get;set;}
		SpriteFont SpriteFont {get;set;}
        ThreadSafeContentManager ContentManager { get; set; }
		bool HasFocus{get;set;}
		event Gladius.events.EventManager.ActionButtonPressed ActionPressed;
        void OnActionPressed(object sender, ActionButtonPressedArgs e);
	}

	
}
