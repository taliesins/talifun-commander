using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Talifun.Commander.Command.Configuration
{
    public partial class CommanderSectionForm : Form
    {
        private readonly string[] _excludedElements = new[] { "project", "folder", "fileMatch" };
        private readonly ICommanderManager _commandManager;
        private readonly IEnumerable<CurrentConfigurationElementCollection> _currentConfigurationElementCollections;
        private readonly ImageList _imageList;

        public CommanderSectionForm(ICommanderManager commandManager)
        {
            _commandManager = commandManager;

            _currentConfigurationElementCollections = GetConfiguration();
            _imageList = GetConfigurationImages();

            InitializeComponent();
            BuildTree();
        }

        protected IEnumerable<CurrentConfigurationElementCollection> GetConfiguration()
        {
            return _commandManager.Container.GetExportedValues<CurrentConfigurationElementCollection>()
                .Where(x => !_excludedElements.Contains(x.Setting.ElementSettingName));
        }

        protected ImageList GetConfigurationImages()
        {
            var images = new ImageList
            {
                ImageSize = new Size(24, 24)
            };
            images.Images.Add("project", Properties.Resource.project);
            images.Images.Add("folders", Properties.Resource.folders);
            images.Images.Add("folder", Properties.Resource.folder);
            images.Images.Add("fileMatch", Properties.Resource.fileMatch);

            foreach (var currentConfigurationElementCollection in _currentConfigurationElementCollections)
            {
                images.Images.Add(currentConfigurationElementCollection.Setting.ElementSettingName, currentConfigurationElementCollection.Setting.ElementImage);
                images.Images.Add(currentConfigurationElementCollection.Setting.CollectionSettingName, currentConfigurationElementCollection.Setting.ElementCollectionImage);
            }

            return images;
        }

        protected IEnumerable<CommandConfigurationPanelBase> GetCommandConfigurationPanels()
        {
            return _commandManager.Container.GetExportedValues<CommandConfigurationPanelBase>();
        }

        protected void BuildTree()
        {
            CommandSectionTreeView.Nodes.Clear(); // Clear any existing items
            CommandSectionTreeView.BeginUpdate(); // prevent overhead and flicker
            CommandSectionTreeView.ImageList = _imageList;
            LoadNodes(CommandSectionTreeView.Nodes);
            CommandSectionTreeView.EndUpdate(); // re-enable the tree
            CommandSectionTreeView.Refresh(); // refresh the treeview display
        }

        protected void LoadNodes(TreeNodeCollection nodes)
        {
            AddProjects(nodes, _commandManager.Configuration.Projects);
        }

        protected void AddProjects(TreeNodeCollection nodes, ProjectElementCollection projects)
        {
            for (var i = 0; i < projects.Count; i++)
            {
                var project = projects[i];
                var key = "project|" + project.Name;
                var value = "project - " + project.Name;
                var projectNode = nodes.Add(key, value, "project", "project");
                projectNode.Tag = project;

                AddFolders(projectNode.Nodes, project.Folders);
                
                foreach (var configurationElementCollection in _currentConfigurationElementCollections)
                {
                    var collectionSettingName = configurationElementCollection.Setting.CollectionSettingName;
                    var configurationProperty = project.GetConfigurationProperty(collectionSettingName);
                    var commandElementCollection = project.GetCommandConfiguration<CurrentConfigurationElementCollection>(configurationProperty);

                    AddCommandConfigurationBase(projectNode.Nodes, commandElementCollection);
                }
            }
        }

        protected void AddFolders(TreeNodeCollection nodes, FolderElementCollection folders)
        {
            var foldersNode = nodes.Add("folder|", "Folders", "folders", "folders");
            foldersNode.Tag = folders;

            for (var i = 0; i < folders.Count; i++)
            {
                var folder = folders[i];
                var key = "folder|" + folder.Name;
                var value = "folder - " + folder.Name;
                var folderNode = foldersNode.Nodes.Add(key, value, "folder", "folder");
                folderNode.Tag = folder;

                AddFileMatches(folderNode.Nodes, folder.FileMatches);
            }
        }

        protected void AddFileMatches(TreeNodeCollection nodes, FileMatchElementCollection fileMatches)
        {
            for (var i = 0; i < fileMatches.Count; i++)
            {
                var fileMatch = fileMatches[i];
                var key = "fileMatch|" + fileMatch.Name;
                var value = "fileMatch - " + fileMatch.Name;
                var fileMatchNode = nodes.Add(key, value, "fileMatch", "fileMatch");
                fileMatchNode.Tag = fileMatch;
            }
        }

        protected void AddCommandConfigurationBase(TreeNodeCollection nodes, CurrentConfigurationElementCollection commandElementCollection)
        {
            var elementCollectionNode = nodes.Add(commandElementCollection.Setting.ElementSettingName + "|", commandElementCollection.Setting.CollectionSettingName, commandElementCollection.Setting.CollectionSettingName);
            elementCollectionNode.Tag = commandElementCollection;

            for (var i = 0; i < commandElementCollection.Count; i++)
            {
                var commandElement = commandElementCollection[i];
                var key = commandElementCollection.Setting.ElementSettingName + "|" + commandElement.Name;
                var value = commandElementCollection.Setting.ElementSettingName + " - " + commandElement.Name;
                var elementNode = elementCollectionNode.Nodes.Add(key, value, commandElementCollection.Setting.ElementSettingName, commandElementCollection.Setting.ElementSettingName);
                elementNode.Tag = commandElement;
            }
        }

        private void CommandSectionTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var selectedNode = e.Node;
            var key = selectedNode.Name;
        }

        private void CommandSectionTreeView_MouseDown(object sender, MouseEventArgs e)
        {
            var treeView = sender as TreeView;
            var selectedNode = treeView.GetNodeAt(e.X, e.Y);

            //Show context menu
        }
    }
}
