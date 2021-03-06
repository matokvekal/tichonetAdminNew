﻿using Business_Logic.MessagesModule.DataObjects;
using Business_Logic.MessagesModule.EntitiesExtensions;
using Business_Logic.SqlContext;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Business_Logic.MessagesModule.Mechanisms {

    public class MessageProductionData {
        /// <summary>
        /// Contains text data:
        /// groupped by recepient adress,
        /// IDictionary contains: key: name of field of DataBase Table, value: value of this field.
        /// </summary>
        public IEnumerable< IGrouping<string, IDictionary<string, object>> > TextProductionData { get; set; }
        /// <summary>
        /// Contains wildcards presented as:
        /// key: {code} word thas should be in text template;
        /// value: name of field in DataBase Table.
        /// </summary>
        public IEnumerable<KeyValuePair<string,string>> wildCards { get; set; }
    }

    public class MessageDataCollector : BatchCreationComponent {

        public MessageDataCollector (BatchCreationManager manager) : base(manager) {
        }

        /// <summary>
        /// Returns MessageProductionData for each Recepient in template.
        /// </summary>
        public IEnumerable< MessageProductionData > Collect (IMessageTemplate templ) {
            var output = new List<MessageProductionData>();
            //Migrate all needed data to memory, because it will be intensive processed
            var filters = templ.Filters!=null ? templ.Filters.ToArray() : new tblFilter[] { };
            var wildcards = templ.Wildcards!=null ? templ.Wildcards.ToArray() : new tblWildcard[] { };
            var recepients = templ.Recepients!=null ? templ.Recepients.ToArray(): new tblRecepientCard[] { };
            var userInputedValues = templ.FilterValueContainers!=null ? templ.FilterValueContainers.ToArray() : new FilterValueContainer[] { };

            Dictionary<int, ValueOperatorPair[]> filtsToValOps = GetFiltersActualSettings(filters, userInputedValues);

            var Condition = SqlPredicate.BuildAndNode();
            if (filters.Length > 0) {
                foreach (var f in filters) {
                    var orNode = SqlPredicate.BuildOrNode();
                    var valops = filtsToValOps[f.Id];
                    foreach (var valop in valops) {
                        orNode.Append(SqlPredicate.BuildEndNode(f.Key, valop.Operator, valop.Value, f.Type));
                    }
                    Condition.Append(orNode);
                }
            }
            
            //Build list of needed colomns
            var colomns = wildcards.Select(x => x.Key)
                .Concat(recepients.Select(x => x.EmailKey))
                .Concat(recepients.Select(x => x.NameKey))
                .Concat(recepients.Select(x => x.PhoneKey)).Distinct();

            var sqlData = Manager.SqlLogic.FetchData(colomns, templ.TableWithKeysName, "dbo", Condition);

            var wildcardsSummed = wildcards.SelectMany(x => x.ToKeyValues());

            bool IsSms = templ.IsSms;
            foreach (var rec in recepients) {
                var prodData = new MessageProductionData();
                var GroupKey = IsSms ? rec.PhoneKey : rec.EmailKey;
                prodData.TextProductionData = sqlData
                    .Where(x => !string.IsNullOrWhiteSpace(x[GroupKey].ToString()))
                    .GroupBy(x => x[GroupKey].ToString());
                prodData.wildCards = wildcardsSummed.Concat(rec.ToKeyValues());
                output.Add(prodData);
            }

            return output;
        }

        //------------------------------------------
        //Private Part

        /// <summary>
        /// Returns Dictionary:
        /// key: filter ID
        /// value: actual settings
        /// </summary>
        Dictionary<int, ValueOperatorPair[]> GetFiltersActualSettings(tblFilter[] filters, FilterValueContainer[] userInputedValues) {
            var filtsToValOps = new Dictionary<int, ValueOperatorPair[]>();
            foreach (var f in filters) {
                var valops = tblFilterHelper.GetValueOperatorPairs(f, Manager.SqlLogic);
                if (NullBoolToBool(f.allowUserInput)) {
                    //if user was selecting from list.
                    if (NullBoolToBool(f.autoUpdatedList) || valops.Length > 1) {
                        valops = valops
                            //HERE WE USE STRING CHECK to Compare... =\
                            .Where(x => userInputedValues.Any
                                (y => y.FilterId == f.Id && y.Values != null
                                    && y.Values.Any(z => GetValueFormatted(z) == x.Value.ToString())
                                )
                            )
                            .ToArray();
                    }
                    //if user typed value directly
                    else {
                        var val = userInputedValues.First(x => x.FilterId == f.Id);
                        string formattedValue;
                        if (IfValueValidGetValueFormatted(val.Values, f.Type, out formattedValue))
                            valops = new[] { new ValueOperatorPair(formattedValue, valops[0].Operator, f.Type) };
                        else
                            valops = new ValueOperatorPair[0];
                    }

                }
                filtsToValOps.Add(f.Id, valops);
            }

            return filtsToValOps;
        }


        //------------------------------------------
        //Utility Part

        static bool NullBoolToBool (bool? b) {
            return b.HasValue && b.Value;
        }

        /// <summary>
        /// Checks - if no value, that means that filter not be used.
        /// Also handle BOOL case - if no value, it return valid "False" value
        /// </summary>
        static bool IfValueValidGetValueFormatted(object[] values,string SqlType, out string FormattedValue) {
            if (values == null || values.Length == 0 || values[0] == null) {
                if (SqlType.ToLower() == "bit") {
                    FormattedValue = bool.FalseString;
                    return true;
                }
                FormattedValue = null;
                return false;
            }
            FormattedValue = GetValueFormatted(values[0]);
            return true;
        }

        static string GetValueFormatted(object obj) {
            if (obj == null)
                return "";
            return obj.ToString();
        }

    }


}
