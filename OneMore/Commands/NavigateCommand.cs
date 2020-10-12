﻿//************************************************************************************************
// Copyright © 2020 Steven M Cohn.  All rights reserved.
//************************************************************************************************

namespace River.OneMoreAddIn
{
	using System;


	internal class NavigateCommand : Command
	{
		public NavigateCommand() : base()
		{
		}


		public void Execute(string pageTag)
		{
			int retry = 0;
			while (retry < 4)
			{
				try
				{
					using (var manager = new ApplicationManager())
					{
						manager.NavigateTo(pageTag);
					}

					retry = int.MaxValue;
				}
				catch (Exception exc)
				{
					logger.WriteLine($"Error navigating to {pageTag}", exc);

					retry++;
					System.Threading.Thread.Sleep(250 * retry);
				}
			}
		}
	}
}
