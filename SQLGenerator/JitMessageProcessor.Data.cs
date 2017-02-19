//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Data.Entity;
//using JITMessageProcessor.Common;

//namespace JITMessageProcessor.Data  
//{
//    public class ProcessorDataContext:DbContext
//    {
//        public virtual DbSet<ProcessorConfig> ProcessorConfigs { get; set; }
//        public virtual DbSet<ProcessType> ProcessTypes { get; set; }
//        public virtual DbSet<VINPrefix> VINPrefixes { get; set; }
//        public virtual DbSet<ProcessZone> ProcessZones { get; set; }
//        public virtual DbSet<Processor> Processors { get; set; }
//        public virtual DbSet<ProcessorDetail> ProcessorDetails { get; set; }
//    }
//}

//static void Main(string[] args)
//{
//    //var client = new FTPClient("ftp://localhost", "foldename", null, "ftpUser", "ftpPass");
//    //var files = client.ListDirectory(); //ftp://localhost/public_html/

//    using (var db = new ProcessorDataContext())
//    {
//        var processorConfig = new ProcessorConfig { FTPServerIP = "1.2.3.4", FTPUserName = "usernametest", FTPPassword = "pass", FTPPort = 1 };
//        db.ProcessorConfigs.Add(processorConfig);
//        db.SaveChanges();

//        var query = from c in db.ProcessorConfigs
//                    orderby c.FTPUserName
//                    select c;
//        foreach (var item in query)
//        {
//            Console.WriteLine(item.FTPUserName);
//        }
//    }

//}
