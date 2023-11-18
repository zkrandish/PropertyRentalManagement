namespace PropertyRentalManagementWebSite.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class Apartment
    {
        // These properties will use the foreign key properties but give you a clearer name in your code.
        [ForeignKey("ManagerId")]
        public User Manager { get; set; }

        [ForeignKey("TenantId")]
        public User Tenant { get; set; }
        // It uses the StatusId foreign key to reference the Status entity.
        public virtual Status Status { get; set; }
    }
}