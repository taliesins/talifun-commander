using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

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
        private readonly Dictionary<string, BitmapSource> _icons;

        public CommanderSectionWindow(ICommanderManager commandManager)
        {
            _commandManager = commandManager;

            _currentConfigurationElementCollections = GetConfiguration();
            _elementPanels = GetElementPanels();
            _elementCollectionPanels = GetElementCollectionPanels();
            _icons = GetConfigurationIcons();

            InitializeComponent();
            this.Icon = Properties.Resource.Commander.ToBitmap().ToBitmapSource();

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

    	private Dictionary<string, BitmapSource> GetConfigurationIcons()
        {
            var images = new Dictionary<string, BitmapSource>();

            foreach (var currentConfigurationElementCollection in _commandManager.Container.GetExportedValues<CurrentConfigurationElementCollection>())
            {
                images.Add(currentConfigurationElementCollection.Setting.ElementSettingName, currentConfigurationElementCollection.Setting.ElementImage.ToBitmapSource());
                images.Add(currentConfigurationElementCollection.Setting.ElementCollectionSettingName, currentConfigurationElementCollection.Setting.ElementCollectionImage.ToBitmapSource());
            }

            return images;
        }

        protected void BuildTree()
        {
            CommandSectionTreeView.Items.Clear();
            CommandSectionTreeView.ItemsSource = GetTreeViewItems();
        }

        protected IEnumerable<TreeViewWithIcons> GetTreeViewItems()
        {
            return AddProjects(_commandManager.Configuration.Projects);
        }

        protected IEnumerable<TreeViewWithIcons> AddProjects(ProjectElementCollection projects)
        {
            var projectsTreeViewItems = new List<TreeViewWithIcons>();

            for (var i = 0; i < projects.Count; i++)
            {
                var project = projects[i];
                var projectTreeViewItem = new TreeViewWithIcons
                                              {
                                                  Tag = project, 
                                                  HeaderText = project.Name,
                                                  Icon = _icons[projects.Setting.ElementSettingName]
                                              };

                projectsTreeViewItems.Add(projectTreeViewItem);

                AddFolders(projectTreeViewItem, project.Folders);

                foreach (var configurationElementCollection in _currentConfigurationElementCollections)
                {
                    var collectionSettingName = configurationElementCollection.Setting.ElementCollectionSettingName;
                    var configurationProperty = project.GetConfigurationProperty(collectionSettingName);
                    var commandElementCollection = project.GetCommandConfiguration<CurrentConfigurationElementCollection>(configurationProperty);

                    AddCommandConfigurationBase(projectTreeViewItem, commandElementCollection);
                }
            }

            return projectsTreeViewItems;
        }

        protected void AddFolders(TreeViewWithIcons projectTreeViewItem, FolderElementCollection folders)
        {
            var foldersTreeViewItem = new TreeViewWithIcons
            {
                Tag = folders,
                HeaderText = folders.Setting.ElementCollectionSettingName,
                Icon = _icons[folders.Setting.ElementCollectionSettingName]
            };

            projectTreeViewItem.Items.Add(foldersTreeViewItem);

            for (var i = 0; i < folders.Count; i++)
            {
                var folder = folders[i];
                var folderTreeViewItem = new TreeViewWithIcons
                {
                    Tag = folder,
                    HeaderText = folder.Name,
                    Icon = _icons[folders.Setting.ElementSettingName]
                };
                foldersTreeViewItem.Items.Add(folderTreeViewItem);

                AddFileMatches(folderTreeViewItem, folder.FileMatches);
            }
        }

        protected void AddFileMatches(TreeViewWithIcons folderTreeViewItem, FileMatchElementCollection fileMatches)
        {
            for (var i = 0; i < fileMatches.Count; i++)
            {
                var fileMatch = fileMatches[i];
                var fileMatchTreeViewItem = new TreeViewWithIcons
                {
                    Tag = fileMatch,
                    HeaderText = fileMatch.Name,
                    Icon = _icons[fileMatches.Setting.ElementSettingName]
                };
                folderTreeViewItem.Items.Add(fileMatchTreeViewItem);
            }
        }

        protected void AddCommandConfigurationBase(TreeViewWithIcons projectTreeViewItem, CurrentConfigurationElementCollection elementSettingCollection)
        {
            var collectionSettingTreeViewItem = new TreeViewWithIcons
            {
                Tag = elementSettingCollection,
                HeaderText = elementSettingCollection.Setting.ElementCollectionSettingName,
                Icon = _icons[elementSettingCollection.Setting.ElementCollectionSettingName]
            };

            projectTreeViewItem.Items.Add(collectionSettingTreeViewItem);

            for (var i = 0; i < elementSettingCollection.Count; i++)
            {
                var elementSetting = elementSettingCollection[i];
                var elementSettingTreeViewItem = new TreeViewWithIcons
                {
                    Tag = elementSetting,
                    HeaderText = elementSetting.Name,
                    Icon = _icons[elementSettingCollection.Setting.ElementSettingName]
                };
                collectionSettingTreeViewItem.Items.Add(elementSettingTreeViewItem);
            }
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
            var treeViewItem = (TreeViewWithIcons)treeView.SelectedItem;

            if (treeViewItem.Tag is CurrentConfigurationElementCollection)
            {
                var elementCollection = (CurrentConfigurationElementCollection)treeViewItem.Tag;
                var elementCollectionType = treeViewItem.Tag.GetType();
                DisplayElementCollectionPanel(elementCollection, elementCollectionType);
            }
            else if (treeViewItem.Tag is NamedConfigurationElement)
            {
                var element = (NamedConfigurationElement)treeViewItem.Tag;
                var elementType = treeViewItem.Tag.GetType();
                DisplayElementPanel(element, elementType);
            }
        }

        private void DisplayElementPanel(NamedConfigurationElement element, Type elementType)
        {
            var elementSettingPanel = _elementPanels.Where(x => x.Settings.ElementType == elementType).FirstOrDefault();

            if (CommandConfigurationContentControl.Content == null || !CommandConfigurationContentControl.Content.Equals(elementSettingPanel))
            {
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
    }
}
