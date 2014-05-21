using System;
using System.Collections.Generic;
using System.Data.Common;
using FluentAssertions;
using FluentAssertions;

namespace xpf.IO.Test
{
    public class TestTable
    {
        public int Id { get; set; }
        public string Field1 { get; set; }
        public DateTime Field2 { get; set; }
        public Guid Field3 { get; set; }
    }
}
