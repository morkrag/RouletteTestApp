using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace RouletteApp
{
    internal class Notification
    {
        public string notificationHeader = "Header";
        public string playerName = "Player Name";

        public Notification(string header, string player)
        {
            notificationHeader = header;
            playerName = player;
        }

        private void ModifyNotificationWindow(TextBlock PlayerName_TextBlock)
        {
            PlayerName_TextBlock.Text = playerName;
        }

        private void ModifyNotificationHeader(TextBlock NotificationHeader_TextBlock)
        {
            NotificationHeader_TextBlock.Text = notificationHeader;
        }
    }
}
