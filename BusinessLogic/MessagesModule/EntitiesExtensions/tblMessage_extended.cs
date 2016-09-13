using Business_Logic.MessagesModule.DataObjects;
using Business_Logic.MessagesModule.EntitiesExtensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic.MessagesModule {
    public partial class tblMessage : IMessagesModuleEntity, IEmailMessage, ISmsMessage {

        #region ISmsMessage realization
        string ISmsMessage.PhoneNumber {
            get {
                return Adress;
            }
        }

        string ISmsMessage.Text {
            get {
                return Header + " \n" + Body;
            }
        }
        #endregion

        #region IEmailMessage realization

        string IEmailMessage.Body {
            get {
                return Body;
            }
        }

        bool IEmailMessage.IsBodyHtml {
            get {
                return true;
            }
        }

        string IEmailMessage.RecepientAdress {
            get {
                return Adress;
            }
        }

        string IEmailMessage.RecepientName {
            get {
                //TODO RecepientName
                return Adress;
            }
        }

        string IEmailMessage.Subject {
            get {
                return Header;
            }
        }


        #endregion

        void IErrorLoged.AddError(string errorMessage) {
            ErrorLog = string.IsNullOrEmpty(ErrorLog) ? errorMessage : ErrorLog + "; " + errorMessage;
        }

        DateTime? IMessage.SendDate {
            get {
                return SentOn;
            }

            set {
                SentOn = value;
            }
        }
    }
}
