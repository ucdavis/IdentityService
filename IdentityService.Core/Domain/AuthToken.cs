using System;
using Newtonsoft.Json;
using UCDArch.Core.DomainModel;

namespace IdentityService.Core.Domain
{

    [Serializable]
    [JsonObject]
    public class AuthToken : DomainObject
    {
        public virtual string Token { get; set; }
        public virtual bool Active { get; set; }
        public virtual string Name { get; set; }
        public virtual string Abbr { get; set; }
    }

    public class AuthTokenMap : FluentNHibernate.Mapping.ClassMap<AuthToken>
    {
        public AuthTokenMap()
        {
            Table("TokenV");
            ReadOnly();

            Id(x => x.Id).GeneratedBy.Assigned();

            Map(x => x.Token);
            Map(x => x.Active);
            Map(x => x.Name);
            Map(x => x.Abbr);
        }
    }

}
