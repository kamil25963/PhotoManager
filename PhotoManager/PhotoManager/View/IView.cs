﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoManager
{
	public interface IView
	{
        event Action<User> LoggingEvent;
		
		void ShowMessage(bool success, string message);
        IRegisterView IVievRegister { get; }
        IForm1View IVievForm { get; }
	}
}
