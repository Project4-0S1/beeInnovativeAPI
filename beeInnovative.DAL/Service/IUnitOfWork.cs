using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using beeInnovative.DAL.Models;


namespace beeInnovative.DAL.Service
{
    public interface IUnitOfWork
    {
        GenericRepository<Beehive> BeehiveRepository { get; }
        GenericRepository<Color> ColorRepository { get; }
        GenericRepository<Hornet> HornetRepository { get; }
        GenericRepository<HornetDetection> HornetDetectionRepository { get; }
        GenericRepository<NestLocation> NestLocationRepository { get; }
        GenericRepository<User> UserRepository { get; }
        GenericRepository<UserBeehive> UserBeehiveRepository { get; }

        Task SaveAsync();
    }
}
