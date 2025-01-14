using beeInnovative.DAL.Data;
using beeInnovative.DAL.Models;
using beeInnovative.DAL.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace beeInnovative.DAL
{
    public class UnitOfWork : IUnitOfWork
    {
        private BeeInnovativeContext _context;

        private GenericRepository<Beehive> beehiveRepository;
        private GenericRepository<Color> colorRepository;
        private GenericRepository<Hornet> hornetRepository;
        private GenericRepository<HornetDetection> hornetDetectionRepository;
        private GenericRepository<NestLocation> nestLocationRepository;
        private GenericRepository<User> userRepository;
        private GenericRepository<UserBeehive> userBeehiveRepository;

        public UnitOfWork(BeeInnovativeContext context)
        {
            _context = context;
        }

        public GenericRepository<Beehive> BeehiveRepository
        {
            get
            {
                if (beehiveRepository == null)
                {
                    beehiveRepository = new GenericRepository<Beehive>(_context);
                }
                return beehiveRepository;
            }
        }

        public GenericRepository<Color> ColorRepository
        {
            get
            {
                if (colorRepository == null)
                {
                    colorRepository = new GenericRepository<Color>(_context);
                }
                return colorRepository;
            }
        }

        public GenericRepository<Hornet> HornetRepository
        {
            get
            {
                if (hornetRepository == null)
                {
                    hornetRepository = new GenericRepository<Hornet>(_context);
                }
                return hornetRepository;
            }
        }

        public GenericRepository<HornetDetection> HornetDetectionRepository
        {
            get
            {
                if (hornetDetectionRepository == null)
                {
                    hornetDetectionRepository = new GenericRepository<HornetDetection>(_context);
                }
                return hornetDetectionRepository;
            }
        }

        public GenericRepository<NestLocation> NestLocationRepository
        {
            get
            {
                if (nestLocationRepository == null)
                {
                    nestLocationRepository = new GenericRepository<NestLocation>(_context);
                }
                return nestLocationRepository;
            }
        }

        public GenericRepository<User> UserRepository
        {
            get
            {
                if (userRepository == null)
                {
                    userRepository = new GenericRepository<User>(_context);
                }
                return userRepository;
            }
        }

        public GenericRepository<UserBeehive> UserBeehiveRepository
        {
            get
            {
                if (userBeehiveRepository == null)
                {
                    userBeehiveRepository = new GenericRepository<UserBeehive>(_context);
                }
                return userBeehiveRepository;
            }
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
