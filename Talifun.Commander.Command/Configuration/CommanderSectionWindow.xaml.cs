using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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

		private ProjectElement GetProjectElementForFileMatchElement(FileMatchElement fileMatchElement)
		{
			var projects = _commandManager.Configuration.Projects;
			for (var i = 0; i < projects.Count; i++)
            {
                var project = projects[i];
            	var folders = project.Folders;
				for (var j = 0; j < folders.Count; j++)
				{
					var folder = folders[j];
					var fileMatches = folder.FileMatches;
					for (var k = 0; k < fileMatches.Count; k++)
					{
						var fileMatch = fileMatches[k];

						if (fileMatch == fileMatchElement)
						{
							return project;
						}
					}
				}
            }

			return null;
		}

		private Dictionary<string, List<string>> GetCommandSettings(FileMatchElement fileMatchElement)
		{
			var project = GetProjectElementForFileMatchElement(fileMatchElement);
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

			//var commandTreeViewModel = new CommandTreeViewModel(_currentConfigurationElementCollections, _icons);
			//var commandTreeViewItemViewModels = commandTreeViewModel.AddProjects(_commandManager.Configuration.Projects);

			//viewModel.CommandTreeViewItemViewModels = commandTreeViewItemViewModels;

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

			//treeViewItem.Focus();
			//e.Handled = true;

			var selectedItem = contentPresenter.DataContext;

			if (selectedItem is ProjectElement)
			{
				CommandSectionTreeView.ContextMenu = Resources["ProjectContextMenu"] as ContextMenu;
			}
			else if (selectedItem is FolderElementCollection)
			{
				CommandSectionTreeView.ContextMenu = Resources["FolderCollectionContextMenu"] as ContextMenu;
			}
			else if (selectedItem is FolderElement)
			{
				CommandSectionTreeView.ContextMenu = Resources["FolderContextMenu"] as ContextMenu;
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

		private void DisplayElementContextMenu(NamedConfigurationElement element)
		{
			var contextMenu = Resources["ElementContextMenu"] as ContextMenu;
			var menuItem = (MenuItem)contextMenu.Items[0];
			menuItem.Header = "Delete " + element.GetType().Name;
			CommandSectionTreeView.ContextMenu = contextMenu;
		}

		private void DisplayElementCollectionContextMenu(CurrentConfigurationElementCollection elementCollection)
		{
			var contextMenu = Resources["ElementCollectionContextMenu"] as ContextMenu;
			var menuItem = (MenuItem)contextMenu.Items[0];
			menuItem.Header = "Add " + elementCollection.Setting.ElementType.Name;
			CommandSectionTreeView.ContextMenu = contextMenu;
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
			project.Name = "New Project " + projects.Count;
			projects[project.Name] = project;
		}

		private void DeleteProjectMenuItemClick(object sender, RoutedEventArgs e)
		{

		}

		private void AddFolderMenuItemClick(object sender, RoutedEventArgs e)
		{

		}

		private void DeleteFolderMenuItemClick(object sender, RoutedEventArgs e)
		{

		}

		private void AddFileMatchMenuItemClick(object sender, RoutedEventArgs e)
		{

		}

		private void DeleteFileMatchMenuItemClick(object sender, RoutedEventArgs e)
		{

		}

		private void AddElementMenuItemClick(object sender, RoutedEventArgs e)
		{

		}

		private void DeleteElementMenuItemClick(object sender, RoutedEventArgs e)
		{

		}
    }
}