using Library.SQL.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Base
{
    public abstract class LibraryRepository
    {
        protected readonly LibraryDBContext Context;
        public LibraryRepository(LibraryDBContext context)
        {
            Context = context;
        }
    }
}
