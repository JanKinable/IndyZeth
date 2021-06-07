using System;
using System.Collections.Generic;
using System.Text;

namespace Elbanique.IndyZeth.Model
{
    public class Relationship
    {
        public string From { get; set; }
        public string To { get; set; }
        public RelationshipKind RelationshipKind { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Relationship relationship &&
                   From == relationship.From &&
                   To == relationship.To &&
                   RelationshipKind == relationship.RelationshipKind;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(From, To, RelationshipKind);
        }
    }
}
