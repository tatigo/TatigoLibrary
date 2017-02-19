using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace SQLGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            List<TableGenerator> tables = new List<TableGenerator>();

            // Pass assembly name via argument
            Assembly a = Assembly.LoadFrom(@"C:\Develop\Faurecia\Golcuk\JITMessageProcessor\JITMessageProcessorConsole\bin\Debug\JITMessageProcessorConsole.exe");
            Type[] types = a.GetTypes();

            // Get Types in the assembly.
            foreach (Type t in types)
            {
                TableGenerator tg = new TableGenerator(t);
                tables.Add(tg);
            }

            // Create SQL for each table
            foreach (TableGenerator table in tables)
            {
                Console.WriteLine(table.CreateTableScript());
                Console.WriteLine();
            }

            // Total Hacked way to find FK relationships! Too lazy to fix right now
            foreach (TableGenerator table in tables)
            {
                foreach (KeyValuePair<String, Type> field in table.Fields)
                {
                    foreach (TableGenerator t2 in tables)
                    {
                        if (field.Value.Name == t2.ClassName)
                        {
                            // We have a FK Relationship!
                            Console.WriteLine("GO");
                            Console.WriteLine("ALTER TABLE " + table.ClassName + " WITH NOCHECK");
                            Console.WriteLine("ADD CONSTRAINT FK_" + field.Key + " FOREIGN KEY (" + field.Key + ") REFERENCES " + t2.ClassName + "(ID)");
                            Console.WriteLine("GO");

                        }
                    }
                }
            }
        }
    }
}
