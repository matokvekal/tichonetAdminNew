using Business_Logic.MessagesModule.EntitiesExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_Logic.MessagesModule.DataObjects;
using Newtonsoft.Json;

namespace Business_Logic.MessagesModule {
    public partial class tblSmsSenderDataProvider : IMessagesModuleEntity, ISmsServiceProvider {

        #region ISmsServiceProvider realization
        int ISmsServiceProvider.MessageInterval {get {return MessageInterval;}
        }

        string ISmsServiceProvider.Password {get {return Password;}}

        SendProviderRestrictionData _RestrictionData;
        SendProviderRestrictionData ISendServiceProvider.RestrictionData {
            get {
                if (_RestrictionData == null) {
                    if (string.IsNullOrWhiteSpace(SendProviderRestrictionDataJSON))
                        _RestrictionData = new SendProviderRestrictionData();
                    else
                        _RestrictionData = JsonConvert
                            .DeserializeObject<SendProviderRestrictionData>(SendProviderRestrictionDataJSON);
                }
                return _RestrictionData;
            }
        }

        SendProviderRestrictionDataLog _RestrictionDataLog;
        SendProviderRestrictionDataLog ISendServiceProvider.RestrictionDataLog {
            get {
                if (_RestrictionDataLog == null) {
                    if (string.IsNullOrWhiteSpace(SendProviderRestrictionDataLogJSON))
                        _RestrictionDataLog = new SendProviderRestrictionDataLog();
                    else
                        _RestrictionDataLog = JsonConvert
                            .DeserializeObject<SendProviderRestrictionDataLog>(SendProviderRestrictionDataLogJSON);
                }
                return _RestrictionDataLog;
            }
            set {
                SendProviderRestrictionDataLogJSON = JsonConvert.SerializeObject(value);
            }
        }

        string ISmsServiceProvider.SenderName { get { return FromDisplayName; } }

        string ISmsServiceProvider.SenderNumber { get { return FromPhoneNumber; } }

        string ISmsServiceProvider.UserName { get { return Username; } }
        #endregion
    }
}
