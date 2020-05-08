using System;
using System.Collections.Generic;
using System.Text;
using LanguageLearningTool.ViewModels;

namespace LanguageLearningTool.Models
{
	public enum MenuItemType
	{
		Browse,
		About,
		Quiz,
	}
	public class HomeMenuItem
	{
		public BaseViewModel ViewModel { get; set; }

		public string Title { get; set; }
	}
}
