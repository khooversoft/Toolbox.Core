using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.MessageNet.Host
{
    public class MessageNetHostContainerRegistrations : ContainerRegistrationModule
    {
        public MessageNetHostContainerRegistrations()
        {
            Add(typeof(MessageAwaiterManager), typeof(IMessageAwaiterManager), true);
            Add(typeof(MessageRepository), typeof(IMessageRepository), true);
            Add(typeof(MessageNetHost), typeof(IMessageNetHost), true);
        }
    }
}
