using System;
using System.Collections.Generic;
using System.Configuration;
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
        private readonly string[] _excludedElements = new[] { "project", "folder", "fileMatch" };
        private readonly ICommanderManager _commandManager;
        private readonly IEnumerable<CurrentConfigurationElementCollection> _currentConfigurationElementCollections;
        private readonly IEnumerable<ElementPanelBase> _elementPanels;
        private readonly IEnumerable<ElementCollectionPanelBase> _elementCollectionPanels;

        public CommanderSectionWindow(ICommanderManager commandManager)
        {
            _commandManager = commandManager;

            _currentConfigurationElementCollections = GetConfiguration();
            _elementPanels = GetElementPanels();
            _elementCollectionPanels = GetElementCollectionPanels();

            InitializeComponent();
            this.Icon = Properties.Resource.Commander.ToBitmap().ToBitmapSource();
        	this.DataContext = new CommanderSectionWindowViewModel();
            BuildTree();
        }

        private IEnumerable<CurrentConfigurationElementCollection> GetConfiguration()
        {
            return _commandManager.Container.GetExportedValues<CurrentConfigurationElementCollection>()
                .Where(x => !_excludedElements.Contains(x.Setting.ElementSettingName));
        }

        private IEnumerable<ElementPanelBase> GetElementPanels()
        {
            return _commandManager.Container.GetExportedValues<ElementPanelBase>()
                .Where(x => x.Settings != null);
        }

        private IEnumerable<ElementCollectionPanelBase> GetElementCollectionPanels()
        {
            return _commandManager.Container.GetExportedValues<ElementCollectionPanelBase>()
                .Where(x => x.Settings != null);
        }

		private Dictionary<string, List<string>> GetCommandSettings(FileMatchElement fileMatchElement)
		{
			var project = GetProjectElement(fileMatchElement);
			var commandSettings = new Dictionary<string, List<string>>();
			foreach (var configurationElementCollection in _currentConfigurationElementCollections)
			{
				var collectionSettingName = configurationElementCollection.Setting.ElementCollectionSettingName;
				var configurationProperty = project.GetConfigurationProperty(collectionSettingName);
				var commandElementCollection = project.GetCommandConfiguration<CurrentConfigurationElementCollection>(configurationProperty);
				var conversionType = commandElementCollection.Setting.ConversionType;
				var commandSettingKeys = new List<string>();
				for (var i = 0; i < commandElementCollection.Count; i++)
				{
					var commandElement = commandElementCollection[i];
					commandSettingKeys.Add(commandElement.Name);
				}
				commandSettings.Add(conversionType, commandSettingKeys);
			}

			return commandSettings;
		}

    	private void BuildTree()
        {
        	var viewModel = ((CommanderSectionWindowViewModel) this.DataContext);
			viewModel.CommandTreeViewItemViewModels = _commandManager.Configuration.Projects;
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

        private void CommandSectionTreeViewSelected(object sender, RoutedEventArgs e)
        {
            var treeView = sender as TreeView;
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
					((NamedConfigurationElement) ((ElementPanelBase) CommandConfigurationContentControl.Content).DataContext).PropertyChanged -= OnElementPropertyChanged;
				}

                CommandConfigurationContentControl.Content = null;

                if (elementSettingPanel != null)
                {
                    CommandConfigurationContentControl.Content = elementSettingPanel;
                }
            }

            if (elementSettingPanel != null && CommandConfigurationContentControl.Content != null)
            {
				if (elementSettingPanel is FileMatchElementPanel)
				{
					var fileMatchElementPanel = ((FileMatchElementPanel)elementSettingPanel);
					var fileMatchElement = (FileMatchElement) element;
					var commandSettings = GetCommandSettings(fileMatchElement);
					fileMatchElementPanel.OnBindCommandSettings(commandSettings);
				}
                elementSettingPanel.OnBindToElement(element);
				((NamedConfigurationElement)elementSettingPanel.DataContext).PropertyChanged += OnElementPropertyChanged;
            }            
        }

        private void DisplayElementCollectionPanel(CurrentConfigurationElementCollection elementCollection, Type elementCollectionType)
        {
            var elementCollectionPanel = _elementCollectionPanels.Where(x => x.Settings.ElementCollectionType == elementCollectionType).FirstOrDefault();

            if (CommandConfigurationContentControl.Content == null || !CommandConfigurationContentControl.Content.Equals(elementCollectionPanel))
            {
                CommandConfigurationContentControl.Content = null;

                if (elementCollectionPanel != null)
                {
                    CommandConfigurationContentControl.Content = elementCollectionPanel;
                    elementCollectionPanel.OnBindToElementCollection(elementCollection);
                }
            }

            if (elementCollectionPanel != null && CommandConfigurationContentControl.Content != null)
            {
                elementCollectionPanel.OnBindToElementCollection(elementCollection);
            }                        
        }

		private void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			SaveButton.IsEnabled = true;
		}

		private void SaveButtonClick(object sender, RoutedEventArgs e)
		{
			SaveButton.IsEnabled = false;
			_commandManager.Configuration.CurrentConfiguration.Save(ConfigurationSaveMode.Minimal);
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
						CommandSectionTreeView.ContextMenu = Resources["FolderCollectionContextMenu"] as ContextMenu;
						CommandSectionTreeView.ContextMenu.Tag = projectElement;
					}
				}
				else if (bindingGroup.ElementType == typeof(CurrentConfigurationElementCollection))
				{
					CommandSectionTreeView.ContextMenu = null;
				}
				else
				{
					throw new Exception("Unknown binding group type");
				}
			}
			else
			{
				selectedItem = contentPresenter.DataContext;
			}

			if (selectedItem is ProjectElement)
			{
				CommandSectionTreeView.ContextMenu = Resources["ProjectContextMenu"] as ContextMenu;
				CommandSectionTreeView.ContextMenu.Tag = selectedItem;
			}
			else if (selectedItem is FolderElementCollection)
			{
				CommandSectionTreeView.ContextMenu = Resources["FolderCollectionContextMenu"] as ContextMenu;
				CommandSectionTreeView.ContextMenu.Tag = selectedItem;
			}
			else if (selectedItem is FolderElement)
			{
				CommandSectionTreeView.ContextMenu = Resources["FolderContextMenu"] as ContextMenu;
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
			var contextMenu = Resources["ElementContextMenu"] as ContextMenu;
			var menuItem = (MenuItem)contextMenu.Items[0];
			menuItem.Header = "Delete " + element.GetType().Name;
			CommandSectionTreeView.ContextMenu = contextMenu;
			CommandSectionTreeView.ContextMenu.Tag = element;
		}

		private void DisplayElementCollectionContextMenu(CurrentConfigurationElementCollection elementCollection)
		{
			var contextMenu = Resources["ElementCollectionContextMenu"] as ContextMenu;
			var menuItem = (MenuItem)contextMenu.Items[0];
			menuItem.Header = "Add " + elementCollection.Setting.ElementType.Name;
			CommandSectionTreeView.ContextMenu = contextMenu;
			CommandSectionTreeView.ContextMenu.Tag = elementCollection;
		}

		private void ContextMenuClosed(object sender, RoutedEventArgs e)
		{
			if (!_changingContextMenu)
			{
				CommandSectionTreeView.ContextMenu = Resources["DefaultContextMenu"] as ContextMenu;
			}
			_changingContextMenu = false; 
		}

		private void AddProjectMenuItemClick(object sender, RoutedEventArgs e)
		{
			var projects = _commandManager.Configuration.Projects;
			var project = projects.CreateNew();

			var counter = 0;
			do
			{
				counter++;
				project.Name = "New Project " + counter;
			} while (projects.Cast<NamedConfigurationElement>().Where(x => x.Name == project.Name).Any());

			projects[project.Name] = project;
		}

		private void DeleteProjectMenuItemClick(object sender, RoutedEventArgs e)
		{
			var projectElement = CommandSectionTreeView.ContextMenu.Tag as ProjectElement;

			if (projectElement == null)
			{
				return;
			}

			_commandManager.Configuration.Projects.Remove(projectElement.Name);
		}

		private void AddFolderMenuItemClick(object sender, RoutedEventArgs e)
		{
			var folderElementCollection = CommandSectionTreeView.ContextMenu.Tag as FolderElementCollection;

			if (folderElementCollection == null)
			{
				return;
			}
		}

		private void DeleteFolderMenuItemClick(object sender, RoutedEventArgs e)
		{
			var folderElement = CommandSectionTreeView.ContextMenu.Tag as FolderElement;

			if (folderElement == null)
			{
				return;
			}

			var folderElementCollection = GetFolderElementCollection(folderElement);
			if (folderElementCollection == null)
			{
				return;
			}

			folderElementCollection.Remove(folderElement.Name);
		}

		private void AddFileMatchMenuItemClick(object sender, RoutedEventArgs e)
		{

		}

		private void DeleteFileMatchMenuItemClick(object sender, RoutedEventArgs e)
		{
			var fileMatchElement = CommandSectionTreeView.ContextMenu.Tag as FileMatchElement;

			if (fileMatchElement == null)
			{
				return;
			}

			var fileMatchElementCollection = GetFileMatchElementCollection(fileMatchElement);
			if (fileMatchElementCollection == null)
			{
				return;
			}

			fileMatchElementCollection.Remove(fileMatchElement.Name);
		}

		private void AddElementMenuItemClick(object sender, RoutedEventArgs e)
		{

		}

		private void DeleteElementMenuItemClick(object sender, RoutedEventArgs e)
		{
			var namedConfigurationElement = CommandSectionTreeView.ContextMenu.Tag as NamedConfigurationElement;

			if (namedConfigurationElement == null)
			{
				return;
			}

			var currentConfigurationElementCollection = GetCurrentConfigurationElementCollection(namedConfigurationElement);
			if (currentConfigurationElementCollection == null)
			{
				return;
			}
			
			currentConfigurationElementCollection.Remove(namedConfigurationElement.Name);
		}

		private FolderElementCollection GetFolderElementCollection(FolderElement folderElement)
		{
			var folderElementCollection = _commandManager.Configuration.Projects.Cast<ProjectElement>()
				.Select(x => x.Folders)
				.Where(x => x.Cast<FolderElement>().Where(y => y == folderElement).Any())
				.FirstOrDefault();

			return folderElementCollection;
		}

		private FileMatchElementCollection GetFileMatchElementCollection(FileMatchElement fileMatchElement)
		{
			var fileMatchElementCollection = _commandManager.Configuration.Projects.Cast<ProjectElement>()
				.SelectMany(x => x.Folders.Cast<FolderElement>())
				.Select(x => x.FileMatches)
				.Where(x => x.Cast<FileMatchElement>().Where(y => y == fileMatchElement).Any())
				.FirstOrDefault();

			return fileMatchElementCollection;
		}

    	private ProjectElement GetProjectElement(FileMatchElement fileMatchElement)
    	{
    		var project = _commandManager.Configuration.Projects.Cast<ProjectElement>()
    			.Where(x => x.Folders.Cast<FolderElement>()
    			            	.Where(y => y.FileMatches.Cast<FileMatchElement>()
    			            	            	.Where(z => z == fileMatchElement)
    			            	            	.Any())
    			            	.Any())
    			.FirstOrDefault();

			return project;
		}

		private CurrentConfigurationElementCollection GetCurrentConfigurationElementCollection(NamedConfigurationElement namedConfigurationElement)
		{
			var currentConfigurationElementCollection = _commandManager.Configuration.Projects.Cast<ProjectElement>()
				.SelectMany(x => x.CommandPlugins)
				.Where(x => x.Setting == namedConfigurationElement.Setting)
				.Where(x => x.Cast<NamedConfigurationElement>()
					.Where(y => y == namedConfigurationElement)
					.Any())
				.First();

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

    }
}