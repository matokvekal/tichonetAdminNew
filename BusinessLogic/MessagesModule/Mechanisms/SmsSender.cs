﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic.MessagesModule.Mechanisms {
    public class SmsSender : BatchSendingComponent {

        public SmsSender(BatchSendingManager manager) : base(manager) {


        }

        //public void SendSingle (ISmsMessage msg, ISmsProvider prov) {

        //}

    }
}
