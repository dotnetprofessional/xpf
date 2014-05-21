using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xpf.Scripting
{
    public class ResultItem
    {
        public ResultItem(Dictionary<string, object> values)
        {
            this.Properties = new ResultProperties(values);
        }

        /// <summary>
        /// Accesses the properties of the first executed script
        /// </summary>
        public dynamic Properties { get; private set; }
    }

    public class Result
    {
        object syncLoc = new object();
        public Result()
        {
            lock (syncLoc)
            {
                this.Results = new List<ResultItem>();
            }
        }

        public void AddResult(Dictionary<string, object> values)
        {
            lock (syncLoc)
            {
                this.Results.Add(new ResultItem(values));
                if (this.Results.Count == 1)
                    this.Properties = this.Results[0].Properties;
            }
        }

        public List<ResultItem> Results { get; private set; }

        /// <summary>
        /// Accesses the properties of the first executed script
        /// </summary>
        public dynamic Properties { get; private set; }

    }

    /*
     *  Usage
     *
     * xpf.Script("sql").
     * xpf.Script("ps").
     * 
     * 
     *  xpf.Script()
     *      .WithDatabase("xxx")
     *      .TakeSnapshot.
     *      .UsingScript("xxx")
     *          .WithIn(new {Name = "ssss"})
     *          .WithOut(nwe {Name = DbType.String})
     *      .UsingCommand("xxx")
     *          .Execute();
     * 
     * 
     */
}
