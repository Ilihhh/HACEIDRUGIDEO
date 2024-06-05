using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Model
{
    public enum CommandType { NavCommand, EntityManipulation, CanvasManipulation, LineManipulation }

    /// <summary>
    /// This class models an application save state, specified by the command that initiated it's state
    /// </summary>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    public class SaveState<TFirst, TSecond>
    {
        public SaveState(TFirst commandType, TSecond state)
        {
            CommandType = commandType;
            SavedState = state;
        }
        public SaveState() { }

        public TFirst CommandType { get; set; }
        public TSecond SavedState { get; set; }
    }
}
