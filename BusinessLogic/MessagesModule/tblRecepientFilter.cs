//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Business_Logic.MessagesModule
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblRecepientFilter
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblRecepientFilter()
        {
            this.tblFilters = new HashSet<tblFilter>();
            this.tblWildcards = new HashSet<tblWildcard>();
            this.tblTemplates = new HashSet<tblTemplate>();
            this.tblRecepientCards = new HashSet<tblRecepientCard>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public int tblRecepientFilterTableNameId { get; set; }
    
        public virtual tblRecepientFilterTableName tblRecepientFilterTableName { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblFilter> tblFilters { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblWildcard> tblWildcards { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblTemplate> tblTemplates { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblRecepientCard> tblRecepientCards { get; set; }
    }
}
