using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseORMGenerator.Internal
{
    public interface ISchemaRepository
    {
        Schema GetSchema();
        void SaveSchema(Schema Schema);
    }
}
