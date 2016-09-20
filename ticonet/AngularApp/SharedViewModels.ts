﻿namespace AngularApp.Controllers {

    export interface INgViewModel {
        ng_JustCreated:boolean
        ng_ToDelete:boolean
    }

    export interface IIndeficated {
        Id: number
    }

    export function FindById<T extends IIndeficated>(arr: T[], Id: number) {
        return arr.first(x => x.Id === Id)
    }

    export interface IValidable {
        Invalid: boolean
        ValidationErrors: string[]
    }

    export interface IHasFilterValueContainer {
        FilterValueContainers: FilterValueContainer[]
    }

    export class BaseTableVM implements IIndeficated, INgViewModel {
        Id: number
        Name: string
        ReferencedTableName: string
        ng_JustCreated: boolean
        ng_ToDelete: boolean
    }

    export class MetaFilterVM implements IIndeficated, INgViewModel, IValidable {
        Id: number
        filters: FilterVM[] = []
        wildcards: WildcardVM[] = []
        reccards: RecepientCardVM[] = []

        Name: string = "New Filter"
        BaseTableId: number = -1

        ng_JustCreated: boolean
        ng_ToDelete: boolean

        //local
        //validate filter enitities, Invalid Filter can't be saved
        ValidationErrors: string[]

        Invalid = false
    }

    export class KeyVM {
        name: string
        type: string
    }

    export class ValOp {
        Value: any
        Operator: string

        constructor() {
            this.Value = ""
            this.Operator = "="
        }
    }

    export class FilterVM implements IIndeficated, INgViewModel, IValidable {
        Id: number
        Name: string
        RecepientFilterId: number
        Key: string
        ValsOps: ValOp[]
        Type: string

        allowMultipleSelection:boolean
        allowUserInput: boolean
        autoUpdatedList:boolean

        ng_JustCreated: boolean
        ng_ToDelete: boolean

        //local
        Invalid = false
        ValidationErrors: string[]

        //Show:? boolean
    }

    export class FilterValueContainer {
        FilterId: number
        Values: any[] = []
    }

    export class WildcardVM implements IIndeficated, INgViewModel, IValidable {
        Id: number
        RecepientFilterId: number
        Name: string
        Code: string
        Key: string

        ng_JustCreated: boolean
        ng_ToDelete: boolean

        //local
        _Code: string
        Invalid = false
        ValidationErrors: string[]

        FilterValueContainers: FilterValueContainer[]
    }

    export class RecepientCardVM implements IIndeficated, INgViewModel, IValidable {
        Id: number
        RecepientFilterId: number
        Name: string

        NameKey: string
        EmailKey: string
        PhoneKey: string

        ng_JustCreated: boolean
        ng_ToDelete: boolean

        //local
        Invalid = false
        ValidationErrors: string[]
    }

    export class FiltOperator {
        SQLString: string
        ShortString: string
        Operator: number
        RawInt: number
    }

    export class TemplateVM implements IIndeficated, INgViewModel, IHasFilterValueContainer {
        Id: number
        RecepientFilterId: number = -1
        Name: string = "New Template"
        IsSms: boolean
        MsgHeader: string
        MsgBody: string

        ng_JustCreated: boolean
        ng_ToDelete: boolean

        FilterValueContainers: FilterValueContainer[] = []
        ChoosenReccards: number[] = []
    }

    export class MessageScheduleVM implements IIndeficated, INgViewModel, IHasFilterValueContainer {
        Id: number
        TemplateId: number = -1
        Name: string = "New Template"

        ScheduleDate: Date
        RepeatMode: string

        IsActive: boolean
        InArchive: boolean

        IsSms: boolean
        MsgHeader: string
        MsgBody: string

        BatchesCount: number

        FilterValueContainers: FilterValueContainer[] = []
        ChoosenReccards: number[] = []

        ng_JustCreated: boolean
        ng_ToDelete: boolean
    }

    export class BatchReportVM implements IIndeficated, INgViewModel {
        Id: number

        BaseScheduleName: string
        CreatedOn: Date
        FinishedOn: Date
        Errors: string
        IsSms: boolean
        MessagesCount: number

        ng_JustCreated: boolean
        ng_ToDelete: boolean

        static ServerDataMarshall = (data: BatchReportVM) => {
            data.CreatedOn = formatVal(data.CreatedOn, "datetime")
            data.FinishedOn = formatVal(data.FinishedOn, "datetime")
            return data;
        }
    }

    export class MessageReportVM implements IIndeficated, INgViewModel {
        Id: number

        Header: string
        Body: string
        Adress: string
        IsSms: boolean
        SentOn: Date
        MessageBatchId: number
        ErrorLog: string
        IsPending: boolean

        ng_JustCreated: boolean
        ng_ToDelete: boolean
    }

    export class SendProviderRestrictionData {
        MaxMessagesInHour: number = 1
        MaxMessagesInDay: number = 1
    }

    export function IsEmailSenderDataProviderVM(prov: EmailSenderDataProviderVM | SmsSenderDataProviderVM) {
        return typeof (prov as any).EnableSsl !== 'undefined'
    }

    export class EmailSenderDataProviderVM implements IIndeficated, INgViewModel {
        Id: number = -1

        Name: string = "NewEmailServiceProvider"
        IsActive: boolean = false
        FromEmailAddress: string = "from@mail.com"
        FromEmailDisplayName: string = "FromName"
        FromEmailPassword: string = ""
        SmtpHostName: string = ""
        SmtpPort: number = 25
        EnableSsl: boolean = true
        RestrictionData: SendProviderRestrictionData = new SendProviderRestrictionData()

        ng_JustCreated: boolean = true
        ng_ToDelete: boolean = false
    }

    export class SmsSenderDataProviderVM implements IIndeficated, INgViewModel {
        Id: number = -1

        Name: string = "NewSmsServiceProvider"
        IsActive: boolean = false
        FromDisplayName: string = "FromName"
        FromPhoneNumber: string = "000"
        Username: string = "username"
        Password: string = ""
        MessageInterval: number = 5
        RestrictionData: SendProviderRestrictionData = new SendProviderRestrictionData()

        ng_JustCreated: boolean = true
        ng_ToDelete: boolean = false
    }


}