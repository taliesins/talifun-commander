using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Talifun.Commander.UI;

namespace Talifun.Commander.Command.Configuration
{
    /// <summary>
    /// Interaction logic for CommanderSectionForm.xaml
    /// </summary>
    public partial class CommanderSectionWindow : Window
    {
    	private readonly ExportProvider _container;
		private readonly AppSettingsSection _appSettings;
    	private readonly CommanderSection _commanderSettings;
        
        private readonly IEnumerable<ElementPanelBase> _elementPanels;
        private readonly IEnumerable<ElementCollectionPanelBase> _elementCollectionPanels;

		public CommanderSectionWindow(ExportProvider container, AppSettingsSection appSettings, CommanderSection commanderSettings)
        {
			_container = container;
			_appSettings = appSettings;
			_commanderSettings = commanderSettings;

            _elementPanels = GetElementPanels();
            _elementCollectionPanels = GetElementCollectionPanels();

            InitializeComponent();
            this.Icon = Properties.Resource.Commander.ToBitmap().ToBitmapSource();

			this.DataContextChanged += OnCommanderSectionWindowDataContextChanged;
			this.DataContext = new CommanderSectionWindowViewModel
			                   	{
			                   		CommandTreeViewItemViewModels = _commanderSettings.Projects
			                   	};
        }

		private void OnCommanderSectionWindowDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			SaveButton.IsEnabled = false;
			CreateFoldersButton.IsEnabled = !AreAllFoldersCreated();
		}

        private IEnumerable<ElementPanelBase> GetElementPanels()
        {
			return _container.GetExportedValues<ElementPanelBase>()
                .Where(x => x.Settings != null);
        }

        private IEnumerable<ElementCollectionPanelBase> GetElementCollectionPanels()
        {
			return _container.GetExportedValues<ElementCollectionPanelBase>()
                .Where(x => x.Settings != null);
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            Exit();
        }

        private void OkButtonClick(object sender, RoutedEventArgs e)
        {
            Exit();
        }

        private void Exit()
        {
            CommandConfigurationContentControl.Content = null;
            Close();
        }

		private void CreateFoldersButtonClick(object sender, RoutedEventArgs e)
		{
			CreateAllFolders();
			CreateFoldersButton.IsEnabled = !AreAllFoldersCreated();
		}

        private void CommandSectionTreeViewSelected(object sender, RoutedEventArgs e)
        {
            var treeView = (TreeView)sender;
            var selectedItem = treeView.SelectedItem;

            if (selectedItem is CurrentConfigurationElementCollection)
            {
                var elementCollection = (CurrentConfigurationElementCollection)selectedItem;
                var elementCollectionType = selectedItem.GetType();
                DisplayElementCollectionPanel(elementCollection, elementCollectionType);
            }
            else if (selectedItem is NamedConfigurationElement)
            {
                var element = (NamedConfigurationElement)selectedItem;
                var elementType = selectedItem.GetType();
                DisplayElementPanel(element, elementType);
            }
        }

        private void DisplayElementPanel(NamedConfigurationElement element, Type elementType)
        {
            var elementSettingPanel = _elementPanels.Where(x => x.Settings.ElementType == elementType).FirstOrDefault();

            if (CommandConfigurationContentControl.Content == null || !CommandConfigurationContentControl.Content.Equals(elementSettingPanel))
            {
				if (CommandConfigurationContentControl.Content != null)
				{
					((INotifyPropertyChanged)((SettingPanelBase)CommandConfigurationContentControl.Content).DataContext).PropertyChanged -= OnElementPropertyChanged;
				}

                CommandConfigurationContentControl.Content = null;

                if (elementSettingPanel != null)
                {
                    CommandConfigurationContentControl.Content = elementSettingPanel;
                }
            }

            if (elementSettingPanel != null && CommandConfigurationContentControl.Content != null)
            {
                elementSettingPanel.OnBindToElement(_appSettings, _commanderSettings, element);
				((INotifyPropertyChanged)elementSettingPanel.DataContext).PropertyChanged += OnElementPropertyChanged;
            }            
        }

        private void DisplayElementCollectionPanel(CurrentConfigurationElementCollection elementCollection, Type elementCollectionType)
        {
            var elementCollectionPanel = _elementCollectionPanels.Where(x => x.Settings.ElementCollectionType == elementCollectionType).FirstOrDefault();

            if (CommandConfigurationContentControl.Content == null || !CommandConfigurationContentControl.Content.Equals(elementCollectionPanel))
            {
				if (CommandConfigurationContentControl.Content != null)
				{
					((INotifyPropertyChanged)((SettingPanelBase)CommandConfigurationContentControl.Content).DataContext).PropertyChanged -= OnElementPropertyChanged;
				}

                CommandConfigurationContentControl.Content = null;

                if (elementCollectionPanel != null)
                {
                    CommandConfigurationContentControl.Content = elementCollectionPanel;
					elementCollectionPanel.OnBindToElementCollection(_appSettings, _commanderSettings, elementCollection);
                }
            }

            if (elementCollectionPanel != null && CommandConfigurationContentControl.Content != null)
            {
				elementCollectionPanel.OnBindToElementCollection(_appSettings, _commanderSettings, elementCollection);
				((INotifyPropertyChanged)elementCollectionPanel.DataContext).PropertyChanged += OnElementPropertyChanged;
            }                        
        }

		private void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			SaveButton.IsEnabled = _commanderSettings.Projects.IsModified;
			CreateFoldersButton.IsEnabled = !SaveButton.IsEnabled && !AreAllFoldersCreated();
		}

    	private void SaveButtonClick(object sender, RoutedEventArgs e)
		{
			SaveButton.IsEnabled = false;
			_commanderSettings.CurrentConfiguration.Save(ConfigurationSaveMode.Minimal);
			CreateFoldersButton.IsEnabled = !AreAllFoldersCreated();
		}

    	private bool _changingContextMenu;
		private void OnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			var contentPresenter = e.Source as ContentPresenter;

			if (contentPresenter == null)
			{
				return;
			}
			_changingContextMenu = true;

			object selectedItem = null;
			if (contentPresenter.DataContext is BindingGroup)
			{
				var bindingGroup = (BindingGroup)contentPresenter.DataContext;

				var bindingGroupTreeViewItem = VisualUpwardSearch<TreeViewItem>(e.OriginalSource as DependencyObject) as TreeViewItem;
				var projectTreeViewItem = VisualUpwardSearch<TreeViewItem>(VisualTreeHelper.GetParent(bindingGroupTreeViewItem)) as HeaderedItemsControl;
				var projectElement = projectTreeViewItem.Header as ProjectElement;

				if (bindingGroup.ElementType == typeof(FolderElement) )
				{
					if (projectElement != null)
					{
						CommandSectionTreeView.ContextMenu = (ContextMenu)Resources["FolderCollectionContextMenu"];
						CommandSectionTreeView.ContextMenu.Tag = projectElement.Folders;
					}
				}
				else
				{
					CommandSectionTreeView.ContextMenu = null;
				}
			}
			else
			{
				selectedItem = contentPresenter.DataContext;
			}

			if (selectedItem is ProjectElement)
			{
				CommandSectionTreeView.ContextMenu = (ContextMenu)Resources["ProjectContextMenu"];
				CommandSectionTreeView.ContextMenu.Tag = selectedItem;
			}
			else if (selectedItem is FolderElementCollection)
			{
				CommandSectionTreeView.ContextMenu = (ContextMenu)Resources["FolderCollectionContextMenu"];
				CommandSectionTreeView.ContextMenu.Tag = selectedItem;
			}
			else if (selectedItem is FolderElement)
			{
				CommandSectionTreeView.ContextMenu = (ContextMenu)Resources["FolderContextMenu"];
				CommandSectionTreeView.ContextMenu.Tag = selectedItem;
			}
			else if (selectedItem is FileMatchElement)
			{
				CommandSectionTreeView.ContextMenu = (ContextMenu)Resources["FileMatchContextMenu"];
				CommandSectionTreeView.ContextMenu.Tag = selectedItem;
			}
			else if (selectedItem is CurrentConfigurationElementCollection)
			{
				var elementCollection = (CurrentConfigurationElementCollection)selectedItem;
				DisplayElementCollectionContextMenu(elementCollection);
			}
			else if (selectedItem is NamedConfigurationElement)
			{
				var element = (NamedConfigurationElement)selectedItem;
				DisplayElementContextMenu(element);
			}	
		}

    	private void DisplayElementContextMenu(NamedConfigurationElement element)
		{
			var contextMenu = (ContextMenu)Resources["ElementContextMenu"];
			var menuItem = (MenuItem)contextMenu.Items[0];
			menuItem.Header = string.Format(Properties.Resource.ContextMenuDeleteElement, element.Setting.ElementType.Name);
			CommandSectionTreeView.ContextMenu = contextMenu;
			CommandSectionTreeView.ContextMenu.Tag = element;
		}

		private void DisplayElementCollectionContextMenu(CurrentConfigurationElementCollection elementCollection)
		{
			var contextMenu = (ContextMenu)Resources["ElementCollectionContextMenu"];
			var menuItem = (MenuItem)contextMenu.Items[0];
			menuItem.Header = string.Format(Properties.Resource.ContextMenuAddElement, elementCollection.Setting.ElementType.Name);
			CommandSectionTreeView.ContextMenu = contextMenu;
			CommandSectionTreeView.ContextMenu.Tag = elementCollection;
		}

		private void ContextMenuClosed(object sender, RoutedEventArgs e)
		{
			if (!_changingContextMenu)
			{
				CommandSectionTreeView.ContextMenu = (ContextMenu)Resources["DefaultContextMenu"];
			}
			_changingContextMenu = false; 
		}

		private void AddProjectMenuItemClick(object sender, RoutedEventArgs e)
		{
			var projects = _commanderSettings.Projects;
			AddNewElementToElementCollection(projects);
		}

		private void DeleteProjectMenuItemClick(object sender, RoutedEventArgs e)
		{
			var projectElement = (ProjectElement)CommandSectionTreeView.ContextMenu.Tag;

			_commanderSettings.Projects.Remove(projectElement.Name);
		}

		private void AddFolderMenuItemClick(object sender, RoutedEventArgs e)
		{
			var folders = (FolderElementCollection)CommandSectionTreeView.ContextMenu.Tag;
			AddNewElementToElementCollection(folders);
			//todo: We have to manually refresh the filematches node as its using custom multi binding
		}

		private void DeleteFolderMenuItemClick(object sender, RoutedEventArgs e)
		{
			var folderElement = (FolderElement)CommandSectionTreeView.ContextMenu.Tag;

			var folderElementCollection = GetFolderElementCollection(folderElement);
			if (folderElementCollection == null)
			{
				return;
			}

			folderElementCollection.Remove(folderElement.Name);
			//todo: We have to manually refresh the filematches node as its using custom multi binding
		}

		private void AddFileMatchMenuItemClick(object sender, RoutedEventArgs e)
		{
			var folder = (FolderElement)CommandSectionTreeView.ContextMenu.Tag;
			var fileMatches = folder.FileMatches;
			AddNewElementToElementCollection(fileMatches);
		}

		private void DeleteFileMatchMenuItemClick(object sender, RoutedEventArgs e)
		{
			var fileMatchElement = (FileMatchElement)CommandSectionTreeView.ContextMenu.Tag;
			var fileMatchElementCollection = GetFileMatchElementCollection(fileMatchElement);
			if (fileMatchElementCollection == null)
			{
				return;
			}

			fileMatchElementCollection.Remove(fileMatchElement.Name);
		}

		private void AddElementMenuItemClick(object sender, RoutedEventArgs e)
		{
			var currentConfigurationElementCollection = (CurrentConfigurationElementCollection)CommandSectionTreeView.ContextMenu.Tag;
			AddNewElementToElementCollection(currentConfigurationElementCollection);
		}

		private void DeleteElementMenuItemClick(object sender, RoutedEventArgs e)
		{
			var namedConfigurationElement = (NamedConfigurationElement)CommandSectionTreeView.ContextMenu.Tag;

			var currentConfigurationElementCollection = GetCurrentConfigurationElementCollection(namedConfigurationElement);
			if (currentConfigurationElementCollection == null)
			{
				return;
			}
			
			currentConfigurationElementCollection.Remove(namedConfigurationElement.Name);
		}

		private void AddNewElementToElementCollection(CurrentConfigurationElementCollection currentConfigurationElementCollection)
		{
			if (currentConfigurationElementCollection == null)
			{
				return;
			}

			var namedConfigurationElement = currentConfigurationElementCollection.CreateNew();

			var counter = 0;
			do
			{
				counter++;
				var name = namedConfigurationElement.Setting.ElementType.Name + " " + counter;
				namedConfigurationElement.Name = string.Format(Properties.Resource.ContextMenuNewElement, name);
			} while (currentConfigurationElementCollection.Cast<NamedConfigurationElement>().Where(x => x.Name == namedConfigurationElement.Name).Any());

			currentConfigurationElementCollection[namedConfigurationElement.Name] = namedConfigurationElement;
		}

		private FolderElementCollection GetFolderElementCollection(FolderElement folderElement)
		{
			var folderElementCollection = _commanderSettings.Projects
				.Select(x => x.Folders)
				.Where(x => x.Where(y => y == folderElement).Any())
				.FirstOrDefault();

			return folderElementCollection;
		}

		private FileMatchElementCollection GetFileMatchElementCollection(FileMatchElement fileMatchElement)
		{
			var fileMatchElementCollection = _commanderSettings.Projects
				.SelectMany(x => x.Folders)
				.Select(x => x.FileMatches)
				.Where(x => x.Where(y => y == fileMatchElement).Any())
				.FirstOrDefault();

			return fileMatchElementCollection;
		}

		private CurrentConfigurationElementCollection GetCurrentConfigurationElementCollection(NamedConfigurationElement namedConfigurationElement)
		{
			var currentConfigurationElementCollection = _commanderSettings.Projects
				.SelectMany(x => x.CommandPlugins)
				.Where(x => x.Setting == namedConfigurationElement.Setting)
				.Where(x => x.Cast<NamedConfigurationElement>()
					.Where(y => y == namedConfigurationElement)
					.Any())
				.FirstOrDefault();

			return currentConfigurationElementCollection;
		}

		static DependencyObject VisualUpwardSearch<T>(DependencyObject source)
		{
			while (source != null && source.GetType() != typeof(T))
			{
				if (source is Visual || source is Visual3D)
				{
					source = VisualTreeHelper.GetParent(source);
				}
				else
				{
					source = LogicalTreeHelper.GetParent(source);
				}
			}
			return source;
		}

		/// <summary>
		/// Are all the folders required to run created?
		/// </summary>
		/// <returns>True: if all the folders needed are created; otherwise false;</returns>
		public bool AreAllFoldersCreated()
		{
			return AreAllFoldersCreatedForFolderElement() && AreAllFoldersCreatedForPlugins();
		}

		/// <summary>
		/// Are all the folders required for FolderElements to run created?
		/// </summary>
		/// <returns>True: if all the folders needed for FolderElements are created; otherwise false;</returns>
		private bool AreAllFoldersCreatedForFolderElement()
		{
			var allFoldersCreatedForFolderElement = _commanderSettings.Projects
				.SelectMany(x => x.Folders);

			if (!allFoldersCreatedForFolderElement.Any()) return true;

			var result = !allFoldersCreatedForFolderElement
				.Where(y =>
					(!string.IsNullOrEmpty(y.GetFolderToWatchOrDefault()) && !Directory.Exists(y.GetFolderToWatchOrDefault()))
					|| (!string.IsNullOrEmpty(y.GetWorkingPathOrDefault()) && !Directory.Exists(y.GetWorkingPathOrDefault()))
					|| (!string.IsNullOrEmpty(y.GetCompletedPathOrDefault()) && !Directory.Exists(y.GetCompletedPathOrDefault()))
				)
				.Any();

			return result;
		}

		/// <summary>
		/// Are all the folder required for plugins to run created?
		/// </summary>
		/// <returns>True: if all the folders needed for plugins are created; otherwise false;</returns>
		private bool AreAllFoldersCreatedForPlugins()
		{
			var allFoldersCreatedForPlugins = _commanderSettings.Projects
				.SelectMany(x => x.CommandPlugins)
				.SelectMany(x => x.Cast<CommandConfigurationBase>());

			if (!allFoldersCreatedForPlugins.Any())
			{
				return true;
			}

			var result = !allFoldersCreatedForPlugins
				.Where(x =>
					(!string.IsNullOrEmpty(x.GetWorkingPathOrDefault()) && !Directory.Exists(x.GetWorkingPathOrDefault()))
					|| (!string.IsNullOrEmpty(x.GetErrorProcessingPathOrDefault()) && !Directory.Exists(x.GetErrorProcessingPathOrDefault()))
					|| (!string.IsNullOrEmpty(x.GetOutPutPathOrDefault()) && !Directory.Exists(x.GetOutPutPathOrDefault()))
				)
				.Any();

			return result;
		}

		/// <summary>
		/// Create any folders that are needed.
		/// </summary>
		public void CreateAllFolders()
		{
			CreateAllFoldersForFolderElement();
			CreateAllFoldersCreatedForPlugins();
		}

		/// <summary>
		/// Create all the folders needed by FolderElements.
		/// </summary>
		private void CreateAllFoldersForFolderElement()
		{
			var allFoldersCreatedForFolderElement = _commanderSettings.Projects
				.SelectMany(x => x.Folders);

			foreach (var folderElement in allFoldersCreatedForFolderElement)
			{
				if (!string.IsNullOrEmpty(folderElement.GetFolderToWatchOrDefault()) && !Directory.Exists(folderElement.GetFolderToWatchOrDefault()))
				{
					Directory.CreateDirectory(folderElement.GetFolderToWatchOrDefault());
				}

				if (!string.IsNullOrEmpty(folderElement.GetWorkingPathOrDefault()) && !Directory.Exists(folderElement.GetWorkingPathOrDefault()))
				{
					Directory.CreateDirectory(folderElement.GetWorkingPathOrDefault());
				}

				if (!string.IsNullOrEmpty(folderElement.GetCompletedPathOrDefault()) && !Directory.Exists(folderElement.GetCompletedPathOrDefault()))
				{
					Directory.CreateDirectory(folderElement.GetCompletedPathOrDefault());
				}
			}
		}

		/// <summary>
		/// Create all the folders needed by plugins.
		/// </summary>
		private void CreateAllFoldersCreatedForPlugins()
		{
			var allFoldersCreatedForPlugins = _commanderSettings.Projects
				.SelectMany(x => x.CommandPlugins)
				.SelectMany(x => x.Cast<CommandConfigurationBase>());

			foreach (var allFoldersCreatedForPlugin in allFoldersCreatedForPlugins)
			{
				if (!string.IsNullOrEmpty(allFoldersCreatedForPlugin.GetWorkingPathOrDefault()) && !Directory.Exists(allFoldersCreatedForPlugin.GetWorkingPathOrDefault()))
				{
					Directory.CreateDirectory(allFoldersCreatedForPlugin.GetWorkingPathOrDefault());
				}

				if (!string.IsNullOrEmpty(allFoldersCreatedForPlugin.GetErrorProcessingPathOrDefault()) && !Directory.Exists(allFoldersCreatedForPlugin.GetErrorProcessingPathOrDefault()))
				{
					Directory.CreateDirectory(allFoldersCreatedForPlugin.GetErrorProcessingPathOrDefault());
				}

				if (!string.IsNullOrEmpty(allFoldersCreatedForPlugin.GetOutPutPathOrDefault()) && !Directory.Exists(allFoldersCreatedForPlugin.GetOutPutPathOrDefault()))
				{
					Directory.CreateDirectory(allFoldersCreatedForPlugin.GetOutPutPathOrDefault());
				}
			}
		}
    }
}