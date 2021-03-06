using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Collections;

namespace DataRead
{
    public class DataReadConfig : ScriptConfiguration, INotifyPropertyChanged
    {
         #region Private Fields
         private testValue2 testValue3;
         #endregion

         #region Public Properties
         public bool ValidationActive
         {
              get { return this._validationActive;}
              set { this._validationActive = value; }
         }

         public new string Name
         {
              get { return base.Name; }
              set {
                     if (String.IsNullOrEmpty(value) && this.ValidationActive)
                     {
                           throw new Exception("*");
                     }
                     base.Name = value;
                     this.NotifyPropertyChanged("Name");
              }
         }

         public testValue2 publicName
         {
             get
             {
                 if (this["testValue4"] != null)
                     return this["testValue4"].ToString();
                 else
                     return null;

             }
             set
             {

             }
         }
         #endregion

         #region INotifyPropertyChanged

         public event PropertyChangedEventHandler PropertyChanged;

         #endregion

         private void NotifyPropertyChanged(string propertyName)
         {
              if (PropertyChanged != null)
              {
                  PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
              }
         }
    }
}
