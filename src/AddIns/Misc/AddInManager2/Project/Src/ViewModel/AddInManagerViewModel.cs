﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

using ICSharpCode.AddInManager2.Model;
using ICSharpCode.AddInManager2.View;
using ICSharpCode.SharpDevelop;
using NuGet;

namespace ICSharpCode.AddInManager2.ViewModel
{
	public class AddInManagerViewModel : Model<AddInManagerViewModel>, IDisposable
	{
		private string _message;
		private bool _hasError;
		
		private ObservableCollection<AddInsViewModelBase> _viewModels;
		
		public AddInManagerViewModel()
			: base()
		{
			Initialize();
		}
		
		public AddInManagerViewModel(IAddInManagerServices services)
			: base(services)
		{
			Initialize();
		}
		
		private void Initialize()
		{
			// Visuals
			this.Title = SD.ResourceService.GetString("AddInManager.Title");
			
			// Add event handlers
			AddInManager.Events.OperationStarted += AddInManager_Events_OperationStarted;
			AddInManager.Events.AddInOperationError += AddInManager_Events_AddInOperationError;
			AddInManager.Events.AcceptLicenses += AddInManager_Events_AcceptLicenses;
			
			_viewModels = new ObservableCollection<AddInsViewModelBase>();
			
			// Create and collect the models
			InstalledAddInsViewModel = new InstalledAddInsViewModel();
			AvailableAddInsViewModel = new AvailableAddInsViewModel();
			UpdatedAddInsViewModel = new UpdatedAddInsViewModel();
			
			_viewModels.Add(InstalledAddInsViewModel);
			_viewModels.Add(AvailableAddInsViewModel);
			_viewModels.Add(UpdatedAddInsViewModel);
			
			foreach (var viewModel in _viewModels)
			{
				viewModel.PropertyChanged += ViewModel_PropertyChanged;
			}
			
			// Expand the first view
			InstalledAddInsViewModel.IsExpandedInView = true;
			
			// Read the packages
			AvailableAddInsViewModel.ReadPackages();
			InstalledAddInsViewModel.ReadPackages();
			UpdatedAddInsViewModel.ReadPackages();
		}
		
		public AvailableAddInsViewModel AvailableAddInsViewModel
		{
			get;
			private set;
		}
		
		public InstalledAddInsViewModel InstalledAddInsViewModel
		{
			get;
			private set;
		}
		
		public UpdatedAddInsViewModel UpdatedAddInsViewModel
		{
			get;
			private set;
		}
		
		public ObservableCollection<AddInsViewModelBase> ViewModels
		{
			get
			{
				return _viewModels;
			}
		}
		
		public string Title
		{
//			get { return viewTitle.Title; }
			get;
			private set;
		}
		
		public void Dispose()
		{
			AddInManager.Events.OperationStarted -= AddInManager_Events_OperationStarted;
			AddInManager.Events.AddInOperationError -= AddInManager_Events_AddInOperationError;
			AddInManager.Events.AcceptLicenses -= AddInManager_Events_AcceptLicenses;
			foreach (var viewModel in _viewModels)
			{
				viewModel.PropertyChanged -= ViewModel_PropertyChanged;
			}
		}
		
		private void ShowErrorMessage(string message)
		{
			this.Message = message;
			this.HasError = true;
		}
		
		public string Message
		{
			get
			{
				return _message;
			}
			set
			{
				_message = value;
				OnPropertyChanged(model => model.Message);
			}
		}
		
		public bool HasError
		{
			get
			{
				return _hasError;
			}
			set
			{
				_hasError = value;
				OnPropertyChanged(model => model.HasError);
			}
		}
		
		private void AddInManager_Events_OperationStarted(object sender, EventArgs e)
		{
			ClearMessage();
		}
		
		private void ClearMessage()
		{
			this.Message = null;
			this.HasError = false;
		}
		
		private void AddInManager_Events_AddInOperationError(object sender, AddInOperationErrorEventArgs e)
		{
			ShowErrorMessage(e.Message);
		}
		
		private void AddInManager_Events_AcceptLicenses(object sender, AcceptLicensesEventArgs e)
		{
			// Show a license acceptance prompt to the user
			e.IsAccepted = ShowLicenseAcceptancePrompt(e.Packages);
		}
		
		private bool ShowLicenseAcceptancePrompt(IEnumerable<IPackage> packages)
		{
			if (packages == null)
			{
				// No package -> nothing to accept
				return true;
			}
			
			// Create a license acceptance view
			var viewModel = new LicenseAcceptanceViewModel(packages);
			var view = new LicenseAcceptanceView();
			view.DataContext = viewModel;
			view.Owner = SD.Workbench.MainWindow;
			return view.ShowDialog() ?? false;
		}
		
		private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "IsExpandedInView")
			{
				AddInsViewModelBase expandedViewModel = sender as AddInsViewModelBase;
				if (expandedViewModel != null)
				{
					if (expandedViewModel.IsExpandedInView)
					{
						// Unexpand all view models besides this one
						foreach (var viewModel in _viewModels)
						{
							if (viewModel != expandedViewModel)
							{
								viewModel.IsExpandedInView = false;
							}
						}
					}
					else if (!expandedViewModel.IsExpandedInView && (_viewModels.Count(v => v.IsExpandedInView) == 0))
					{
						// This is the last unexpanded view => leave it open
						expandedViewModel.IsExpandedInView = true;
					}
				}
			}
		}
	}
}