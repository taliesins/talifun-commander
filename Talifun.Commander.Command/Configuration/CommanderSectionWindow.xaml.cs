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
        private readonly IEnumerable<ElementPanelBase> _elementSettingPanels;
        private readonly Dictionary<string, BitmapSource> _icons;

        public CommanderSectionWindow(ICommanderManager commandManager)
        {
            _commandManager = commandManager;

            _currentConfigurationElementCollections = GetConfiguration();
            _elementSettingPanels = GetElementSettingPanels();
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

        private IEnumerable<ElementPanelBase> GetElementSettingPanels()
        {
            return _commandManager.Container.GetExportedValues<ElementPanelBase>()
                .Where(x => x.Settings != null);
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

        //private void CommandSectionTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        //{
        //    var selectedNode = e.Node;
        //    var elementSettingName = selectedNode.Name.Split('|').First();

        //    var commandConfigurationPanel = _elementSettingPanels.Where(x => x.Settings.ElementSettingName == elementSettingName).FirstOrDefault();

        //    if (CommandConfigurationContentControl.Controls.Count == 0 || !CommandConfigurationContentControl.Controls[0].Equals(commandConfigurationPanel))
        //    {
        //        CommandConfigurationContentControl.Controls.Clear();

        //        if (commandConfigurationPanel != null)
        //        {
        //            CommandConfigurationContentControl.Controls.Add(commandConfigurationPanel);
        //            commandConfigurationPanel.Visible = true;
        //        }
        //    }
        //}

        //private void CommandSectionTreeView_MouseDown(object sender, MouseEventArgs e)
        //{
        //    var treeView = sender as TreeView;
        //    var selectedNode = treeView.GetNodeAt(e.X, e.Y);

        //    //Show context menu
        //}

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OkButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CommandSectionTreeViewSelected(object sender, RoutedEventArgs e)
        {
            var treeView = sender as TreeView;
            var treeViewItem = (TreeViewWithIcons)treeView.SelectedItem;

            if (treeViewItem.Tag is CurrentConfigurationElementCollection)
            {
                var elementSettingCollectionType = treeViewItem.Tag.GetType();
            }
            else if (treeViewItem.Tag is NamedConfigurationElement)
            {
                var elementSettingType = treeViewItem.Tag.GetType();

                var elementSettingPanel = _elementSettingPanels.Where(x => x.Settings.ElementType == elementSettingType).FirstOrDefault();

                if (CommandConfigurationContentControl.Content == null || !CommandConfigurationContentControl.Content.Equals(elementSettingPanel))
                {
                    CommandConfigurationContentControl.Content = null;

                    if (elementSettingPanel != null)
                    {
                        CommandConfigurationContentControl.Content = elementSettingPanel;
                    }
                }
            }
        }
    }
}
