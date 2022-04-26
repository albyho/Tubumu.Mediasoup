using System.Threading.Tasks;

namespace Tubumu.Meeting.Server
{
    public interface IPeer
    {
        Task Notify(MeetingNotification notification);
    }
}
