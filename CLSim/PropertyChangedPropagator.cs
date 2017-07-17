using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simocracy.CLSim
{
    /// <summary>
    /// Helper class for notify readonly properties in referenced classes
    /// </summary>
    /// <remarks>See https://stackoverflow.com/questions/43653750 </remarks>
    public static class PropertyChangedPropagator
    {
        public static PropertyChangedEventHandler Create(string sourcePropertyName, string dependantPropertyName, Action<string> raisePropertyChanged)
        {
            var infiniteRecursionDetected = false;
            return (sender, args) =>
            {
                try
                {
                    if (args.PropertyName != sourcePropertyName) return;
                    if (infiniteRecursionDetected)
                    {
                        throw new InvalidOperationException("Infinite recursion detected");
                    }
                    infiniteRecursionDetected = true;
                    raisePropertyChanged(dependantPropertyName);
                }
                finally
                {
                    infiniteRecursionDetected = false;
                }
            };
        }
    }
}
