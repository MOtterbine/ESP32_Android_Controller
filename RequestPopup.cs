using System.Threading.Tasks;

namespace Models
{
    public delegate Task<bool> RequestPopup(string title, string prompt, bool isYesNo = true);
}
