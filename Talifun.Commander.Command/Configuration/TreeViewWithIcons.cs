using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Talifun.Commander.Command.Configuration
{
    public class TreeViewWithIcons : TreeViewItem
    {
        private ImageSource _iconSource;
        private readonly TextBlock _textBlock;
        private readonly Image _icon;

        public TreeViewWithIcons()
        {
            var stack = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };

            _icon = new Image
            {
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 4, 0),
                Source = _iconSource
            };

            _textBlock = new TextBlock
            {
                VerticalAlignment = VerticalAlignment.Center
            };

            Header = stack;
            stack.Children.Add(_icon);
            stack.Children.Add(_textBlock);
        }

        /// <summary>
        /// Event Handler on UnSelected Event
        /// </summary>
        /// <param name="args">Eventargs</param>
        protected override void OnUnselected(RoutedEventArgs args)
        {
            base.OnUnselected(args);
            _icon.Source = _iconSource;
        }
        /// <summary>
        /// Event Handler on Selected Event 
        /// </summary>
        /// <param name="args">Eventargs</param>
        protected override void OnSelected(RoutedEventArgs args)
        {
            base.OnSelected(args);
            _icon.Source = _iconSource;
        }

        /// <summary>
        /// Gets/Sets the Selected Image for a TreeViewNode
        /// </summary>
        public ImageSource Icon
        {
            set
            {
                _iconSource = value;
                _icon.Source = _iconSource;
            }
            get
            {
                return _iconSource;
            }
        }

        /// <summary>
        /// Gets/Sets the HeaderText of TreeViewWithIcons
        /// </summary>
        public string HeaderText
        {
            set
            {
                _textBlock.Text = value;
            }
            get
            {
                return _textBlock.Text;
            }
        }
    }
}
