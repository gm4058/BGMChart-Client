using System.Windows.Controls;


namespace BGMChart.Themes
{   public partial class Conversation : UserControl
    {
        public Conversation()
        {
            InitializeComponent();
        }

        public void ScrollToLatestMessage()
        {
            if (ChatListBox.Items.Count > 0)
            {
                var lastMessage = ChatListBox.Items[ChatListBox.Items.Count - 1];
                ChatListBox.ScrollIntoView(lastMessage);
            }
        }
    }
}
