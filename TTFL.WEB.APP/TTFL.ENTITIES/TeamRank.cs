﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace TTFL.ENTITIES
{
    public partial class TeamRank
    {
        public int TeamId { get; set; }
        public int PickId { get; set; }
        public int Rank { get; set; }

        public virtual Pick Pick { get; set; }
        public virtual Team Team { get; set; }
    }
}