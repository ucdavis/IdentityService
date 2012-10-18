using System;
using Newtonsoft.Json;
using UCDArch.Core.DomainModel;

namespace IdentityService.Core.Domain
{

    [Serializable]
    [JsonObject]
    public class People : DomainObject
    {
        public virtual string KerberosId { get; set; }
        public virtual string PpsId { get; set; }
        public virtual string StudentId { get; set; }
        public virtual string BannerPidm { get; set; }
        public virtual string ExternalId { get; set; }
        public virtual string OfficialFirstName { get; set; }
        public virtual string OfficialMiddleName { get; set; }
        public virtual string OfficialLastName { get; set; }
        public virtual string OfficialSuffix { get; set; }
        public virtual string OfficialFullName { get; set; }
        public virtual string DisplayFirstName { get; set; }
        public virtual string DisplayMiddleName { get; set; }
        public virtual string DisplayLastName { get; set; }
        public virtual string DisplaySuffix { get; set; }
        public virtual string DisplayFullName { get; set; }
        public virtual string EmailAddress { get; set; }
        public virtual string PhoneNumber { get; set; }
        public virtual bool IsFaculty { get; set; }
        public virtual bool IsStudent { get; set; }
        public virtual bool IsStaff { get; set; }
        public virtual bool IsExternal { get; set; }
        public virtual string PrivacyCode { get; set; }
        public virtual DateTime LastUpdate { get; set; }
    }

    public class PeopleMap : FluentNHibernate.Mapping.ClassMap<People>
    {
        public PeopleMap()
        {
            Table("PeopleV");
            ReadOnly();

            Id(x => x.Id).Column("IAMID").GeneratedBy.Assigned();

           // Map(x => x.Id).Column("IAMID");
            Map(x => x.KerberosId);
            Map(x => x.PpsId);
            Map(x => x.StudentId);
            Map(x => x.BannerPidm);
            Map(x => x.ExternalId);
            Map(x => x.OfficialFirstName);
            Map(x => x.OfficialMiddleName);
            Map(x => x.OfficialLastName);
            Map(x => x.OfficialSuffix);
            Map(x => x.OfficialFullName);
            Map(x => x.DisplayFirstName);
            Map(x => x.DisplayMiddleName);
            Map(x => x.DisplayLastName);
            Map(x => x.DisplaySuffix);
            Map(x => x.DisplayFullName);
            Map(x => x.EmailAddress);
            Map(x => x.PhoneNumber);
            Map(x => x.IsFaculty);
            Map(x => x.IsStudent);
            Map(x => x.IsStaff);
            Map(x => x.IsExternal);
            Map(x => x.PrivacyCode);
            Map(x => x.LastUpdate);

            Cache.ReadOnly();
        }
    }

}
