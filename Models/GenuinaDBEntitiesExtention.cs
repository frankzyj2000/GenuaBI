namespace GenuinaBI.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;

    public partial class GenuinaDBEntities
    {
        public GenuinaDBEntities(string connectionString)
            : base(connectionString)
        {

        }
    }
}