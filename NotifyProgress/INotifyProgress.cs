using System;
using System.Collections.Generic;
using System.Text;

namespace o2Mate
{
    /// <summary>
    /// Interface declaration to display a progress bar
    /// and steps through the progression
    /// </summary>
    public interface INotifyProgress
    {
        /// <summary>
        /// Says if the user has canceled the process
        /// </summary>
        bool IsCanceled { get; }
        /// <summary>
        /// Gets the ending condition or sets to true
        /// </summary>
        bool IsFinished { get; set; }
        /// <summary>
        /// Sets the total number of work
        /// </summary>
        /// <param name="count"></param>
        void SetCount(int count);
        /// <summary>
        /// Sets the current number as done work
        /// </summary>
        /// <param name="position">the current number</param>
        void SetProgress(int position);
        /// <summary>
        /// Advances the counter to inc
        /// </summary>
        /// <param name="inc">value to advance</param>
        void Step(int inc);
        /// <summary>
        /// Progress bar format
        /// </summary>
        void SetMarquee();
        /// <summary>
        /// Sets the text description to display
        /// </summary>
        /// <param name="t">text to display</param>
        void SetText(string t);
        /// <summary>
        /// Starts the progress bar (if cancelable, adds a cancel button)
        /// </summary>
        /// <param name="cancelable">can be canceled by user</param>
        void Start(bool cancelable);

        /// <summary>
        /// Notifies the terminated work
        /// </summary>
        /// <param name="noError">with error or not</param>
        void Terminate(bool noError);

        /// <summary>
        /// Displays a dialog box with the exception
        /// </summary>
        /// <param name="ex">exception object</param>
        void GiveException(Exception ex);
    }
}
