﻿using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Basecode.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Repositories
{
    public class UserDetailsRepository : BaseRepository, IUserDetailsRepository
    {
        public UserDetailsRepository(IUnitOfWork unitOfWork) : base(unitOfWork) 
        {

        }

        public IQueryable<MUserDetail> GetUserDetails()
        {
            return this.GetDbSet<MUserDetail>();
        }
    }
}
