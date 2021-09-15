using System.Collections.Generic;
using Commander.DAL.Models;

namespace Commander.DAL.Repositories
{
    public interface ICommandRepository
    {
        IEnumerable<Command> GetAll();
        Command GetById(int id);
        void Create(Command item);
        void Update(Command item);
        void Delete(Command item);

        bool SaveChanges();
    }
}